﻿using UnityEngine;
using System.Collections;

//---------------------------------------------------
// LgButtonHold
// This type of button must be held down to work.
//---------------------------------------------------

public abstract class LgButtonHold : LgButton{
	
	// pure abstract functions
	protected abstract void _Update();
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
	
	//---------------------------------------------------
	// OnPress()
	// 2D sprite buttons will receive this event.  NGUI
	// does not have support for detecting button holds,
	// so this is how we do it.
	//---------------------------------------------------	
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
	
	//---------------------------------------------------
	// Update()
	//---------------------------------------------------	
	void Update(){
		// if this button is enabled and is currently being held...
		if(enabled && isSprite && isHeld){
			// if the user moved off the button, it is no longer being held
			if(UICamera.lastHit.collider != gameObject.collider){
				SetHeld(false);
				ButtonReleased();
			}
		}
		
		// let children do their updating
		_Update();
	}	
	
	//---------------------------------------------------
	// OnFingerStationary()
	// For 3D objects.
	//---------------------------------------------------		
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
	
	//---------------------------------------------------
	// ButtonReleased()
	// Abstract function called when this held button is
	// released.
	//---------------------------------------------------	
	protected abstract void ButtonReleased();
		
}
