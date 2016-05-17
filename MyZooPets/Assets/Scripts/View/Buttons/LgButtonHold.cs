using UnityEngine;
using System.Collections;

/// <summary>
/// Lg button hold. This type of button must be held down to work.
/// </summary>
public abstract class LgButtonHold : LgButton{
	
	// pure abstract functions
	protected abstract void _Update();
	protected abstract void ButtonReleased();
	//------------------------

	// is this button being held?
	private bool isHeld;

	private void SetHeld(bool b){
		isHeld = b;	
	}
	
	//---------------------------------------------------
	// OnClick()
	// Eat OnClick.  Maybe the hierarchy should be a bit
	// different...
	//---------------------------------------------------	
	void OnClick(){
	}	

	/// <summary>
	/// Raises the press event.
	/// 2D sprite buttons will receive this event.  NGUI
	/// does not have support for detecting button holds,
	/// so this is how we do it.
	/// </summary>
	/// <param name="isPressed">If set to <c>true</c> is pressed.</param>
	void OnPress(bool isPressed){
		if(enabled && isSprite){
			bool wasHeld = isHeld;
			SetHeld(isPressed);
			
			if(isPressed)
				ButtonClicked();				// user pressed down on the button, so click it
			else if(!isPressed && wasHeld)
				ButtonReleased();				// user let go and is no longer holding, so release
		}
	}	

	void Update(){
		// if this button is enabled and is currently being held...
		if(enabled && isSprite && isHeld){
			// if the user moved off the button, it is no longer being held
			if(UICamera.lastHit.collider != gameObject.GetComponent<Collider>()){
				SetHeld(false);
				ButtonReleased();
			}
		}
		
		// let children do their updating
		_Update();
	}	

	/// <summary>
	/// Raises the finger stationary event.
	/// For 3D objects
	/// </summary>
	/// <param name="e">E.</param>
	void OnFingerStationary(FingerMotionEvent e){
		if(e.Phase == FingerMotionPhase.Started){
			// when the motion starts, it means the user has clicked the button
			ButtonClicked();
		}
		else if(e.Phase == FingerMotionPhase.Ended){
			// when the motion ends, it means the user is no longer holding the button	
			ButtonReleased();
		}
	}
}
