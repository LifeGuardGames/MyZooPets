using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// FireMeter
// This is the object that is created when the user
// presses down on the fire button to begin the
// pet breathing fire.
//---------------------------------------------------	

public class FireMeter : MonoBehaviour {
	// the slider that the meter fills up
	public UISlider slider;
	
	// every frame, the meter will fill this %
	public float fFillRate;
	
	// should this meter be filling?
	private bool bShouldFill = false;
	private void SetFillStatus( bool bStatus ) {
		bShouldFill = bStatus;	
	}	
	
	//=======================Events========================
    public static EventHandler<EventArgs> OnMeterFilled;   // when the meter is 100% full
	
	//---------------------------------------------------
	// Start()
	//---------------------------------------------------		
	void Start() {
		// reset the slider vlaue
		slider.sliderValue = 0;	
	}
	
	//---------------------------------------------------
	// StartFilling()
	// Call this function when the meter should start
	// filling.
	//---------------------------------------------------		
	public void StartFilling() {
		SetFillStatus( true );
	}
	
	//---------------------------------------------------
	// Empty()
	// Stops filling the meter and empties it.
	//---------------------------------------------------	
	public void Empty() {
		SetFillStatus( false );
		slider.sliderValue = 0;
	}
	
	//---------------------------------------------------
	// Update()
	//---------------------------------------------------
	void Update () {
		// if the meter shouldn't be filling, don't run the update
		if ( !bShouldFill )
			return;
		
		// fill the slider by the fill rate
		slider.sliderValue += fFillRate;
		
		if ( slider.sliderValue >= 1 ) {
			// process any callbacks for when the meter is full
			if ( OnMeterFilled != null )
				OnMeterFilled( this, EventArgs.Empty );
			
			// stop filling
			SetFillStatus( false );
		}
	}
	
	//---------------------------------------------------
	// IsFull()
	// Returns whether or not this meter is full.
	//---------------------------------------------------	
	public bool IsFull() {
		bool bFull = slider.sliderValue >= 1;
		
		return bFull;
	}
}
