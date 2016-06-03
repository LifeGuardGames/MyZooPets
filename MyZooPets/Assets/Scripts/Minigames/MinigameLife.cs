using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//---------------------------------------------------
// MinigameLife
// This is a UI element representing a single life
// on the HUD of the player in a minigame.
//---------------------------------------------------

public class MinigameLife : MonoBehaviour{
	// index of this life
	public int nIndex;
	
	// the sprite that this life is uing
	public Image sprite;
	
	// particle systems for this object
	public ParticleSystemController systemOff;
	public ParticleSystemController systemOn;

	public DegradAlert bloodPanelControl;

	// the state of this life
	//private bool bOn;
	
	void Start(){
		// FFFFFFUFUUUUUUUUUUU
		if(NinjaManager.Instance != null){
			NinjaManager.OnNewGame += OnNewGame;
			NinjaManager.OnLivesChanged += OnLivesChanged;			
		}
	}

	void OnDestroy(){
		if(NinjaManager.Instance != null){
			NinjaManager.OnNewGame -= OnNewGame;
			NinjaManager.OnLivesChanged -= OnLivesChanged;			
		}
	}
	
	//---------------------------------------------------
	// OnNewGame()
	// When the user restarts the game and a new game
	// begins.
	//---------------------------------------------------
	private void OnNewGame(object sender, EventArgs args){
		// since a new game is beginning, toggle on
		Toggle(true);
	}	
	
	//---------------------------------------------------
	// OnLivesChanged()
	//---------------------------------------------------
	private void OnLivesChanged(object sender, LivesChangedArgs args){
		// get the number of lives there are
		int nLives = GetLives();
		int nChange = args.GetChange();
		//Debug.Log("Preparing life..." + nLives + " " + nChange);
		if(nChange < 0 && nLives + 1 == nIndex){
			//Debug.Log("----Loosing a life");
			// if we are LOSING a life and the current lives +1 == this life's index, it means that this life was just lost, so toggle off
			Toggle(false);

			// Play the camera shake animation
			if(Camera.main.GetComponent<Animation>() != null){
				Camera.main.GetComponent<Animation>().Play();
			}
		}
		else if(nChange > 0 && nLives == nIndex){
			//Debug.Log("---Gaining a life");
			// else if we are GAINING a life and the current lives == this life's index, it means this life was just gained, so toggl eon
			Toggle(true);
		}
	}
	
	private int GetLives(){
		int nLives = 0;
		if(NinjaManager.Instance != null){
			nLives = NinjaManager.Instance.GetLives();
		}
		return nLives;
	}
	
	//---------------------------------------------------
	// Toggle()
	// Turn this life on or off.
	//---------------------------------------------------	
	public void Toggle(bool bOn){
		//Debug.Log("TOGGLING " + bOn);
		// cache the state of this life
		//this.bOn = bOn;
		
		// change the tint based on on/off
		Color tint = bOn ? new Color(255, 255, 255, 255) : new Color(0, 0, 0, 255);
		sprite.color = tint;
		
		// play the particle system associated with this toggle, if it exists
		ParticleSystemController system = bOn ? systemOn : systemOff;
		if(system){
			system.Play();
		}
	}
}
