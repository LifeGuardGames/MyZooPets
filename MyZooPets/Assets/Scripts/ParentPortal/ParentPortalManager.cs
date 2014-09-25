using UnityEngine;
using System;
using Parse;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ParentPortalManager : Singleton<ParentPortalManager> {
	public static EventHandler<EventArgs> OnDataLoadSucessful;
	public static EventHandler<EventArgs> OnDataLoadFailed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

#if UNITY_EDITOR
	void OnGUI(){
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("parent portal refresh data")){
			RefreshData();
		}
		if(GUILayout.Button("Add Kid Account")){
			try{
				ExtraParseLogic.Instance.CreateNewKidAccount().ContinueWith(t => {
					if(t.IsFaulted || t.IsCanceled){
						foreach(ParseException e in t.Exception.InnerExceptions){
							Debug.Log(e.Message);
							Debug.Log(e.Code);
						}
					}
					else{
						Debug.Log("Kid Account created");
					}
				});
			}
			catch(InvalidOperationException e){
				Debug.Log(e.Message);
			}

		}
		if(GUILayout.Button("Logout")){
			ParseUser.LogOut();
		}
		GUILayout.EndHorizontal();
	}
	#endif
		
	public void RefreshData(){
		var parentPortalQuery = new ParseQuery<KidAccount>()
			.WhereEqualTo("createdBy", ParseUser.CurrentUser);

		try{
			ExtraParseLogic.Instance.UserCheck().ContinueWith(t => {
				return parentPortalQuery.FindAsync();
			}).Unwrap().ContinueWith(t => {
				if(t.IsFaulted || t.IsCanceled){
					// Errors from Parse Cloud and network interactions
					foreach(ParseException e in t.Exception.InnerExceptions){
						Debug.Log(e.Message);
						Debug.Log(e.Code);
					}

					if(OnDataLoadFailed != null)
						OnDataLoadFailed(this, EventArgs.Empty);
				}
				else{
					Debug.Log("pass");
					IEnumerable<KidAccount> kidAccounts = t.Result;
					foreach(KidAccount account in kidAccounts){
						Debug.Log("kid account: " + account.ObjectId);
					}
					if(OnDataLoadSucessful != null)
						OnDataLoadSucessful(this, EventArgs.Empty);
				}
			});
		}
		catch(InvalidOperationException e){
			//Errors from parse SDK logic check
			Debug.Log(e.Message);

			if(OnDataLoadFailed != null)
				OnDataLoadFailed(this, EventArgs.Empty);
		}
	}
}
