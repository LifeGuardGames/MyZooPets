using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Notification user interface manager.
/// Make sure this object follows the camera, either by reference or child object
/// This is persistent throughout the scene.
/// </summary>

public class NotificationUIManager : Singleton<NotificationUIManager>{

	public GameObject popupNotificationOneButton;
	public GameObject popupNotificationTwoButtons;
	public GameObject popupLevelUpMessage;
	public GameObject popupTipWithImage;
	public GameObject popupGameOverRewardMessageOneButton;
	public GameObject popupGameOverRewardMessageTwoButtons;
	public GameObject popupBadgeUnlockedMessage;
	public GameObject popupFireLevelUpMessage;


	public GameObject popupPremiumMessage;
	public GameObject popupInhalerRechargeMessage;
	public GameObject popupInhalerTutorialMessage;

	//TODO: need to be removed
	public GameObject popupPremiumTest;

	private bool isNotificationActive = false;
	private GameObject anchorCenter; //parent of notificationCenterPanel
	private GameObject mainCamera;
	private GameObject notificationCenterPanel; //where all the notifications will be created.
	private GameObject notificationBackDrop3D; //a giant collider put infront of the 3d camera to prevent 3D scene from clicked
	
	public bool IsNotificationActive(){
		return isNotificationActive;	
	}

	void Awake(){
		anchorCenter = GameObject.Find("Anchor-Center");

		//not every scene has a main camera
		if(Camera.main != null){
			mainCamera = Camera.main.transform.gameObject;
		}
	}
	
	void Start(){
		// Start is called after some notifications pushed!!! Check beforehand
		if(!isNotificationActive){		
			// Check the static queue to see if anything is there on level load
			TryNextNotification();
		}
	}
	
	/// <summary>
	/// Cleanups all currently spawned the notification.
	/// </summary>
	public void CleanupNotification(){
		if(notificationCenterPanel != null){
			Destroy(notificationCenterPanel);
			isNotificationActive = false;
		}

		if(notificationBackDrop3D != null)
			Destroy(notificationBackDrop3D);
	}
	
