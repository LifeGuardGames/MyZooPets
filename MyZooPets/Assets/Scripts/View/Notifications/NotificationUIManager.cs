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

	public bool flipped;

	// Queue variables
	private Queue q = new Queue();
	private bool isActive = false;
	private bool qLock = true;
	private bool isFirstPop = true; // Aux to stop subsequent AddToQueue() to trigger queue check

	void Start(){
		if(!flipped){
			gameObject.transform.position = new Vector3(cameraObject.transform.position.x,
				cameraObject.transform.position.y - 1f, cameraObject.transform.position.z + 4f);
		}
		else{
			gameObject.transform.position = new Vector3(cameraObject.transform.position.x,
				cameraObject.transform.position.y - 1f, cameraObject.transform.position.z - 4f);
		}

		backDrop.SetActive(false);
	}

	void Update(){
		if(isActive){						// Keep polling queue if it is active and something in it
			if(q.Count >= 1 && !qLock){
				PopQueueAndDisplay();
			}
			if(q.Count == 0 && !qLock){		// Shut self off if nothing in queue, reset
				isActive = false;
				qLock = true;
				isFirstPop = true;
				backDrop.SetActive(false);
			}
		}
	}

	/////////////// QUEUE HANDLERS /////////////////

	// TODO-S The unlock queue method is assigned to the callback of respective notifications, i need to abstract these away into this class, or embed them into the prefabs themselves

	// Used to resume notifications after pausing
	public void StartQueue(){
		isActive = true;
	}

	// Pauses the popping of the queue
	public void PauseQueue(){
		isActive = false;
	}

	private void AddToQueue(GameObject notificationGameObject){
		notificationGameObject.SetActive(false);
		q.Enqueue(notificationGameObject);
		if(isFirstPop){		// If it is the first time, start queue check, else let Update check
			isFirstPop = false;
			qLock = false;
		}
		StartQueue();
	}

	private void PopQueueAndDisplay(){
		Debug.Log("POP");
		backDrop.SetActive(true);

		GameObject notificationGameObject = q.Dequeue() as GameObject;
		notificationGameObject.SetActive(true);
		PopupNotificationNGUI popupNotifNgui = notificationGameObject.GetComponent<PopupNotificationNGUI>();
		D.Assert(popupNotifNgui != null, "Object is not a valid popup notification");

		popupNotifNgui.Display();
		qLock = true;
	}

	public void CheckNextInQueue(){
		Debug.Log ("UNLOCKED");
		//Debug.Log(UnityEngine.StackTraceUtility.ExtractStackTrace());
		qLock = false;
	}

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
		return obj.GetComponent<PopupNotificationNGUI>();
	}

	/////////////// ENQUEUE FUNCTIONS /////////////////

	/*
		Desc: creates popup that has a popup texture and 2 buttons
		Params: notificationType, call back for yes button, call back for no button
	*/
	public void EnqueuePopupNotificationTwoButtons(string message, PopupNotificationNGUI.Callback yesCallBack,
		PopupNotificationNGUI.Callback noCallBack){

		PopupNotificationNGUI twoButtonMessage = CreatePopupNotificationNGUI(popupNotificationTwoButtons);
		twoButtonMessage.Message = message;
		twoButtonMessage.Button1Callback = yesCallBack;
		twoButtonMessage.Button2Callback = noCallBack;
		twoButtonMessage.Button1Text = "Yes";
		twoButtonMessage.Button2Text = "Ignore";
//		twoButtonMessage.Display();
		AddToQueue(twoButtonMessage.gameObject);
	}

	public void EnqueuePopupNotificationTwoButtons(string message, PopupNotificationNGUI.Callback yesCallBack,
		PopupNotificationNGUI.Callback noCallBack, string button1, string button2){

		PopupNotificationNGUI twoButtonMessage = CreatePopupNotificationNGUI(popupNotificationTwoButtons);
		twoButtonMessage.Message = message;
		twoButtonMessage.Button1Callback = yesCallBack;
		twoButtonMessage.Button2Callback = noCallBack;
		twoButtonMessage.Button1Text = button1;
		twoButtonMessage.Button2Text = button2;
//		twoButtonMessage.Display();
		AddToQueue(twoButtonMessage.gameObject);
	}

	/*
		Desc: creates popup that has a popup texture and 1 button
		Params: notificationType, call back for button
	*/
	public void EnqueuePopupNotificationOneButton(string message, PopupNotificationNGUI.Callback okCallBack){

		PopupNotificationNGUI oneButtonMessage = CreatePopupNotificationNGUI(popupNotificationOneButton);
		oneButtonMessage.Message = message;
		oneButtonMessage.Button1Callback = okCallBack;
		oneButtonMessage.Button1Text = "Yes";
//		oneButtonMessage.Display();
		AddToQueue(oneButtonMessage.gameObject);
	}
	public void EnqueuePopupNotificationOneButton(string message, PopupNotificationNGUI.Callback okCallBack, string button){

		PopupNotificationNGUI oneButtonMessage = CreatePopupNotificationNGUI(popupNotificationOneButton);
		oneButtonMessage.Message = message;
		oneButtonMessage.Button1Callback = okCallBack;
		oneButtonMessage.Button1Text = button;
//		oneButtonMessage.Display();
		AddToQueue(oneButtonMessage.gameObject);
	}

	/*
		Desc: creates popup that shows an image of the badge, along with a corresponding message
		Params: badge, call back for button
	*/
	public void EnqueueLevelUpMessage(BadgeTier badge, PopupNotificationNGUI.Callback okCallBack){

		LevelUpMessageNGUI oneButtonMessage = CreatePopupNotificationNGUI(levelUpMessageNGUI) as LevelUpMessageNGUI;
		oneButtonMessage.GetTrophyMessageAndImage(badge);
		oneButtonMessage.Button1Callback = okCallBack;
		oneButtonMessage.Button1Text = "OK";
		AddToQueue(oneButtonMessage.gameObject);
	}

	/*
		Desc: creates popup that shows a tip, along with an image on the left and a message on the right.
		Params: message to display, tipImage, call back for button, starts hidden?, kill after used?
	*/
	public void EnqueuePopupTipWithImage(string message, string spriteName, PopupNotificationNGUI.Callback okCallBack, bool startsHidden, bool hideImmediately){

		PopupNotificationWithImageNGUI tip = CreatePopupNotificationNGUI(popupTipWithImageNGUI, startsHidden) as PopupNotificationWithImageNGUI;
		tip.HideImmediately = hideImmediately;
		tip.Message = message;
		tip.Title = "Tip";
		tip.SetSprite(spriteName);
		tip.Button1Callback = okCallBack;
		tip.Button1Text = "OK";
		AddToQueue(tip.gameObject);
	}

	/*
		Desc: creates a popup that can be used at the end of a game to show points rewarded
		Params: stars, points, yes button call back, no button call back
		Note: pass in 0 for stars or points will result in the gui not showing up
	*/
	public void EnqueueGameOverRewardMessage(int deltaStars, int deltaPoints,
		PopupNotificationNGUI.Callback yesButtonCallBack,
		PopupNotificationNGUI.Callback noButtonCallBack){

		GameOverRewardMessageNGUI twoButtonMessage = CreatePopupNotificationNGUI(gameOverRewardMessageTwoButtons) as GameOverRewardMessageNGUI;
		twoButtonMessage.SetRewardMessage(deltaStars, deltaPoints);
		twoButtonMessage.Button1Callback = yesButtonCallBack;
		twoButtonMessage.Button2Callback = noButtonCallBack;
		twoButtonMessage.Button1Text = "Play";
		twoButtonMessage.Button2Text = "Quit";
		//twoButtonMessage.Display(false);
		AddToQueue(twoButtonMessage.gameObject);
	}

	/*
		Desc: creates a popup reward message that only has one button(exiting button)
		Params: stars, points, yes button call back
	*/
	public void EnqueueGameOverRewardMessage(int deltaStars, int deltaPoints, PopupNotificationNGUI.Callback yesButtonCallBack){

		GameOverRewardMessageNGUI oneButtonMessage = CreatePopupNotificationNGUI(gameOverRewardMessageOneButton) as GameOverRewardMessageNGUI;
		oneButtonMessage.SetRewardMessage(deltaStars, deltaPoints);
		oneButtonMessage.Button1Callback = yesButtonCallBack;
		oneButtonMessage.Button1Text = "Quit";
		//oneButtonMessage.Display(false);
		AddToQueue(oneButtonMessage.gameObject);
	}

	/*
		Desc: creates a popup for tutorial
		Param: tutorial sprite name, the target for call back, the function name for call back
	*/
	public void EnqueueTutorialMessage(TutorialImageType imageType, PopupNotificationNGUI.Callback buttonCallBack, string buttonText){
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
		AddToQueue(script.gameObject);
	}
}
