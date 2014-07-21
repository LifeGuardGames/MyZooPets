using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// MinigameLife
// This is a UI element representing a single life
// on the HUD of the player in a minigame.
//---------------------------------------------------

public class MinigameLife : MonoBehaviour{
	// index of this life
	public int nIndex;
	
	// the sprite that this life is uing
	public UISprite sprite;
	
	// particle systems for this object
	public ParticleSystemController systemOff;
	public ParticleSystemController systemOn;
	
	// the state of this life
	//private bool bOn;
	
	void Start(){
		// FFFFFFUFUUUUUUUUUUU
		if(NinjaManager.Instance != null){
			NinjaManager.OnNewGame += OnNewGame;
			NinjaManager.OnLivesChanged += OnLivesChanged;			
		}
		else if(DGTManager.Instance != null){
			DGTManager.OnNewGame += OnNewGame;
			DGTManager.OnLivesChanged += OnLivesChanged;			
		}
		else if(DoctorMatchManager.Instance != null){
			DoctorMatchManager.OnNewGame += OnNewGame;
			DoctorMatchManager.OnLivesChanged += OnLivesChanged;			
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
		if(nChange < 0 && nLives + 1 == nIndex){
			// if we are LOSING a life and the current lives +1 == this life's index, it means that this life was just lost, so toggle off
			Toggle(false);

			// Play the camera shake animation
			if(Camera.main.animation != null){
				Camera.main.animation.Play();
			}
		}
		else if(nChange > 0 && nLives == nIndex){
			// else if we are GAINING a life and the current lives == this life's index, it means this life was just gained, so toggl eon
			Toggle(true);
		}
	}
	
	private int GetLives(){
		int nLives = 0;
		if(NinjaManager.Instance != null){
			nLives = NinjaManager.Instance.GetLives();
		}
		else if(DGTManager.Instance != null){
			nLives = DGTManager.Instance.GetLives();
		}
		else if(DoctorMatchManager.Instance != null){
			nLives = DoctorMatchManager.Instance.GetLives();
		}
		return nLives;
	}
	
	//---------------------------------------------------
	// Toggle()
	// Turn this life on or off.
	//---------------------------------------------------	
	public void Toggle(bool bOn){
		// cache the state of this life
		//this.bOn = bOn;
		
		// change the tint based on on/off
		Color tint = bOn ? new Color(255, 255, 255, 255) : new Color(0, 0, 0, 255);
		sprite.color = tint;
		
		// play the particle system associated with this toggle, if it exists
		ParticleSystemController system = bOn ? systemOn : systemOff;
		if(system)
			system.Play();
	}
}
