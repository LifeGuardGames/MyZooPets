using UnityEngine;
using System.Collections;

/// <summary>
/// Notification user interface manager.
/// Make this a child of Main Camera so it inherits the position when camera moves
/// </summary>

public class NotificationUIManager : MonoBehaviour {

	public GameObject cameraObject;

	public GameObject popupTextureGreat;
	public GameObject popupTextureUseTheInhaler;
	public GameObject popupAward;
	public GameObject popupSpeech;
	public GameObject popupNotification;

	public bool flipped;


		
	void Start(){
		if (!flipped){
			gameObject.transform.position = new Vector3(cameraObject.transform.position.x, cameraObject.transform.position.y - 1f, cameraObject.transform.position.z + 4f);
		}
		else {
			gameObject.transform.position = new Vector3(cameraObject.transform.position.x, cameraObject.transform.position.y - 1f, cameraObject.transform.position.z - 4f);
		}
	}

	//TODO-s some kind of complex hashmap storage for references? TODO-s particle not used
	//use this method to display any sort of notification or popup
	public void PopupTexture(string message, int deltaPoints, int deltaStars, int deltaHealth, int deltaMood, int deltaHunger){
		if(message == "great"){
			GameObject go = Instantiate(popupTextureGreat, gameObject.transform.position, Quaternion.identity) as GameObject;
			Destroy(go, 3.0f);
		}
		if(message == "award"){
			GameObject go = Instantiate(popupTextureGreat, gameObject.transform.position, Quaternion.identity) as GameObject;
			Destroy(go, 3.0f);

			GameObject go2 = Instantiate(popupAward, gameObject.transform.position, Quaternion.identity) as GameObject;
			PopupAward script = go2.GetComponent<PopupAward>();
			if(script != null){
				script.Populate(deltaPoints, deltaStars, deltaHealth, deltaMood, deltaHunger);
				//Invoke("AwardPoints", 1.5f); // Awardance of points in done in gameLogic
			}
			else{
				Debug.LogError("Script attachment missing");
			}
			Destroy(go2, 3.0f);
		}
	}

	//use this method to display an notification that has an icon, message, yes button, and no button
	public void PopupNotification(string message, PopupNotification.OnButtonClicked yesCallBack, 
		PopupNotification.OnButtonClicked noCallBack){
		GameObject go = Instantiate(popupNotification, gameObject.transform.position, Quaternion.identity) as GameObject;
		PopupNotification script = go.GetComponent<PopupNotification>();
		if(script != null){
			print("working");
			script.Init(message, yesCallBack, noCallBack);
		}
	}

	public void PopupTexture(string message){
		if(message == "great"){
			GameObject go = Instantiate(popupTextureGreat, gameObject.transform.position, Quaternion.identity) as GameObject;
			Destroy(go, 3.0f);
		}
		if (message == "intro"){
			GameObject go = Instantiate(popupTextureUseTheInhaler, gameObject.transform.position, Quaternion.identity) as GameObject;
			Destroy(go, 3.0f);
		}
	}

	public void PopupSpeechBubble(){

	}
}
