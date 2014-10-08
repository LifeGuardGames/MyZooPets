using UnityEngine;
using System;
using Parse;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class FriendsUIManager : SingletonUI<FriendsUIManager> {
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
	private string deleteUserIDAux = string.Empty;

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
			
			// Destroy all children
			foreach(Transform child in grid.transform) {
				Destroy(child.gameObject);
			}
		}
	}
	#endregion

	void OnDestroy(){
		SocialManager.OnDataRefreshed -= FinishConnectionUIRefresh;
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

	private void FinishConnectionUIRefresh(object sender, ServerEventArgs args){
		if(args.IsSuccessful){
			Debug.Log("Connection Success");
			// Hide the connection display
			internetConnectionDisplay.Stop(true, string.Empty);

			buttonAdd.SetActive(true);
			buttonRequest.SetActive(true);
			buttonCode.SetActive(true);
			giftGroupController.gameObject.SetActive(true);

			foreach(Transform child in grid.transform){
				child.gameObject.SetActive(false);
				Destroy(child.gameObject);
			}

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

			hiddenCode.transform.FindChild("LabelCode").GetComponent<UILabel>().text = SocialManager.Instance.AccountCode;

			// Reposition the grid
			grid.GetComponent<UIGrid>().Reposition();

			// No friends! show no friend message
			if(friendList.Count() == 0){
				noFriendsParent.SetActive(true);
			}

			RefreshGiftReward();
		}
		else{
			// Custom errors that we handle
			if(args.ErrorCode == ParseException.ErrorCode.ConnectionFailed){
				// Pass the error message directly as localize key
				codeInputConnectionDisplay.Stop(false, args.ErrorMessage);
				Debug.LogWarning(args.ErrorCode.ToString() + " " + args.ErrorMessage);
			}
			// Untracked errors, show generic error
			else{
				codeInputConnectionDisplay.Stop(false, "NOTIFICATION_INTERNET_ERROR_GENERIC");
				Debug.LogWarning("Internet connection untracked error: " + args.ErrorCode.ToString() + " " + args.ErrorMessage);
			}
		}
	}

	public void RefreshGiftReward(){
		giftGroupController.Refresh(1, 0);
	}

	#region Delete Friend
	public void OpenDeleteFriendWindowCallback(GameObject sourceObject){
		if(isActive){
			string deleteUserID = sourceObject.transform.parent.name;
			deleteUserIDAux = deleteUserID;	// Cache this
			deleteUserLabel.text = deleteUserID;
			deleteFriendTween.Show();
		}
	}

	public void CloseDeleteFriendWindow(){
		if(isActive){
			deleteFriendTween.Hide();
			deleteUserIDAux = string.Empty;	// Clear cache
		}
	}

	public void DeleteFriendCallback(){
		Debug.Log("deleting " + deleteUserIDAux);
		//TODO do delete here
	}

	public void FinishConnectionDeleteFriendDone(object obj, ServerEventArgs args){
		if(args.IsSuccessful){
			Debug.Log("Connection Success");
			// Hide the connection display
			deleteFriendConnectionDisplay.Stop(true, string.Empty);

			CloseDeleteFriendWindow();
			
			foreach(Transform child in grid.transform){
				child.gameObject.SetActive(false);
				Destroy(child.gameObject);
			}
			
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

			// Reposition the grid
			grid.GetComponent<UIGrid>().Reposition();
			
			// No friends! show no friend message
			if(friendList.Count() == 0){
				noFriendsParent.SetActive(true);
			}
		}
		else{
			// Custom errors that we handle
			if(args.ErrorCode == ParseException.ErrorCode.ConnectionFailed){
				// Pass the error message directly as localize key
				deleteFriendConnectionDisplay.Stop(false, args.ErrorMessage);
				Debug.LogWarning(args.ErrorCode.ToString() + " " + args.ErrorMessage);
			}
			// Untracked errors, show generic error
			else{
				deleteFriendConnectionDisplay.Stop(false, "NOTIFICATION_INTERNET_ERROR_GENERIC");
				Debug.LogWarning("Internet connection untracked error: " + args.ErrorCode.ToString() + " " + args.ErrorMessage);
			}
		}
	}
	#endregion

	#region Code Input
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
			// Custom errors that we handle
			if(args.ErrorCode == ParseException.ErrorCode.ObjectNotFound ||
			   args.ErrorCode == ParseException.ErrorCode.ConnectionFailed ||
			   args.ErrorCode == ParseException.ErrorCode.DuplicateValue ||
			   args.ErrorCode == ParseException.ErrorCode.InternalServerError){
				// Pass the error message directly as localize key
				codeInputConnectionDisplay.Stop(false, args.ErrorMessage);
				Debug.LogWarning(args.ErrorCode.ToString() + " " + args.ErrorMessage);
			}
			// Untracked errors, show generic error
			else{
				codeInputConnectionDisplay.Stop(false, "NOTIFICATION_INTERNET_ERROR_GENERIC");
				Debug.LogWarning("Internet connection untracked error: " + args.ErrorCode.ToString() + " " + args.ErrorMessage);
			}
			codeInputTitle.SetActive(true);
			codeInputInput.gameObject.SetActive(true);
		}
	}
	#endregion

	#region Friend Requests
	public void OpenRequestWindow(){
		if(isActive){
			requestTween.Show();
			requestConnectionDisplay.Play("NOTIFICATION_INTERNET_CONNECTION_WAIT");
			SocialManager.Instance.GetFriendRequests();
		}
	}
	
	public void CloseRequestWindow(){
		if(isActive){
			requestTween.Hide();
		}
	}

	public void RequestAccept(string requestId){
		SocialManager.Instance.AcceptFriendRequest(requestId);
	}

	public void RequestDecline(string requestId){
		SocialManager.Instance.RejectFriendRequest(requestId);
	}

	public void FinishConnectionRequestRefresh(object obj, ServerEventArgs args){
		if(args.IsSuccessful){
			// Hide the connection display
			codeInputConnectionDisplay.Stop(true, string.Empty);

			List<SocialManager.FriendRequest> friendRequests = SocialManager.Instance.FriendRequests;
			Debug.Log(friendRequests.Count());
			foreach(Transform child in requestGrid.transform){
				child.gameObject.SetActive(false);
				Destroy(child.gameObject);
			}
			
			foreach(SocialManager.FriendRequest request in friendRequests){
				GameObject requestObject = NGUITools.AddChild(requestGrid.gameObject, requestEntryPrefab);
				RequestEntryController requestEntryController = requestObject.GetComponent<RequestEntryController>();
				requestEntryController.Initialize(request.RequestId, request.FriendName);
			}
			
			requestGrid.Reposition();
		}
		else{
			// Custom errors that we handle
			if(args.ErrorCode == ParseException.ErrorCode.ObjectNotFound ||
			   args.ErrorCode == ParseException.ErrorCode.ConnectionFailed ||
			   args.ErrorCode == ParseException.ErrorCode.InternalServerError){
				// Pass the error message directly as localize key
				codeInputConnectionDisplay.Stop(false, args.ErrorMessage);
				Debug.LogWarning(args.ErrorCode.ToString() + " " + args.ErrorMessage);
			}
			// Untracked errors, show generic error
			else{
				codeInputConnectionDisplay.Stop(false, "NOTIFICATION_INTERNET_ERROR_GENERIC");
				Debug.LogWarning("Internet connection untracked error: " + args.ErrorCode.ToString() + " " + args.ErrorMessage);
			}
		}
	}
	#endregion

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
