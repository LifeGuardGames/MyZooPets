using UnityEngine;
using System.Collections;

//---------------------------------------------------
// MinigamePopup
// Parent class for popups that appear during
// minigames.
//---------------------------------------------------

public class MinigamePopup : MonoBehaviour {
	// tween script for this popup
	public PositionTweenToggle tween;
	
	// a popular concept may be showing the HUD when a popup is shown
	public TweenToggleDemux demuxHUD;
	
	//---------------------------------------------------
	// Toggle()
	// Turns this UI on and off.
	//---------------------------------------------------		
	public void Toggle( bool bShow ) {
		if ( bShow ) {
			tween.Show();
			
			_OnShow();
			
			if ( demuxHUD )
				demuxHUD.Show();
			
			// lock clicks
			ClickManager.Instance.ClickLock();
			ClickManager.SetActiveGUIModeLock( true );
		}
		else {
			tween.Hide();
			
			_OnHide();
			
			if ( demuxHUD )
				demuxHUD.Hide();
			
			// clicks are ok
			ClickManager.Instance.ReleaseClickLock();
			ClickManager.SetActiveGUIModeLock( false );
		}		
	}
	
	void Update() {
		_OnUpdate();	
	}
	
	//---------------------------------------------------
	// IsShowing()
	//---------------------------------------------------		
	public bool IsShowing() {
		return tween.IsShowing;	
	}
	
	protected virtual void _OnShow() {}
	protected virtual void _OnHide() {}
	protected virtual void _OnUpdate() {}
}
