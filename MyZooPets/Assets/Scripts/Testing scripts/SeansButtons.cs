using UnityEngine;
using System.Collections;

public class SeansButtons : MonoBehaviour {
	
	public GameObject notificationManagerObject;
	private NotificationUIManager nuim;
	
	void Start () {
		nuim = notificationManagerObject.GetComponent<NotificationUIManager>();
	}
	
	void Update () {
	}
	
	void OnGUI(){
		if(GUI.Button(new Rect(10, 10, 100, 100), "KABOOYA")){
			nuim.PopupTexture("great","");
		}
	}
}
