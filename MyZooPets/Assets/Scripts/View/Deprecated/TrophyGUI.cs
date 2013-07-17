using UnityEngine;
using System.Collections;
using System;

public class TrophyGUI : MonoBehaviour {

	//======================Event=============================
    public delegate void CallBack(object senders, EventArgs e);
    public static event CallBack OnTrophyClosed;
    //=======================================================

	public GameObject cameraMoveObject;
	private CameraMove cameraMove;


	public GUISkin defaultSkin;
	public Texture2D backButton;
	public GUIStyle blankButtonStyle;

	private bool isActive = false;

	void Start(){
		cameraMove = cameraMoveObject.GetComponent<CameraMove>();
	}

	public void TrophyClicked(){
		if(!isActive){
			isActive = true;
			//cameraMove.ShelfZoomToggle();
			cameraMove.ZoomToggle(ZoomItem.TrophyShelf);
		}
	}

	void OnGUI(){
		GUI.skin = defaultSkin;
		if(isActive && !ClickManager.isClickLocked){ // checking isClickLocked because trophy shelf back button should not be clickable if there is a notification
        	if(GUI.Button(new Rect(10, 10, backButton.width, backButton.height), backButton, blankButtonStyle)){
        		if(OnTrophyClosed != null) OnTrophyClosed(this, EventArgs.Empty);
				isActive = false;
			}
		}
	}
}
