using UnityEngine;
using System;
using Parse;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public enum FriendsConnectionType{
	BaseUI, CodeInput, Request, Delete,
}

public class FriendsUIManager : SingletonUI<FriendsUIManager> {
	#region Variables
	public InternetConnectionDisplay baseConnectionDisplay;
	public GameObject friendEntryPrefab;
	public GameObject friendArea;
	public GameObject grid;
	public GameObject hiddenCode;
	public GameObject buttonCode;
	public GameObject buttonAdd;
	public GameObject buttonRequest;
	public GameObject noFriendsParent;
	public GiftGroupController giftGroupController;
	public UILabel friendsCount;

	public TweenToggleDemux deleteFriendTween; 
	public GameObject deleteExitButton;
	public InternetConnectionDisplay deleteFriendConnectionDisplay;
	public UILabel deleteUserLabel;
	public GameObject deleteContentParent;
	private string deleteUserIDAux = string.Empty;

	public TweenToggleDemux codeInputTween;
	public GameObject codeInputExitButton;
	public GameObject codeInputOkButton;
	public GameObject codeInputTitle;
	public UIInput codeInputInput;
	public InternetConnectionDisplay codeInputConnectionDisplay;
	public UILocalize codeInputErrorLabelLocalize;

	public TweenToggleDemux requestTween;
	public GameObject requestExitButton;
	public GameObject requestNoRequestsObject;
	public UIGrid requestGrid;
	public InternetConnectionDisplay requestConnectionDisplay;
	public GameObject requestEntryPrefab;
	public EntranceHelperController entranceHelper;

	private bool isActive = false;
	#endregion

	#region Protected Overrides
	protected override void Awake(){
		base.Awake();
		eModeType = UIModeTypes.Friends;
	}
	
	protected override void OnDestroy(){
		base.OnDestroy();
		SocialManager.OnDataRefreshed -= FinishConnectionUIRefresh;
		SocialManager.OnFriendCodeAdded -= FinishConnectionFriendCodeAdd;
		SocialManager.OnFriendRequestRefreshed -= FinishConnectionRequestRefresh;
	}

	protected override void Start(){
		base.Start();
		SocialManager.OnDataRefreshed += FinishConnectionUIRefresh;
		SocialManager.OnFriendCodeAdded += FinishConnectionFriendCodeAdd;
		SocialManager.OnFriendRequestRefreshed += FinishConnectionRequestRefresh;

		noFriendsParent.SetActive(false);
		ToggleCodeButton(false);
		RepositionGridBorders();
	}

	protected override void _OpenUI(){
		if(!isActive){
			GetComponent<TweenToggleDemux>().Show();
			
			ToggleCodeButton(false);
			buttonAdd.SetActive(false);
			buttonRequest.SetActive(false);
			buttonCode.SetActive(false);
			noFriendsParent.SetActive(false);
			giftGroupController.gameObject.SetActive(false);
			friendsCount.gameObject.SetActive(false);

			// Enable all the sub exit buttons on open base
			codeInputExitButton.SetActive(true);
			deleteExitButton.SetActive(true);
			requestExitButton.SetActive(true);
			
			// Hide other UI objects
			NavigationUIManager.Instance.HidePanel();
			InventoryUIManager.Instance.HidePanel();
			RoomArrowsUIManager.Instance.HidePanel();
			HUDUIManager.Instance.HidePanel();
			isActive = true;
			PetAudioManager.Instance.EnableSound = false;

			Analytics.Instance.EnterFriendTree();
			entranceHelper.EntranceUsed();
			
			// Try internet connection
			baseConnectionDisplay.Play("FRIENDS_LOADING");
			SocialManager.Instance.RefreshData();
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
			PetAudioManager.Instance.EnableSound = true;
			
			// Destroy all children
			foreach(Transform child in grid.transform) {
				Destroy(child.gameObject);
			}
		}
	}
	#endregion

