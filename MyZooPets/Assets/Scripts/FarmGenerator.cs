using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Farm generator.
/// This a 3d object in the scene that spawns a utility (coins) stored up to a capacity.
/// Much like how money is harvested like Clash of Clans.
/// </summary>
[RequireComponent (typeof (Collider))]
public class FarmGenerator : MonoBehaviour {
	public int level;
	public float capacity;
	public float current = 0;
	public float ratePerHour;
	public float showButtonThreshold; // Amount in current when it starts to show the button
	public float allowTapThreshold; // Amount in current when you are allowed to tap the collider

	public GameObject buttonParent;

	private bool isFull;
	private TimeSpan lastTimeDurationAux;

	void Start(){
		Debug.Log("STARTING");
		if(allowTapThreshold > showButtonThreshold){
			Debug.LogWarning(gameObject.name + " - Farm button shown before it is allowed to tap!");
		}

		float initialAmount = 0; 	// TODO get the initial amount here
		RefreshLastTimeSinceLastPlayedWithBase(initialAmount);

		CheckShowButton();	// Hide this by default, enabled later on
	}

	void OnApplicationPause(bool isPaused){
		if(isPaused == false){
			Debug.Log("resumed from pause");
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
			CancelInvoke("RepeatFarm");	// Clear previous invoke before starting new one 
			InvokeRepeating("RepeatFarm", 1f, 1f);
		}
		CheckShowButton();
	}

	public void ItemTapped(){
		if(current >= allowTapThreshold){
			Debug.Log("spewing " + (int)current + " coins");
			
			// Spew out the reward here
			RewardQueueData.GenericDelegate function = delegate(){
				StatsController.Instance.ChangeStats(deltaStars: (int)current, starsLoc: transform.position, is3DObject: true);
			};
			RewardManager.Instance.AddToRewardQueue(function);
			
			// Reset the generator
			CancelInvoke("RepeatFarm");	// Clear previous invoke before starting new one 
			InvokeRepeating("RepeatFarm", 1f, 1f);
			isFull = false;
			current = 0;
			CheckShowButton();
		}
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
		// Show the button
		if(current >= showButtonThreshold){
			buttonParent.SetActive(true);
		}
		// Hide the button
		else{
			buttonParent.SetActive(false);
		}
	}
}
