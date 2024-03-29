﻿using UnityEngine;
using System.Collections.Generic;

/* Sean Duane
 * 8:26:2013   14:43
 * Description:
 * Handles updating the current translation of the spawned ParallaxingBackgrounds.
 * The mesh never moves, we just set the texture offset to let that do the "moving:
 */
public class ParallaxingBackgroundGroup : MonoBehaviour {
    public string GroupID = "";
    public List<Parallax> ParralaxingTextures = new List<Parallax>();
	
    void Start(){
		if(!RunnerGameManager.Instance.IsPaused) {
			PlayParallax();
		}
    }

    public void SetAlpha(float alpha) {
         foreach(Parallax currentParallax in ParralaxingTextures) {
            Color currentColor = currentParallax.GetComponent<Renderer>().material.color;
            currentColor.a = alpha;
            currentParallax.GetComponent<Renderer>().material.color = currentColor;
         }
    }

	public void PlayParallax(){
		foreach(Parallax currentParallax in ParralaxingTextures)
			currentParallax.Play();
	}

	public void PauseParallax(){
		foreach(Parallax currentParallax in ParralaxingTextures)
			currentParallax.Pause();
	}

    private void GameStateChange(object sender, GameStateArgs args){
        switch(args.GetGameState()){
			case MinigameStates.GameOver:
				PauseParallax();
			break;
            case MinigameStates.Paused:
                PauseParallax();
            break;
            case MinigameStates.Playing:
                PlayParallax();
            break;
        }
    }
}
