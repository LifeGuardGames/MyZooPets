using UnityEngine;
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
	
	// event that fires when the user enters or exits a UI mode
	public event EventHandler<UIManagerEventArgs> OnManagerOpen;	
	
	// if true, opening this UI will lock the GUI (put up giant box collider blocking input)
	public bool blockGUI;	
	protected bool ShouldLockUI() {
		return blockGUI;
	}
	
	// the mode type of this manager
	public UIModeTypes eModeType;
	
	// is this ui open?
	private bool bOpen = false;
	public bool IsOpen() {
		return bOpen;	
	}
	
	void Start() {
		_Start();	
	}
	
	protected virtual void _Start() {}
	
	//---------------------------------------------------
	// OpenUI()
	// When a button wants to open a given UI, this is
	// what should be called.  From a high level, the UI
	// manager locks clicks/modes, and then the child
	// class does its unique thing via _OpenUI.
	//---------------------------------------------------	
	public void OpenUI() {
		// a ui is opening, so we need to lock things down
		ClickManager.Instance.ClickLock();
		ClickManager.Instance.ModeLock( eModeType );
		
		if ( ShouldLockUI() )
			ClickManager.SetActiveGUIModeLock(true);
		
		// fire callback
		UIManagerEventArgs args = new UIManagerEventArgs();
		args.Opening = true;
        if( OnManagerOpen != null )
            OnManagerOpen(this, args);		
		
		// the UI is now open
		bOpen = true;
		
		_OpenUI();
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
		ClickManager.Instance.ReleaseClickLock();
		ClickManager.Instance.ReleaseModeLock();
		
		if ( ShouldLockUI() )
			ClickManager.SetActiveGUIModeLock(false);
		
		// fire callback
		UIManagerEventArgs args = new UIManagerEventArgs();
		args.Opening = false;
        if( OnManagerOpen != null )
            OnManagerOpen(this, args);			
		
		// the ui is no longer open
		bOpen = false;
		
		_CloseUI();
	}
}