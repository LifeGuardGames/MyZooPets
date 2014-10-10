using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Parse;
using System.Threading.Tasks;

public class ParseTestScript : MonoBehaviour {
	public string friendCode = "friend code";
	public string requestId = "request id";
	public string username = "user name";
	public string friendObjectId = "friendObjectId";

	void Start(){
		SocialManager.OnDataRefreshed += EventListener;
		SocialManager.OnFriendCodeAdded += EventListener;
		SocialManager.OnFriendRequestRefreshed += EventListener;
		ParentPortalManager.OnDataRefreshed += EventListener;

	}

	#if UNITY_EDITOR
	public void FakeTutorial(){
		List<string> tutorials = new List<string>(){
			"FOCUS_INHALER", "TUT_SUPERWELLA_INHALER", "FOCUS_WELLAPAD",
			"TUT_SMOKE_INTRO", "TUT_FLAME_CRYSTAL", "TUT_FLAME",
			"TUT_TRIGGERS", "TUT_DECOS"
		};
		
		DataManager.Instance.GameData.Tutorial.ListPlayed.AddRange(tutorials);
		
	}
	#endif

	#if UNITY_EDITOR
	void OnGUI(){
		#region ExtraParseLogic Test
			GUILayout.BeginHorizontal();
			GUILayout.Label("ExtraParseLogic Test");
			if(GUILayout.Button("User check")){
				ExtraParseLogic.Instance.UserCheck();
			}

			GUILayout.EndHorizontal();
		#endregion

		#region SocialManager Test
			GUILayout.BeginHorizontal();
			GUILayout.Label("SocialManager Test");

			GUILayout.BeginVertical();
			if(GUILayout.Button("Get friend list")){
				SocialManager.Instance.RefreshData();
			}
			friendObjectId = GUILayout.TextField(friendObjectId, GUILayout.MinWidth(60));
			if(GUILayout.Button("Remove friend")){
				SocialManager.Instance.RemoveFriend(friendObjectId);
			}
			GUILayout.EndVertical();

			GUILayout.BeginVertical();
			friendCode = GUILayout.TextField(friendCode, GUILayout.MinWidth(60));
			if(GUILayout.Button("Send friend request")){
				SocialManager.Instance.SendFriendRequest(friendCode);
			}
			requestId = GUILayout.TextField(requestId, GUILayout.MinWidth(60));
			if(GUILayout.Button("Accept friend request")){
				SocialManager.Instance.AcceptFriendRequest(requestId);
			}
			if(GUILayout.Button("Get friend requests")){
				SocialManager.Instance.GetFriendRequests();
			}
			
			GUILayout.EndVertical()	;

			GUILayout.EndHorizontal();
		#endregion

		#region ParentPortal / MenuScene Test

			//menu scene
			GUILayout.BeginHorizontal();
				GUILayout.Label("MenuScene Test");
					
				if(GUILayout.Button("Initialize game data")){
					DataManager.Instance.InitializeGameDataForNewPet();
					FakeTutorial();
				}
			GUILayout.EndHorizontal();

			//parent portal
			GUILayout.BeginHorizontal();
			GUILayout.Label("ParentPortal Test");
			if(GUILayout.Button("kid account of current user")){
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
			
			GUILayout.EndHorizontal();
		#endregion

		#region Pet Info save test
		GUILayout.BeginHorizontal();
		GUILayout.Label("Pet Info save Test");
//		if(GUILayout.Button("add pet accessory")){
//			DataManager.Instance.GameData.Accessories.SetAccessoryAtNode("testnode1", "testItem1" + DateTime.Now);
//		}
//		if(GUILayout.Button("remove pet accessory")){
//			DataManager.Instance.GameData.Accessories.RemoveAccessoryAtNode("testnode1");
//		}
		GUILayout.EndHorizontal();
		#endregion
		if(GUILayout.Button("sync to parse test")){
			DataManager.Instance.GameData.SaveAsyncToParse();
		}
		if(GUILayout.Button("Logout")){
			ParseUser.LogOut();
		}

		GUILayout.BeginHorizontal();
		username = GUILayout.TextField(username, GUILayout.MinWidth(150));
		if(GUILayout.Button("login")){
			ParseUser.LogInAsync(username, username).ContinueWith(t => {
				if(t.IsCompleted){
					Debug.Log("log in successful");
				}
			});
		}

		if(GUILayout.Button("create 20 test users")){
			for(int i=0; i<20; i++)
				ExtraParseLogic.Instance.CreateTestUser();
		}
		GUILayout.EndHorizontal();
	}
	#endif

	private void EventListener(object sender, ServerEventArgs args){
		Debug.Log("IsSuccessful: " + args.IsSuccessful);
		if(!args.IsSuccessful){
			Debug.Log("Error code: " + args.ErrorCode);
			Debug.Log("Error message: " + args.ErrorMessage);
		}
	}
}
