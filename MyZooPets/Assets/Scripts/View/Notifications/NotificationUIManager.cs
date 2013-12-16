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

	public GameObject popupNotificationOneButton; 		
	public GameObject popupNotificationTwoButtons; 	
	public GameObject popupLevelUpMessage;
	public GameObject popupTipWithImage;
	public GameObject popupGameOverRewardMessageOneButton; 	
	public GameObject popupGameOverRewardMessageTwoButtons;
	public GameObject popupBadgeUnlockedMessage;
	public GameObject popupFireLevelUpMessage;

	private bool isNotificationActive = false;
	public bool IsNotificationActive() {
		return isNotificationActive;	
	}
	
	private GameObject anchorCenter; //parent of notificationCenterPanel
	private GameObject notificationCenterPanel; //where all the notifications will be created.
	
	void Awake(){
		anchorCenter = GameObject.Find("Anchor-Center");
	}
	
	void Start(){
		// Start is called after some notifications pushed!!! Check beforehand
		if(!isNotificationActive){		
			// Check the static queue to see if anything is there on level load
			TryNextNotification();
		}
	}

	//---------------------------------------------
	// AddToQueue()
	// Use this method to add new notification to the queue
	//---------------------------------------------
	public void AddToQueue(Hashtable notificationEntry){
		NotificationQueueData.AddNotification(notificationEntry);
		
		if(!isNotificationActive){
			TryNextNotification();
		}
	}
	
	public void TryNextNotification(){
		if(!NotificationQueueData.IsEmpty()){
			isNotificationActive = true;
			Hashtable entry = NotificationQueueData.PopNotification();

			//Check if notification panel exist. load it if not
			if(notificationCenterPanel == null){
				GameObject notificationPanelPrefab = (GameObject) Resources.Load("notificationCenterPanel");
				notificationCenterPanel = LgNGUITools.AddChildWithPosition(anchorCenter, notificationPanelPrefab);
			}
			
			switch((NotificationPopupType)entry[NotificationPopupFields.Type]){
				case NotificationPopupType.OneButton:
					ShowPopupNotificationOneButton(
						(string) entry[NotificationPopupFields.Message],
						(PopupNotificationNGUI.HashEntry) entry[NotificationPopupFields.Button1Callback],
						(string) entry[NotificationPopupFields.Button1Label]
					);
					break;
				
				case NotificationPopupType.TwoButtons:
					ShowPopupNotificationTwoButtons(
						(string) entry[NotificationPopupFields.Message],
						(PopupNotificationNGUI.HashEntry) entry[NotificationPopupFields.Button1Callback],
						(PopupNotificationNGUI.HashEntry) entry[NotificationPopupFields.Button2Callback],
						(string) entry[NotificationPopupFields.Button1Label],
						(string) entry[NotificationPopupFields.Button2Label]
					);
					break;
				
				case NotificationPopupType.GameOverRewardOneButton:
					ShowGameOverRewardMessage(
						(int) entry[NotificationPopupFields.DeltaStars],
						(int) entry[NotificationPopupFields.DeltaPoints],
						(PopupNotificationNGUI.HashEntry) entry[NotificationPopupFields.Button1Callback]
					);
					break;
				
				case NotificationPopupType.GameOverRewardTwoButton:
					ShowGameOverRewardMessage(
						(int) entry[NotificationPopupFields.DeltaStars],
						(int) entry[NotificationPopupFields.DeltaPoints],
						(PopupNotificationNGUI.HashEntry) entry[NotificationPopupFields.Button1Callback],
						(PopupNotificationNGUI.HashEntry) entry[NotificationPopupFields.Button2Callback]
					);
					break;
				
				case NotificationPopupType.TipWithImage:
					ShowPopupTipWithImage(
						(string) entry[NotificationPopupFields.Message],
						(string) entry[NotificationPopupFields.SpriteName],
						(PopupNotificationNGUI.HashEntry) entry[NotificationPopupFields.Button1Callback],
						(bool) entry[NotificationPopupFields.StartsHidden],
						(bool) entry[NotificationPopupFields.HideImmediately]
					);
					break;
				
				case NotificationPopupType.LevelUp:
					ShowLevelUpMessage(
						(string) entry[NotificationPopupFields.Message],
						(PopupNotificationNGUI.HashEntry) entry[NotificationPopupFields.Button1Callback],
						(string) entry[NotificationPopupFields.Sound]
					);
					break;
				case NotificationPopupType.BadgeUnlocked:
					ShowBadgeRewardMessage(
						(string) entry[NotificationPopupFields.Message],
						(string) entry[NotificationPopupFields.SpriteName],
						(PopupNotificationNGUI.HashEntry) entry[NotificationPopupFields.Button1Callback]
					);
					break;
				case NotificationPopupType.FireLevelUp:
					ShowFireLevelUpMessage(
						(string) entry[NotificationPopupFields.Message],
						(string) entry[NotificationPopupFields.SpriteName],
						(PopupNotificationNGUI.HashEntry) entry[NotificationPopupFields.Button1Callback]
					);
					break;
				default:
					Debug.LogError("Invalid Notification");
					break;
			}
		}
		else{
			isNotificationActive = false;
			Destroy(notificationCenterPanel);
			notificationCenterPanel = null;
		}
	}

	//----------------------------------------------------------	
	// ShowPopupNotificationTwoButtons()
	// Desc: creates popup that has a popup texture and 2 buttons
	//Params: notificationType, call back for yes button, call back for no button
	//----------------------------------------------------------	
	public void ShowPopupNotificationTwoButtons(string message, PopupNotificationNGUI.HashEntry button1CallBack,
		PopupNotificationNGUI.HashEntry button2Callback, string button1, string button2){
		
		PopupNotificationNGUI twoButtonMessage = CreatePopupNotificationNGUI(popupNotificationTwoButtons);
		twoButtonMessage.Message = message;
		twoButtonMessage.Button1Callback = button1CallBack;
		twoButtonMessage.Button2Callback = button2Callback;
		twoButtonMessage.Button1Text = button1;
		twoButtonMessage.Button2Text = button2;
		twoButtonMessage.OnHideFinished += TryNextNotification; 	// Assign queue behavior to notification
		
		StartCoroutine(DisplayAfterInit(twoButtonMessage));
	}

	//----------------------------------------------------------	
	// ShowPopupNotificationOneButton
	// Desc: creates popup that has a popup texture and 1 button
	//Params: notificationType, call back for button
	//----------------------------------------------------------	
	public void ShowPopupNotificationOneButton(string message, PopupNotificationNGUI.HashEntry okCallBack, string button){
		PopupNotificationNGUI oneButtonMessage = CreatePopupNotificationNGUI(popupNotificationOneButton);
		oneButtonMessage.Message = message;
		oneButtonMessage.Button1Callback = okCallBack;
		oneButtonMessage.Button1Text = button;
		oneButtonMessage.OnHideFinished += TryNextNotification; 	// Assign queue behavior to notification
		
		StartCoroutine(DisplayAfterInit(oneButtonMessage));
	}

	//----------------------------------------------------------	
	// ShowLevelUpMessage()
	// Desc: creates popup that shows an image of the badge, along with a corresponding message
	// Params: badge, call back for button
	//----------------------------------------------------------	
	public void ShowLevelUpMessage(string message, PopupNotificationNGUI.HashEntry okCallBack, string sound){
		PopupNotificationNGUI oneButtonMessage = CreatePopupNotificationNGUI(popupLevelUpMessage);
		oneButtonMessage.Message = message;
		oneButtonMessage.Button1Callback = okCallBack;
		oneButtonMessage.SetSound( sound );
		oneButtonMessage.OnHideFinished += TryNextNotification; 	// Assign queue behavior to notification
		
		StartCoroutine(DisplayAfterInit(oneButtonMessage));
	}

	//----------------------------------------------------------	
	// ShowPopupTipWithImage()
	// Desc: creates popup that shows a tip, along with an image on the left and a message on the right.
	// Params: message to display, tipImage, call back for button, starts hidden?, kill after used?
	//----------------------------------------------------------	
	public void ShowPopupTipWithImage(string message, string spriteName, PopupNotificationNGUI.HashEntry okCallBack, bool startsHidden, bool hideImmediately){

		PopupNotificationWithImageNGUI tip = CreatePopupNotificationNGUI(popupTipWithImage, startsHidden) as PopupNotificationWithImageNGUI;
		tip.HideImmediately = hideImmediately;
		tip.Message = message;
		tip.Title = Localization.Localize("TIP");
		tip.SetSprite(spriteName);
		tip.Button1Callback = okCallBack;
		tip.Button1Text = Localization.Localize("OK");
		tip.OnHideFinished += TryNextNotification; 	// Assign queue behavior to notification
		
		StartCoroutine(DisplayAfterInit(tip));
	}

	//----------------------------------------------------------	
	// ShowGameOverRewardMessage()
	// Desc: creates a popup that can be used at the end of a game to show points rewarded
	// Params: stars, points, yes button call back, no button call back
	// Note: pass in 0 for stars or points will result in the gui not showing up
	//----------------------------------------------------------	
	public void ShowGameOverRewardMessage(int deltaStars, int deltaPoints,
		PopupNotificationNGUI.HashEntry yesButtonCallBack,
		PopupNotificationNGUI.HashEntry noButtonCallBack){

		GameOverRewardMessageNGUI twoButtonMessage = CreatePopupNotificationNGUI(popupGameOverRewardMessageTwoButtons) as GameOverRewardMessageNGUI;
		twoButtonMessage.SetRewardMessage(deltaStars, deltaPoints);
		twoButtonMessage.Button1Callback = yesButtonCallBack;
		twoButtonMessage.Button2Callback = noButtonCallBack;
		twoButtonMessage.Button1Text = Localization.Localize("PLAY");
		twoButtonMessage.Button2Text = Localization.Localize("QUIT");
		twoButtonMessage.OnHideFinished += TryNextNotification; 	// Assign queue behavior to notification
		
		StartCoroutine(DisplayAfterInit(twoButtonMessage));
	}

	//----------------------------------------------------------	
	// ShowGameOverRewardMessage()
	// Desc: creates a popup reward message that only has one button(exiting button)
	// Param: stars, points, button call back
	//----------------------------------------------------------	
	public void ShowGameOverRewardMessage(int deltaStars, int deltaPoints, PopupNotificationNGUI.HashEntry buttonCallBack){

		GameOverRewardMessageNGUI oneButtonMessage = CreatePopupNotificationNGUI(popupGameOverRewardMessageOneButton) as GameOverRewardMessageNGUI;
		oneButtonMessage.SetRewardMessage(deltaStars, deltaPoints);
		oneButtonMessage.Button1Callback = buttonCallBack;
		oneButtonMessage.Button1Text = Localization.Localize("QUIT");
		oneButtonMessage.OnHideFinished += TryNextNotification; 	// Assign queue behavior to notification
		
		StartCoroutine(DisplayAfterInit(oneButtonMessage));
	}

	//----------------------------------------------------------	
	// ShowBadgeRewardMessage()
	// Desc: creates a popup reward for new unlocked badge
	// Param: message, spriteName, button callback
	//----------------------------------------------------------	
	public void ShowBadgeRewardMessage(string message, string spriteName, PopupNotificationNGUI.HashEntry buttonCallBack){
		PopupNotificationWithImageNGUI spawnedPopupBadge = CreatePopupNotificationNGUI(popupBadgeUnlockedMessage) as PopupNotificationWithImageNGUI;
		spawnedPopupBadge.Message = message;
		spawnedPopupBadge.SetSprite(spriteName);
		spawnedPopupBadge.Button1Callback = buttonCallBack;
		spawnedPopupBadge.OnHideFinished += TryNextNotification;

		StartCoroutine(DisplayAfterInit(spawnedPopupBadge));
	}

	//----------------------------------------------------------	
	// ShowFireLevelUpMessage()
	// Desc: creates a popup to show unlocked fire
	// Param: message, spriteName, button callback
	//----------------------------------------------------------	
	private void ShowFireLevelUpMessage(string message, string spriteName, PopupNotificationNGUI.HashEntry buttonCallBack){
		PopupNotificationWithImageNGUI oneButtonMessage = CreatePopupNotificationNGUI(popupFireLevelUpMessage) as PopupNotificationWithImageNGUI;
		oneButtonMessage.Message = message;
		oneButtonMessage.SetSprite(spriteName);
		oneButtonMessage.Button1Callback = buttonCallBack;
		oneButtonMessage.OnHideFinished += TryNextNotification;

		StartCoroutine(DisplayAfterInit(oneButtonMessage));
	}
	
	// Displaying after one frame, make sure the notification is loaded nicely
	private IEnumerator DisplayAfterInit(PopupNotificationNGUI notification){
		yield return 0;
		
		notification.Display();
	}

	private PopupNotificationNGUI CreatePopupNotificationNGUI(GameObject prefab){ 
		return CreatePopupNotificationNGUI(prefab, true);
	}

	private PopupNotificationNGUI CreatePopupNotificationNGUI(GameObject prefab, bool startsHidden){ 
		// GameObject obj = LgNGUITools.AddChildWithPosition(anchorCenter, prefab);
		GameObject obj = LgNGUITools.AddChildWithPosition(notificationCenterPanel, prefab);

		TweenToggleDemux demux = obj.GetComponent<TweenToggleDemux>();
		if(demux != null){
			demux.startsHidden = startsHidden;
			demux.Reset();
		}
		
		PopupNotificationNGUI popup = obj.GetComponent<PopupNotificationNGUI>();

		return popup;
	}
}
