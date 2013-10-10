using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// NinjaTrigger
// This is an actual object that the player can cut.
//---------------------------------------------------

public class NinjaTrigger : MonoBehaviour {		
	// saved velocities on this object for when it is paused/resumed
	private Vector3 savedVelocity;
	private Vector3 savedAngularVelocity;	
	
	// sounds
	public string strSoundHit;
	public string strSoundMissed;
	
	//---------------------------------------------------
	// Start()
	//---------------------------------------------------	
	void Start() {
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
	// When this trigger gets cut.
	//---------------------------------------------------	
	public void OnCut() {
		// call child first
		_OnCut();
		
		// play a sound (if it exists)
		if ( !string.IsNullOrEmpty( strSoundHit ) )
			AudioManager.Instance.PlayClip( strSoundHit );
		
		// then destroy the object
		Destroy( gameObject );
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
	// Update()
	//---------------------------------------------------	
	void Update() {
		// gah...I hate to do this in Update()...I'm not sure where else to put it though
		float fFloor = NinjaManager.Instance.GetFloor();
		if ( transform.position.y <= fFloor ) {
			// if the object reached this location, the player did NOT cut it
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
