using UnityEngine;
using System.Collections;
using System;
using Parse;
using System.Threading.Tasks;

public class ParseTestScript : MonoBehaviour {

	void Start(){
		SocialManager.OnDataRefreshed += OnDataRefreshed;
	}

	#if UNITY_EDITOR
	void OnGUI(){



		#region ExtraParseLogic Test
		GUILayout.BeginHorizontal();

		if(GUILayout.Button("User & Kid Account case 1")){
			UserSignupAndKidAccountTest();
		}
		if(GUILayout.Button("User & Kid Account case 2")){
			ExtraParseLogic.Instance.UserSignupCase2();
		}
		if(GUILayout.Button("User & Kid Account case 3")){
			RegularUserSignupAndKidAccountTest();
		}

		GUILayout.EndHorizontal();
		#endregion

		#region SocialManager Test
		GUILayout.BeginHorizontal();

		if(GUILayout.Button("Get friend list")){
			SocialManager.Instance.RefreshData();
		}
		if(GUILayout.Button("Add friend code")){
			SocialManager.Instance.AddFriendCode("xcrusNVAjo");
		}

		GUILayout.EndHorizontal();
		#endregion

		if(GUILayout.Button("Logout")){
			ParseUser.LogOut();
		}


	}
	#endif

	private void OnDataRefreshed(object sender, ServerEventArgs args){
		Debug.Log("IsSuccessful: " + args.IsSuccessful);
	}

	private void RegularUserSignupAndKidAccountTest(){
		try{
			ExtraParseLogic.Instance.UserAndKidAccountCheck().ContinueWith(t => {
				if(t.IsCanceled || t.IsFaulted){
					// Errors from Parse Cloud and network interactions
					foreach(ParseException e in t.Exception.InnerExceptions){
						Debug.Log(e.Message);
						Debug.Log(e.Code);
					}
				}
				else{
					KidAccount account = t.Result;
					Debug.Log(account.ObjectId);
				}
			});
		}
		catch(InvalidOperationException e){
			// Error from the SDK logic checks
			Debug.Log(e.Message);
		}
	}

	/// <summary>
	/// Test case 1:
	/// user sign up --> pass
	/// kid account creation --> pass
	/// 
	/// User and Kid Account are removed after Test
	/// </summary>
	private void UserSignupAndKidAccountTest(){
		try{
			ExtraParseLogic.Instance.UserAndKidAccountCheck().ContinueWith(t => {
				if(t.IsCanceled || t.IsFaulted){
					// Errors from Parse Cloud and network interactions
					foreach(ParseException e in t.Exception.InnerExceptions){
						Debug.Log(e.Message);
						Debug.Log(e.Code);
					}
				}
				else{
					Debug.Log("User and KidAccount check Succesful");
					Loom.DispatchToMainThread(() => {
						Debug.Log("Kid Account ID: " + DataManager.Instance.GameData.PetInfo.ParseKidAccountID);
						DataManager.Instance.GameData.PetInfo.ParseKidAccountID = "";
					});
					ParseUser.LogOut();
				}
			});
		}
		catch(InvalidOperationException e){
			// Error from the SDK logic checks
			Debug.Log(e.Message);
		}
	}	
}
