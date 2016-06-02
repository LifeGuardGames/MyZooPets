using UnityEngine;
using System.Collections;

/// <summary>
/// Notification user interface manager.
/// Make sure this object follows the camera, either by reference or child object
/// This is persistent throughout the scene.
/// </summary>
public class NotificationUIManager : Singleton<NotificationUIManager>{

	//NOTE: This class went from a generic notification system to fairly non generic
	//We don't really use the popupNotifcationOneButton/TwoButton prefab anymore. The reason
	//is that most of the pop we require special notification layout for different events
	//in the game. We should probably add a feature where you can just specify a notification
	//prefab when you call NotificationUIManager. Will save us from creating so many switch statements
	public GameObject popupLevelUpMessage;
	public GameObject popupTipWithImage;
	public GameObject popupGameOverRewardMessageOneButton;
	public GameObject popupGameOverRewardMessageTwoButtons;
	public GameObject popupBadgeUnlockedMessage;
	public GameObject popupFireLevelUpMessage;

	public GameObject popupMiniGameQuitCheck;
	public GameObject popupZeroHealth;

	public GameObject popupSuperWellaInhaler;
	public GameObject popupSuperWellaSick;
	public GameObject popupNeedFoodTutorial;

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
				if(Application.loadedLevelName == SceneUtils.BEDROOM || Application.loadedLevelName == SceneUtils.INHALERGAME|| Application.loadedLevelName == SceneUtils.YARD){
				GameObject notificationPanelPrefab = (GameObject)Resources.Load("NotificationCenterPanel");
				notificationCenterPanel = GameObjectUtils.AddChildWithPositionAndScale(anchorCenter, notificationPanelPrefab);
				}
			}

			//load the 3D click blocker	
			if(notificationBackDrop3D == null && mainCamera != null){
				if(Application.loadedLevelName == SceneUtils.BEDROOM || Application.loadedLevelName == SceneUtils.INHALERGAME|| Application.loadedLevelName == SceneUtils.YARD){
					GameObject notificationBackDrop3DPrefab = (GameObject)Resources.Load("NotificationBackDrop3D");
					notificationBackDrop3D = GameObjectUtils.AddChildWithPositionAndScale(mainCamera, notificationBackDrop3DPrefab);
				}
			}
			
			switch((NotificationPopupType)entry[NotificationPopupFields.Type]){
			case NotificationPopupType.TipWithImage:
				ShowPopupTipWithImage(
						(string)entry[NotificationPopupFields.Message],
						(string)entry[NotificationPopupFields.SpriteName],
						(PopupNotificationNGUI.Callback)entry[NotificationPopupFields.Button1Callback]
				);
				break;
			case NotificationPopupType.LevelUp:
				ShowLevelUpMessage(
						(string)entry[NotificationPopupFields.Message],
						(PopupNotificationNGUI.Callback)entry[NotificationPopupFields.Button1Callback],
						(string)entry[NotificationPopupFields.Sound]
				);
				break;
			case NotificationPopupType.FireLevelUp:
				ShowFireLevelUpMessage(
						(string)entry[NotificationPopupFields.Message],
						(string)entry[NotificationPopupFields.SpriteName],
						(PopupNotificationNGUI.Callback)entry[NotificationPopupFields.Button1Callback]
				);
				break;
			case NotificationPopupType.NeedFoodTutorial:
				ShowNeedFoodTutorialMessage(
					(PopupNotificationNGUI.Callback)entry[NotificationPopupFields.Button1Callback],
					(string)entry[NotificationPopupFields.Message]
				);
				break;
			case NotificationPopupType.SuperWellaInhaler:
				ShowSuperWellaInhalerMessage(
					(string)entry[NotificationPopupFields.Message],
					(PopupNotificationNGUI.Callback)entry[NotificationPopupFields.Button1Callback]
				);
				break;
			case NotificationPopupType.ZeroHealth:
				ShowZeroHealthMessage(
					(string)entry[NotificationPopupFields.Message],
					(PopupNotificationNGUI.Callback)entry[NotificationPopupFields.Button1Callback]
				);
				break;
			case NotificationPopupType.SuperWellaSickReminder:
				ShowSuperWellaSickReminder(
					(PopupNotificationNGUI.Callback)entry[NotificationPopupFields.Button1Callback]
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
	/// Shows the level up message.
	/// </summary>
	/// <param name="message">Message.</param>
	/// <param name="okCallBack">Ok call back.</param>
	/// <param name="sound">Sound.</param>
	public void ShowLevelUpMessage(string message, PopupNotificationNGUI.Callback okCallBack, string sound){
		PopupNotificationNGUI oneButtonMessage = CreatePopupNotificationNGUI(popupLevelUpMessage);
		oneButtonMessage.Message = message;
		oneButtonMessage.Button1Callback = okCallBack;
		oneButtonMessage.SetSound(sound);
		oneButtonMessage.OnHideFinished += TryNextNotification; 	// Assign queue behavior to notification
		if(Application.loadedLevelName == SceneUtils.BEDROOM || Application.loadedLevelName == SceneUtils.INHALERGAME|| Application.loadedLevelName == SceneUtils.YARD){
		StartCoroutine(DisplayAfterInit(oneButtonMessage));
		}
		else{
			oneButtonMessage.Hide();
		}
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
		PopupNotificationNGUI.Callback okCallBack, bool startsHidden = true, bool hideImmediately = true){

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
	public void ShowGameOverRewardMessage(int deltaStars, int deltaPoints, PopupNotificationNGUI.Callback buttonCallBack){

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
	public void ShowBadgeRewardMessage(string badgeName, string message, string spriteName, PopupNotificationNGUI.Callback buttonCallBack){
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
	private void ShowFireLevelUpMessage(string message, string spriteName, PopupNotificationNGUI.Callback buttonCallBack){
		PopupNotificationWithImageNGUI oneButtonMessage = CreatePopupNotificationNGUI(popupFireLevelUpMessage) as PopupNotificationWithImageNGUI;
		oneButtonMessage.Message = message;
		oneButtonMessage.SetSprite(spriteName);
		oneButtonMessage.Button1Callback = buttonCallBack;
		oneButtonMessage.OnHideFinished += TryNextNotification;

		StartCoroutine(DisplayAfterInit(oneButtonMessage));
	}

	/// <summary>
	/// Shows the inhaler recharging message.
	/// </summary>
	/// <param name="okButtonCallBack">Ok button call back.</param>
//	private void ShowInhalerRechargingMessage(PopupNotificationNGUI.Callback okButtonCallBack){
//		PopupNotificationNGUI twoButtonMessage = CreatePopupNotificationNGUI(popupInhalerRechargeMessage);
//		Debug.Log("Inhaler recharging message");
//		twoButtonMessage.Button1Callback = okButtonCallBack;
//		twoButtonMessage.OnHideFinished += TryNextNotification;
//
//		StartCoroutine(DisplayAfterInit(twoButtonMessage));
//	}

	private void ShowNeedFoodTutorialMessage(PopupNotificationNGUI.Callback okButtonCallBack, string message){
		PopupNotificationNGUI twoButtonMessage = CreatePopupNotificationNGUI(popupNeedFoodTutorial);
		twoButtonMessage.Message = message;
		twoButtonMessage.Button1Callback = okButtonCallBack;
		twoButtonMessage.OnHideFinished += TryNextNotification;
		
		StartCoroutine(DisplayAfterInit(twoButtonMessage));
	}

	/// <summary>
	/// Shows the super wella inhaler message.
	/// </summary>
	/// <param name="message">Message.</param>
	/// <param name="okButtonCallBack">Ok button call back.</param>
	private void ShowSuperWellaInhalerMessage(string message, PopupNotificationNGUI.Callback okButtonCallBack){
		PopupNotificationNGUI oneButtonMessage = CreatePopupNotificationNGUI(popupSuperWellaInhaler);

		oneButtonMessage.Message = message;
		oneButtonMessage.Button1Callback = okButtonCallBack;
		oneButtonMessage.OnHideFinished += TryNextNotification;

		StartCoroutine(DisplayAfterInit(oneButtonMessage));
	}

	private void ShowMiniGameQuitCheckMessage(PopupNotificationNGUI.Callback okButtonCallBack,
	                                          PopupNotificationNGUI.Callback cancelButtonCallBack){

		PopupNotificationNGUI twoButtonMessage = CreatePopupNotificationNGUI(popupMiniGameQuitCheck);
		
		twoButtonMessage.Button1Callback = okButtonCallBack;
		twoButtonMessage.Button2Callback = cancelButtonCallBack;
		twoButtonMessage.OnHideFinished += TryNextNotification;
		
		StartCoroutine(DisplayAfterInit(twoButtonMessage));
	}

	private void ShowZeroHealthMessage(string message, PopupNotificationNGUI.Callback okButtonCallBack){
		PopupNotificationNGUI oneButtonMessage = CreatePopupNotificationNGUI(popupZeroHealth);

		oneButtonMessage.Message = message;
		oneButtonMessage.Button1Callback = okButtonCallBack;
		oneButtonMessage.OnHideFinished += TryNextNotification;
		
		StartCoroutine(DisplayAfterInit(oneButtonMessage));
	}

	private void ShowSuperWellaSickReminder(PopupNotificationNGUI.Callback okButtonCallBack){
		PopupNotificationNGUI oneButtonMessage = CreatePopupNotificationNGUI(popupSuperWellaSick);

		oneButtonMessage.Button1Callback = okButtonCallBack;
		oneButtonMessage.OnHideFinished += TryNextNotification;

		StartCoroutine(DisplayAfterInit(oneButtonMessage));
	}

	
	// Displaying after one frame, make sure the notification is loaded nicely
	private IEnumerator DisplayAfterInit(PopupNotificationNGUI notification){
		yield return 0;

		if(notification != null)
			notification.Display();
	}

	private PopupNotificationNGUI CreatePopupNotificationNGUI(GameObject prefab){ 
		return CreatePopupNotificationNGUI(prefab, true);
	}

	private PopupNotificationNGUI CreatePopupNotificationNGUI(GameObject prefab, bool startsHidden){ 
		// GameObject obj = LgNGUITools.AddChildWithPosition(anchorCenter, prefab);
		GameObject obj = GameObjectUtils.AddChildWithPositionAndScale(notificationCenterPanel, prefab);

		TweenToggleDemux demux = obj.GetComponent<TweenToggleDemux>();
		if(demux != null){
			demux.startsHidden = startsHidden;
			demux.LgReset();
		}
		
		PopupNotificationNGUI popup = obj.GetComponent<PopupNotificationNGUI>();

		return popup;
	}
}
