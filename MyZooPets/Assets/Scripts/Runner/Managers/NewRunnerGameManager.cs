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

public class NewRunnerGameManager : NewMinigameManager<NewRunnerGameManager> {
	private bool paused = true;
	private bool tutorial = false;

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

	void Awake() {
		// Parent settings
		minigameKey = "RUNNER";
		quitGameScene = SceneUtils.BEDROOM;
		ResetScore();
	}
	public void ActivateGameOver(){	
		UpdateScore(ScoreManager.Instance.Score);
		GameOver();	

		// Disable the player
		PlayerController.Instance.MakePlayerVisible(false);

		// play game over sound
		AudioManager.Instance.PlayClip("runnerGameOver");

		//Reset level items
		RunnerItemManager.Instance.Reset();
	}
	public IEnumerator StartTutorial() {
		ResetScore();
		tutorial = true;
		yield return new WaitForEndOfFrame();
	}
	// Use this for initialization
	protected override void _Start() {
		Application.targetFrameRate = 60;
	}

	protected override void _NewGame() {	//Reset everything and start again, not called during tutorial
		PlayerController.Instance.MakePlayerVisible(true);
		PlayerController.Instance.Reset();
		ScoreManager.Instance.Reset();
//		ScoreUIManager.Instance.Show(); //TODO: Custom UI for Runner and reset it here

		RunnerLevelManager.Instance.Reset();
		MegaHazard.Instance.Reset();
		ParallaxingBackgroundManager.Instance.Reset();
	}

	protected override void _PauseGame(bool isShow) {
		paused = !isShow;
		if (isShow) {
			MegaHazard.Instance.PlayParticles();
		} else {
			MegaHazard.Instance.PauseParticles();
		}
		/* Player.StopMoving()
		 * MegaHazard.StopMoving()
		 * Pause coins under magnetic field?
		 */
	}
	protected override void _GameOver() {
		/* Player.StopMoving()
		 * MegaHazard.StopMoving()
		 */
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

	private void ResetScore() {
		rewardXPMultiplier = 1f;
		rewardMoneyMultiplier = 1f;
		rewardShardMultiplier = 1f;
		score = 0;
	}
}
