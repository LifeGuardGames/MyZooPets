using UnityEngine;
using System.Collections;

/// <summary>
/// Notification user interface manager.
/// Make sure this object follows the camera, either by reference or child object
/// This is persistent throughout the scene.
/// </summary>

public class NotificationUIManager : Singleton<NotificationUIManager> {
	// References
	public GameObject cameraObject;
	public GameObject centerPanel;
	public GameObject leftPanel;
	public GameObject backDrop;						// This class will handle the backdrop as well
	public GameObject popupTextureGreatNGUI;
	public GameObject popupTextureNiceTryNGUI;
	public GameObject popupTextureUseTheInhalerNGUI;
	public GameObject popupTexturePracticeInhalerNGUI;
	public GameObject popupTextureDiagnoseSymptomsNGUI;

	public GameObject popupNotificationOneButton; // NGUI as well
	public GameObject popupNotificationTwoButtons; // NGUI as well
	public GameObject levelUpMessageNGUI;
	public GameObject popupTipWithImageNGUI;
	public GameObject gameOverRewardMessageOneButton; // NGUI as well
	public GameObject gameOverRewardMessageTwoButtons; // NGUI as well
	public GameObject popupNotificiationTutorialLeft;

//	public bool flipped;

	// Queue variables
//	private Queue q = new Queue();
	private bool isNotificationActive = false;
//	private bool qLock = true;
//	private bool isFirstPop = true; // Aux to stop subsequent AddToQueue() to trigger queue check

	void Start(){
//		if(!flipped){
//			gameObject.transform.position = new Vector3(cameraObject.transform.position.x,
//				cameraObject.transform.position.y - 1f, cameraObject.transform.position.z + 4f);
//		}
//		else{
//			gameObject.transform.position = new Vector3(cameraObject.transform.position.x,
//				cameraObject.transform.position.y - 1f, cameraObject.transform.position.z - 4f);
//		}

		backDrop.SetActive(false);
		
		// Check the static queue to see if anything is there on level load
		TryNextNotification();
	}

//	void Update(){
//		if(isActive){						// Keep polling queue if it is active and something in it
//			if(q.Count >= 1 && !qLock){
//				PopQueueAndDisplay();
//			}
//			if(q.Count == 0 && !qLock){		// Shut self off if nothing in queue, reset
//				isActive = false;
//				qLock = true;
//				isFirstPop = true;
//				backDrop.SetActive(false);
//			}
//		}
//	}

	/////////////// QUEUE HANDLERS /////////////////
	
	public void AddToQueue(Hashtable notificationEntry){
		NotificationQueueData.AddNotification(notificationEntry);
		
		Debug.Log ("ADDING");
		
		if(!isNotificationActive){
			TryNextNotification();
		}
	}
	
