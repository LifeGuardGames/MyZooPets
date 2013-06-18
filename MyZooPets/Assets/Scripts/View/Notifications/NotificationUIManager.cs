using UnityEngine;
using System.Collections;

/// <summary>
/// Notification user interface manager.
/// Make this a child of Main Camera so it inherits the position when camera moves
/// </summary>

public class NotificationUIManager : MonoBehaviour {

	public GameObject cameraObject;

	public GameObject popupTextureGreat;
	public GameObject popupAward;
	public GameObject popupSpeech;

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
	
	public void PopupSpeechBubble(){
		
	}
}
