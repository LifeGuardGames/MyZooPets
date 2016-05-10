/* Sean Duane
 * ParallaxingBackgroundGroup.cs
 * 8:26:2013   14:43
 * Description:
 * Handles updating the current translation of the spawned ParralaxingBackgrounds.
 * The mesh never moves, we jiust set the texture offset to let that do the "moving:
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParallaxingBackgroundGroup : MonoBehaviour {
    public string GroupID = "";
    public List<Parallax> ParralaxingTextures = new List<Parallax>();
	
    void Start(){
        RunnerGameManager.OnStateChanged += GameStateChange;
        if(RunnerGameManager.Instance.GameRunning)
            PlayParallax();

    }
	
    void OnDestroy(){
        RunnerGameManager.OnStateChanged -= GameStateChange;
    }

    public void SetAlpha(float alpha) {
         foreach(Parallax currentParallax in ParralaxingTextures) {
            Color currentColor = currentParallax.GetComponent<Renderer>().material.color;
            currentColor.a = alpha;
            currentParallax.GetComponent<Renderer>().material.color = currentColor;
         }
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

    private void PlayParallax(){
        foreach(Parallax currentParallax in ParralaxingTextures)
            currentParallax.Play();
    }

    private void PauseParallax(){
        foreach(Parallax currentParallax in ParralaxingTextures)
            currentParallax.Pause();
    }
}
