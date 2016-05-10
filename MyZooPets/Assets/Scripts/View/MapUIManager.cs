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

	private float zoomTime = 0.5f;
	public Vector3 vOffset;
	public Vector3 vRotation;

	public GameObject goBackButton; // back button for the chart
	public BoxCollider draggableCollider; // The collider that is dragged, to be enabled only when zoomed in
	public DragRecognizer dragRecognizerFingerGesture; //need to temporary disable this script when zoomed into map
													//OnDrag event from FingerGesture conflicts with NGUI. Need to
													//set them on diff layer. this is only a quick fix
	public List<MapEntry> mapEntries = new List<MapEntry>();

	protected override void Start(){
		base.Start();
		draggableCollider.enabled = false;

		GatingManager.OnDamageGate += RefreshMapEntry;
		RefreshMapEntry(this, EventArgs.Empty);
	}

	protected override void OnDestroy(){
		base.OnDestroy();
		GatingManager.OnDamageGate -= RefreshMapEntry;
	}

	//---------------------------------------------------
	// _OpenUI()
	//---------------------------------------------------
	protected override void _OpenUI(){
		// Zoom into the chart
		Vector3 vPos = transform.position + vOffset;
		CameraManager.Instance.ZoomToTarget(vPos, vRotation, zoomTime, null);
		
		// Hide other UI objects
		NavigationUIManager.Instance.HidePanel();
		HUDUIManager.Instance.HidePanel();
		InventoryUIManager.Instance.HidePanel();
		RoomArrowsUIManager.Instance.HidePanel();

		// disable the collider so the user can't click the chart again
		gameObject.GetComponent<Collider>().enabled = false;

		// disable FingerGesture OnDrag event
		dragRecognizerFingerGesture.enabled = false;
		
		// enable the back button for the user to back out
		goBackButton.SetActive(true);
		draggableCollider.enabled = true;
	}
	
	//---------------------------------------------------
	// _CloseUI()
	//---------------------------------------------------
	protected override void _CloseUI(){
		// enable the collider so that the board can be clicked again
		gameObject.GetComponent<Collider>().enabled = true;

		// enable FingerGesture OnDrag event
		dragRecognizerFingerGesture.enabled = true;
		
		// zoom out
		CameraManager.Instance.ZoomOutMove();
		
		//Show other UI Objects
		NavigationUIManager.Instance.ShowPanel();
		HUDUIManager.Instance.ShowPanel();
		InventoryUIManager.Instance.ShowPanel();
		RoomArrowsUIManager.Instance.ShowPanel();

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
