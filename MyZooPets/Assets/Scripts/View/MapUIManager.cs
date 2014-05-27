using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// MapUIManager
// UI Manager for the map that displays the
// user's current and future room progressions.
//---------------------------------------------------

public class MapUIManager : SingletonUI<MapUIManager> {

	public ZoomHelper zoomHelper; // zoom helper
	public GameObject goBackButton; // back button for the chart
	public BoxCollider draggableCollider; // The collider that is dragged, to be enabled only when zoomed in
	public Color bedroomActiveColor;
	public Color bedroomInactiveColor;
	public Color yardActiveColor;
	public Color yardInactiveColor;
	public List<MapEntry> mapEntries = new List<MapEntry>();

	protected override void _Start(){
		draggableCollider.enabled = false;

		GatingManager.OnDamageGate += RefreshMapEntry;
		RefreshMapEntry(this, EventArgs.Empty);
	}

	void OnDestroy(){
		GatingManager.OnDamageGate -= RefreshMapEntry;
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

	private void RefreshMapEntry(object sender, EventArgs args){
		foreach(MapEntry entry in mapEntries){
			bool hasActiveGate = GatingManager.Instance.HasActiveGate(entry.Area, entry.RoomPartition);
			
			if(hasActiveGate)
				entry.Lock();
			else
				entry.Unlock();
		}
	}
}
