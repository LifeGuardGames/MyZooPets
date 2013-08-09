using UnityEngine;
using System.Collections;

/// <summary>
/// Notification user interface manager.
/// Make this a child of Main Camera so it inherits the position when camera moves
/// </summary>

public class NotificationUIManager : MonoBehaviour {

	// References
	public GameObject cameraObject;
	public GameObject centerAnchor;
	public GameObject leftAnchor;
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

	void Start(){
		if (!flipped){
			gameObject.transform.position = new Vector3(cameraObject.transform.position.x,
				cameraObject.transform.position.y - 1f, cameraObject.transform.position.z + 4f);
		}
		else {
			gameObject.transform.position = new Vector3(cameraObject.transform.position.x,
				cameraObject.transform.position.y - 1f, cameraObject.transform.position.z - 4f);
		}
	}
	//==========================================================
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
		GameObject obj = NGUITools.AddChild(centerAnchor, prefab);
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

	/*
		Desc: creates popup that has a popup texture and 2 buttons
		Params: notificationType, call back for yes button, call back for no button
	*/
	public void PopupNotificationTwoButtons(string message, PopupNotificationNGUI.Callback yesCallBack,
		PopupNotificationNGUI.Callback noCallBack){

		PopupNotificationNGUI twoButtonMessage = CreatePopupNotificationNGUI(popupNotificationTwoButtons);
		twoButtonMessage.Message = message;
		twoButtonMessage.Button1Callback = yesCallBack;
		twoButtonMessage.Button2Callback = noCallBack;
		twoButtonMessage.Button1Text = "Yes";
		twoButtonMessage.Button2Text = "Ignore";
		twoButtonMessage.Display();
	}

	public void PopupNotificationTwoButtons(string message, PopupNotificationNGUI.Callback yesCallBack,
		PopupNotificationNGUI.Callback noCallBack, string button1, string button2){

		PopupNotificationNGUI twoButtonMessage = CreatePopupNotificationNGUI(popupNotificationTwoButtons);
		twoButtonMessage.Message = message;
		twoButtonMessage.Button1Callback = yesCallBack;
		twoButtonMessage.Button2Callback = noCallBack;
		twoButtonMessage.Button1Text = button1;
		twoButtonMessage.Button2Text = button2;
		twoButtonMessage.Display();
	}

	/*
		Desc: creates popup that has a popup texture and 1 button
		Params: notificationType, call back for button
	*/
	public void PopupNotificationOneButton(string message, PopupNotificationNGUI.Callback okCallBack){

		PopupNotificationNGUI oneButtonMessage = CreatePopupNotificationNGUI(popupNotificationOneButton);
		oneButtonMessage.Message = message;
		oneButtonMessage.Button1Callback = okCallBack;
		oneButtonMessage.Button1Text = "Yes";
		oneButtonMessage.Display();
	}
	public void PopupNotificationOneButton(string message, PopupNotificationNGUI.Callback okCallBack, string button){

		PopupNotificationNGUI oneButtonMessage = CreatePopupNotificationNGUI(popupNotificationOneButton);
		oneButtonMessage.Message = message;
		oneButtonMessage.Button1Callback = okCallBack;
		oneButtonMessage.Button1Text = button;
		oneButtonMessage.Display();
	}

	PopupNotificationNGUI CreatePopupNotificationNGUI(GameObject prefab){ // doesn't call Show(). Show() is called in Display()
		return CreatePopupNotificationNGUI(prefab, true);
	}

