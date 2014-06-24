﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//---------------------------------------------------
// SingletonUI
// This class is a singleton that is also a UI
// manager.
//---------------------------------------------------

public abstract class SingletonUI<T> : Singleton<T> where T : MonoBehaviour {
	//------------------ Pure Abstract ----------------------------
	protected abstract void _OpenUI();		// when the UI manager is opened
	protected abstract void _CloseUI();		// when the UI manager is closed
	//------------------------------------------------------------- 
	
	// =======================Events========================
    public EventHandler<UIManagerEventArgs> OnTweenDone;   	// event that fires when the UI finishes a tween
	public EventHandler<UIManagerEventArgs> OnManagerOpen;	// event that fires when the user enters or exits a UI mode
	// =====================================================	

	// is this ui open?
	private bool isOpen = false;

	// the mode type of this manager
	protected UIModeTypes eModeType = UIModeTypes.Generic;


	// if true, opening this UI will lock the GUI (put up giant box collider blocking input)
	public bool blockGUI;	

	protected bool ShouldLockUI() {
		return blockGUI;
	}

	public bool IsOpen() {
		return isOpen;	
	}
	
	void Start() {
		_Start();	
	}
	
	protected virtual void _Start() {}

	/// <summary>
	/// When a button wants to open a given UI, this is
	/// what should be called.  From a high level, the UI
	/// manager locks clicks/modes, and then the child
	/// class does its unique thing via _OpenUI.
	/// </summary>
	public void OpenUI() {
		
		// a ui is opening, so we need to lock things down
		List<ClickLockExceptions> listExceptions = GetClickLockExceptions();
		ClickManager.Instance.Lock( eModeType, listExceptions );
		
		if ( ShouldLockUI() )
			ClickManager.SetActiveGUIModeLock(true);
		
		// fire callback
		UIManagerEventArgs args = new UIManagerEventArgs();
		args.Opening = true;
        if( OnManagerOpen != null )
            OnManagerOpen(this, args);		
		
		// the UI is now open
		isOpen = true;
		
		_OpenUI();
	}

	/// <summary>
	/// Raises the show event. Tween callback for when this UI finishes its 
	/// show tween.
	/// </summary>
	private void OnShow() {
		// send callback
		UIManagerEventArgs args = new UIManagerEventArgs();
		args.Opening = true;
        if( OnTweenDone != null )
            OnTweenDone(this, args);			
	}
	
	//---------------------------------------------------
	// CloseUI()
	// From a high level, releases all appropriate locks
	// on the UI.
	//---------------------------------------------------		
	public void CloseUI() {
		// you can't close a UI if there is UI tweening going on
		if ( ClickManager.Instance.IsTweeningUI() )
			return;
		
		// a ui is closing, so release our locks
		ClickManager.Instance.ReleaseLock();
		
		if ( ShouldLockUI() )
			ClickManager.SetActiveGUIModeLock(false);
		
		// fire callback
		UIManagerEventArgs args = new UIManagerEventArgs();
		args.Opening = false;
        if( OnManagerOpen != null )
            OnManagerOpen(this, args);			
		
		// the ui is no longer open
		isOpen = false;
		
		_CloseUI();
	}
	
	//---------------------------------------------------
	// GetClickLockExceptions()
	// It's possible some managers may have click lock
	// exceptions.
	//---------------------------------------------------
	protected virtual List<ClickLockExceptions> GetClickLockExceptions() {
		return null;
	}
		
}