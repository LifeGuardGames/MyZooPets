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
	// is the alert currently playing?
	private bool bPlaying = false;
	
	// the actually animation tweener object
	public UITweener tweenAnimation;

	//---------------------------------------------------
	// Start()
	//---------------------------------------------------
	void Start(){
		// begin listening for the callback for when the pet gets hit
		if(DegradationLogic.Instance != null)
			DegradationLogic.Instance.OnPetHit += OnPetHit;
		
		// because the tween is playing at start, stop it right away
		tweenAnimation.Play(false);
	}
	
	//---------------------------------------------------
	// OnPetHit()
	// Callback for when the pet is hit by a trigger.
	//---------------------------------------------------	
	private void OnPetHit(object sender, EventArgs args){
		Play();
	}
	
	//---------------------------------------------------
	// OnAnimationDone()
	// Callback for when the tween animation is done playing.
	//---------------------------------------------------	
	private void OnAnimationDone(){
		bPlaying = false;	
	}

	/// <summary>
	/// Manual call to play the sequence
	/// </summary>
	public void Play(){
		// if the alert animation is currently playing, we don't want to do anything
		if(bPlaying)
			return;
		
		// otherwise the animation is not playing, so we need to react to the pet being hit and play the animation
		tweenAnimation.Reset();
		tweenAnimation.Play(true);
		
		// mark the animation as playing
		bPlaying = true;
	}
}
