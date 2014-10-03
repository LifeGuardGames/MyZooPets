using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Parse;
using System.Threading.Tasks;

public class ParseTestScript : MonoBehaviour {
	public string friendCode = "friend code";
	void Start(){
		SocialManager.OnDataRefreshed += EventListener;
		SocialManager.OnFriendCodeAdded += EventListener;
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
			if(GUILayout.Button("Get friend list")){
				SocialManager.Instance.RefreshData();
			}

			friendCode = GUILayout.TextField(friendCode, GUILayout.MinWidth(60));
			if(GUILayout.Button("Add good friend code")){
			SocialManager.Instance.AddFriendCode(friendCode);
			}
			if(GUILayout.Button("Add bad friend code")){
				SocialManager.Instance.AddFriendCode("x5r4s4VAj1");
			}

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
	}
	#endif

	private void EventListener(object sender, ServerEventArgs args){
		Debug.Log("IsSuccessful: " + args.IsSuccessful);
		if(!args.IsSuccessful)
			Debug.Log("Error code: " + args.ErrorCode);
	}
}
