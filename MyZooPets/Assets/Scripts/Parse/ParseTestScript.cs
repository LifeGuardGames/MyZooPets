using UnityEngine;
using System.Collections;
using System;
using Parse;
using System.Threading.Tasks;

public class ParseTestScript : MonoBehaviour {

	void Start(){
		SocialManager.OnDataRefreshed += EventListener;
		SocialManager.OnFriendCodeAdded += EventListener;
		ParentPortalManager.OnDataRefreshed += EventListener;
	}

	#if UNITY_EDITOR
	void OnGUI(){
		#region ExtraParseLogic Test
		GUILayout.BeginHorizontal();
		GUILayout.Label("ExtraParseLogic Test");
		if(GUILayout.Button("User & Kid Account ")){
			RegularUserSignupAndKidAccountTest();
		}

		GUILayout.EndHorizontal();
		#endregion

		#region SocialManager Test
		GUILayout.BeginHorizontal();
		GUILayout.Label("SocialManager Test");
		if(GUILayout.Button("Get friend list")){
			SocialManager.Instance.RefreshData();
		}
		if(GUILayout.Button("Add good friend code")){
			SocialManager.Instance.AddFriendCode("5P8QH1PfME");
		}
		if(GUILayout.Button("Add bad friend code")){
			SocialManager.Instance.AddFriendCode("x5r4s4VAj1");
		}

		GUILayout.EndHorizontal();
		#endregion

		#region ParentPortal Test
		GUILayout.BeginHorizontal();
		GUILayout.Label("ParentPortal Test");
		if(GUILayout.Button("Get all kid accounts of current user")){
			ParentPortalManager.Instance.RefreshData();
		}

		GUILayout.EndHorizontal();
		#endregion

		#region Pet Accessory save test
		GUILayout.BeginHorizontal();
		GUILayout.Label("Pet Accessory save Test");
		if(GUILayout.Button("add pet accessory")){
			DataManager.Instance.GameData.Accessories.SetAccessoryAtNode("testnode1", "testItem1" + DateTime.Now);
		}
		if(GUILayout.Button("remove pet accessory")){
			DataManager.Instance.GameData.Accessories.RemoveAccessoryAtNode("testnode1");
		}
		if(GUILayout.Button("save test")){
			DataManager.Instance.GameData.SaveAsyncToParse();
		}
		GUILayout.EndHorizontal();

		#endregion

		if(GUILayout.Button("Logout")){
			ParseUser.LogOut();
		}
	}
	#endif

	private void EventListener(object sender, ServerEventArgs args){
		Debug.Log("IsSuccessful: " + args.IsSuccessful);
		if(!args.IsSuccessful)
			Debug.Log("Error code: " + args.ErrorCode);
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
