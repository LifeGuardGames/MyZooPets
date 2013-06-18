using UnityEngine;
using System.Collections;

public class SeansButtons : MonoBehaviour {
	
	public GameObject notificationManagerObject;
	private NotificationUIManager nuim;
	
	public GameObject inhalerGameGUIObject;
//	private InhalerGameGUI igg;
	
	void Start(){
		nuim = notificationManagerObject.GetComponent<NotificationUIManager>();
//		igg = inhalerGameGUIObject.GetComponent<InhalerGameGUI>();
	}
	
	void Update(){
	}
	
	void OnGUI(){
		if(GUI.Button(new Rect(10, 10, 100, 100), "KABOOYA")){
			nuim.PopupTexture("great", 0, 0, 0, 0, 0);
		}
		if(GUI.Button(new Rect(120, 10, 100, 100), "WAPOWWW")){
			//igg.IncreaseBar();
		}
		if(GUI.Button(new Rect(230, 10, 100, 100), "CHACHING")){
			//igg.IncreaseBar();
		}
	}
}
