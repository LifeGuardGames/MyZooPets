/// <summary>
// The sort of 'center' of the game.
// However, all it really does is track the games running, handles the timescale, and acts as a cheap way to grab popular variables.
// This singleton design, and caching out of certain game component, makes anything using this "glued" to the runner game.
// You must 'unglue' everything you want to use elsewhere, sorry!
// The upside is it's a bit faster.
// 
// Handles Resetting, Game Ending, and TimeScale.
/// </summary>
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class RunnerGameManager : MinigameManager<RunnerGameManager>{
	public string deathLevel;
	void Awake(){
		quitGameScene = SceneUtils.YARD;
	}
	
	// Use this for initialization
	protected override void _Start(){
		Application.targetFrameRate = 60;
	}
	
	protected override void _OnDestroy(){
		Application.targetFrameRate = 30;
	}

	//public SceneTransition scriptTransition;
	public bool GameRunning{
		get { return GetGameState() == MinigameStates.Playing; }
	}

	public override int GetScore(){
		// the score was previously being calculated in that variable as some kind of weird distance traveled / stuff...
		// return ScoreManager.Instance.Score;    
		
		// just changing it to distance run + coins
		int nDistance = ScoreManager.Instance.Distance;
		int nCoins = ScoreManager.Instance.Coins;
		int nScore = nDistance + nCoins;
		
		return  nScore;
	}
	
	/// <summary>
	/// Gets the minigame key.
	/// </summary>
	/// <returns>The minigame key.</returns>
	protected override string GetMinigameKey(){
		return "Runner";	
	}

	protected override bool IsTutorialOn(){
		return Constants.GetConstant<bool>("IsRunnerTutorialOn");
	}

	/// <summary>
	/// Starts a new game.
	/// </summary>
	protected override void _NewGame(){	
		//check for tutorial here.
		if(IsTutorialOn() && (IsTutorialOverride() || 
			!DataManager.Instance.GameData.Tutorial.IsTutorialFinished(RunnerTutorial.TUT_KEY))){
            
			StartTutorial();
			ResetGameTutorial();
		}
		else{
			ResetGame();
		}
	}	

	/// <summary>
	/// game over. 
	/// </summary>
	protected override void _GameOver(){

		// send out distance task
		int distance = ScoreManager.Instance.Distance;
		WellapadMissionController.Instance.TaskCompleted("Distance" + GetMinigameKey(), distance);
		int score = GetScore();
		WellapadMissionController.Instance.TaskCompleted("ScoreRunner",score);
		Analytics.Instance.RunnerGameData(DataManager.Instance.GameData.HighScore.MinigameHighScore[GetMinigameKey()],deathLevel,distance);

		
		// send out coins task
		int coins = ScoreManager.Instance.Coins;
		WellapadMissionController.Instance.TaskCompleted("Coins" + GetMinigameKey(), coins);

#if UNITY_IOS
		LeaderBoardManager.Instance.EnterScore((long)GetScore(), "RunnerLeaderBoard");
#endif
	}		

	/// <summary>
	/// Resets all game components to initial state
	/// </summary>
	public void ResetGame(){
		PlayerController.Instance.MakePlayerVisible(true);
		PlayerController.Instance.Reset();
		ScoreManager.Instance.Reset();
		ScoreUIManager.Instance.Show();

		RunnerLevelManager.Instance.Reset();
		MegaHazard.Instance.Reset();
		ParallaxingBackgroundManager.Instance.Reset();
	}

	public void ResetGameTutorial(){
		PlayerController.Instance.MakePlayerVisible(true);
		PlayerController.Instance.Reset();
		ScoreManager.Instance.Reset();
		RunnerLevelManager.Instance.ResetTutorial();
		MegaHazard.Instance.Reset();
		ParallaxingBackgroundManager.Instance.Reset();
	}

	public void PauseGameWithoutPopup(){
		PauseGameWithPopup(false);   
	}
	
	/// <summary>
	/// Wrapper class to ResumeGame from parent class. yield one frame before
	/// calling the actual resume so the click on the resume button will not
	/// be picked up by the gesture listener.
	/// </summary>
	public void UnPauseGame(){
		StartCoroutine(ResumeGameHelper());
	}

	private IEnumerator ResumeGameHelper(){
		yield return 0; 
		base.ResumeGame();  
	}
	
	/// <summary>
	/// Stop the game and resets the game
	/// </summary>
	public void ActivateGameOver(){
		GameOver();	

		// Disable the player
		PlayerController.Instance.MakePlayerVisible(false);
		
		// play game over sound
		AudioManager.Instance.PlayClip("runnerGameOver");

		//Reset level items
		ItemManager.Instance.Reset();
	}
	
	/// <summary>
	/// Gets the reward.
	/// </summary>
	/// <returns>The reward.</returns>
	/// <param name="eType">type.</param>
	public override int GetReward(MinigameRewardTypes rewardType){
		// for now, just use the standard way
		return GetStandardReward(rewardType);
	}

	/// <summary>
	/// Begin runner game tutorial
	/// </summary>
	private void StartTutorial(){
		SetTutorial(new RunnerTutorial());
	}
}
