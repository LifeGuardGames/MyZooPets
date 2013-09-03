using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//---------------------------------------------------
// SingletonUI
// This class is a singleton that is also a UI
// manager.
//---------------------------------------------------

public class SingletonUI<T> : Singleton<T> where T : MonoBehaviour {
	// event that fires when the user enters or exits a UI mode
	public static event EventHandler<UIManagerEventArgs> OnManagerOpen;	
	public class UIManagerEventArgs : EventArgs{
		public bool Opening{get; set;}
	}	
	
	// if true, opening this UI will lock the GUI (put up giant box collider blocking input)
	public bool bLockGUI;	
	protected bool ShouldLockUI()
	{
		return bLockGUI;
	}
	
	// the mode type of this manager
	public UIModeTypes eModeType;
	
	// is this ui open?
	
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
		
		_OpenUI();
	}
	
	//---------------------------------------------------
	// _OpenUI()
	// Children classes implement this to do their own
	// unique behaviour when a UI is opened.
	//---------------------------------------------------		
	protected virtual void _OpenUI() {
		Debug.Log("Children should implement _OpenUI()");
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
		
		_CloseUI();
	}
	
	//---------------------------------------------------
	// _CloseUI()
	// Child classes implement this to do their own
	// unique thing when a UI is closed.
	//---------------------------------------------------		
	protected virtual void _CloseUI() {
		Debug.Log("Children should implement _CloseUI()");
	}
}