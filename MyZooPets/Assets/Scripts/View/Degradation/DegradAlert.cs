using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//---------------------------------------------------
// DegradAlert
// This is an alert that causes the screen to flash
// red when the player gets hit by a degrad trigger.
//---------------------------------------------------

public class DegradAlert : MonoBehaviour{
	// the actually animation tweener object
	public Animation tweenAnimation;

	//---------------------------------------------------
	// Start()
	//---------------------------------------------------
	void Start(){
		// begin listening for the callback for when the pet gets hit
		if(DegradationLogic.Instance != null)
			DegradationLogic.Instance.OnPetHit += OnPetHit;
	}

	//---------------------------------------------------
	// OnDestroy()
	//---------------------------------------------------
	void OnDestroy(){
		if(DegradationLogic.Instance != null)
			DegradationLogic.Instance.OnPetHit -= OnPetHit;
	}

	//---------------------------------------------------
	// OnPetHit()
	// Callback for when the pet is hit by a trigger.
	//---------------------------------------------------	
	private void OnPetHit(object sender, EventArgs args){
		Play();
	}

	/// <summary>
	/// Manual call to play the sequence
	/// </summary>
	public void Play(){
		tweenAnimation.Play();
	}
}
