﻿using UnityEngine;
using System.Collections;

//---------------------------------------------------
// MapUIManager
// UI Manager for the map that displays the
// user's current and future room progressions.
//---------------------------------------------------

public class MapUIManager : SingletonUI<MapUIManager> {
	// zoom helper
	public ZoomHelper zoomHelper;
	
	// back button for the chart
	public GameObject goBackButton;

	public BoxCollider draggableCollider;	// The collider that is dragged, to be enabled only when zoomed in

	public Color bedroomActiveColor;
	public Color bedroomInactiveColor;

	public Color yardActiveColor;
	public Color yardInactiveColor;

	protected override void _Start(){
		draggableCollider.enabled = false;
	}

	//---------------------------------------------------
	// _OpenUI()
	//---------------------------------------------------
	protected override void _OpenUI(){
		// zoom into the chart
		zoomHelper.Zoom();
		
		//Hide other UI objects
		NavigationUIManager.Instance.HidePanel();
		HUDUIManager.Instance.HidePanel();
		InventoryUIManager.Instance.HidePanel();
		EditDecosUIManager.Instance.HideNavButton();
		
		// disable the collider so the user can't click the chart again
		gameObject.collider.enabled = false;
		
		// enable the back button for the user to back out
		goBackButton.SetActive(true);
		draggableCollider.enabled = true;
	}
	
	//---------------------------------------------------
	// _CloseUI()
	//---------------------------------------------------
	protected override void _CloseUI(){
		// enable the collider so that the board can be clicked again
		gameObject.collider.enabled = true;
		
		// zoom out
		CameraManager.Instance.ZoomOutMove();
		
		//Show other UI Objects
		NavigationUIManager.Instance.ShowPanel();
		HUDUIManager.Instance.ShowPanel();
		InventoryUIManager.Instance.ShowPanel();
		EditDecosUIManager.Instance.ShowNavButton();
		
		// deactivate the back button
		goBackButton.SetActive(false);
		draggableCollider.enabled = false;
	}	
}
