using UnityEngine;
using System;
using Parse;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class FriendsUIManager : SingletonUI<FriendsUIManager> {

	public InternetConnectionDisplay internetConnectionDisplay;
	public UISprite radialFillRewardSprite;

	private bool isActive = false;

	void Awake(){
		eModeType = UIModeTypes.Friends;
	}

	void Start(){
		SocialManager.OnDataRefreshed += FinishInternetConnection;
	}

	void OnDestroy(){
		SocialManager.OnDataRefreshed -= FinishInternetConnection;
	}

	public void FinishInternetConnection(object sender, ServerEventArgs args){
		// Valid response
		if(args.IsSuccessful){
			List<ParseObject> friendList = SocialManager.Instance.FriendList;
			// TODO Jason fill in the required components here, 6 at a time per page


		}
		// Error state
		else{
			internetConnectionDisplay.Stop("NOTIFICATION_INTERNET_CONNECTION_FAIL");
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
}
