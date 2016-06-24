using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//---------------------------------------------------
// FireMeter
// This is the object that is created when the user
// presses down on the fire button to begin the
// pet breathing fire.
//---------------------------------------------------	

public class FireMeter : MonoBehaviour{
	//=======================Events========================
	//listeners will probably have to
	//unregister themselves when they receive the event; otherwise, the listeners will
	//keep getting the event
	public static EventHandler<EventArgs> OnMeterFilled;   		// when the meter is 100% full
	public static EventHandler<EventArgs> OnMeterStartFilling;	// when meter is filling up 

	public Image slider;				// the slider that the meter fills up
	public float fillTime = 2f;
	public string fillSound;

	void Start(){
		slider.fillAmount = 0;		// reset the slider vlaue
	}
	
	// Call this function when the meter should start filling.	
	public void StartFilling(){
		LeanTween.value(gameObject, UpdateSliderValueCallback, 0f, 1f, fillTime)
			.setEase(LeanTweenType.easeInQuad).setOnComplete(OnFillComplete);

		if(OnMeterStartFilling != null){
			OnMeterStartFilling(this, EventArgs.Empty);
		}

		Hashtable option = new Hashtable();
		option.Add("IsSoundClipManaged", true);
		AudioManager.Instance.PlayClip(fillSound, option: option);
	}

	// Callback update value function for start filling
	public void UpdateSliderValueCallback(float value){
		slider.fillAmount = value;
	}

	// Stops filling the meter and empties it.
	public void Empty(){
		LeanTween.cancel(gameObject);	// Cancel any leantweens going on
		slider.fillAmount = 0;
		AudioManager.Instance.StopClip(fillSound);
	}

	public void OnFillComplete(){
		if(OnMeterFilled != null){		// Process any callbacks for when the meter is full
			OnMeterFilled(this, EventArgs.Empty);
		}
		AudioManager.Instance.StopClip(fillSound);
	}

	// Returns whether or not this meter is full.
	public bool IsMeterFull(){
		return slider.fillAmount >= 1;
	}
}
