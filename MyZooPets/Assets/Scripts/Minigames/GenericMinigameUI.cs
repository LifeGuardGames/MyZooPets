using UnityEngine;
using System;

/// <summary>
/// This is modular generic minigame UI manager meant for all minigames, simply drag
/// and drop this prefab into the canvas and you are good to go!
/// 
/// Handles: StartUI, PauseUI, GameOverUI calls
/// </summary>
[RequireComponent(typeof(GenericMinigameUIInterface))]
public class GenericMinigameUI : Singleton<GenericMinigameUI> {
	public TweenToggle pauseButtonTween;
	public MinigameStartController startController;
	public MinigamePauseController pauseController;
	public MinigameContinueController continueController;
	public MinigameGameOverController gameOverController;
	public MinigameExitConfirmController exitConfirmController;
	public GenericMinigameUIInterface minigameUIInterface;

	// Storing function for game over if user dont want to continue
	private Action storedGameOverFunction;

	public string GetMinigameKey() {
		return minigameUIInterface.GetMinigameKey();
	}

	public void OnPauseButton() {
		pauseController.ShowPanel();
		minigameUIInterface.OnPause();
		TogglePauseButton(false);
	}

	public void TogglePauseButton(bool isShow) {
		if(isShow) {
			pauseButtonTween.Show();
		}
		else {
			pauseButtonTween.Hide();
		}
	}

	#region GameManager Calls
	public void StartUI() {
		startController.ShowPanel();
	}

	public void GameOverUI(bool allowContinue, int score, int starCount, int coinCount, int shardCount) {
		// If continue is allowed, 40% chance to show ads granted that it is ready
		Debug.Log("CONTINUE DEBUG CHANGE HERE");
		if(allowContinue && AdManager.Instance.IsAdReady()) { // && UnityEngine.Random.Range(0, 10) <= 3) {
			Debug.Log("Ads check pass");
			continueController.ShowPanel();
			minigameUIInterface.OnPause(); //NOTE: Pause the game when the continue button shows up. Unpaused under OnContinue
			storedGameOverFunction = null;
			storedGameOverFunction = () => gameOverController.PopulateAndShow(score, starCount, coinCount, shardCount);
		}
		else {
			Debug.Log("Ads not playing");
			gameOverController.PopulateAndShow(score, starCount, coinCount, shardCount);
			TogglePauseButton(false);
		}
	}
	#endregion

	#region Calls from components, NOT from buttons
	public void OnTutorial() {
		minigameUIInterface.OnTutorial();
		TogglePauseButton(true);
	}

	public void OnResume() {
		minigameUIInterface.OnResume();
		TogglePauseButton(true);
	}

	public void OnRestart() {
		minigameUIInterface.OnRestart();
		TogglePauseButton(true);
	}

	public void OnExitGame(bool isExitDirectly) {
		if(isExitDirectly) {
			minigameUIInterface.QuitGame();
			Time.timeScale = 1.0f;
		}
		else {
			exitConfirmController.ShowPanel();
		}
	}

	public void OnPlayAd() {
		AdManager.Instance.ShowAd(delegate (bool result) {
			if(result) {    // Finished ads
				minigameUIInterface.OnContinue();
				//minigameUIInterface.OnPause();
			}
			else {          // Ads failed somehow, fail gracefully
				OnContinueRejected();
			}
		});
	}

	// Actually reward the player, UI logic is separate
	public void OnReward() {
		minigameUIInterface.OnReward();
	}

	// When the user chooses not to watch an ad
	public void OnContinueRejected() {
		// Continue the stored game over function
		storedGameOverFunction();
	}
	#endregion

	// Use for rewarding
	public Vector3 GetXPPanelPosition() {
		return Vector3.zero;
	}

	// Use for rewarding
	public Vector3 GetCoinPanelPosition() {
		return Vector3.zero;
	}
}
