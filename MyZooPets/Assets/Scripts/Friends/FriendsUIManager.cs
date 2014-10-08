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
	public GameObject noFriendsParent;
//	public GameObject giftGroup;

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
		SocialManager.OnDataRefreshed += FinishConnectionUIRefresh;
		SocialManager.OnFriendCodeAdded += FinishConnectionFriendCodeAdd;
		SocialManager.OnFriendRequestRefreshed += FinishConnectionRequestRefresh;

		noFriendsParent.SetActive(false);
		ToggleCodeButton(false);
		RepositionGridBorders();
	}

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

	private void FinishConnectionUIRefresh(object sender, ServerEventArgs args){
		if(args.IsSuccessful){
			Debug.Log("Connection Success");
			// Hide the connection display
			internetConnectionDisplay.Stop(true, string.Empty);

			buttonAdd.SetActive(true);
			buttonRequest.SetActive(true);
			buttonCode.SetActive(true);
//			giftGroup.SetActive(true);

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
//			GameObjectUtils.ResetLocalPosition(friendArea);
//			GameObjectUtils.ResetLocalPosition(grid);
			grid.GetComponent<UIGrid>().Reposition();

			// No friends! show no friend message
			if(friendList.Count() == 0){
				noFriendsParent.SetActive(true);
			}
		}
		else{
			// Check for errorcode first then erromessage. only OtherCause
			internetConnectionDisplay.Stop(false, "NOTIFICATION_INTERNET_CONNECTION_FAIL");
			Debug.LogWarning(args.ErrorCode.ToString() + " " + args.ErrorMessage);
		}
	}

	#region Protected Overrides
	protected override void _OpenUI(){
		if(!isActive){
			GetComponent<TweenToggleDemux>().Show();

			ToggleCodeButton(false);
			buttonAdd.SetActive(false);
			buttonRequest.SetActive(false);
			buttonCode.SetActive(false);
			noFriendsParent.SetActive(false);
//			giftGroup.SetActive(false);

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
			string errorMessage = "NOTIFICATION_INTERNET_CONNECTION_FAIL";
			switch(args.ErrorCode){
			case ParseException.ErrorCode.OtherCause:
				break;
			case ParseException.ErrorCode.ObjectNotFound:
				errorMessage = "FRIENDS_ADD_FRIEND_ERROR_INVALID";
				break;
			case ParseException.ErrorCode.ConnectionFailed:
				errorMessage = "NOTIFICATION_INTERNET_CONNECTION_FAIL";
				break;
			case ParseException.ErrorCode.DuplicateValue:
//				errorMessage = ""
			case ParseException.ErrorCode.InternalServerError:
				break;
			default:
				break;
			}

//			FRIENDS_ADD_FRIEND_ERROR_EMPTY_INPUT = No friend code entered!
//				FRIENDS_ADD_FRIEND_ERROR_ALREADY_HAVE_FRIEND = You already have this friend!
//					FRIENDS_ADD_FRIEND_ERROR_INVALID = Double check your friend code!
//					FRIENDS_ADD_FRIEND_ERROR_ALREADY_REQUESTED = Friend invite already sent!
//					FRIENDS_ADD_FRIEND_ERROR_OWN_CODE = Can't add your own code!
	
			codeInputConnectionDisplay.Stop(false, "NOTIFICATION_INTERNET_CONNECTION_FAIL");

			Debug.LogWarning(args.ErrorCode.ToString() + " " + args.ErrorMessage);

			codeInputTitle.SetActive(true);
			codeInputInput.gameObject.SetActive(true);
		}
	}
	#endregion

	#region Friend Requests
	public void OpenRequestWindow(){
		if(isActive){
			requestTween.Show();
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
		// Refresh the UI with the same arguments as a openUI refresh
//		FinishConnectionUIRefresh(obj, args);

		if(args.IsSuccessful){
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
			switch(args.ErrorCode){
			case ParseException.ErrorCode.OtherCause:
				break;
			case ParseException.ErrorCode.ObjectNotFound:
				break;
			case ParseException.ErrorCode.ConnectionFailed:
				break;
			case ParseException.ErrorCode.InternalServerError:
				break;
			default:
				break;
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