	PopupNotificationNGUI CreatePopupNotificationNGUI(GameObject prefab, bool startsHidden){ // doesn't call Show(). Show() is called in Display()
		// save z-value, because it gets reset when using NGUITools.AddChild(...)
		float zVal = prefab.transform.localPosition.z;
		GameObject obj = NGUITools.AddChild(centerAnchor, prefab);
		obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, zVal);
		MoveTweenToggle mtt = obj.GetComponent<MoveTweenToggle>();
		mtt.startsHidden = startsHidden;
		mtt.Reset();
		return obj.GetComponent<PopupNotificationNGUI>();
	}

	/*
		Desc: creates popup that shows an image of the badge, along with a corresponding message
		Params: badge, call back for button
	*/
	public void LevelUpMessage(BadgeTier badge, PopupNotificationNGUI.Callback okCallBack){

		LevelUpMessageNGUI oneButtonMessage = CreatePopupNotificationNGUI(levelUpMessageNGUI) as LevelUpMessageNGUI;
		oneButtonMessage.GetTrophyMessageAndImage(badge);
		oneButtonMessage.Button1Callback = okCallBack;
		oneButtonMessage.Button1Text = "OK";
		oneButtonMessage.Display();
	}

	/*
		Desc: creates popup that shows a tip, along with an image on the left and a message on the right.
		Params: message to display, tipImage, call back for button, starts hidden?, kill after used?
	*/
	public void PopupTipWithImage(string message, string spriteName, PopupNotificationNGUI.Callback okCallBack, bool startsHidden, bool killImmediately){

		PopupNotificationWithImageNGUI tip = CreatePopupNotificationNGUI(popupTipWithImageNGUI, startsHidden) as PopupNotificationWithImageNGUI;
		tip.killImmediately = killImmediately;
		tip.Message = message;
		tip.Title = "Tip";
		tip.SetSprite(spriteName);
		tip.Button1Callback = okCallBack;
		tip.Button1Text = "OK";
		tip.Display();
	}

	/*
		Desc: creates a popup that can be used at the end of a game to show points rewarded
		Params: stars, points, yes button call back, no button call back
		Note: pass in 0 for stars or points will result in the gui not showing up
	*/
	public void GameOverRewardMessage(int deltaStars, int deltaPoints,
		PopupNotificationNGUI.Callback yesButtonCallBack,
		PopupNotificationNGUI.Callback noButtonCallBack){

		GameOverRewardMessageNGUI twoButtonMessage = CreatePopupNotificationNGUI(gameOverRewardMessageTwoButtons) as GameOverRewardMessageNGUI;
		twoButtonMessage.SetRewardMessage(deltaStars, deltaPoints);
		twoButtonMessage.Button1Callback = yesButtonCallBack;
		twoButtonMessage.Button2Callback = noButtonCallBack;
		twoButtonMessage.Button1Text = "Play";
		twoButtonMessage.Button2Text = "Quit";
		twoButtonMessage.Display(false);
	}

	/*
		Desc: creates a popup reward message that only has one button(exiting button)
		Params: stars, points, yes button call back
	*/
	public void GameOverRewardMessage(int deltaStars, int deltaPoints,
		PopupNotificationNGUI.Callback yesButtonCallBack){

		GameOverRewardMessageNGUI oneButtonMessage = CreatePopupNotificationNGUI(gameOverRewardMessageOneButton) as GameOverRewardMessageNGUI;
		oneButtonMessage.SetRewardMessage(deltaStars, deltaPoints);
		oneButtonMessage.Button1Callback = yesButtonCallBack;
		oneButtonMessage.Button1Text = "Quit";
		oneButtonMessage.Display(false);

	}

	/*
		Desc: creates a popup for tutorial
		Param: tutorial sprite name, the target for call back, the function name for call back
	*/
	public void TutorialMessage(TutorialImageType imageType, GameObject target, string functionName){
		string spriteName = "";
		switch(imageType){
			case TutorialImageType.CalendarGreenStamp: spriteName = "tutorialCalendar1"; break;
			case TutorialImageType.CalendarRedStamp: spriteName = "tutorialCalendar2"; break;
			case TutorialImageType.CalendarBonus: spriteName = "tutorialCalendar3"; break;
		}

		//Spawn tutorial prefab
		float zVal = popupNotificiationTutorialLeft.transform.localPosition.z;
		GameObject obj = NGUITools.AddChild(centerAnchor, popupNotificiationTutorialLeft);
		obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, 
			obj.transform.localPosition.y, zVal);

		//Set content
		TutorialPopupManager script = obj.GetComponent<TutorialPopupManager>();
		script.SetContent(spriteName);
		script.SetButtonCallBack(target, functionName);
		script.Display();
	}
}
