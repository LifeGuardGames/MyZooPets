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
	public InternetConnectionDisplay internetConnectionDisplay;
	public GameObject friendEntryPrefab;
	public GameObject friendArea;
	public GameObject grid;
	public GameObject hiddenCode;
	public GameObject buttonCode;
	public GameObject buttonAdd;
	public GameObject buttonRequest;
	public GameObject noFriendsParent;
	public GiftGroupController giftGroupController;

	public TweenToggleDemux deleteFriendTween; 
	public InternetConnectionDisplay deleteFriendConnectionDisplay;
	public UILabel deleteUserLabel;
	public GameObject labelParent;
	private string deleteUserIDAux = string.Empty;

	public TweenToggleDemux codeInputTween;
	public GameObject codeInputTitle;
	public UIInput codeInputInput;
	public InternetConnectionDisplay codeInputConnectionDisplay;
	public UILocalize codeInputErrorLabelLocalize;

	public TweenToggleDemux requestTween;
	public GameObject requestNoRequestsObject;
	public UIGrid requestGrid;
	public InternetConnectionDisplay requestConnectionDisplay;
	public GameObject requestEntryPrefab;

	private bool isActive = false;
	#endregion

	#region Unity MonoBehaviour Functions
	void Awake(){
		eModeType = UIModeTypes.Friends;
	}

	void OnDestroy(){
		SocialManager.OnDataRefreshed -= FinishConnectionUIRefresh;
		SocialManager.OnFriendCodeAdded -= FinishConnectionFriendCodeAdd;
		SocialManager.OnFriendRequestRefreshed -= FinishConnectionRequestRefresh;
	}
	#endregion

	#region Protected Overrides
	protected override void _Start(){
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
			
			// Hide other UI objects
			NavigationUIManager.Instance.HidePanel();
			InventoryUIManager.Instance.HidePanel();
			RoomArrowsUIManager.Instance.HidePanel();
			HUDUIManager.Instance.HidePanel();
			isActive = true;
			
			// Try internet connection
			internetConnectionDisplay.Play("FRIENDS_LOADING");
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
		friendAreaPanel.clipRange = new Vector4(0, oldRange.y, (float)(CameraManager.Instance.GetNativeWidth()), oldRange.w);

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
		giftGroupController.Refresh(SocialManager.Instance.UserSocial.NumOfStars,
		                            SocialManager.Instance.UserSocial.RewardCount);
	}

	#region Refresh Data Handler
	private void FinishConnectionUIRefresh(object sender, ServerEventArgs args){
		if(args.IsSuccessful){
			Debug.Log("Connection Success");
			// Hide the connection display
			internetConnectionDisplay.Stop(true, string.Empty);

			buttonAdd.SetActive(true);
			buttonRequest.SetActive(true);
			buttonCode.SetActive(true);
			giftGroupController.gameObject.SetActive(true);

			//remove any old game objects
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
				string friendColor = "OrangeYellow";
				Hashtable petInitHash = null;
				if(friendPetInfo != null && friendPetInfo.IsDataAvailable){
					friendName = friendPetInfo.Name;
					petInitHash = new Hashtable();
					petInitHash.Add("Color", friendColor);
					// Add more pet info here

				}
				friendEntryController.Initilize(friendName, friendAccount.ObjectId, petInitHash);
			}

			if(friendList.Count == 0){
				noFriendsParent.SetActive(true);
			}else{
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
			Debug.Log("delete user:" + deleteUserID);
			deleteUserIDAux = deleteUserID;	// Cache this
			deleteUserLabel.text = sourceObject.transform.parent.gameObject.GetComponent<FriendEntryController>().FriendName;
			deleteFriendTween.Show();
			labelParent.SetActive(true);
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
		Debug.Log("deleting " + deleteUserIDAux);
		if(!string.IsNullOrEmpty(deleteUserIDAux)){
			SocialManager.Instance.RemoveFriend(deleteUserIDAux);

			labelParent.SetActive(false);
			deleteFriendConnectionDisplay.Play("FRIENDS_DELETE_LOADING");
		}
		else{
			Debug.LogError("FriendId can't be empty");
		}
	}

	public void FinishConnectionDeleteFriendDone(object obj, ServerEventArgs args){
		if(args.IsSuccessful){
			Debug.Log("Connection Success");
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
			SocialManager.Instance.SendFriendRequest(input);
			codeInputConnectionDisplay.Play("FRIENDS_ADD_LOADING");
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
			HandleError(FriendsConnectionType.CodeInput, args);

			codeInputTitle.SetActive(true);
			codeInputInput.gameObject.SetActive(true);
		}
	}
	#endregion

	#region Friend Requests
	public void OpenRequestWindow(){ 
		if(isActive){
			requestTween.Show();
			requestNoRequestsObject.SetActive(false);
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
		requestConnectionDisplay.Play("FRIENDS_REQUESTS_ACCEPT_LOADING");
	}

	public void RequestDecline(string requestId){
		SocialManager.Instance.RejectFriendRequest(requestId);

		requestGrid.gameObject.SetActive(false);
		requestConnectionDisplay.Play("FRIENDS_REQUESTS_DECLINE_LOADING");
	}

	public void FinishConnectionRequestRefresh(object obj, ServerEventArgs args){
		if(args.IsSuccessful){
			requestGrid.gameObject.SetActive(true);

			// Hide the connection display
			requestConnectionDisplay.Stop(true, string.Empty);

			List<SocialManager.FriendRequest> friendRequests = SocialManager.Instance.FriendRequests;
			Debug.Log(friendRequests.Count());

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
		switch(args.ErrorCode){
		case ParseException.ErrorCode.ObjectNotFound:	// Only applies for friends
			codeInputConnectionDisplay.Stop(false, "FRIENDS_ADD_ERROR_INVALID");
			break;
		case ParseException.ErrorCode.DuplicateValue:	// Only applies for friends
			codeInputConnectionDisplay.Stop(false, "FRIENDS_ADD_ERROR_ALREADY_REQUESTED");
			break;
		case ParseException.ErrorCode.ConnectionFailed:
			codeInputConnectionDisplay.Stop(false, "NOTIFICATION_INTERNET_CONNECTION_FAIL");
			break;
		case ParseException.ErrorCode.OtherCause:
			codeInputConnectionDisplay.Stop(false, "NOTIFICATION_INTERNET_ERROR_GENERIC");
			break;
		default:
			codeInputConnectionDisplay.Stop(false, "NOTIFICATION_INTERNET_ERROR_GENERIC");
			Debug.LogWarning("Internet connection untracked error: " + args.ErrorCode.ToString() + " " + args.ErrorMessage);
			break;
		}
	}
}
