using UnityEngine;
using System;
using Parse;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class FriendsUIManager : SingletonUI<FriendsUIManager> {

	public UISprite radialFillRewardSprite;

	public InternetConnectionDisplay internetConnectionDisplay;
	public GameObject friendEntryPrefab;
	public GameObject friendArea;
	public GameObject grid;
	public GameObject hiddenCode;
	public GameObject buttonCode;
	public GameObject buttonAdd;
	public GameObject buttonRequest;
	public GameObject giftGroup;

	public TweenToggleDemux codeInputTween;
	public GameObject codeInputTitle;
	public UIInput codeInputInput;
	public InternetConnectionDisplay codeInputConnectionDisplay;
	public UILocalize codeInputErrorLabelLocalize;

	public TweenToggleDemux requestTween;
	public UIGrid requestGrid;
	public InternetConnectionDisplay requestConnectionDisplay;
	public GameObject requestEntryPrefab;

	private bool isActive = false;

	void Awake(){
		eModeType = UIModeTypes.Friends;
	}

	protected override void _Start(){
		SocialManager.OnDataRefreshed += FinishConnectionUIOpen;
		SocialManager.OnFriendCodeAdded += FinishConnectionFriendCodeAdd;
		SocialManager.OnFriendRequestRefreshed += FinishConnectionRequestRefresh;

		ToggleCodeButton(false);
		RepositionGridBorders();
	}

	void OnDestroy(){
		SocialManager.OnDataRefreshed -= FinishConnectionUIOpen;
		SocialManager.OnFriendCodeAdded -= FinishConnectionFriendCodeAdd;
		SocialManager.OnFriendRequestRefreshed -= FinishConnectionRequestRefresh;
	}

	// Reposition all the things nicely to stretch to the end of the screen
	private void RepositionGridBorders(){
		
		// Position the UIPanel clipping range
		UIPanel friendAreaPanel = friendArea.GetComponent<UIPanel>();
		Vector4 oldRange = friendAreaPanel.clipRange;
		friendAreaPanel.transform.localPosition = new Vector3(0, friendAreaPanel.transform.localPosition.y, 0f);
		friendAreaPanel.clipRange = new Vector4(0, oldRange.y, (float)(CameraManager.Instance.GetNativeWidth()), oldRange.w);
		
		// Position the grid origin to the left of the screen
//		Vector3 gridPosition = grid.transform.localPosition;
		grid.transform.localPosition = new Vector3(0f, 0f, 0f);
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

//	private void RadialFill(float fraction){
//		radialFillRewardSprite.fillAmount = fraction;
//	}

	public void FinishConnectionUIOpen(object sender, ServerEventArgs args){
		if(args.IsSuccessful){
			Debug.Log("Connection Success");
			// Hide the connection display
			internetConnectionDisplay.Stop(true, string.Empty);

			buttonAdd.SetActive(true);
			buttonRequest.SetActive(true);
			buttonCode.SetActive(true);
			giftGroup.SetActive(true);

			List<ParseObjectKidAccount> friendList = SocialManager.Instance.FriendList;
			foreach(ParseObjectKidAccount friendAccount in friendList){
				Debug.Log("initiating friend");
				GameObject friendObject = NGUITools.AddChild(grid, friendEntryPrefab);
				FriendEntryController friendEntryController = friendObject.GetComponent<FriendEntryController>();

				// TODO rename friendObject to friendACcount.ObjectId

				ParseObjectPetInfo friendPetInfo = friendAccount.PetInfo;
				if(friendPetInfo != null && friendPetInfo.IsDataAvailable){
					// TODO create the pet into hashtable down the road and pass in here v
					friendEntryController.Initilize(friendPetInfo.Name, null);
				}
			}
			grid.GetComponent<UIGrid>().Reposition();
		}
		else{
			// Check for errorcode first then erromessage. only OtherCause
			internetConnectionDisplay.Stop(false, "NOTIFICATION_INTERNET_CONNECTION_FAIL");
			Debug.LogWarning(args.ErrorCode.ToString() + " " + args.ErrorMessage);
		}
	}

	protected override void _OpenUI(){
		if(!isActive){
			GetComponent<TweenToggleDemux>().Show();

			buttonAdd.SetActive(false);
			buttonRequest.SetActive(false);
			buttonCode.SetActive(false);
			giftGroup.SetActive(false);

			// Hide other UI objects
			NavigationUIManager.Instance.HidePanel();
			InventoryUIManager.Instance.HidePanel();
			RoomArrowsUIManager.Instance.HidePanel();
			HUDUIManager.Instance.HidePanel();
			isActive = true;

			// Try internet connection
			internetConnectionDisplay.Play("NOTIFICATION_INTERNET_CONNECTION_WAIT");
			Debug.Log("trying connection");
			SocialManager.Instance.RefreshData();

			Debug.Log("opening ui");
		}
	}

	protected override void _CloseUI(){
		if(isActive){
			GetComponent<TweenToggleDemux>().Hide();

			// Show other UI Objects
			NavigationUIManager.Instance.ShowPanel();
			InventoryUIManager.Instance.ShowPanel();
			RoomArrowsUIManager.Instance.ShowPanel();
			HUDUIManager.Instance.ShowPanel();

			isActive = false;
		}
	}

	//////////////// Code Input ////////////////////////////

	public void OpenCodeInputWindow(){
		if(isActive){
			codeInputTween.Show();
		}
	}

	public void CloseCodeInputWindow(){
		if(isActive){
			codeInputTween.Hide();
		}
	}

	public void CodeInputSubmitButton(){
		string input = codeInputInput.text;
		if(input == string.Empty){
			// Show blank input error
			codeInputConnectionDisplay.Stop(false, "FRIENDS_ADD_FRIEND_ERROR_EMPTY_INPUT");
		}
		else{
			codeInputTitle.SetActive(false);
			codeInputInput.gameObject.SetActive(false);
			SocialManager.Instance.SendFriendRequest(input);
			codeInputConnectionDisplay.Play("NOTIFICATION_INTERNET_CONNECTION_WAIT");
		}
	}

	public void FinishConnectionFriendCodeAdd(object obj, ServerEventArgs args){
		if(args.IsSuccessful){
			Debug.Log("friend add Connection Success");
			// Hide the connection display
			codeInputConnectionDisplay.Stop(true, string.Empty);

			CloseCodeInputWindow();
		}
		else{
			// TODO add custom error handling
			if(true){
				codeInputConnectionDisplay.Stop(false, "NOTIFICATION_INTERNET_CONNECTION_FAIL");
			}
			else if(true){
				codeInputConnectionDisplay.Stop(false, "NOTIFICATION_INTERNET_CONNECTION_FAIL");
			}
			else if(true){
				codeInputConnectionDisplay.Stop(false, "NOTIFICATION_INTERNET_CONNECTION_FAIL");
			}

			Debug.LogWarning(args.ErrorCode.ToString() + " " + args.ErrorMessage);

			codeInputTitle.SetActive(true);
			codeInputInput.gameObject.SetActive(true);
		}
	}

	//////////////// Friend Requests ////////////////////////

	public void OpenRequestWindow(){
		if(isActive){
			requestTween.Show();
		}
	}
	
	public void CloseRequestWindow(){
		if(isActive){
			requestTween.Hide();
		}
	}

	public void RequestAccept(string id){
		// TODO
	}

	public void RequestDecline(string id){
		// TODO
	}

	public void FinishConnectionRequestRefresh(object obj, ServerEventArgs args){
		if(args.IsSuccessful){
			Debug.Log("Request List Connection Success");
			// Hide the connection display
			requestConnectionDisplay.Stop(true, string.Empty);

			// TODO Refresh list here
//			List<ParseObjectKidAccount> friendList = SocialManager.Instance.FriendList;
//			
//			foreach(ParseObjectKidAccount friendAccount in friendList){
//				Debug.Log("initiating request");
//				GameObject requestObject = NGUITools.AddChild(grid, requestGrid);
//				RequestEntryController requestEntryController = requestObject.GetComponent<RequestEntryController>();
//				ParseObjectPetInfo friendPetInfo = friendAccount.PetInfo;
//				if(friendPetInfo != null && friendPetInfo.IsDataAvailable){
//					// TODO create the pet into hashtable down the road and pass in here v
//					requestEntryController.Initialize(friendPetInfo.Name);
//				}
//			}
			requestGrid.Reposition();
		}
		else{
			codeInputConnectionDisplay.Stop(false, "NOTIFICATION_INTERNET_CONNECTION_FAIL");
			Debug.LogWarning(args.ErrorCode.ToString() + " " + args.ErrorMessage);
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
