using UnityEngine;
using System.Collections;

//---------------------------------------------------
// MinigamePopup
// Parent class for popups that appear during
// minigames.
//---------------------------------------------------

public class MinigamePopup : MonoBehaviour{
	// tween script for this popup
	public PositionTweenToggle tween;
	
	// a popular concept may be showing the HUD when a popup is shown
	public TweenToggleDemux demuxHUD;
	public TweenToggle pauseTween;	// Hide the pause button when any of the panels are shown
	
	//---------------------------------------------------
	// Toggle()
	// Turns this UI on and off.
	//---------------------------------------------------		
	public void Toggle(bool bShow){
		if(bShow){
			tween.Show();
			
			_OnShow();
			
			if(demuxHUD)
				demuxHUD.Show();

			pauseTween.Hide();

			// lock clicks
			ClickManager.Instance.Lock();
			Debug.LogWarning("ACTIVE MODE LOCK BYPASS");
			//ClickManager.SetActiveGUIModeLock(true);
		}
		else{
			tween.Hide();
			
			_OnHide();
			
			if(demuxHUD)
				demuxHUD.Hide();

			pauseTween.Show();

			// clicks are ok
			ClickManager.Instance.ReleaseLock();
			Debug.LogWarning("ACTIVE MODE LOCK BYPASS");
			//ClickManager.SetActiveGUIModeLock(false);
		}		
	}
	
	void Update(){
		_OnUpdate();	
	}
	
	//---------------------------------------------------
	// IsShowing()
	//---------------------------------------------------		
	public bool IsShowing(){
		return tween.IsShown;
	}
	
	protected virtual void _OnShow(){
	}

	protected virtual void _OnHide(){
	}

	protected virtual void _OnUpdate(){
	}
}
