using UnityEngine;
using System.Collections;
using System;

public class TrophyGUI : MonoBehaviour {

	//======================Event=============================
    public static event EventHandler<EventArgs> OnTrophyClosed;
    //=======================================================

	public GameObject cameraMoveObject;
	private CameraMove cameraMove;

	/*
	    trophyShelf: Used mainly for shelf and trophies right now. The shelf's collider blocks the trophy colliders, so we
	    are disabling the shelf's collider here.
	*/
	public GameObject trophyShelf;

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
			trophyShelf.collider.enabled = false;
		}
	}

	void OnGUI(){
		GUI.skin = defaultSkin;
		if(isActive && !ClickManager.Instance.isClickLocked){ // checking isClickLocked because trophy shelf back button should not be clickable if there is a notification
        	if(GUI.Button(new Rect(10, 10, backButton.width, backButton.height), backButton, blankButtonStyle)){
        		if(OnTrophyClosed != null) OnTrophyClosed(this, EventArgs.Empty);
				isActive = false;
				trophyShelf.collider.enabled = true;
			}
		}
	}
}
