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
using System;
using System.Collections;
using System.Collections.Generic;
public class RunnerGameManager : NewMinigameManager<RunnerGameManager> {
	public GameObject pausePanel;
	public GameObject starFloatyPrefab;
	public Transform floatyParent;
	private bool paused = true;
	private bool tutorial = false;
	private bool acceptInput = true;
	//Used for the start of the tutorial, we are not allowed to jump or drop
	private bool specialInput = false;
	//Used for tutorial when a popup shows, everything but the player should be paused and the player can only either jump or drop, but not both
	private RunnerTutorial runnerTutorial;

	public bool GameRunning {
		get {
			return !paused;
		}
	}

	public bool IsTutorialRunning {
		get {
			return tutorial;
		}
	}

	public bool AcceptInput {
		get {
			return acceptInput;
		}
		set {
			acceptInput = value;
		}
	}

	public bool SpecialInput {
		get {
			return specialInput;
		}
		set {
			specialInput = value;
		}
	}

	void Awake() {
		// Parent settings
		minigameKey = "RUNNER";
		quitGameScene = SceneUtils.BEDROOM;
		ResetScore();
	}

	public void EndGame() {	
		GameOver();	
	}

	public IEnumerator StartTutorial() {
		PlayerController.Instance.MakePlayerVisible(true);
		PlayerController.Instance.Reset();
		acceptInput = false;
		ScoreManager.Instance.Reset();

		RunnerLevelManager.Instance.ResetTutorial();
		MegaHazard.Instance.Reset();
		ParallaxingBackgroundManager.Instance.Reset();

		yield return new WaitForSeconds(1f);
		runnerTutorial = new RunnerTutorial();
		SetTutorial(runnerTutorial);
	}

	public void AdvanceTutorial() {
		runnerTutorial.Advance();
	}
	// Use this for initialization
	protected override void _Start() {
		Application.targetFrameRate = 60;
	}

	protected override void _NewGame() {	//Reset everything and start again, not called during tutorial
		PlayerController.Instance.MakePlayerVisible(true);
		PlayerController.Instance.Reset();
		ScoreManager.Instance.Reset();

		RunnerLevelManager.Instance.Reset();
		MegaHazard.Instance.Reset();
		ParallaxingBackgroundManager.Instance.Reset();
		RunnerItemManager.Instance.Reset();

		RunnerGameTutorialText.Instance.StartCoroutine(RunnerGameTutorialText.Instance.HideAll());
	}

	protected override void _PauseGame(bool isShow) {
		if (!isShow) {
			paused = !isShow;
			MegaHazard.Instance.PauseParticles();
			ParallaxingBackgroundManager.Instance.PauseParallax();
			PlayerController.Instance.PauseAnimation();
		} else if (!RunnerGameTutorialText.Instance.IsVisible) { //Don't continue the game unless there are no popups on screen
			paused = !isShow;
			MegaHazard.Instance.PlayParticles();
			ParallaxingBackgroundManager.Instance.PlayParallax();
			PlayerController.Instance.PlayAnimation();
		}
	}

	protected override void _GameOver() {
		AudioManager.Instance.PlayClip("runnerDie");
		PlayerController.Instance.MakePlayerVisible(false);

		// play game over sound
		AudioManager.Instance.PlayClip("runnerGameOver");
		RunnerLevelManager.Instance.mCurrentLevelGroup.ReportDeath();


	}

	protected override void _GameOverReward() {
		StatsManager.Instance.ChangeStats(
			xpDelta: rewardXPAux,
			xpPos: GenericMinigameUI.Instance.GetXPPanelPosition(),
			coinsDelta: rewardMoneyAux,
			coinsPos: GenericMinigameUI.Instance.GetCoinPanelPosition(),
			animDelay: 0.5f);
		FireCrystalManager.Instance.RewardShards(rewardShardAux);
		//BadgeManager.Instance.CheckSeriesUnlockProgress(BadgeType.DoctorMatch, NumOfCorrectDiagnose, true);
		//TODO: Implement badges under RunnerGame
	}

	protected override void _QuitGame() {
		Application.targetFrameRate = 30;
	}

	protected override void _ContinueGame() {
		PlayerController.Instance.MakePlayerVisible(true);
		MegaHazard.Instance.Reset();
		PlayerController.Instance.ResetSpeed();
		Vector3 spawnPos = FindObjectOfType<PlayerPhysics>().FindGroundedPosition(MegaHazard.Instance.bottomPosition.position);
		PlayerController.Instance.transform.position = spawnPos;
		acceptInput=false; //Prevent us from input anything until we have waited 3 seconds
		StartCoroutine(WarmUp());
	}
	private IEnumerator WarmUp() {
		int seconds=3;
		TweenToggleDemux demux = pausePanel.GetComponent<TweenToggleDemux>();
		demux.Show();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame(); //Skip 2 frames and then pause the game so we are physically on the ground
		PauseGame(false);
		while (seconds>0) { //Count from 3 to 0, by 1 second
			pausePanel.GetComponentInChildren<Text>().text=seconds.ToString();
			seconds--;
			yield return new WaitForSeconds(1f);
		}
		demux.Hide();
		PauseGame(true);
		acceptInput=true;
	}
	private void ResetScore() {
		rewardXPMultiplier = 1f;
		rewardMoneyMultiplier = 1f;
		rewardShardMultiplier = 1f;
		score = 0;
	}
}
