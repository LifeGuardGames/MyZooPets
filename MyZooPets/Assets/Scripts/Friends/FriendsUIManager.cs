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

	public TweenToggleDemux codeInputTween;
	public InternetConnectionDisplay codeInputConnectionDisplay;
	public UILocalize codeInputErrorLabelLocalize;

	public TweenToggleDemux requestsTween;
	public InternetConnectionDisplay requestsConnectionDisplay;
	public GameObject requestEntryPrefab;

	private bool isActive = false;

	void Awake(){
		eModeType = UIModeTypes.Friends;
	}

	protected override void _Start(){
		SocialManager.OnDataRefreshed += FinishInternetConnection;

		RepositionGrid();

		ToggleCodeButton(false);
	}

	void OnDestroy(){
		SocialManager.OnDataRefreshed -= FinishInternetConnection;
	}

	private void RepositionGrid(){
		// Reposition all the things nicely to stretch to the end of the screen
		
		// Position the UIPanel clipping range
		UIPanel friendAreaPanel = friendArea.GetComponent<UIPanel>();
		Vector4 oldRange = friendAreaPanel.clipRange;
		friendAreaPanel.transform.localPosition = new Vector3(0, friendAreaPanel.transform.localPosition.y, 0f);
		friendAreaPanel.clipRange = new Vector4(0, oldRange.y, (float)(CameraManager.Instance.GetNativeWidth()), oldRange.w);
		
		// Position the grid origin to the left of the screen
//		Vector3 gridPosition = grid.transform.localPosition;
		grid.transform.localPosition = new Vector3(0f, 0f, 0f);
		grid.GetComponent<UIGrid>().Reposition();
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

//	private void RadialFill(float fraction){
//		radialFillRewardSprite.fillAmount = fraction;
//	}

	public void FinishInternetConnection(object sender, ServerEventArgs args){
		// Valid response
		if(args.IsSuccessful){
			Debug.Log("Connection Success");
			// Hide the connection display
			internetConnectionDisplay.Stop(true, string.Empty);

			List<ParseObjectKidAccount> friendList = SocialManager.Instance.FriendList;

			foreach(ParseObjectKidAccount friendAccount in friendList){
				Debug.Log("initiating friend");
				GameObject friendObject = NGUITools.AddChild(grid, friendEntryPrefab);
				FriendEntryController friendEntryController = friendObject.GetComponent<FriendEntryController>();

				// TODO rename friendObject to friendACcount.ObjectId

				ParseObjectPetInfo friendPetInfo = friendAccount.PetInfo;
				if(friendPetInfo != null && friendPetInfo.IsDataAvailable){
					// TODO create the pet into hashtable down the road and pass in here v
					friendEntryController.Populate(friendPetInfo.Name, null);
				}
			}
			RepositionGrid();


		}
		// Error state
		else{
			// Check for errorcode first then erromessage. only OtherCause

			internetConnectionDisplay.Stop(false, "NOTIFICATION_INTERNET_CONNECTION_FAIL");
			Debug.LogWarning(args.ErrorCode.ToString() + " " + args.ErrorMessage);
		}
	}

	private void TryInternetConnection(){
		internetConnectionDisplay.Play("NOTIFICATION_INTERNET_CONNECTION_WAIT");
		Debug.Log("trying connection");
		SocialManager.Instance.RefreshData();
	}

	protected override void _OpenUI(){
		if(!isActive){
			GetComponent<TweenToggleDemux>().Show();

			// Hide other UI objects
			NavigationUIManager.Instance.HidePanel();
			InventoryUIManager.Instance.HidePanel();
			RoomArrowsUIManager.Instance.HidePanel();
			HUDUIManager.Instance.HidePanel();

			isActive = true;

			TryInternetConnection();
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

	public void CodeInputShowConnectionDisplay(){
//		codeInputConnectionDisplay.Play();
	}

	public void CodeInputHideConnectionDisplay(){
//		codeInputConnectionDisplay.Stop();
	}

	public void ShowErrorMessage(string errorMessageKey){
		codeInputErrorLabelLocalize.gameObject.SetActive(true);
		codeInputErrorLabelLocalize.key = errorMessageKey;
		codeInputErrorLabelLocalize.Localize();
	}

	//////////////// Friend Requests ////////////////////////

	public void OpenRequestsWindow(){
		if(isActive){
			requestsTween.Show();
		}
	}

	public void CloseRequestsWindow(){
		if(isActive){
			requestsTween.Hide();
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
