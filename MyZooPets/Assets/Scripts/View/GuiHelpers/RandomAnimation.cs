using UnityEngine;
using System.Collections;

/// <summary>
/// Script for objects to play animations by itself.
/// Can play at random intervals if not clamped
/// </summary>

public class RandomAnimation : MonoBehaviour {	
	
	//Timer stuff
	public bool isActive;
	public float delayBeforeStart;
	public float minInterval;
	public float maxInterval;
	private float generatedValue;
	private float timeBegin;
	
	void Start(){
		if(maxInterval < minInterval){
			Debug.LogError("Max interval is less than min interval, clamping to min");
			maxInterval = minInterval;
		}
		generatedValue = Random.Range(minInterval, maxInterval);
		timeBegin = Time.time;
	}
	
	void Update(){
		if(isActive){	
			if(Time.time > timeBegin + generatedValue){
				timeBegin = Time.time;
				generatedValue = Random.Range(minInterval, maxInterval);
				
				// Do the action here
				animation.Play();
			}
		}
	}
	
	// Note: If some script enables a bunch of buttons at the same time they will all animate at once the first time
	public void Enable(){
		isActive = true;
	}
	
	public void Disable(){
		isActive = false;
	}
}
