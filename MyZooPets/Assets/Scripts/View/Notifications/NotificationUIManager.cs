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


	public Texture2D healthIcon;
	public Texture2D moodIcon;
	public Texture2D hungerIcon;
	public Texture2D starIcon;
	public bool flipped;

	void Start(){
		if (!flipped){
			gameObject.transform.position = new Vector3(cameraObject.transform.position.x, cameraObject.transform.position.y - 1f, cameraObject.transform.position.z + 4f);
		}
		else {
			gameObject.transform.position = new Vector3(cameraObject.transform.position.x, cameraObject.transform.position.y - 1f, cameraObject.transform.position.z - 4f);
		}
	}

	void Update(){

	}

	//TODO-s some kind of complex hashmap storage for references? TODO-s particle not used
	public void PopupTexture(string message, string particle){
		if(message == "great"){
			GameObject go = Instantiate(popupTextureGreat, gameObject.transform.position, Quaternion.identity) as GameObject;
			Destroy(go, 3.0f);

			//GameObject go2 = Instantiate(popupAward, gameObject.transform.position, Quaternion.identity) as GameObject;
			//Destroy(go2, 3.0f);
		}



	}
}
