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
	public float capacity;
	public float current = 0;
	public float ratePerHour;
	public float showButtonThreshold;	// Amount in current when it starts to show the button
	public float allowTapThreshold;		// Amount in current when you are allowed to tap the collider

	public GameObject buttonParent;
	public Animation clickAnimation;

	private string itemId;
	private bool isFull;
	private TimeSpan lastTimeDurationAux;
	private bool isFlagResetWhenStart = false;

	/// <summary>
	/// Initialize variables before start is called, done on initalize
	/// </summary>
	/// <param name="isPlacedFromDecoMode">If set to <c>true</c> is placed from deco mode.</param>
	public void Initialize(string farmItemId, bool isPlacedFromDecoMode){
		itemId = farmItemId;

		// If it is placed manually, not automatically if it is placed already as a deco
		if(isPlacedFromDecoMode){
			isFlagResetWhenStart = true;
		}
	}

	void Start(){
		if(allowTapThreshold > showButtonThreshold){
			Debug.LogWarning(gameObject.name + " - Farm button shown before it is allowed to tap!");
		}

		DateTime lastRedeemTime;
		if(isFlagResetWhenStart){	// Start from zero
			ItemLogic.Instance.UpdateFarmLastRedeemTime(itemId, LgDateTime.GetTimeNow());
			lastRedeemTime = LgDateTime.GetTimeNow();
		}
		else{						// Get last redeemed time and calculate
			lastRedeemTime = ItemLogic.Instance.GetFarmLastRedeemTime(itemId);
		}

		RefreshLastTimeSinceLastPlayed(lastRedeemTime);
		CheckShowButton();	// Hide this by default, enabled later on
	}

	void OnApplicationPause(bool isPaused){
		if(isPaused == false){
			DateTime lastRedeemTime = ItemLogic.Instance.GetFarmLastRedeemTime(itemId);
			RefreshLastTimeSinceLastPlayed(lastRedeemTime);
		}
	}

	// Check and calculate how much time until last redeem and populate current
	private void RefreshLastTimeSinceLastPlayed(DateTime lastRedeemTime){
		lastTimeDurationAux = LgDateTime.GetTimeNow() - lastRedeemTime;
		// Initialize current
		current = ratePerHour / 3600f * (float)lastTimeDurationAux.TotalSeconds;
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

	private void RepeatFarm(){
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

	public void ItemTapped(){
		if(current >= allowTapThreshold){
//			Debug.Log("spewing " + (int)current + " coins");

			if(clickAnimation != null){
				clickAnimation.Play();
			}
			
			// Spew out the reward here
			StatsController.Instance.ChangeStats(coinsDelta: (int)current, coinsPos: transform.position, is3DObject: true);

			// Update mutable datas
			ItemLogic.Instance.UpdateFarmLastRedeemTime(itemId, LgDateTime.GetTimeNow());

			// Reset the generator
			CancelInvoke("RepeatFarm");	// Clear previous invoke before starting new one 
			InvokeRepeating("RepeatFarm", 1f, 1f);
			isFull = false;
			current = 0;
			CheckShowButton();
		}
	}
}
