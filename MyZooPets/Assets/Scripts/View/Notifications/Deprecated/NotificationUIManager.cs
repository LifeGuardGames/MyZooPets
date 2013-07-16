using UnityEngine;
using System.Collections;

/// <summary>
/// Notification user interface manager.
/// Make this a child of Main Camera so it inherits the position when camera moves
/// </summary>

public class NotificationUIManager : MonoBehaviour {

	// References
	public GameObject cameraObject;
	public GameObject NguiAnchor;
	public GameObject popupTextureGreatNGUI;
	public GameObject popupTextureNiceTryNGUI;
	public GameObject popupTextureUseTheInhalerNGUI;
	public GameObject popupTexturePracticeInhalerNGUI;
	public GameObject popupTextureDiagnoseSymptomsNGUI;

	public GameObject popupNotificationOneButton; // NGUI as well
	public GameObject popupNotificationTwoButtons; // NGUI as well
	public GameObject levelUpMessageNGUI;
	public GameObject gameOverRewardMessageOneButton; // NGUI as well
	public GameObject gameOverRewardMessageTwoButtons; // NGUI as well
	public bool flipped;

	//

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
			GameObject go = CreateNGUIObject(prefab);
			Destroy(go, 3.0f);

			// show regular intro after announcing that it is a practice game
			Invoke("ShowIntro", 3.0f);
		}
		else if(prefab != null){
			// GameObject go = Instantiate(prefab, gameObject.transform.position, Quaternion.identity) as GameObject;
			GameObject go = CreateNGUIObject(prefab);
			Destroy(go, 3.0f);
		}
	}

	GameObject CreateNGUIObject(GameObject prefab){
		GameObject obj = Instantiate(prefab) as GameObject;
		Vector3 originalPos = obj.transform.position;
        obj.transform.parent = NguiAnchor.transform;
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = originalPos;
        return obj;
	}

	// used to show regular intro after announcing that it is a practice game
	private void ShowIntro () {
		// GameObject intro = Instantiate(popupTextureUseTheInhaler, gameObject.transform.position, Quaternion.identity) as GameObject;
		// Destroy(intro, 3.0f);
		GameObject go = CreateNGUIObject(popupTextureUseTheInhalerNGUI);
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

	PopupNotificationNGUI CreatePopupNotificationNGUI(GameObject prefab){
		GameObject message = Instantiate(prefab) as GameObject;
		Vector3 originalPos = message.transform.position;
        message.transform.parent = NguiAnchor.transform;
        message.transform.localScale = Vector3.one;
        message.transform.localPosition = originalPos;
        return message.GetComponent<PopupNotificationNGUI>();
	}

	/*
		Desc: creates popup that shows an image of the trophy, along with a corresponding message
		Params: trophy, call back for button
	*/
	public void LevelUpMessage(TrophyTier trophy, PopupNotificationNGUI.Callback okCallBack){

		LevelUpMessageNGUI oneButtonMessage = CreatePopupNotificationNGUI(levelUpMessageNGUI) as LevelUpMessageNGUI;
		oneButtonMessage.GetTrophyMessageAndImage(trophy);
		oneButtonMessage.Button1Callback = okCallBack;
		oneButtonMessage.Button1Text = "OK";
		oneButtonMessage.Display();
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
		twoButtonMessage.Display();
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
		oneButtonMessage.Display();

	}
}
