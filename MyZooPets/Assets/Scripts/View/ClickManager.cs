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

public class ClickManager : Singleton<ClickManager>{

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

	#region debug variable dont use
	public string stackPeek;
	public int count;
	#endregion
	
	/// <summary>
	/// Gets the current mode. The mode that the click manager is locked at
	/// </summary>
	/// <value>The current mode.</value>
	public UIModeTypes CurrentMode{
		get{
			UIModeTypes retVal = UIModeTypes.None;
			if(stackModes.Count != 0)
				retVal = stackModes.Peek();

			return retVal;
		}
	}

	/// <summary>
	/// Indicate clicking is not locked at the moment
	/// </summary>
	public bool IsModeLockEmpty{
		get{ return stackModes.Count == 0;}
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
	
	/// <summary>
	/// Sets the active GUI mode lock.
	/// </summary>
	/// <param name="GUIActive">If set to <c>true</c> GUI active.</param>
	public static void SetActiveGUIModeLock(bool GUIActive){
		if(UIRoot == null){
			UIRoot = GameObject.Find("UI Root (2D)");
		}
		BoxCollider col = UIRoot.collider as BoxCollider;
		if(UIRoot.collider == null){
			col = UIRoot.AddComponent<BoxCollider>();
			col.center = new Vector3(0, 0, 50); // so this collider is behind all actual GUI elements and won't interfere with them
			col.size = new Vector3(3000, 3000, 1); // this should be big enough to account for all different resolutions
		}
		col.enabled = GUIActive;
	}

	//TODO: Refactor this spageti code
	/// <summary>
	/// UI buttons call this to make sure that they can process their clicks.
	/// Note: the order of these checks is actually important, so don't go changing
	/// things willy-nilly.
	/// </summary>
	/// <returns><c>true</c> if this instance can respond to tap; otherwise, <c>false</c>.</returns>
	/// <param name="goCaller">Go caller.</param>
	/// <param name="eException">E exception.</param>
	public bool CanRespondToTap(GameObject goCaller = null, ClickLockExceptions eException = ClickLockExceptions.None){

		// hard stop (for now): If the partition is transitioning, don't allow anything
		if(CameraManager.Instance && CameraManager.Instance.IsCameraMoving())
			return false;

		// if pet is currently attacking gate. can't do anything else
		if(AttackGate.Instance)
			return false;

		// if a tutorial is playing, check with that tutorial
		if(TutorialManager.Instance && !TutorialManager.Instance.CanProcess(goCaller))
			return false;

		// if there is an exception in effect for the incoming action, then it can bypass the mode check
		// this check should appear BEFORE the tweening checks because exceptions should be an auto-accept
		if(listExceptions.Contains(eException) || listTempExceptions.Contains(eException))
			return true;		
		
		// if the UI is tweening, no soup for you
		bool isTweening = IsTweeningUI();
		if(isTweening)
			return false;

		// get the current mode
		UIModeTypes currentUIMode = UIModeTypes.None;
		if(stackModes.Count > 0)
			currentUIMode = stackModes.Peek();

		// get the mode key from the incoming object, if it is an LgButton.
		// it's possible goCaller is not an LgButton, which should be fine.
		UIModeTypes callingMode = UIModeTypes.None;
		if(goCaller != null){
			LgButton button = goCaller.GetComponent<LgButton>();
			if(button){
				List<UIModeTypes> modeTypes = button.GetModes();
				foreach(UIModeTypes mode in modeTypes){
					if(currentUIMode == UIModeTypes.None || mode == currentUIMode || 
					  (mode == UIModeTypes.None && currentUIMode == UIModeTypes.None))
						return true;
				}
			}
		}
		
		// we are at the end of our checks now, so if we have made it this far, if the click manager is not actually locked
		// (current mode is None), the current mode is equal to the incoming button's mode, or the incoming button's mode is none,
		// then the click can be processed.
		if(currentUIMode == UIModeTypes.None || callingMode == currentUIMode || 
		   (callingMode == UIModeTypes.None && currentUIMode == UIModeTypes.None))
			return true;
		
		// otherwise some condition(s) above was not met, so return false
		return false;
	}
	
	///////////////////////////////////////////
	// AddTemporaryException()
	// Adds an exception temporarily.
	///////////////////////////////////////////		
	public void AddTemporaryException(ClickLockExceptions eException){
		listTempExceptions.Add(eException);
	}

	/// <summary>
	/// Lock the click manager with a mode and exceptions.
	/// </summary>
	/// <param name="eMode">mode.</param>
	/// <param name="listExceptions">List exceptions.</param>
	public void Lock(UIModeTypes mode = UIModeTypes.Generic, List<ClickLockExceptions> clickLockExceptions = null){		
		// because the screen is being locked, we must now reset temporary exceptions (but only if the stack of modes was empty)
		if(stackModes.Count == 0){
			listTempExceptions = new List<ClickLockExceptions>();
			
			if(clickLockExceptions != null)
				this.listExceptions = clickLockExceptions;			
		}
		else if(stackModes.Count > 0 && clickLockExceptions != null){
			Debug.Log("Something is trying to lock the click manager without an empty stack but with exceptions...this is not currently supported");	
			foreach(ClickLockExceptions e in clickLockExceptions){
				print(e);
			}
		}
			
		// push this latest mode
		stackModes.Push(mode);
	}

	/// <summary>
	/// Releases the lock. Pops off one level of click manager locking
	/// </summary>
	public void ReleaseLock(){
		try{
			// lock is being released, so pop the stack
			stackModes.Pop();
		} catch(InvalidOperationException e){
			Debug.LogError("Trying to pop an empty stack. (ClickManager) " + e.Message);
		}
		
		// if the stack is empty, reset our exceptions
		if(stackModes.Count == 0)
			listExceptions = new List<ClickLockExceptions>();
	}	

	/// <summary>
	/// Increments the tween count. When a move tween kicks off, it should lock
	/// the UI
	/// </summary>
	public void IncrementTweenCount(){
		nTweenCount++;
	}

	/// <summary>
	/// Decrements the tween count. When a move tween finishes.
	/// </summary>
	public void DecrementTweenCount(){
		if(nTweenCount <= 0){
			Debug.LogError("Warning...something decrementing tween count when it is 0");
			return;
		}
		
		nTweenCount--;
	}

	/// <summary>
	/// Determines whether there are tweens that lock the UI still running
	/// </summary>
	public bool IsTweeningUI(){
		bool bIsTweening = nTweenCount > 0;
		
		return bIsTweening;
	}

	public bool IsStackContainsType(UIModeTypes type){
		return stackModes.Contains(type);
	}
}