	public void TryNextNotification(){
		Debug.Log ("Checking for notification");
		if(!NotificationQueueData.IsEmpty()){
			Debug.Log ("Yes notification detected");
			isNotificationActive = true;
			Hashtable entry = NotificationQueueData.PopNotification();
			
			switch((NotificationPopupType)entry[NotificationPopupFields.Type]){
				case NotificationPopupType.OneButton:
					ShowPopupNotificationOneButton(	(string)						entry[NotificationPopupFields.Message],
													(PopupNotificationNGUI.HashEntry)entry[NotificationPopupFields.Button1Callback],
													(string)						entry[NotificationPopupFields.Button1Label]);
					break;
				
				case NotificationPopupType.TwoButtons:
					ShowPopupNotificationTwoButtons((string)						entry[NotificationPopupFields.Message],
													(PopupNotificationNGUI.HashEntry)entry[NotificationPopupFields.Button1Callback],
													(PopupNotificationNGUI.HashEntry)entry[NotificationPopupFields.Button2Callback],
													(string)						entry[NotificationPopupFields.Button1Label],
													(string)						entry[NotificationPopupFields.Button2Label]);
					break;
				
				case NotificationPopupType.GameOverRewardOneButton:
					ShowGameOverRewardMessage(		(int)							entry[NotificationPopupFields.DeltaStars],
													(int)							entry[NotificationPopupFields.DeltaPoints],
													(PopupNotificationNGUI.HashEntry)entry[NotificationPopupFields.Button1Callback]);
					break;
				
				case NotificationPopupType.GameOverRewardTwoButton:
					ShowGameOverRewardMessage(		(int)							entry[NotificationPopupFields.DeltaStars],
													(int)							entry[NotificationPopupFields.DeltaPoints],
													(PopupNotificationNGUI.HashEntry)entry[NotificationPopupFields.Button1Callback],
													(PopupNotificationNGUI.HashEntry)entry[NotificationPopupFields.Button2Callback]);
					break;
				
				case NotificationPopupType.TipWithImage:
					ShowPopupTipWithImage(			(string)						entry[NotificationPopupFields.Message],
													(string)						entry[NotificationPopupFields.SpriteName],
													(PopupNotificationNGUI.HashEntry)entry[NotificationPopupFields.Button1Callback],
													(bool)							entry[NotificationPopupFields.StartsHidden],
													(bool)							entry[NotificationPopupFields.HideImmediately]);
					break;
				
				case NotificationPopupType.LevelUp:
					ShowLevelUpMessage(				(BadgeTier)						entry[NotificationPopupFields.Badge],
													(PopupNotificationNGUI.HashEntry)entry[NotificationPopupFields.Button1Callback]);
					break;
				
				case NotificationPopupType.TutorialLeft:
					ShowTutorialMessage(			(TutorialImageType)				entry[NotificationPopupFields.TutorialImageType],
													(PopupNotificationNGUI.HashEntry)	entry[NotificationPopupFields.Button1Callback],
													(string)						entry[NotificationPopupFields.Button1Label]);
					break;
			}
		}
		else{
			isNotificationActive = false;
			Debug.Log ("No notification detected");
		}
	}
	
	// TODO-S The unlock queue method is assigned to the callback of respective notifications, i need to abstract these away into this class, or embed them into the prefabs themselves

	// Used to resume notifications after pausing
//	public void StartQueue(){
//		isActive = true;
//	}
//
//	// Pauses the popping of the queue
//	public void PauseQueue(){
//		isActive = false;
//	}
//
//	private void AddToQueue(GameObject notificationGameObject){
////		Debug.Log("Adding to Q");
//		notificationGameObject.SetActive(false);
//		q.Enqueue(notificationGameObject);
//		if(isFirstPop){		// If it is the first time, start queue check, else let Update check
//			isFirstPop = false;
//			qLock = false;
//		}
//		StartQueue();
//	}
//
//	private void PopQueueAndDisplay(){
////		Debug.Log("POP");
//		backDrop.SetActive(true);
//
//		GameObject notificationGameObject = q.Dequeue() as GameObject;
//		notificationGameObject.SetActive(true);
//		PopupNotificationNGUI popupNotifNgui = notificationGameObject.GetComponent<PopupNotificationNGUI>();
//		D.Assert(popupNotifNgui != null, "Object is not a valid popup notification");
//
//		popupNotifNgui.Display();
//		qLock = true;
//	}
//
//	public void CheckNextInQueue(){
////		Debug.Log ("UNLOCKED");
//		qLock = false;
//	}

	////////////////////////////////////////////////////////////////

	/*
		Desc: creates a popup with only a texture
		Params: notificationType
	*/
	public void PopupTexture(string notificationType){
		GameObject prefab = null;
		switch(notificationType){
			case "great":
				prefab = popupTextureGreatNGUI;
			break;

			case "practice intro":
				prefab = popupTexturePracticeInhalerNGUI;
			break;

			case "intro":
				prefab = popupTextureUseTheInhalerNGUI;
			break;

			case "nice try":
				prefab = popupTextureNiceTryNGUI;
			break;

			case "diagnose":
				prefab = popupTextureDiagnoseSymptomsNGUI;
			break;
		}
		if(prefab == popupTexturePracticeInhalerNGUI){
			// GameObject go = Instantiate(prefab, gameObject.transform.position, Quaternion.identity) as GameObject;
			GameObject go = ShowPopupTexture(prefab);
			Destroy(go, 3.0f);

			// show regular intro after announcing that it is a practice game
			Invoke("ShowIntro", 3.0f);
		}
		else if(prefab != null){
			// GameObject go = Instantiate(prefab, gameObject.transform.position, Quaternion.identity) as GameObject;
			GameObject go = ShowPopupTexture(prefab);
			Destroy(go, 3.0f);
		}
	}

