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

	private int nTweenCount = 0;		// if this is > 0, a tween that affects the UI is happening
	
	// list of things that can still happen despite a click lock being in effect...this may be a bad idea...(Joe)
	private List<ClickLockExceptions> listExceptions = new List<ClickLockExceptions>();
	
	// list of TEMPORARY exceptions; these exceptions go away whenever the screen is LOCKED
	// "temporary" might be a bad way to describe these exceptions.  Normal exceptions are added when the screen is locked.
	// These exceptions can be added anyway, but expire the next time the screen is locked.
	private List<ClickLockExceptions> listTempExceptions = new List<ClickLockExceptions>();
	
	// stack of modes that have locked the click manager
	private Stack<UIModeTypes> stackModes = new Stack<UIModeTypes>();
	
    public string stackPeek;
	public int count;
	
    void Awake(){
    }
	
	void Update(){
		count = stackModes.Count;
		if(count != 0){
			UIModeTypes eCurrentMode = stackModes.Peek();
			stackPeek = eCurrentMode.ToString();
		}
		else{
			stackPeek = null;
		}
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
	// their clicks.
	// Note that the order of these checks is actually important,
	// so don't go changing things willy-nilly.
	//---------------------------------------------------------
	public bool CanRespondToTap( GameObject goCaller = null, ClickLockExceptions eException = ClickLockExceptions.None ){
		// hard stop (for now): If the partition is transitioning, don't allow anything
		if ( CameraManager.Instance && CameraManager.Instance.IsCameraMoving() )
			return false;
		
		// if a tutorial is playing, check with that tutorial
		if ( TutorialManager.Instance && !TutorialManager.Instance.CanProcess( goCaller ) )
			return false;
		
		// if there is an exception in effect for the incoming action, then it can bypass the mode check
		// this check should appear BEFORE the tweening checks because exceptions should be an auto-accept
		if ( listExceptions.Contains( eException ) || listTempExceptions.Contains( eException ) )
			return true;		
		
		// if the UI is tweening, no soup for you
		bool bIsTweening = IsTweeningUI();
		if ( bIsTweening )
			return false;
		
		// get the mode key from the incoming object, if it is an LgButton.
		// it's possible goCaller is not an LgButton, which should be fine.
		UIModeTypes eCallingMode = UIModeTypes.None;
		if ( goCaller != null ) {
			LgButton button = goCaller.GetComponent<LgButton>();
			if ( button )
				eCallingMode = button.GetMode();
			
			// removing this because I wasn't sure if it was really necessary
			//else if ( button == null && eException == ClickLockExceptions.None )
			//	Debug.LogError( "Non-button is checking click manager without an exception...not sure what to do", goCaller);
		}
		
		// get the current mode
		UIModeTypes eCurrentMode = UIModeTypes.None;
		if ( stackModes.Count > 0 )
			eCurrentMode = stackModes.Peek();
		
		// we are at the end of our checks now, so if we have made it this far, if the click manager is not actually locked
		// (current mode is None), the current mode is equal to the incoming button's mode, or the incoming button's mode is none,
		// then the click can be processed.
		if ( eCurrentMode == UIModeTypes.None || eCallingMode == eCurrentMode || ( eCallingMode == UIModeTypes.None && eCurrentMode == UIModeTypes.None ) )
			return true;
		
		// otherwise some condition(s) above was not met, so return false
		return false;
	}
	
	///////////////////////////////////////////
	// AddTemporaryException()
	// Adds an exception temporarily.
	///////////////////////////////////////////		
	public void AddTemporaryException( ClickLockExceptions eException ) {
		listTempExceptions.Add( eException );
	}
	
	///////////////////////////////////////////
	// Lock()
	// Locks the click manager with an incoming
	// mode and list of exceptions.
	///////////////////////////////////////////		
	public void Lock( UIModeTypes eMode = UIModeTypes.Generic, List<ClickLockExceptions> listExceptions = null ) {		
		// because the screen is being locked, we must now reset temporary exceptions (but only if the stack of modes was empty)
		if ( stackModes.Count == 0 ) {
			listTempExceptions = new List<ClickLockExceptions>();
			
			if ( listExceptions != null )
				this.listExceptions = listExceptions;			
		}
		else if ( stackModes.Count > 0 && listExceptions != null ){
			Debug.Log("Something is trying to lock the click manager without an empty stack but with exceptions...this is not currently supported");	
			foreach(ClickLockExceptions e in listExceptions){
				print(e);
			}
		}
			
		// push this latest mode
		stackModes.Push( eMode );
	}
	
	//---------------------------------------------------
	// ReleaseLock()
	// Effectively pops off one level of click manager
	// locking.
	//---------------------------------------------------	
	public void ReleaseLock() {
		try{
			// lock is being released, so pop the stack
			stackModes.Pop();
		}
		catch(InvalidOperationException e){
			Debug.LogError("Trying to pop an empty stack. (ClickManager) " + e.Message);
		}
		
		// if the stack is empty, reset our exceptions
		if ( stackModes.Count == 0 )
			listExceptions = new List<ClickLockExceptions>();
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
			Debug.LogError("Warning...something decrementing tween count when it is 0");
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
