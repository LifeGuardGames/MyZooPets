using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Farm generator.
/// This a 3d object in the scene that spawns a utility (coins/gems) stored up to a capacity.
/// Much like how money is harvested like Clash of Clans.
/// </summary>
[RequireComponent (typeof (Collider))]
public class FarmGenerator : MonoBehaviour {
	public int level;
	public float capacity;
	public float current = 0;
	public float ratePerHour;
	public float showButtonThreshold; // amount in current when it starts to show the button

	private bool isFull;
	private TimeSpan lastTimeDurationAux;

	void Start(){
		float initialAmount = 0; 	// TODO get the initial amount here
		RefreshLastTimeSinceLastPlayedWithBase(initialAmount);
	}

	void OnApplicationPause(bool isPaused){
		if(isPaused = false){
			RefreshLastTimeSinceLastPlayed();
		}
	}

	private void RefreshLastTimeSinceLastPlayedWithBase(float initialAmount){
		current += initialAmount;
		RefreshLastTimeSinceLastPlayed();
	}

	// Check and calculate how much time until last session and populate current
	private void RefreshLastTimeSinceLastPlayed(){
		lastTimeDurationAux = LgDateTime.GetTimeSinceLastPlayed();
		// Current should be populated already
		current += ratePerHour / 3600f * lastTimeDurationAux.Seconds;
		if(current >= capacity){
			current = capacity;
			isFull = true;
			CancelInvoke("RepeatFarm");
		}
		else{
			InvokeRepeating("RepeatFarm", 0f, 1f);
		}
		CheckShowButton();
	}

	void OnTap(TapGesture gesture){
		// Spew out the reward here
		//TODO

		// Reset the generator
		isFull = false;
		InvokeRepeating("RepeatFarm", 0f, 1f);
	}

	void RepeatFarm(){
		if(!isFull){
			current += ratePerHour / 3600f;
			if(current >= capacity){
				current = capacity;
				isFull = true;
				CancelInvoke("RepeatFarm");
			}
			CheckShowButton();
		}
	}

	private void CheckShowButton(){
		if(current >= showButtonThreshold){
			ShowButton();
		}
	}

	private void ShowButton(){
		// TODO
	}

	private void HideButton(){
		// TODO
	}
}