	GameObject ShowPopupTexture(GameObject prefab){
		GameObject obj = NGUITools.AddChild(centerPanel, prefab);
		obj.GetComponent<MoveTweenToggle>().Reset();
		obj.GetComponent<MoveTweenToggle>().Show(1.0f);
		return obj;
	}

	// used to show regular intro after announcing that it is a practice game
	private void ShowIntro () {
		// GameObject intro = Instantiate(popupTextureUseTheInhaler, gameObject.transform.position, Quaternion.identity) as GameObject;
		// Destroy(intro, 3.0f);
		GameObject go = ShowPopupTexture(popupTextureUseTheInhalerNGUI);
		Destroy(go, 3.0f);
	}

	/////////////// PREFAB CREATION /////////////////

	PopupNotificationNGUI CreatePopupNotificationNGUI(GameObject prefab){ // doesn't call Show(). Show() is called in Display()
		return CreatePopupNotificationNGUI(prefab, true);
	}

	PopupNotificationNGUI CreatePopupNotificationNGUI(GameObject prefab, bool startsHidden){ // doesn't call Show(). Show() is called in Display()
		// save z-value, because it gets reset when using NGUITools.AddChild(...)
		float zVal = prefab.transform.localPosition.z;
		GameObject obj = NGUITools.AddChild(centerPanel, prefab);
		obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, zVal);
		MoveTweenToggle mtt = obj.GetComponent<MoveTweenToggle>();
		mtt.startsHidden = startsHidden;
		mtt.Reset();
		PopupNotificationNGUI popup = obj.GetComponent<PopupNotificationNGUI>();
		return popup;
	}

	/////////////// ENQUEUE FUNCTIONS /////////////////

	/*
		Desc: creates popup that has a popup texture and 2 buttons
		Params: notificationType, call back for yes button, call back for no button
	*/
	public void ShowPopupNotificationTwoButtons(string message, PopupNotificationNGUI.HashEntry button1CallBack,
		PopupNotificationNGUI.HashEntry button2Callback, string button1, string button2){
		
		PopupNotificationNGUI twoButtonMessage = CreatePopupNotificationNGUI(popupNotificationTwoButtons);
		twoButtonMessage.Message = message;
		twoButtonMessage.Button1Callback = button1CallBack;
		twoButtonMessage.Button2Callback = button2Callback;
		twoButtonMessage.Button1Text = button1;
		twoButtonMessage.Button2Text = button2;
		twoButtonMessage.OnHideFinished += TryNextNotification; 	// Assign queue behavior to notification
		twoButtonMessage.Display();
	}

	/*
		Desc: creates popup that has a popup texture and 1 button
		Params: notificationType, call back for button
	*/
	public void ShowPopupNotificationOneButton(string message, PopupNotificationNGUI.HashEntry okCallBack, string button){
		PopupNotificationNGUI oneButtonMessage = CreatePopupNotificationNGUI(popupNotificationOneButton);
		oneButtonMessage.Message = message;
		oneButtonMessage.Button1Callback = okCallBack;
		oneButtonMessage.Button1Text = button;
		oneButtonMessage.OnHideFinished += TryNextNotification; 	// Assign queue behavior to notification
		oneButtonMessage.Display();
	}

	/*
		Desc: creates popup that shows an image of the badge, along with a corresponding message
		Params: badge, call back for button
	*/
	public void ShowLevelUpMessage(BadgeTier badge, PopupNotificationNGUI.HashEntry okCallBack){
		LevelUpMessageNGUI oneButtonMessage = CreatePopupNotificationNGUI(levelUpMessageNGUI) as LevelUpMessageNGUI;
		oneButtonMessage.GetTrophyMessageAndImage(badge);
		oneButtonMessage.Button1Callback = okCallBack;
		oneButtonMessage.Button1Text = "OK";
		oneButtonMessage.OnHideFinished += TryNextNotification; 	// Assign queue behavior to notification
		oneButtonMessage.Display();
	}

	/*
		Desc: creates popup that shows a tip, along with an image on the left and a message on the right.
		Params: message to display, tipImage, call back for button, starts hidden?, kill after used?
	*/
	public void ShowPopupTipWithImage(string message, string spriteName, PopupNotificationNGUI.HashEntry okCallBack, bool startsHidden, bool hideImmediately){

		PopupNotificationWithImageNGUI tip = CreatePopupNotificationNGUI(popupTipWithImageNGUI, startsHidden) as PopupNotificationWithImageNGUI;
		tip.HideImmediately = hideImmediately;
		tip.Message = message;
		tip.Title = "Tip";
		tip.SetSprite(spriteName);
		tip.Button1Callback = okCallBack;
		tip.Button1Text = "OK";
		tip.OnHideFinished += TryNextNotification; 	// Assign queue behavior to notification
		tip.Display();
	}

	/*
		Desc: creates a popup that can be used at the end of a game to show points rewarded
		Params: stars, points, yes button call back, no button call back
		Note: pass in 0 for stars or points will result in the gui not showing up
	*/
	public void ShowGameOverRewardMessage(int deltaStars, int deltaPoints,
		PopupNotificationNGUI.HashEntry yesButtonCallBack,
		PopupNotificationNGUI.HashEntry noButtonCallBack){

		GameOverRewardMessageNGUI twoButtonMessage = CreatePopupNotificationNGUI(gameOverRewardMessageTwoButtons) as GameOverRewardMessageNGUI;
		twoButtonMessage.SetRewardMessage(deltaStars, deltaPoints);
		twoButtonMessage.Button1Callback = yesButtonCallBack;
		twoButtonMessage.Button2Callback = noButtonCallBack;
		twoButtonMessage.Button1Text = "Play";
		twoButtonMessage.Button2Text = "Quit";
		twoButtonMessage.OnHideFinished += TryNextNotification; 	// Assign queue behavior to notification
		twoButtonMessage.Display();
	}

	/*
		Desc: creates a popup reward message that only has one button(exiting button)
		Params: stars, points, yes button call back
	*/
	public void ShowGameOverRewardMessage(int deltaStars, int deltaPoints, PopupNotificationNGUI.HashEntry buttonCallBack){

		GameOverRewardMessageNGUI oneButtonMessage = CreatePopupNotificationNGUI(gameOverRewardMessageOneButton) as GameOverRewardMessageNGUI;
		oneButtonMessage.SetRewardMessage(deltaStars, deltaPoints);
		oneButtonMessage.Button1Callback = buttonCallBack;
		oneButtonMessage.Button1Text = "Quit";
		oneButtonMessage.OnHideFinished += TryNextNotification; 	// Assign queue behavior to notification
		oneButtonMessage.Display();
	}

	/*
		Desc: creates a popup for tutorial
		Param: tutorial sprite name, the target for call back, the function name for call back
	*/
	public void ShowTutorialMessage(TutorialImageType imageType, PopupNotificationNGUI.HashEntry buttonCallBack, string buttonText){
		string spriteName = "";

		//Spawn tutorial prefab
		float zVal = popupNotificiationTutorialLeft.transform.localPosition.z;
		Vector3 prefabPosition = popupNotificiationTutorialLeft.transform.localPosition;
		GameObject obj = NGUITools.AddChild(leftPanel, popupNotificiationTutorialLeft);
		obj.transform.localPosition = prefabPosition;

		//Set content
		PopupNotificationTutorial script = obj.GetComponent<PopupNotificationTutorial>();
		script.SetContent(imageType);
		if(buttonText != "") script.SetButtonText(buttonText);
		script.Button1Callback = buttonCallBack;
		script.OnHideFinished += TryNextNotification; 	// Assign queue behavior to notification
		script.Display();
	}
}
