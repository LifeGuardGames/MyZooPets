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
using UnityEngine.UI;
using System.Collections;
using System;

public class RunnerGameManager : NewMinigameManager<RunnerGameManager>{
	public GameObject pausePanel;
	public GameObject starFloatyPrefab;
	public Transform floatyParent;
	private bool tutorial = false;
	private bool acceptInput = true;
	public bool isGameOver;

	//Used for the start of the tutorial, we are not allowed to jump or drop
	private bool specialInput = false;

	//Used for tutorial when a popup shows, everything but the player should be paused and the player can only either jump or drop, but not both
	private RunnerTutorial runnerTutorial;

	public bool IsTutorialRunning{
		get { return tutorial; }
	}

	public bool AcceptInput{
		get { return acceptInput; }
		set { acceptInput = value; }
	}

	public bool SpecialInput{
		get { return specialInput; }
		set { specialInput = value; }
	}

	void Awake(){
		// Parent settings
		minigameKey = "RUNNER";
		quitGameScene = SceneUtils.BEDROOM;
		ResetScore();
	}
	
	// Use this for initialization
	protected override void _Start(){
		Application.targetFrameRate = 60;
		PauseGame();
	}

	// Entry point for tutorial
	protected override void _StartTutorial(){
		isPaused = false; //HACK: This pause should really be set by our parent under StartTutorial() but that could break other games, so it will be used here
		StartCoroutine(StartTutorialHelper());

	}

	public IEnumerator StartTutorialHelper(){
		PlayerController.Instance.MakePlayerVisible(true);
		PlayerController.Instance.Reset();
		acceptInput = false;
		ScoreManager.Instance.Reset();

		RunnerLevelManager.Instance.ResetTutorial();
		MegaHazard.Instance.Reset();
		ParallaxingBackgroundManager.Instance.Reset();

		//The character should run for a second before we pause the game and show the first panel
		yield return new WaitForSeconds(1f);
		runnerTutorial = new RunnerTutorial();
		base.tutorial = runnerTutorial;
	}

	//Called by RUNNERGameTutorialText b/c we are the only ones w/access to our tutorial
	public void AdvanceTutorial(){
		runnerTutorial.Advance();
	}

	protected override void _NewGame(){    //Reset everything and start again, not called during tutorial
		PlayerController.Instance.MakePlayerVisible(true);
		PlayerController.Instance.Reset();
		ScoreManager.Instance.Reset();

		RunnerLevelManager.Instance.Reset();
		MegaHazard.Instance.Reset();
		ParallaxingBackgroundManager.Instance.Reset();
		RunnerItemManager.Instance.Reset();
		FindObjectOfType<CameraFollow>().Reset();
		ResetScore();
		isGameOver = false;
		//RunnerGameTutorialText.Instance.HideAll();
	}

	protected override void _PauseGame(){
		MegaHazard.Instance.PauseParticles();
		ParallaxingBackgroundManager.Instance.PauseParallax();
		PlayerController.Instance.PauseAnimation();
	}

	protected override void _ResumeGame(){
		MegaHazard.Instance.PlayParticles();
		ParallaxingBackgroundManager.Instance.PlayParallax();
		PlayerController.Instance.PlayAnimation();
	}

	protected override void _ContinueGame() {
		PlayerController.Instance.MakePlayerVisible(true);
		MegaHazard.Instance.Reset();
		PlayerController.Instance.ResetSpeed();
		Vector3 spawnPos = FindObjectOfType<PlayerPhysics>().FindGroundedPosition(MegaHazard.Instance.bottomPosition.position);
		PlayerController.Instance.transform.position = spawnPos;
		acceptInput = false; //Prevent us from input anything until we have waited 3 seconds
		StartCoroutine(WarmUp());
	}

	protected override void _GameOver(){
		isGameOver = true;
        AudioManager.Instance.PlayClip("runnerDie");
		PlayerController.Instance.MakePlayerVisible(false);
		isPaused = false; //HACK: This pause should really be called by our parent but that could break other games, so it will be used here
		// play game over sound
		AudioManager.Instance.PlayClip("runnerGameOver");
		RunnerLevelManager.Instance.mCurrentLevelGroup.ReportDeath();
	}

	// Award the actual xp and money, called when tween is complete (Mission, Stats, Crystal, Badge, Analytics, Leaderboard)
	protected override void _GameOverReward(){
		WellapadMissionController.Instance.TaskCompleted("ScoreRUNNER", Score);
		WellapadMissionController.Instance.TaskCompleted("DistanceRUNNER", ScoreManager.Instance.Distance);
		WellapadMissionController.Instance.TaskCompleted("CoinsRUNNER", ScoreManager.Instance.Coins);

		StatsManager.Instance.ChangeStats(
			xpDelta: rewardXPAux,
			xpPos: GenericMinigameUI.Instance.GetXPPanelPosition(),
			coinsDelta: rewardMoneyAux,
			coinsPos: GenericMinigameUI.Instance.GetCoinPanelPosition(),
			animDelay: 0.5f);

		FireCrystalManager.Instance.RewardShards(rewardShardAux);

		BadgeManager.Instance.CheckSeriesUnlockProgress(BadgeType.RUNNER, Score, true);

		Analytics.Instance.RUNNERGameData(Score, RunnerLevelManager.Instance.mCurrentLevelGroup.ToString(), ScoreManager.Instance.Distance);	// TODO level missing!!!

		#if UNITY_IOS
		LeaderBoardManager.Instance.EnterScore((long)GetScore(), "RUNNERLeaderBoard");
		#endif
	}

	protected override void _QuitGame(){
		Application.targetFrameRate = 30;
	}

	private IEnumerator WarmUp(){
		int seconds = 3;
		TweenToggleDemux demux = pausePanel.GetComponent<TweenToggleDemux>();
		demux.Show();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame(); //Skip 2 frames and then pause the game so we are physically on the ground
		PauseGame();
		while(seconds > 0){ //Count from 3 to 0, by 1 second
			pausePanel.GetComponentInChildren<Text>().text = seconds.ToString();
			seconds--;
			yield return new WaitForSeconds(1f);
		}
		demux.Hide();
		ResumeGame();
		acceptInput = true;
	}

	private void ResetScore(){
		rewardXPMultiplier = 1f;
		rewardMoneyMultiplier = 1f;
		rewardShardMultiplier = 1f;
		score = 0;
	}
}
