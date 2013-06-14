using UnityEngine;
using System.Collections;

public class TrophyGUI : MonoBehaviour {
	
	public GameObject cameraMoveObject;
	private CameraMove cameraMove;
	
	public GameObject roomGuiObject;
	private RoomGUI roomGui;
	
	public GUISkin defaultSkin;
	
	private bool isActive = false;
	
	void Start(){
		cameraMove = cameraMoveObject.GetComponent<CameraMove>();
		roomGui	= roomGuiObject.GetComponent<RoomGUI>();
	}
	
	public void TrophyClicked(){
		if(!isActive){
			isActive = true;
			cameraMove.ShelfZoomToggle();
			roomGui.HideGUIs(true, true, true, true);
		}
	}
	
	void Update(){
		
	}
	
	void OnGUI(){
		GUI.skin = defaultSkin;
		if(isActive){
			if(GUI.Button(new Rect(10, 10, 100, 100), "X")){
				ClickManager.ClickLock();
				cameraMove.ShelfZoomToggle();
				roomGui.ShowGUIs();	
				isActive = false;
			}
		}
	}
}
