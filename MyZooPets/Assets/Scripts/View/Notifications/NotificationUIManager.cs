using UnityEngine;
using System.Collections;

/// <summary>
/// Notification user interface manager.
/// Make this a child of Main Camera so it inherits the position when camera moves
/// </summary>

public class NotificationUIManager : MonoBehaviour {

	// References
	public GameObject cameraObject;
	public GameObject popupTextureGreat;
	public GameObject popupTextureNiceTry;
	public GameObject popupTextureUseTheInhaler;
	public GameObject popupTextureInhalerPractice;
	public GameObject popupAward;

	public GameObject popupNotification;
	public GameObject levelUpMessage;
	public GameObject gameOverRewardMessage;
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
	//========================Deprecated===================================

	/*
		Desc: creates a popup with a texture and stats that have been modified
		Params: notificationType, deltaPoints, deltaStars, deltaHealth, deltaMood, deltaHunger
	*/
	public void PopupTexture(string notificationType, int deltaPoints, int deltaStars,
		int deltaHealth, int deltaMood, int deltaHunger){
		switch(notificationType){
			case "award":
				GameObject go = Instantiate(popupTextureGreat, gameObject.transform.position, Quaternion.identity) as GameObject;
				Destroy(go, 3.0f);

				GameObject go2 = Instantiate(popupAward, gameObject.transform.position, Quaternion.identity) as GameObject;
				PopupAward script = go2.GetComponent<PopupAward>();		// Make sure that the award object has a script to populate
				if(script != null){
					script.Populate(deltaPoints, deltaStars, deltaHealth, deltaMood, deltaHunger);
				}else{
					Debug.LogError("Script attachment missing");
				}
				Destroy(go2, 3.0f);
			break;

			case "nice try":
				GameObject go3 = Instantiate(popupTextureNiceTry, gameObject.transform.position, Quaternion.identity) as GameObject;
				Destroy(go3, 3.0f);
			break;
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
				prefab = popupTextureGreat;
			break;

			case "intro":
				prefab = popupTextureUseTheInhaler;
			break;

			case "nice try":
				prefab = popupTextureNiceTry;
			break;
		}
		if(prefab != null){
			GameObject go = Instantiate(prefab, gameObject.transform.position, Quaternion.identity) as GameObject;
			Destroy(go, 3.0f);
		}
	}

	/*
		Desc: creates popup that has a popup texture and 2 buttons
		Params: notificationType, call back for yes button, call back for no button
	*/
	public void PopupNotification(string message, PopupNotification.OnButtonClicked yesCallBack,
		PopupNotification.OnButtonClicked noCallBack){
		GameObject go = Instantiate(popupNotification, gameObject.transform.position,
			Quaternion.identity) as GameObject;
		PopupNotification script = go.GetComponent<PopupNotification>();
		if(script != null){
			script.Init(message, yesCallBack, noCallBack);
		}
	}

	/*
		Desc: creates popup that has a popup texture and 1 button
		Params: notificationType, call back for button
	*/
	public void PopupNotification(string message, PopupNotification.OnButtonClicked okCallBack){
		GameObject go = Instantiate(popupNotification, gameObject.transform.position,
			Quaternion.identity) as GameObject;
		PopupNotification script = go.GetComponent<PopupNotification>();
		if(script != null){
			script.Init(message, okCallBack);
		}
	}

	/*
		Desc: creates popup that shows an image of the trophy, along with a corresponding message
		Params: trophy, call back for button
	*/
	public void LevelUpMessage(TrophyTier trophy, LevelUpMessage.OnButtonClicked okCallBack){

		GameObject go = Instantiate(levelUpMessage, gameObject.transform.position,
			Quaternion.identity) as GameObject;
		LevelUpMessage script = go.GetComponent<LevelUpMessage>();
		if(script != null){
			script.Init(trophy, okCallBack);
		}
	}

	/*
		Desc: creates a popup that can be used at the end of a game to show points rewarded
		Params: stars, points, yes button call back, no button call back
		Note: pass in 0 for stars or points will result in the gui not showing up
	*/
	public void GameOverRewardMessage(int deltaStars, int deltaPoints,
		GameOverRewardMessage.OnButtonClicked yesButtonCallBack,
		GameOverRewardMessage.OnButtonClicked noButtonCallBack){

		GameObject go = Instantiate(gameOverRewardMessage, gameObject.transform.position,
			Quaternion.identity) as GameObject;
		GameOverRewardMessage script = go.GetComponent<GameOverRewardMessage>();
		if(script != null){
			script.Init(deltaStars, deltaPoints, yesButtonCallBack, noButtonCallBack);
		}
	}

	/*
		Desc: creates a popup reward message that only has one button(exiting button)
		Params: stars, points, yes button call back
	*/
	public void GameOverRewardMessage(int deltaStars, int deltaPoints,
		GameOverRewardMessage.OnButtonClicked yesButtonCallBack){

		GameObject go = Instantiate(gameOverRewardMessage, gameObject.transform.position,
			Quaternion.identity) as GameObject;
		GameOverRewardMessage script = go.GetComponent<GameOverRewardMessage>();
		if(script != null){
			script.Init(deltaStars, deltaPoints, yesButtonCallBack);
		}

	}
}