	/// <summary>
	/// Adds new notification to queue.
	/// </summary>
	/// <param name="notificationEntry">Notification entry.</param>
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
				GameObject notificationPanelPrefab = (GameObject)Resources.Load("NotificationCenterPanel");
				notificationCenterPanel = LgNGUITools.AddChildWithPosition(anchorCenter, notificationPanelPrefab);

			}

			//load the 3D click blocker	
			if(notificationBackDrop3D == null && mainCamera != null){
				GameObject notificationBackDrop3DPrefab = (GameObject)Resources.Load("NotificationBackDrop3D");
				notificationBackDrop3D = LgNGUITools.AddChildWithPosition(mainCamera, notificationBackDrop3DPrefab);
			}
			
			switch((NotificationPopupType)entry[NotificationPopupFields.Type]){
			case NotificationPopupType.OneButton:
				ShowPopupNotificationOneButton(
						(string)entry[NotificationPopupFields.Message],
						(PopupNotificationNGUI.HashEntry)entry[NotificationPopupFields.Button1Callback]
				);
				break;

			case NotificationPopupType.TwoButtons:
				ShowPopupNotificationTwoButtons(
						(string)entry[NotificationPopupFields.Message],
						(PopupNotificationNGUI.HashEntry)entry[NotificationPopupFields.Button1Callback],
						(PopupNotificationNGUI.HashEntry)entry[NotificationPopupFields.Button2Callback]
				);
				break;
				
			case NotificationPopupType.GameOverRewardOneButton:
				ShowGameOverRewardMessage(
						(int)entry[NotificationPopupFields.DeltaStars],
						(int)entry[NotificationPopupFields.DeltaPoints],
						(PopupNotificationNGUI.HashEntry)entry[NotificationPopupFields.Button1Callback]
				);
				break;
				
			case NotificationPopupType.TipWithImage:
				ShowPopupTipWithImage(
						(string)entry[NotificationPopupFields.Message],
						(string)entry[NotificationPopupFields.SpriteName],
						(PopupNotificationNGUI.HashEntry)entry[NotificationPopupFields.Button1Callback]
				);
				break;
				
			case NotificationPopupType.LevelUp:
				ShowLevelUpMessage(
						(string)entry[NotificationPopupFields.Message],
						(PopupNotificationNGUI.HashEntry)entry[NotificationPopupFields.Button1Callback],
						(string)entry[NotificationPopupFields.Sound]
				);
				break;
			case NotificationPopupType.BadgeUnlocked:
				ShowBadgeRewardMessage(
						(string)entry[NotificationPopupFields.Badge],
						(string)entry[NotificationPopupFields.Message],
						(string)entry[NotificationPopupFields.SpriteName],
						(PopupNotificationNGUI.HashEntry)entry[NotificationPopupFields.Button1Callback]
				);
				break;
			case NotificationPopupType.FireLevelUp:
				ShowFireLevelUpMessage(
						(string)entry[NotificationPopupFields.Message],
						(string)entry[NotificationPopupFields.SpriteName],
						(PopupNotificationNGUI.HashEntry)entry[NotificationPopupFields.Button1Callback]
				);
				break;
			case NotificationPopupType.Premium:
				ShowBuyPremiumMessage();
				break;
			case NotificationPopupType.InhalerRecharging:
				ShowInhalerRechargingMessage(
					(PopupNotificationNGUI.HashEntry)entry[NotificationPopupFields.Button1Callback]
				);
				break;
			case NotificationPopupType.PremiumTest:
				ShowPremiumTestMessage(
					(PopupNotificationNGUI.HashEntry)entry[NotificationPopupFields.Button2Callback]
				);
				break;
			case NotificationPopupType.InhalerTutorial:
				ShowInhalerTutorialMessage(
					(PopupNotificationNGUI.HashEntry)entry[NotificationPopupFields.Button1Callback]
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
			Destroy(notificationBackDrop3D);
			notificationCenterPanel = null;
			notificationBackDrop3D = null;
		}
	}


	/// <summary>
	/// Shows the popup notification one button.
	/// </summary>
	/// <param name="message">Message.</param>
	/// <param name="okCallBack">Ok call back.</param>
	public void ShowPopupNotificationOneButton(string message, PopupNotificationNGUI.HashEntry okCallBack){
		PopupNotificationNGUI oneButtonMessage = CreatePopupNotificationNGUI(popupNotificationOneButton);
		oneButtonMessage.Message = message;
		oneButtonMessage.Button1Callback = okCallBack;
		// oneButtonMessage.Button1Text = button;
		oneButtonMessage.OnHideFinished += TryNextNotification; 	// Assign queue behavior to notification
		
		StartCoroutine(DisplayAfterInit(oneButtonMessage));
	}
	
	/// <summary>
	/// Shows the popup notification two buttons.
	/// </summary>
	/// <param name="message">Message.</param>
	/// <param name="okCallBack">Ok call back.</param>
	/// <param name="cancelCallBack">Cancel call back.</param>
	public void ShowPopupNotificationTwoButtons(string message, PopupNotificationNGUI.HashEntry okCallBack,
		PopupNotificationNGUI.HashEntry cancelCallBack){
		PopupNotificationNGUI twoButtonMessage = CreatePopupNotificationNGUI(popupNotificationTwoButtons);

		twoButtonMessage.Message = message;
		twoButtonMessage.Button1Callback = okCallBack;
		twoButtonMessage.Button2Callback = cancelCallBack;

		twoButtonMessage.OnHideFinished += TryNextNotification;

		StartCoroutine(DisplayAfterInit(twoButtonMessage));
	}
	
	/// <summary>
	/// Shows the level up message.
	/// </summary>
	/// <param name="message">Message.</param>
	/// <param name="okCallBack">Ok call back.</param>
	/// <param name="sound">Sound.</param>
	public void ShowLevelUpMessage(string message, PopupNotificationNGUI.HashEntry okCallBack, string sound){
		PopupNotificationNGUI oneButtonMessage = CreatePopupNotificationNGUI(popupLevelUpMessage);
		oneButtonMessage.Message = message;
		oneButtonMessage.Button1Callback = okCallBack;
		oneButtonMessage.SetSound(sound);
		oneButtonMessage.OnHideFinished += TryNextNotification; 	// Assign queue behavior to notification
		
		StartCoroutine(DisplayAfterInit(oneButtonMessage));
	}
	
	/// <summary>
	/// Shows the popup tip with image.
	/// </summary>
	/// <param name="message">Message.</param>
	/// <param name="spriteName">Sprite name.</param>
	/// <param name="okCallBack">Ok call back.</param>
	/// <param name="startsHidden">If set to <c>true</c> starts hidden.</param>
	/// <param name="hideImmediately">If set to <c>true</c> hide immediately.</param>
	public void ShowPopupTipWithImage(string message, string spriteName, 
		PopupNotificationNGUI.HashEntry okCallBack, bool startsHidden = true, bool hideImmediately = true){

		PopupNotificationWithImageNGUI tip = 
			CreatePopupNotificationNGUI(popupTipWithImage, startsHidden) as PopupNotificationWithImageNGUI;

		tip.Message = message;
		tip.SetSprite(spriteName);
		tip.Button1Callback = okCallBack;
		tip.OnHideFinished += TryNextNotification; 	// Assign queue behavior to notification
		
		StartCoroutine(DisplayAfterInit(tip));
	}
	
	/// <summary>
	/// Shows the game over reward message.
	/// </summary>
	/// <param name="deltaStars">Delta stars.</param>
	/// <param name="deltaPoints">Delta points.</param>
	/// <param name="buttonCallBack">Button call back.</param>
	public void ShowGameOverRewardMessage(int deltaStars, int deltaPoints, PopupNotificationNGUI.HashEntry buttonCallBack){

		GameOverRewardMessageNGUI oneButtonMessage = CreatePopupNotificationNGUI(popupGameOverRewardMessageOneButton) as GameOverRewardMessageNGUI;
		oneButtonMessage.SetRewardMessage(deltaStars, deltaPoints);
		oneButtonMessage.Button1Callback = buttonCallBack;
		// oneButtonMessage.Button1Text = Localization.Localize("QUIT");
		oneButtonMessage.OnHideFinished += TryNextNotification; 	// Assign queue behavior to notification
		
		StartCoroutine(DisplayAfterInit(oneButtonMessage));
	}
	
	/// <summary>
	/// Shows the badge reward message.
	/// </summary>
	/// <param name="badgeName">Badge name.</param>
	/// <param name="message">Message.</param>
	/// <param name="spriteName">Sprite name.</param>
	/// <param name="buttonCallBack">Button call back.</param>
	public void ShowBadgeRewardMessage(string badgeName, string message, string spriteName, PopupNotificationNGUI.HashEntry buttonCallBack){
		PopupNotificationBadge spawnedPopupBadge = CreatePopupNotificationNGUI(popupBadgeUnlockedMessage) as PopupNotificationBadge;
		spawnedPopupBadge.setTitle(badgeName);
		spawnedPopupBadge.setDescription(message);
		spawnedPopupBadge.SetSprite(spriteName);
		spawnedPopupBadge.Button1Callback = buttonCallBack;
		spawnedPopupBadge.OnHideFinished += TryNextNotification;

		StartCoroutine(DisplayAfterInit(spawnedPopupBadge));
	}
	
	/// <summary>
	/// Shows the fire level up message.
	/// </summary>
	/// <param name="message">Message.</param>
	/// <param name="spriteName">Sprite name.</param>
	/// <param name="buttonCallBack">Button call back.</param>
	private void ShowFireLevelUpMessage(string message, string spriteName, PopupNotificationNGUI.HashEntry buttonCallBack){
		PopupNotificationWithImageNGUI oneButtonMessage = CreatePopupNotificationNGUI(popupFireLevelUpMessage) as PopupNotificationWithImageNGUI;
		oneButtonMessage.Message = message;
		oneButtonMessage.SetSprite(spriteName);
		oneButtonMessage.Button1Callback = buttonCallBack;
		oneButtonMessage.OnHideFinished += TryNextNotification;

		StartCoroutine(DisplayAfterInit(oneButtonMessage));
	}

	/// <summary>
	/// Shows the buy premium message.
	/// </summary>
	private void ShowBuyPremiumMessage(){
		PopupNotificationNGUI oneButtonMessage = CreatePopupNotificationNGUI(popupPremiumMessage);

		oneButtonMessage.OnHideFinished += TryNextNotification;

		StartCoroutine(DisplayAfterInit(oneButtonMessage));
	}

	/// <summary>
	/// Shows the inhaler recharging message.
	/// </summary>
	/// <param name="okButtonCallBack">Ok button call back.</param>
	private void ShowInhalerRechargingMessage(PopupNotificationNGUI.HashEntry okButtonCallBack){
		PopupNotificationNGUI twoButtonMessage = CreatePopupNotificationNGUI(popupInhalerRechargeMessage);

		twoButtonMessage.Button1Callback = okButtonCallBack;
		twoButtonMessage.OnHideFinished += TryNextNotification;

		StartCoroutine(DisplayAfterInit(twoButtonMessage));
	}

	//TODO: need to be removed after IAP test
	private void ShowPremiumTestMessage(PopupNotificationNGUI.HashEntry okButtonCallBack){
		Debug.Log("show premiumtest message");
		PopupNotificationNGUI twoButtonMessage = CreatePopupNotificationNGUI(popupPremiumTest);
		
		twoButtonMessage.Button2Callback = okButtonCallBack;
		twoButtonMessage.OnHideFinished += TryNextNotification;
		
		StartCoroutine(DisplayAfterInit(twoButtonMessage));
	}

	private void ShowInhalerTutorialMessage(PopupNotificationNGUI.HashEntry okButtonCallBack){
		PopupNotificationNGUI oneButtonMessage = CreatePopupNotificationNGUI(popupInhalerTutorialMessage);

		oneButtonMessage.Button1Callback = okButtonCallBack;
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
