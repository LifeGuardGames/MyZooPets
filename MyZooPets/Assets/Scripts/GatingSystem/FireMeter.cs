using UnityEngine;
using System.Collections;

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
