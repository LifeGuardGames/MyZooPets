using UnityEngine;
using System.Collections;

/// <summary>
/// Analytics for facebook SDK
/// We need to use this for facebook install tracking from advertisements
/// </summary>
public class AnalyticsFacebook : MonoBehaviour{
	void Start(){
		// Start facebook SDK
		//FB.Init(OnFBInitComplete);	// TODO Remove when needed
	}
	
	private void OnFBInitComplete(){
		//Debug.Log("Init FB complete");
		// Tell facebook that this is the first install
		//FB.ActivateApp();				// TODO Remove when needed
	}
}