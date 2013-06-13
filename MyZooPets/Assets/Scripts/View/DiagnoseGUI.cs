using UnityEngine;
using System.Collections;

public class DiagnoseGUI : MonoBehaviour {
	
	public GameObject cameraMoveObject;
	private CameraMove cameraMove;
	
	public GameObject roomGuiObject;
	private RoomGUI roomGui;
	
	private bool isActive = false;
	
	public GUISkin defaultSkin;
	
	void Start () {
		roomGui = roomGuiObject.GetComponent<RoomGUI>();
		cameraMove = cameraMoveObject.GetComponent<CameraMove>();
	}
	
	void Update () {
	
	}
	
	void OnGUI(){
		GUI.skin = defaultSkin;
		if(isActive){
			if(GUI.Button(new Rect(10, 10, 100, 100), "X")){
				cameraMove.PetSideZoomToggle();
				roomGui.ShowGUIs();	
				isActive = false;
			}
		}
	}
	
	public void DiagnoseClicked(){
		if(!ClickManager.isClickLocked && !ClickManager.isModeLocked){
			isActive = true;
			ClickManager.ClickLock();
			ClickManager.ModeLock();
			cameraMove.PetSideZoomToggle();
			roomGui.HideGUIs();
		}
	}
}
