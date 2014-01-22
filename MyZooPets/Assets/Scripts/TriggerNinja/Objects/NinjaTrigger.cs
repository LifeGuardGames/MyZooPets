﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// NinjaTrigger
// This is an actual object that the player can cut.
//---------------------------------------------------

public class NinjaTrigger : MonoBehaviour {		
	// number of children objects that are visible
	private int nChildrenVis;
	
	// saved velocities on this object for when it is paused/resumed
	private Vector3 savedVelocity;
	private Vector3 savedAngularVelocity;	
	
	// sounds
	public string strSoundHit;
	public string strSoundMissed;
	
	// is this object in the process of being cut?  Necessary because we use multiple colliders on some objects
	private bool bCut = false;
	
	// particle effect that will play when this object gets hit
	public GameObject goHitFX;
	public GameObject goHitDirectionFX;
	
	//---------------------------------------------------
	// Start()
	//---------------------------------------------------	
	void Start() {
		// count the number of children that have NinjaTriggerChildren scripts -- this will be used when determining if the
		// trigger is being shown by the camera or not.
		NinjaTriggerChild[] children = gameObject.GetComponentsInChildren<NinjaTriggerChild>();
		nChildrenVis = children.Length;
		
		// we don't want our objects colliding with each other
		rigidbody.detectCollisions = false;	
		
		// event listeners
		NinjaManager.OnStateChanged += OnGameStateChanged; 	// game state changes so the character can react appropriately
		NinjaManager.OnNewGame += OnNewGame;				// new game		
	}
	
	void OnDestroy() {
		// event listeners
		NinjaManager.OnStateChanged -= OnGameStateChanged; 	// game state changes so the character can react appropriately
		NinjaManager.OnNewGame -= OnNewGame;				// new game			
	}
	
	//---------------------------------------------------
	// OnCut()
	// When this trigger gets cut. vHit is the 2d location
	// where the trigger was precisely hit.
	//---------------------------------------------------	
	public void OnCut( Vector2 vHit ) {
		// if this object was already cut, return.  This is possible because some objects use multiple primitive colliders
		if ( bCut )
			return;
		
		// mark the object as cut
		bCut = true;
		
		// play a sound (if it exists)
		if ( !string.IsNullOrEmpty( strSoundHit ) )
			AudioManager.Instance.PlayClip( strSoundHit );	
		
		// also create a little explosion particle FX where the user's finger was
		Vector3 vPosWorld = Camera.main.ScreenToWorldPoint( new Vector3(vHit.x, vHit.y, 10) );
		vPosWorld.z = goHitFX.transform.position.z;
		ParticleUtils.CreateParticle( goHitFX, vPosWorld );
		
		// Directional particle spawn
		if(goHitDirectionFX != null){
			GameObject dirParticle = ParticleUtils.CreateParticle( goHitDirectionFX, vPosWorld);
			Vector2 trailMoveDelta = NinjaManager.Instance.GetTrailDeltaMove();
			dirParticle.GetComponent<XYComponentRotateObject>().x = trailMoveDelta.x;
			dirParticle.GetComponent<XYComponentRotateObject>().y = trailMoveDelta.y;	
		}
		
		// call child behaviour
		_OnCut();
	}
	
	//---------------------------------------------------
	// _OnCut()
	//---------------------------------------------------		
	protected virtual void _OnCut() {
		// children implement this	
	}
	
	//---------------------------------------------------
	// OnNewGame()
	// When the user restarts the game and a new game
	// begins.
	//---------------------------------------------------
	private void OnNewGame(object sender, EventArgs args) {
		// since a new game is beginning, regardless of anything, destroy ourselves
		Destroy( gameObject );
	}
	
	//---------------------------------------------------
	// OnGameStateChanged()
	// When the game's state changes, the object may
	// want to react.
	//---------------------------------------------------	
	private void OnGameStateChanged( object sender, GameStateArgs args ) {
		MinigameStates eState = args.GetGameState();
		
		switch ( eState ) {
			case MinigameStates.GameOver:
				// pause the object
				OnPause( true );
				break;
			case MinigameStates.Paused:
				// pause the object
				OnPause( true );
				break;
			case MinigameStates.Playing:
				// unpause the object
				OnPause( false );
				break;
		}
	}	
	
	//---------------------------------------------------
	// OnPause()
	// The object will react when the game is paused or
	// unpaused.
	//---------------------------------------------------		
	private void OnPause( bool bPaused ) {
		if ( bPaused ) {
			// game is pausing, so save velocities and stop movement
			savedVelocity = rigidbody.velocity;
			savedAngularVelocity = rigidbody.angularVelocity;
			rigidbody.isKinematic = true;
		}
		else {
			// game is unpausing, so resume movement and reapply velocities
			rigidbody.isKinematic = false;
			rigidbody.AddForce( savedVelocity, ForceMode.VelocityChange );
			rigidbody.AddTorque( savedAngularVelocity, ForceMode.VelocityChange );			
		}
	}
	
	//---------------------------------------------------
	// ChildBecameInvis()
	// When a child of this trigger becomes invisible.
	// This isn't the best/most completely implementation
	// becaues it assumes that once a children becomes
	// invisible, it will not become visible again.  This
	// is currently the case.
	//---------------------------------------------------	
	public void ChildBecameInvis() {
		nChildrenVis--;
		
		// if there are no more children visible...
		if ( nChildrenVis == 0 )
			TriggerOffScreen();
	}
	
	//---------------------------------------------------
	// OnBecameInvisible()
	// Nifty callback function that will tell us when
	// the trigger is no longer being rendered by the
	// camera.
	//---------------------------------------------------		
	void OnBecameInvisible() {
		TriggerOffScreen();
	}
	
	//---------------------------------------------------
	// TriggerOffScreen()
	// This trigger is no longer on the screen.
	//---------------------------------------------------	
	private void TriggerOffScreen() {
		// check to make sure the proper managers exist.  This check is necessary because this function will be triggered when the editor
		// quits the game, and also when the user quits the game into another scene.
		if ( !AudioManager.Instance || !NinjaManager.Instance )
			return;

		// be absolutely sure that the game is playing...this is kind of hacky, but I was running into problems with this being called
		// despite the game being over (because the object was becoming invisible).
		MinigameStates eState = NinjaManager.Instance.GetGameState();
		if ( eState == MinigameStates.GameOver || eState == MinigameStates.Restarting )
			return;	
		
		// if the object is going invisible and was cut, just destroy it
		if ( bCut )
			Destroy( gameObject );
		else {
			// otherwise, it means the object was missed
			OnMissed();
		}		
	}
	
	//---------------------------------------------------
	// OnMissed()
	// The object was not destroyed by the player.
	//---------------------------------------------------	
	private void OnMissed() {		
		// call children first
		_OnMissed();
		
		// play a sound (if it exists)
		if ( !string.IsNullOrEmpty( strSoundMissed ) )
			AudioManager.Instance.PlayClip( strSoundMissed );		
		
		// destroy the object
		Destroy( gameObject );	
	}
	
	//---------------------------------------------------
	// _OnMissed()
	//---------------------------------------------------	
	protected virtual void _OnMissed() {
		// children should implement this
	}
}
