/* Sean Duane
 * RunnerGameManager.cs
 * 8:26:2013   14:36
 * Description:
 * The sort of 'center' of the game.
 * However, all it really does is track the games running, handles the timescale, and acts as a cheap way to grab popular variables.
 * This singleton design, and caching out of certain game component, makes anything using this "glued" to the runner game.
 * You must 'unglue' everything you want to use elsewhere, sorry!
 * The upside is it's a bit faster.
 * 
 * Handles Resetting, Game Ending, and TimeScale.
 */
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class RunnerGameManager : Singleton<RunnerGameManager> {
    public SceneTransition scriptTransition;
    public bool GameRunning{
        get; 
        protected set;
    }
	
	// Use this for initialization
	void Start() {
		if(DataManager.Instance.GameData.Cutscenes.ListViewed.Contains("Cutscene_Runner") == false ) {
			ShowCutscene();
			GameRunning = false;	
		}else
            GameRunning = true;
	}
	
	//---------------------------------------------------
	// ShowCutscene()
	//---------------------------------------------------	
	private void ShowCutscene() {
		GameObject resourceMovie = Resources.Load("Cutscene_Runner") as GameObject;
		LgNGUITools.AddChildWithPosition( GameObject.Find("Anchor-Center"), resourceMovie );
		CutsceneFrames.OnCutsceneDone += CutsceneDone;	
	}
	
    private void CutsceneDone(object sender, EventArgs args){
		DataManager.Instance.GameData.Cutscenes.ListViewed.Add("Cutscene_Runner");	
		CutsceneFrames.OnCutsceneDone -= CutsceneDone;
        GameRunning = true;
    }	
	
    public void ResetGame() {
        Time.timeScale = 1f;

        RunnerUIManager.Instance.DeActivateGameOverPanel();
        GameRunning = true;

        PlayerController.Instance.gameObject.SetActive(true);
        PlayerController.Instance.Reset();
        ScoreManager.Instance.Reset();
        LevelManager.Instance.Reset();
        MegaHazard.Instance.Reset();
        ParallaxingBackgroundManager.Instance.Reset();
    }

    public void PauseGame(){
        GameRunning = false;
    }

    public void UnPauseGame(){
        GameRunning = true;
    }

    public void ActivateGameOver() {
        GameRunning = false;

        RunnerUIManager.Instance.ActivateGameOverPanel();

        // Disable the player
        PlayerController.Instance.gameObject.SetActive(false);
		
		// play game over sound
		AudioManager.Instance.PlayClip( "runnerGameOver" );

        print("game over");
        ItemManager.Instance.Reset();
    }

    public void QuitGame(){
        scriptTransition.StartTransition( SceneUtils.YARD );
    }

    public void IncreaseTimeSpeed(float inIncreaseTime) {
        //Limit timescale to 2. Beyond 2 the game becomes too fast to be playable
        if(Time.timeScale != 2){
            Time.timeScale += inIncreaseTime;
        }
    }

    public void SlowTimeSpeed(float inTimeDivisor) {
        Time.timeScale /= inTimeDivisor;
        if (Time.timeScale < 1f)
            Time.timeScale = 1f;
    }
}
