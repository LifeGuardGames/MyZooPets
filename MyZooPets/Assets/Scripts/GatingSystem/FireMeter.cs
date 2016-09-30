using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class FireMeter : MonoBehaviour{
	// Listeners will probably have to unregister themselves when they receive the event;
	// otherwise, the listeners will keep getting the event
	public static EventHandler<EventArgs> OnMeterFilled;   		// when the meter is 100% full
	public static EventHandler<EventArgs> OnMeterStartFilling;  // when meter is filling up 

	public float fillSliderLength = 250f;
	public Image slider;										// the slider that the meter fills up
	public float fillTime = 2f;
	public string fillSound;

	void Start(){
		Reset();
    }
	
	// Call this function when the meter should start filling.	
	public void StartFilling(){
		LeanTween.value(gameObject, UpdateSliderValueCallback, 0f, fillSliderLength, fillTime)
			.setEase(LeanTweenType.easeInQuad)
			.setOnComplete(OnFillComplete);

		if(OnMeterStartFilling != null){
			OnMeterStartFilling(this, EventArgs.Empty);
		}

		Hashtable option = new Hashtable();
		option.Add("IsSoundClipManaged", true);
		AudioManager.Instance.PlayClip(fillSound, option: option);
	}

	// Callback update value function for start filling
	private void UpdateSliderValueCallback(float value){
		slider.rectTransform.sizeDelta = new Vector2(value, 32f);
	}

	// Stops filling the meter and empties it.
	public void Reset(){
		LeanTween.cancel(gameObject);   // Cancel any leantweens going on
		slider.rectTransform.sizeDelta = new Vector2(0f, 32f);
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
		return slider.rectTransform.sizeDelta.x >= 250;
	}
}
