using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

//---------------------------------------------------
// MinigameLife
// This is a UI element representing a single life
// on the HUD of the player in a minigame.
//---------------------------------------------------
public class MinigameLife : MonoBehaviour{
	public List<Image> Inhalers;
	
	// particle systems for this object
	public ParticleSystemController systemOff;
	public ParticleSystemController systemOn;

	//---------------------------------------------------
	// OnNewGame()
	// When the user restarts the game and a new game
	// begins.
	//---------------------------------------------------
	private void OnNewGame(){
		// since a new game is beginning, toggle on
		//Toggle(true);
	}	
	
	public void OnLivesChanged(int deltaLife){
		// get the number of lives there are
		int lifeCount = NinjaGameManager.Instance.LifeCount;
		int nChange = deltaLife;
		//Debug.Log("Preparing life..." + nLives + " " + nChange);
		if(nChange < 0 && lifeCount - nChange >= 0){
			//Debug.Log("----Loosing a life");
			// if we are LOSING a life and the current lives +1 == this life's index, it means that this life was just lost, so toggle off
			ToggleLifeIndicator(false, Inhalers[lifeCount]);

			// Play the camera shake animation
			if(Camera.main.GetComponent<Animation>() != null){
				Camera.main.GetComponent<Animation>().Play();
				if(lifeCount == 0) {
					NinjaGameManager.Instance.GameOver();
				}
			}
		}
		else if(nChange > 0 && lifeCount - nChange >= 0) {
			//Debug.Log("---Gaining a life");
			// else if we are GAINING a life and the current lives == this life's index, it means this life was just gained, so toggl eon
			ToggleLifeIndicator(true, Inhalers[lifeCount]);
		}
	}
	
	public void Reset() {
		foreach(Image inhalerSprite in Inhalers) {
			ToggleLifeIndicator(true, inhalerSprite);
		}
	}

	public void ToggleLifeIndicator(bool isOn, Image inhalerSprite){
		// change the tint based on on/off
		inhalerSprite.color = isOn ? Color.white : Color.black;

		// play the particle system associated with this toggle, if it exists
		ParticleSystemController system = isOn ? systemOn : systemOff;
		if(system){
			system.Play();
		}
	}
}
