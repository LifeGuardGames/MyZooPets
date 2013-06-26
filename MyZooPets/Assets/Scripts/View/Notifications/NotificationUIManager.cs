using UnityEngine;
using System.Collections;

/// <summary>
/// Notification user interface manager.
/// Make this a child of Main Camera so it inherits the position when camera moves
/// </summary>

public class NotificationUIManager : MonoBehaviour {

	//references
	public GameObject cameraObject;
	public GameObject popupTextureGreat;
	public GameObject popupTextureUseTheInhaler;
	public GameObject popupAward;
	public GameObject popupSpeech;
	public GameObject popupNotification;

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

	//TODO-s some kind of complex hashmap storage for references? TODO-s particle not used
	/*
		Desc: creates a popup with a texture and stats that have been modified
		Params: notificationType, deltaPoints, deltaStars, deltaHealth, deltaMood, deltaHunger
	*/
	public void PopupTexture(string notificationType, int deltaPoints, int deltaStars, 
		int deltaHealth, int deltaMood, int deltaHunger){
		switch(notificationType){
			case "award":
				GameObject go = Instantiate(popupTextureGreat, gameObject.transform.position, 
					Quaternion.identity) as GameObject;
				Destroy(go, 3.0f);

				GameObject go2 = Instantiate(popupAward, gameObject.transform.position, 
					Quaternion.identity) as GameObject;
				PopupAward script = go2.GetComponent<PopupAward>();
				if(script != null){
					script.Populate(deltaPoints, deltaStars, deltaHealth, deltaMood, deltaHunger);
				}else{
					Debug.LogError("Script attachment missing");
				}
				Destroy(go2, 3.0f);
			break;
			case "nicetry":

			break;
		}

	}

	/*
		Desc: creates a popup with only a texture
		Params: notificationType
	*/
	public void PopupTexture(string notificationType){
		GameObject prefabName = null;
		switch(notificationType){
			case "great":
				prefabName = popupTextureGreat;
			break;
			case "intro":
				prefabName = popupTextureUseTheInhaler;
			break;
		}
		if(prefabName != null){
			GameObject go = Instantiate(prefabName, gameObject.transform.position, 
				Quaternion.identity) as GameObject;
			Destroy(go, 3.0f);
		}
	}

	/*
		Desc: creates popup that has a popup texture and 2 buttons
		Params: notificationType, call back for yes button, call back for no button
	*/
	public void PopupNotification(string notificationType, PopupNotification.OnButtonClicked yesCallBack, 
		PopupNotification.OnButtonClicked noCallBack){
		GameObject go = Instantiate(popupNotification, gameObject.transform.position, 
			Quaternion.identity) as GameObject;
		PopupNotification script = go.GetComponent<PopupNotification>();
		if(script != null){
			script.Init(notificationType, yesCallBack, noCallBack);
		}
	}

	public void PopupSpeechBubble(){

	}
}
