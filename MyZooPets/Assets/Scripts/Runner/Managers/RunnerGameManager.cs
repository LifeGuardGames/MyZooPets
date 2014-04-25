/* 
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

public class RunnerGameManager : MinigameManager<RunnerGameManager> {
    
    //public SceneTransition scriptTransition;
    public bool GameRunning{
        get { return GetGameState() == MinigameStates.Playing; } 
        //protected set;
    }
	
	//---------------------------------------------------
	// GetMinigameKey()
	//---------------------------------------------------	
	protected override string GetMinigameKey() {
		return "Runner";	
	}	

    public override int GetScore() {
		// the score was previously being calculated in that variable as some kind of weird distance traveled / stuff...
       // return ScoreManager.Instance.Score;    
		
		// just changing it to distance run + coins
		int nDistance = ScoreManager.Instance.Distance;
		int nCoins = ScoreManager.Instance.Coins;
		int nScore = nDistance + nCoins;
		
		return  nScore;
    }
	
	// Use this for initialization
	protected override void _Start() {
        Application.targetFrameRate = 60;
	}

    protected override void _OnDestroy(){
        Application.targetFrameRate = 30;
    }
	
	//---------------------------------------------------
	// _NewGame()
	//---------------------------------------------------	
	protected override void _NewGame() {	
        //check for tutorial here.
        if(TutorialOn() && (IsTutorialOverride() || 
            !DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(RunnerTutorial.TUT_KEY))){
            
            StartTutorial();
            ResetGameTutorial();
        }else{

            ResetGame();
        }

	}	
	
	//---------------------------------------------------
	// _GameOver()
	//---------------------------------------------------		
	protected override void _GameOver() {
		// send out distance task
		int nDistance = ScoreManager.Instance.Distance;
		WellapadMissionController.Instance.TaskCompleted( "Distance" + GetMinigameKey(), nDistance );
        Analytics.Instance.RunnerPlayerDistanceRan(nDistance);
		
		// send out coins task
		int nCoins = ScoreManager.Instance.Coins;
		WellapadMissionController.Instance.TaskCompleted( "Coins" + GetMinigameKey(), nCoins );

        // check for badge unlock;
        UpdateBadgeProgress();
		
		// reset the game here so that time scale is returned to normal (for when the user exits the game)
        Time.timeScale = 1f;
        // ResetGame();
	}		
	
    //---------------------------------------------------
    // ResetGame()
    // Set all game components to initial state
    //---------------------------------------------------
    public void ResetGame() {
        Time.timeScale = 1f;

        PlayerController.Instance.gameObject.SetActive(true);
        PlayerController.Instance.Reset();
        ScoreManager.Instance.Reset();
        ScoreUIManager.Instance.Show();

        RunnerLevelManager.Instance.Reset();
        MegaHazard.Instance.Reset();
        ParallaxingBackgroundManager.Instance.Reset();
    }

    public void ResetGameTutorial(){
        Time.timeScale = 1f;

        PlayerController.Instance.gameObject.SetActive(true);
        PlayerController.Instance.Reset();
        ScoreManager.Instance.Reset();
        RunnerLevelManager.Instance.ResetTutorial();
        MegaHazard.Instance.Reset();
        ParallaxingBackgroundManager.Instance.Reset();
    }

    public void PauseGameWithoutPopup(){
        PauseGameWithPopup(false);   
    }

    //---------------------------------------------------
    // UnPauseGame()
    // wrapper class to ResumeGame from parent class
    // yield one frame before calling the actual resume
    // so the click on the resume button will not be picked up
    // by the gesture listener
    //---------------------------------------------------
    public void UnPauseGame(){
        StartCoroutine(ResumeGameHelper());
    }

    private IEnumerator ResumeGameHelper(){
        yield return 0; 
        base.ResumeGame();  
    }

    //---------------------------------------------------
    // ActivateGameOver()
    // Stop the game and resets the game
    //---------------------------------------------------
    public void ActivateGameOver(){
		GameOver();	

        // Disable the player
        PlayerController.Instance.gameObject.SetActive(false);
		
		// play game over sound
		AudioManager.Instance.PlayClip( "runnerGameOver" );

        //Reset level items
        ItemManager.Instance.Reset();
    }

    //---------------------------------------------------
    // IncreaseTimeSpeed()
    // Increase the time scale to make it looks like the
    // player is running faster
    //---------------------------------------------------
    public void IncreaseTimeSpeed(float inIncreaseTime) {
        Time.timeScale += inIncreaseTime;
    }

    public void SlowTimeSpeed(float inTimeDivisor) {
        Time.timeScale /= inTimeDivisor;
        if (Time.timeScale < 1f)
            Time.timeScale = 1f;
    }

    //---------------------------------------------------
    // GetReward()
    //---------------------------------------------------       
    public override int GetReward( MinigameRewardTypes eType ) {
        // for now, just use the standard way
        return GetStandardReward( eType );
    }   

    //---------------------------------------------------
    // UpdateBadgeProgress()
    // Check with BadgeLogic to see if any badge can be unlocked
    //---------------------------------------------------
    private void UpdateBadgeProgress(){
        BadgeLogic.Instance.CheckSeriesUnlockProgress(BadgeType.RunnerDistance, 
            ScoreManager.Instance.Distance, true);
    }
	
    //---------------------------------------------------
    // StartTutorial()
    // Begin runner game tutorial
    //---------------------------------------------------       
    private void StartTutorial(){
        SetTutorial(new RunnerTutorial());
    }
}
