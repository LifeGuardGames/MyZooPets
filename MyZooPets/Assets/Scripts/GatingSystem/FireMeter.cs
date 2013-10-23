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
	
	//---------------------------------------------------
	// Start()
	//---------------------------------------------------		
	void Start() {
		// reset the slider vlaue
		slider.sliderValue = 0;	
	}
	
	//---------------------------------------------------
	// Update()
	//---------------------------------------------------
	void Update () {
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
