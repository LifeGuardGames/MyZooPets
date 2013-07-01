using UnityEngine;
using System.Collections;

public class TrophyGUI : MonoBehaviour {

	public GameObject cameraMoveObject;
	private CameraMove cameraMove;

	public GameObject roomGuiObject;
	private RoomGUI roomGui;

	public GUISkin defaultSkin;
	public Texture2D backButton;
	public GUIStyle blankButtonStyle;

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
        	if(GUI.Button(new Rect(10, 10, backButton.width, backButton.height), backButton, blankButtonStyle)){
				ClickManager.ClickLock();
				cameraMove.ShelfZoomToggle();
				roomGui.ShowGUIs();
				isActive = false;
			}
		}
	}
}
