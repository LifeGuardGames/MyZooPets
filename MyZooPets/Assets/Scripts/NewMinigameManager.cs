using UnityEngine;
using System.Collections;

// event arguments for game state callback
public abstract class NewMinigameManager<T> : Singleton<T> where T : MonoBehaviour {

	protected abstract void _Start();

	protected abstract void _StartTutorial();

	protected abstract void _NewGame();

	protected abstract void _PauseGame();

	protected abstract void _ResumeGame();

	protected abstract void _ContinueGame();

	protected abstract void _GameOver();

	protected abstract void _GameOverReward();

	protected abstract void _QuitGame();

	// Generic variables - needs initialization in children
	protected string quitGameScene = "";
	protected float rewardXPMultiplier = 0.0f;
	protected float rewardMoneyMultiplier = 0.0f;
	protected float rewardShardMultiplier = 0.0f;
	protected string minigameKey = "";

	// Cached values for rewarding
	protected int rewardXPAux;
	protected int rewardMoneyAux;
	protected int rewardShardAux;

	protected MinigameTutorial tutorial;

	public string MinigameKey {
		get { return minigameKey; }
	}

	protected int score;
	public int Score {
		get { return score; }
	}

	protected bool isPaused = true;
	public bool IsPaused {
		get { return isPaused; }
	}

	protected bool isContinueAllowed;

	IEnumerator Start() {
		// Have to yield at start because popup UIs need to run Start()
		yield return 0;

		// Sanity check to see if all variables initialized
		if(string.IsNullOrEmpty(minigameKey) || string.IsNullOrEmpty(quitGameScene) || rewardXPMultiplier == 0.0f
		   || rewardMoneyMultiplier == 0.0f || rewardShardMultiplier == 0.0f) {
			Debug.LogError("Minigame variables not initialized");
		}

		GenericMinigameUI.Instance.StartUI();

		_Start();
	}

	public virtual void UpdateScore(int deltaScore) {
		score += deltaScore;
		if(score < 0) {
			score = 0;
		}
	}

	/// <summary>
	/// Tutorial does not track "inTutorial" state, track it in child if you need it
	/// </summary>
	public void StartTutorial() {
		// Save the tutorial key to complete if not existing already
		if(!DataManager.Instance.GameData.Tutorial.IsTutorialFinished(minigameKey)) {
			DataManager.Instance.GameData.Tutorial.ListPlayed.Add(minigameKey);
		}
		_StartTutorial();
	}

	/// <summary>
	/// Checks if tutorial is already complete. If not, run tutorial logic instead
	/// </summary>
	public void NewGame() {
		if(!DataManager.Instance.isDebug && !DataManager.Instance.GameData.Tutorial.IsTutorialFinished(minigameKey) && tutorial == null ||
			DataManager.Instance.isDebug && Constants.GetConstant<bool>("IsMinigameTutOn") && !DataManager.Instance.GameData.Tutorial.IsTutorialFinished(minigameKey)) {
			StartTutorial();
		}
		else {
			isPaused = false;
			isContinueAllowed = false;

			rewardXPAux = 0;
			rewardMoneyAux = 0;
			rewardShardAux = 0;

			// Decrease the pet's hunger after each new game
			//			StatsManager.Instance.ChangeStats(hungerDelta: -5, isInternal: true);

			_NewGame();
			if(tutorial != null) {
				tutorial.Abort();
			}
		}
	}

	//Game is now paused
	public void PauseGame() {
		isPaused = true;
		_PauseGame();
	}

	//Game is now unpaused
	public void ResumeGame() {
		isPaused = false;
		_ResumeGame();
	}

	//Game has been lost, but player watched ad and gets a second chance
	public void ContinueGame() {
		isContinueAllowed = false;
		_ContinueGame();
	}

	public void GameOver() {
		AudioManager.Instance.PlayClip("minigameGameOver");

		// Record highest score
		HighScoreManager.Instance.UpdateMinigameHighScore(minigameKey, score); //TODO: For RUNNERGame, there needs to be an explicit call to a function get score or the logic of runner must change b/c we are using distance and points to add in to score

		// Calculate reward
		rewardXPAux = (int)(score * rewardXPMultiplier);
		rewardMoneyAux = (int)(score * rewardMoneyMultiplier);
		rewardShardAux = (int)(score * rewardShardMultiplier);

		_GameOver();

		GenericMinigameUI.Instance.GameOverUI(isContinueAllowed, score, rewardXPAux, rewardMoneyAux, rewardShardAux);
	}

	public void GameOverReward() {
		//diabled for playtest
		//_GameOverReward();
	}

	public void QuitGame() {
		_QuitGame();
		LoadLevelManager.Instance.StartLoadTransition(quitGameScene);
	}
}
