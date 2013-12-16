using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// NinjaLife
// This is a UI element representing a single life
// on the HUD of the player in the Ninja game.
//---------------------------------------------------

public class NinjaLife : MonoBehaviour {
	// index of this life
	public int nIndex;
	
	// the sprite that this life is uing
	public UISprite sprite;
	
	// particle system for this object
	public ParticleSystemController systemOff;
	
	// the state of this life
	//private bool bOn;
	
	void Start() {
		MinigameManager<NinjaManager>.OnNewGame += OnNewGame;
		MinigameManager<NinjaManager>.OnLivesChanged += OnLivesChanged;
	}
	
	//---------------------------------------------------
	// OnNewGame()
	// When the user restarts the game and a new game
	// begins.
	//---------------------------------------------------
	private void OnNewGame(object sender, EventArgs args) {
		// since a new game is beginning, toggle on
		Toggle( true );
	}	
	
	//---------------------------------------------------
	// OnLivesChanged()
	//---------------------------------------------------
	private void OnLivesChanged(object sender, LivesChangedArgs args) {
		// get the number of lives there are
		int nLives = MinigameManager<NinjaManager>.Instance.GetLives();
		
		// if we are LOSING a life and the current lives +1 == this life's index, it means that this life was just lost, so toggle off
		if ( args.GetChange() < 0 && nLives+1 == nIndex )
			Toggle( false );
	}		
	
	//---------------------------------------------------
	// Toggle()
	// Turn this life on or off.
	//---------------------------------------------------	
	public void Toggle( bool bOn ) {
		// cache the state of this life
		//this.bOn = bOn;
		
		// change the tint based on on/off
		Color tint = bOn ? new Color(255,255,255,255) : new Color(0,0,0,255);
		sprite.color = tint;
		
		// if the life is toggling off, play a particle fx
		if ( !bOn )
			systemOff.Play();
	}
}
