using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Singleton that is a UI manager.
/// </summary>
public abstract class SingletonUI<T> : Singleton<T> where T : MonoBehaviour{
	protected abstract void _OpenUI();		// when the UI manager is opened
	protected abstract void _CloseUI();		// when the UI manager is closed
	public EventHandler<UIManagerEventArgs> OnTweenDone;   	// event that fires when the UI finishes a tween
	public EventHandler<UIManagerEventArgs> OnManagerOpen;	// event that fires when the user enters or exits a UI mode

	// is this ui open?
	private bool isOpen = false;

	// the mode type of this manager
	protected UIModeTypes eModeType = UIModeTypes.Generic;

	// If true, will set mode lock in clickmanager, Usually this will be true
	public bool isLockModeInClickmanager = true;

	// If true, bypass tweening lock, Usually this will be false
	public bool isIgnoreTweenLockOnClose = false;

	// if true, opening this UI will lock the GUI (put up giant box collider blocking input)
	public bool blockGUI;

	/// <summary>
	/// Determines whether this UI is opened.
	/// </summary>
	/// <returns><c>true</c> if this instance is open; otherwise, <c>false</c>.</returns>
	public bool IsOpen(){
		return isOpen;	
	}

	protected virtual void Awake(){
	}

	protected virtual void Start(){
	}

	protected virtual void OnDestroy(){
	}

	/// <summary>
	/// When a button wants to open a given UI, this is
	/// what should be called.  From a high level, the UI
	/// manager locks clicks/modes, and then the child
	/// class does its unique thing via _OpenUI.
	/// </summary>
	public void OpenUI(){
		if(isLockModeInClickmanager){
			// a ui is opening, so we need to lock things down
			List<ClickLockExceptions> listExceptions = GetClickLockExceptions();
			ClickManager.Instance.Lock(eModeType, listExceptions);
		}

		if(blockGUI){
			ClickManager.SetActiveGUIModeLock(true);
		}

		// fire callback
		UIManagerEventArgs args = new UIManagerEventArgs();
		args.Opening = true;
		if(OnManagerOpen != null){
			OnManagerOpen(this, args);
		}
		
		// the UI is now open
		isOpen = true;
		
		_OpenUI();
	}

	/// <summary>
	/// Raises the show event. Tween callback for when this UI finishes its 
	/// show tween.
	/// </summary>
	private void OnShow(){
		// send callback
		UIManagerEventArgs args = new UIManagerEventArgs();
		args.Opening = true;
		if(OnTweenDone != null)
			OnTweenDone(this, args);			
	}

	/// <summary>
	/// Closes the UI. From a high level, releases all appropriate locks on the UI.
	/// </summary>
	public void CloseUI(){
		// you can't close a UI if there is UI tweening going on
		if(!isIgnoreTweenLockOnClose && ClickManager.Instance.IsTweeningUI()){
			Debug.LogWarning("Something is tweening, cannot close " + name);
			return;
		}

		if(isLockModeInClickmanager){
			// a ui is closing, so release our locks
			ClickManager.Instance.ReleaseLock();
		}
		
		if(blockGUI){
			ClickManager.SetActiveGUIModeLock(false);
		}
		
		// fire callback
		UIManagerEventArgs args = new UIManagerEventArgs();
		args.Opening = false;
		if(OnManagerOpen != null)
			OnManagerOpen(this, args);			
		
		// the ui is no longer open
		isOpen = false;
		
		_CloseUI();
	}

	/// <summary>
	/// Gets the click lock exceptions.
	/// </summary>
	/// <returns>The click lock exceptions.</returns>
	protected virtual List<ClickLockExceptions> GetClickLockExceptions(){
		return null;
	}

	// When we exit UIMode, sometimes there are other modes in the stack,
	// this opens them properly
	protected void CloseUIOpenNext(UIModeTypes mode){
		switch(mode){

		default:	// Default to base mode
			// Only run this chunk if in bedroom or yard scene
			if((Application.loadedLevelName == SceneUtils.BEDROOM || Application.loadedLevelName == SceneUtils.YARD)){
				// Editdeco mode check
				if(ClickManager.Instance.IsStackContainsType(UIModeTypes.EditDecos)){
					if(RoomArrowsUIManager.Instance != null){
						RoomArrowsUIManager.Instance.ShowPanel();
						HUDUIManager.Instance.ShowPanel();
					}
				}
				else if(ClickManager.Instance.IsStackContainsType(UIModeTypes.MiniPet)){
					InventoryUIManager.Instance.ShowPanel();
				}
				else if(ClickManager.Instance.IsStackContainsType(UIModeTypes.Accessory)){
					HUDUIManager.Instance.ShowPanel();
				}
				// Fireblowing room check
				else if(FireButtonUIManager.Instance.IsActive){
					if(RoomArrowsUIManager.Instance != null){
						RoomArrowsUIManager.Instance.ShowPanel();
						HUDUIManager.Instance.ShowPanel();
					}
				}
				// Default behaviour
				else{
					if(RoomArrowsUIManager.Instance != null){
						RoomArrowsUIManager.Instance.ShowPanel();
						HUDUIManager.Instance.ShowPanel();
					}
					if(NavigationUIManager.Instance != null){
						NavigationUIManager.Instance.ShowPanel();
						HUDUIManager.Instance.ShowPanel();
					}
					if(InventoryUIManager.Instance != null){
						InventoryUIManager.Instance.ShowPanel();
						HUDUIManager.Instance.ShowPanel();
					}
				}
			}
			break;
		}
	}
}
