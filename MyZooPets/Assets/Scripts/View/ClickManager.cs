using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// Click manager.
/// All the classes that need a click to enter a certain mode will be handled here (ie. diary, badge, inhaler game)
///
///
/// NOTE: When entering a mode, lock click and mode, when done transitioning, unlock click
///       When exiting a mode, unlock click and mode after finish transitioning
///
/// </summary>

public class ClickManager : Singleton<ClickManager> {

    public static GameObject UIRoot; // this is used to add a collider to the UIRoot to stop non-GUI elements from being clicked when GUI menus are active

	public bool isClickLocked;	// Lock to prevent multiple clicking (diary + trophy modes at the same time)
	public bool isModeLocked;	// Lock to prevent clicking other objects when zoomed into a mode (clicking diary in trophy more)
	private int nTweenCount = 0;		// if this is > 0, a tween that affects the UI is happening
	
	// the current mode the UI is in
	private UIModeTypes eCurMode;
	
	// list of things that can still happen despite a click lock being in effect...this may be a bad idea...(Joe)
	private List<ClickLockExceptions> listExceptions;
	
    //bool trophyMessageShowing = false;

    void Awake(){
		isClickLocked = false;
		isModeLocked = false;
    }

	//Clean all event listeners and static references
	void OnDestroy(){
		UIRoot = null;
	}

	// If set to true (GUI menus are active), add a collider in UIRoot to stop user from clicking on anything else.
	// If set to false, deactivate the collider.
	// we may be able to remove this!
	public static void SetActiveGUIModeLock(bool GUIActive){
		if (UIRoot == null){
			UIRoot = GameObject.Find("UI Root (2D)");
		}
		BoxCollider col = UIRoot.collider as BoxCollider;
		if (UIRoot.collider == null){
			col = UIRoot.AddComponent<BoxCollider>();
			col.center = new Vector3(0,0,50); // so this collider is behind all actual GUI elements and won't interfere with them
			col.size = new Vector3(3000, 3000, 1); // this should be big enough to account for all different resolutions
		}
		col.enabled = GUIActive;
	}
	
	//---------------------------------------------------------
	// CanRespondToTap
	// UI buttons call this to make sure that they can process
	// their clicks.  It makes sure that 
	// A) Clicking is not locked
	// B) Mode is not locked
	// C) The UI isn't tweening
	//---------------------------------------------------------
	public bool CanRespondToTap( ClickLockExceptions eException = ClickLockExceptions.None ){
		// if there is an Exception in effect for the incoming action, then it is okay regardless
		if ( listExceptions != null && listExceptions.Contains( eException ) )
			return true;
		
		bool bIsTweening = IsTweeningUI();
		if(!isClickLocked && !isModeLocked && !bIsTweening){
			return true;
		}
		return false;
	}

	// Disable clicking when transitioning between modes
	public void ClickLock( List<ClickLockExceptions> listExceptions = null){
		isClickLocked = true;
		
		if ( listExceptions != null )
			this.listExceptions = listExceptions;
	}

	// Enable clicking after the transitioning is done, usually called from LeanTween callback
	public void ReleaseClickLock(){
		// clear our exceptions
		listExceptions = null;
		
		isClickLocked = false;
	}

	// Disable clicking other objects inside a mode (ie, cant click shelf when in trophy mode)
	public void ModeLock( UIModeTypes eMode ){
		eCurMode = eMode;
		isModeLocked = true;
	}
	
	// get the current mode
	public UIModeTypes GetCurrentMode() {
		return eCurMode;
	}

	// Enable clicking other objects after completed exiting a mode
	public void ReleaseModeLock(){
		isModeLocked = false;
		SetActiveGUIModeLock(false);
	}
	
	//---------------------------------------------------
	// IncrementTweenCount()
	// When a move tween kicks off, if it should lock the
	// UI, call this function.
	//---------------------------------------------------
	public void IncrementTweenCount() {
		nTweenCount++;
	}
	
	//---------------------------------------------------
	// DecrementTweenCount()
	// When a move tween kicks finishes, if it should lock the
	// UI, call this function.
	//---------------------------------------------------	
	public void DecrementTweenCount() {
		if ( nTweenCount <= 0 ) {
			Debug.Log("Warning...something decrementing tween count when it is 0");
			return;
		}
		
		nTweenCount--;
	}
	
	//---------------------------------------------------
	// IsTweeningUI()
	// Returns if there are tweens that lock the UI
	// currently running.
	//---------------------------------------------------	
	public bool IsTweeningUI() {
		bool bIsTweening = nTweenCount > 0;
		
		return bIsTweening;
	}
}
