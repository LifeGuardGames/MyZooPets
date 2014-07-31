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
	//=======================Events========================
	//these events are fired during Update() so the listeners will probably have to
	//unregister themselves when they receive the event; otherwise, the listeners will
	//keep getting the event
    public static EventHandler<EventArgs> OnMeterFilled;   // when the meter is 100% full
    public static EventHandler<EventArgs> OnMeterStartFilling;   // when meter is filling up 
    public static EventHandler<EventArgs> OnFireReady; //when the fire is ready to be used

	public UISlider slider; // the slider that the meter fills up
	public float fFillRate; // every frame, the meter will fill this %
	
	private bool bShouldFill = false; // should this meter be filling?
	private float fireReadyMeter = 0.3f; //between 0 and 1. the point where pet starts
											//breathing in to attack

	private void SetFillStatus(bool bStatus) {
		bShouldFill = bStatus;	
	}	
	
	//---------------------------------------------------
	// Start()
	//---------------------------------------------------		
	void Start(){
		// reset the slider vlaue
		slider.sliderValue = 0;	
	}
	
	//---------------------------------------------------
	// StartFilling()
	// Call this function when the meter should start
	// filling.
	//---------------------------------------------------		
	public void StartFilling(){
		SetFillStatus( true );
		AudioManager.Instance.PlayClip("barAscend");
	}
	
	//---------------------------------------------------
	// Empty()
	// Stops filling the meter and empties it.
	//---------------------------------------------------	
	public void Empty(){
		SetFillStatus( false );
		slider.sliderValue = 0;
		AudioManager.Instance.Stop("barAscend");
	}
	
	//---------------------------------------------------
	// Update()
	//---------------------------------------------------
	void Update () {
		// if the meter shouldn't be filling, don't run the update
		if(!bShouldFill)
			return;
		
		if(OnMeterStartFilling != null)
			OnMeterStartFilling(this, EventArgs.Empty);

		// fill the slider by the fill rate
		slider.sliderValue += fFillRate;
	
		if(slider.sliderValue >= fireReadyMeter)
			if(OnFireReady != null)
				OnFireReady(this, EventArgs.Empty);

		if(slider.sliderValue >= 1){
			// process any callbacks for when the meter is full
			if(OnMeterFilled != null)
				OnMeterFilled(this, EventArgs.Empty);
			
			// stop filling
			SetFillStatus(false);

			AudioManager.Instance.Stop("barAscend");
		}
	}
	
	//---------------------------------------------------
	// IsFull()
	// Returns whether or not this meter is full.
	//---------------------------------------------------	
	public bool IsFull(){
		bool bFull = slider.sliderValue >= 1;
		return bFull;
	}
}
