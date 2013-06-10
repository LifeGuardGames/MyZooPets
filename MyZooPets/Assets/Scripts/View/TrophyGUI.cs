using UnityEngine;
using System.Collections;

public class TrophyGUI : MonoBehaviour {
	
	public GameObject cameraMoveObject;
	private CameraMove cameraMove;
	
	public GameObject roomGuiObject;
	private RoomGUI roomGui;
	
	private bool isActive = false;
	
	void Start(){
		cameraMove = cameraMoveObject.GetComponent<CameraMove>();
		roomGui	= roomGuiObject.GetComponent<RoomGUI>();
	}
	
	public void TrophyClicked(){
		if(!isActive){
			isActive = true;
			cameraMove.ShelfZoomToggle();
			roomGui.HideGUIs();
		}
	}
	
	void Update(){
		
	}
	
	void OnGUI(){
		if(isActive){
			if(GUI.Button(new Rect(10, 10, 100, 100), "X")){
				cameraMove.ShelfZoomToggle();
				roomGui.ShowGUIs();	
				isActive = false;
			}
		}
	}
}
