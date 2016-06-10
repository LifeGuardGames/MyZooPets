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
	public List<Image> Inhalers;
	
	// particle systems for this object
	public ParticleSystemController systemOff;
	public ParticleSystemController systemOn;

	public DegradAlert bloodPanelControl;

	// the state of this life
	//private bool bOn;
	
	
	//---------------------------------------------------
	// OnNewGame()
	// When the user restarts the game and a new game
	// begins.
	//---------------------------------------------------
	private void OnNewGame(){
		// since a new game is beginning, toggle on
		//Toggle(true);
	}	
	
	//---------------------------------------------------
	// OnLivesChanged()
	//---------------------------------------------------
	public void OnLivesChanged(int changeInLife){
		// get the number of lives there are
		int nLives = GetLives();
		int nChange = changeInLife;
		Debug.Log(nChange);
		//Debug.Log("Preparing life..." + nLives + " " + nChange);
		if(nChange < 0){
			//Debug.Log("----Loosing a life");
			// if we are LOSING a life and the current lives +1 == this life's index, it means that this life was just lost, so toggle off
			Toggle(false, Inhalers[nLives]);

			// Play the camera shake animation
			if(Camera.main.GetComponent<Animation>() != null){
				Camera.main.GetComponent<Animation>().Play();
				if(nLives == 0) {
					NinjaManager.Instance.GameOver();
				}
			}
		}
		else if(nChange > 0){
			//Debug.Log("---Gaining a life");
			// else if we are GAINING a life and the current lives == this life's index, it means this life was just gained, so toggl eon
			Toggle(true, Inhalers[nLives]);
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
	public void Toggle(bool bOn, Image inhal){
		//Debug.Log("TOGGLING " + bOn);
		// cache the state of this life
		//this.bOn = bOn;
		Debug.Log(bOn);
		// change the tint based on on/off
		Color tint = bOn ? new Color(255, 255, 255, 255) : new Color(0, 0, 0, 255);
		inhal.color = tint;
		
		// play the particle system associated with this toggle, if it exists
		ParticleSystemController system = bOn ? systemOn : systemOff;
		if(system){
			system.Play();
		}
	}
}
