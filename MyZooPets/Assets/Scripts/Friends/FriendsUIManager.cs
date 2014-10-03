using UnityEngine;
using System;
using Parse;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class FriendsUIManager : SingletonUI<FriendsUIManager> {

	public UISprite radialFillRewardSprite;

	public InternetConnectionDisplay internetConnectionDisplay;
	public GameObject friendArea;
	public GameObject hiddenCode;
	public GameObject buttonCode;

	private bool isActive = false;

	void Awake(){
		eModeType = UIModeTypes.Friends;
	}

	protected override void _Start(){
		SocialManager.OnDataRefreshed += FinishInternetConnection;

		ToggleCodeButton(false);
	}

	void OnDestroy(){
		SocialManager.OnDataRefreshed -= FinishInternetConnection;
	}

	public void CodeButtonCallback(){
		ToggleCodeButton(true);
	}

	private void ToggleCodeButton(bool isShowCode){
		if(isShowCode){
			buttonCode.SetActive(false);
			hiddenCode.SetActive(true);
		}
		else{
			buttonCode.SetActive(true);
			hiddenCode.SetActive(false);
		}
	}
	
	public void AddFriendCallback(){
		// TODO
	}
		
	public void PreviousPageCallback(){
		// TODO
	}

	public void NextPageCallback(){
		// TODO
	}

	private void RadialFill(float fraction){
		radialFillRewardSprite.fillAmount = fraction;
	}

	public void FinishInternetConnection(object sender, ServerEventArgs args){
		// Valid response
		if(args.IsSuccessful){
			// Hide the connection display
			internetConnectionDisplay.Stop(true, string.Empty);

			List<ParseObject> friendList = SocialManager.Instance.FriendList;
			// TODO Jason fill in the required components here, 6 at a time per page


		}
		// Error state
		else{
			internetConnectionDisplay.Stop(false, "NOTIFICATION_INTERNET_CONNECTION_FAIL");
			Debug.LogWarning(args.ErrorCode.ToString() + " " + args.ErrorMessage);
		}
	}

	private void TryInternetConnection(){
		internetConnectionDisplay.Play("NOTIFICATION_INTERNET_CONNECTION_WAIT");
	}

	protected override void _OpenUI(){
		if(!isActive){
			GetComponent<TweenToggleDemux>().Show();
			TryInternetConnection();
		}
	}

	protected override void _CloseUI(){
		if(isActive){
			GetComponent<TweenToggleDemux>().Hide();
		}
	}

//	void OnGUI(){
//		if(GUI.Button(new Rect(100, 100, 100, 100), "Open")){
//			ToggleCodeButton(true);
//			internetConnectionDisplay.Play("");
//		}
//		if(GUI.Button(new Rect(200, 100, 100, 100), "Close")){
//			ToggleCodeButton(false);
//			internetConnectionDisplay.Stop("");
//		}
//	}
}
