using UnityEngine;
using System;
using Parse;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ParentPortalManager : Singleton<ParentPortalManager> {
	public static EventHandler<ServerEventArgs> OnDataRefreshed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
		
	public void RefreshData(){
		var parentPortalQuery = new ParseQuery<KidAccount>()
			.WhereEqualTo("createdBy", ParseUser.CurrentUser);

		try{
			ExtraParseLogic.Instance.UserCheck().ContinueWith(t => {
				return parentPortalQuery.FindAsync();
			}).Unwrap().ContinueWith(t => {
				if(t.IsFaulted || t.IsCanceled){
					ParseException exception = (ParseException)t.Exception.InnerExceptions[0];
					Debug.Log("Message: " + exception.Message + ", Code: " + exception.Code);
					
					Loom.DispatchToMainThread(() => {
						ServerEventArgs args = new ServerEventArgs();
						args.IsSuccessful = false;
						args.ErrorCode = ErrorCodes.ConnectionError;
						
						if(OnDataRefreshed != null)
							OnDataRefreshed(this, args);
					});
				}
				else{
					IEnumerable<KidAccount> kidAccounts = t.Result;
					foreach(KidAccount account in kidAccounts){
						Debug.Log("kid account: " + account.ObjectId);
					}

					Loom.DispatchToMainThread(() => {
						ServerEventArgs args = new ServerEventArgs();
						args.IsSuccessful = true;
						args.ErrorCode = ErrorCodes.None;
						
						if(OnDataRefreshed != null)
							OnDataRefreshed(this, args);
					});
				}
			});
		}
		catch(InvalidOperationException e){
			//Errors from parse SDK logic check
			Debug.LogException(e);

			ServerEventArgs args = new ServerEventArgs();
			args.IsSuccessful = false;
			args.ErrorCode = ErrorCodes.None;
			
			if(OnDataRefreshed != null)
				OnDataRefreshed(this, args);
		}
	}
}
