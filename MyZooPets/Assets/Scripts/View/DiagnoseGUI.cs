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
				ClickManager.ReleaseLock();
				cameraMove.PetSideZoomToggle();
				roomGui.ShowGUIs();	
				isActive = false;
			}
		}
	}
	
	public void DiagnoseClicked(){
		isActive = true;
		ClickManager.Lock();
		cameraMove.PetSideZoomToggle();
		roomGui.HideGUIs();
	}
}