	// Reposition all the things nicely to stretch to the end of the screen
	private void RepositionGridBorders(){
		// Position the UIPanel clipping range
		UIPanel friendAreaPanel = friendArea.GetComponent<UIPanel>();
		Vector4 oldRange = friendAreaPanel.clipRange;
		friendAreaPanel.transform.localPosition = new Vector3(0, friendAreaPanel.transform.localPosition.y, 0f);
		friendAreaPanel.clipRange = new Vector4(0, oldRange.y, (float)(CameraManager.Instance.NativeWidth), oldRange.w);

		// Position the grid origin to the left of the screen
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

	public void RefreshGiftReward(){
		giftGroupController.Refresh(SocialManager.Instance.UserSocial.NumOfStars, SocialManager.Instance.UserSocial.RewardCount);
	}

	#region Refresh Data Handler
	private void FinishConnectionUIRefresh(object sender, ServerEventArgs args){
		if(args.IsSuccessful){
			// Hide the connection display
			baseConnectionDisplay.Stop(true, string.Empty);

			buttonAdd.SetActive(true);
			buttonRequest.SetActive(true);
			buttonCode.SetActive(true);
			giftGroupController.gameObject.SetActive(true);

			// Remove any old game objects
			foreach(Transform child in grid.transform){
				child.gameObject.SetActive(false);
				Destroy(child.gameObject);
			}

			List<ParseObjectKidAccount> friendList = SocialManager.Instance.FriendList;
			//Initiating friends
			foreach(ParseObjectKidAccount friendAccount in friendList){
				GameObject friendObject = NGUITools.AddChild(grid, friendEntryPrefab);
				FriendEntryController friendEntryController = friendObject.GetComponent<FriendEntryController>();

				ParseObjectPetInfo friendPetInfo = friendAccount.PetInfo;
				string friendName = "";
				string friendColor = PetColor.OrangeYellow.ToString();	// Set default
				Hashtable petInitHash = null;
				if(friendPetInfo != null && friendPetInfo.IsDataAvailable){
					friendName = friendPetInfo.Name;
					friendColor = friendPetInfo.Color;
					petInitHash = new Hashtable();
					petInitHash.Add("Color", friendColor);
					// Add more pet info here

					//
				}
				friendEntryController.Initilize(friendName, friendAccount.ObjectId, petInitHash);
			}

			if(friendList.Count == 0){
				friendsCount.gameObject.SetActive(false);
				noFriendsParent.SetActive(true);
			}
			else{
				friendsCount.gameObject.SetActive(true);
				friendsCount.text = friendList.Count + "/40";
				noFriendsParent.SetActive(false);
			}

			// Assign user friend code
			hiddenCode.transform.FindChild("LabelCode").GetComponent<UILabel>().text = SocialManager.Instance.AccountCode;

			// Reposition the grid
			grid.GetComponent<UIGrid>().Reposition();

			// No friends! show no friend message
			if(friendList.Count() == 0){
				noFriendsParent.SetActive(true);
			}
			RefreshGiftReward();
			ToggleCodeButton(false);
		}
		else{
			HandleError(FriendsConnectionType.BaseUI, args);
		}
	}
	#endregion

	#region Delete Friend
	public void OpenDeleteFriendWindowCallback(GameObject sourceObject){
		if(isActive){
			// Assigning another event listener to base dataRefresh
			// We want to refresh base along with handling popup connection display
			SocialManager.OnFriendRemoved += FinishConnectionDeleteFriendDone;

			string deleteUserID = sourceObject.transform.parent.gameObject.GetComponent<FriendEntryController>().FriendID;
//			Debug.Log("delete user:" + deleteUserID);
			deleteUserIDAux = deleteUserID;	// Cache this
			deleteUserLabel.text = sourceObject.transform.parent.gameObject.GetComponent<FriendEntryController>().FriendName;
			deleteFriendTween.Show();
			deleteContentParent.SetActive(true);
			deleteFriendConnectionDisplay.Reset();
		}
	}

	public void CloseDeleteFriendWindow(){
		if(isActive){
			// Unassigning temporary listener
			SocialManager.OnFriendRemoved -= FinishConnectionDeleteFriendDone;

			deleteFriendTween.Hide();
			deleteUserIDAux = string.Empty;	// Clear cache
			deleteFriendConnectionDisplay.Reset();
		}
	}

	public void DeleteFriendCallback(){
//		Debug.Log("deleting " + deleteUserIDAux);
		if(!string.IsNullOrEmpty(deleteUserIDAux)){
			deleteContentParent.SetActive(false);

			SocialManager.Instance.RemoveFriend(deleteUserIDAux);
			deleteFriendConnectionDisplay.Play("FRIENDS_DELETE_LOADING");
		}
		else{
			Debug.LogError("FriendId can't be empty");
		}
	}

	public void FinishConnectionDeleteFriendDone(object obj, ServerEventArgs args){
		if(args.IsSuccessful){
			// Hide the connection display
			deleteFriendConnectionDisplay.Stop(true, string.Empty);

			CloseDeleteFriendWindow();
		}
		else{
			HandleError(FriendsConnectionType.Delete, args);
		}
	}
	#endregion

	#region Add Friend Code
	public void OpenCodeInputWindow(){
		if(isActive){
			codeInputTween.Show();
			codeInputTitle.SetActive(true);
			codeInputInput.gameObject.SetActive(true);
			codeInputInput.text = "";
			codeInputOkButton.SetActive(true);
			codeInputExitButton.SetActive(true);
			codeInputConnectionDisplay.Reset();
		}
	}

	public void CloseCodeInputWindow(){
		if(isActive){
			codeInputTween.Hide();
			codeInputConnectionDisplay.Reset();
		}
	}

	public void CodeInputSubmitButton(){
		string input = codeInputInput.text;
		if(input == string.Empty){
			// Show blank input error
			codeInputConnectionDisplay.Stop(false, "FRIENDS_ADD_ERROR_EMPTY_INPUT");
		}
		else{
			codeInputTitle.SetActive(false);
			codeInputInput.gameObject.SetActive(false);
			codeInputExitButton.SetActive(false);
			codeInputOkButton.SetActive(false);

			SocialManager.Instance.SendFriendRequest(input);
			codeInputConnectionDisplay.Play("FRIENDS_ADD_LOADING");
		}
		Analytics.Instance.AddFriend();
	}

	public void FinishConnectionFriendCodeAdd(object obj, ServerEventArgs args){
		codeInputExitButton.SetActive(true);
		if(args.IsSuccessful){
			// Hide the connection display
			codeInputConnectionDisplay.Stop(true, string.Empty);

			CloseCodeInputWindow();
		}
		else{
			HandleError(FriendsConnectionType.CodeInput, args);

			codeInputTitle.SetActive(true);
			codeInputOkButton.SetActive(true);
			codeInputInput.gameObject.SetActive(true);
		}
	}
	#endregion

	#region Friend Requests
	public void OpenRequestWindow(){ 
		if(isActive){
			requestTween.Show();
			requestNoRequestsObject.SetActive(false);
			requestExitButton.SetActive(false);
			requestConnectionDisplay.Play("FRIENDS_REQUESTS_LOADING");
			SocialManager.Instance.GetFriendRequests();
		}
	}
	
	public void CloseRequestWindow(){
		if(isActive){
			requestTween.Hide();
			foreach(Transform child in requestGrid.transform){
				child.gameObject.SetActive(false);
				Destroy(child.gameObject);
			}
			requestConnectionDisplay.Reset();
		}
	}

	public void RequestAccept(string requestId){
		SocialManager.Instance.AcceptFriendRequest(requestId);

		requestGrid.gameObject.SetActive(false);
		requestExitButton.SetActive(false);
		requestConnectionDisplay.Play("FRIENDS_REQUESTS_ACCEPT_LOADING");

		Analytics.Instance.AcceptFriendRequest();
	}

	public void RequestDecline(string requestId){
		SocialManager.Instance.RejectFriendRequest(requestId);

		requestGrid.gameObject.SetActive(false);
		requestExitButton.SetActive(false);
		requestConnectionDisplay.Play("FRIENDS_REQUESTS_DECLINE_LOADING");
	}

	public void FinishConnectionRequestRefresh(object obj, ServerEventArgs args){
		requestExitButton.SetActive(true);
		if(args.IsSuccessful){
			requestGrid.gameObject.SetActive(true);

			// Hide the connection display
			requestConnectionDisplay.Stop(true, string.Empty);

			List<SocialManager.FriendRequest> friendRequests = SocialManager.Instance.FriendRequests;
//			Debug.Log(friendRequests.Count());

			//remove existing objects
			foreach(Transform child in requestGrid.transform){
				child.gameObject.SetActive(false);
				Destroy(child.gameObject);
			}

			if(friendRequests.Count > 0){
				requestNoRequestsObject.SetActive(false);
				foreach(SocialManager.FriendRequest request in friendRequests){
					GameObject requestObject = NGUITools.AddChild(requestGrid.gameObject, requestEntryPrefab);
					RequestEntryController requestEntryController = requestObject.GetComponent<RequestEntryController>();
					requestEntryController.Initialize(request.RequestId, request.FriendName);
				}
			}
			else{
				requestNoRequestsObject.SetActive(true);
			}
			 
			requestGrid.Reposition();
		}
		else{
			HandleError(FriendsConnectionType.Request, args);
		}
	}
	#endregion

	public void HandleError(FriendsConnectionType source, ServerEventArgs args){
		Debug.LogWarning(args.ErrorCode.ToString() + " " + args.ErrorMessage);

		string errorKey;
		switch(args.ErrorCode){
		case ParseException.ErrorCode.ObjectNotFound:	// Only applies for friends
			errorKey = "FRIENDS_ADD_ERROR_INVALID";
			break;
		case ParseException.ErrorCode.DuplicateValue:	// Only applies for friends
			errorKey = "FRIENDS_ADD_ERROR_ALREADY_REQUESTED";
			break;
		case ParseException.ErrorCode.ConnectionFailed:
			errorKey = "NOTIFICATION_INTERNET_CONNECTION_FAIL";
			break;
		case ParseException.ErrorCode.OtherCause:
			errorKey = "NOTIFICATION_INTERNET_ERROR_GENERIC";
			break;
		default:
			errorKey = "NOTIFICATION_INTERNET_ERROR_GENERIC";
			Debug.LogWarning("Internet connection untracked error: " + args.ErrorCode.ToString() + " " + args.ErrorMessage);
			break;
		}

		switch(source){
		case FriendsConnectionType.BaseUI:
			baseConnectionDisplay.Stop(false, errorKey);
			break;
		case FriendsConnectionType.CodeInput:
			codeInputConnectionDisplay.Stop(false, errorKey);
			break;
		case FriendsConnectionType.Delete:
			deleteFriendConnectionDisplay.Stop(false, errorKey);
			break;
		case FriendsConnectionType.Request:
			requestConnectionDisplay.Stop(false, errorKey);
			break;
		}
	}
}
