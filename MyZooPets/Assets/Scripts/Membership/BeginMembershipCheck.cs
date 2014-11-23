using UnityEngine;
using System.Collections;

/// <summary>
/// Begin membership check.
/// This script is attached to MembershipCheck gameobject. It's main purpose is
/// to activate membership check when appropriate. Membership check only happens 
/// when the game is in LoadingScene.unity. 
/// This class 
/// </summary>
public class BeginMembershipCheck : MonoBehaviour {

	// Use this for initialization
	void Start(){
		//remove unwanted loading screen carry over from other scenes
		GameObject loadingScreen = GameObject.Find("UI Root LoadingScreen");
		if(loadingScreen != null){
			loadingScreen.SetActive(false);
			Destroy(loadingScreen);
		}
		
		Debug.Log(Application.loadedLevelName);
		MembershipCheck.Instance.StartCheck();
	}
}
