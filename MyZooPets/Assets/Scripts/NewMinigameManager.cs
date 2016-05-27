using UnityEngine;
using System.Collections;

public abstract class NewMinigameManager<T> : Singleton<T> where T : MonoBehaviour {
	protected abstract void _Start();
	protected abstract void _NewGame();
	protected abstract void _PauseGame(bool isShow);
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

	public string MinigameKey {
		get { return minigameKey; }
	}

	protected int score;
	public int Score {
		get { return score; }
	}

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

	public void NewGame() {
		rewardXPAux = 0;
		rewardMoneyAux = 0;
		rewardShardAux = 0;
		// Decrease the pet's hunger after each new game
		StatsManager.Instance.ChangeStats(hungerDelta: -5, isInternal: true);

		_NewGame();
	}

	public void PauseGame(bool isShow) {
		_PauseGame(isShow);
    }

	public void GameOver() {
		AudioManager.Instance.PlayClip("minigameGameOver");

		// Record highest score
		HighScoreManager.Instance.UpdateMinigameHighScore(minigameKey, score);

		// Calculate reward
		rewardXPAux = (int)(score * rewardXPMultiplier);
		rewardMoneyAux = (int)(score * rewardXPMultiplier);
		rewardShardAux = (int)(score * rewardXPMultiplier);

		_GameOver();

		GenericMinigameUI.Instance.GameOverUI(score, rewardXPAux, rewardMoneyAux, rewardShardAux);
    }

	public void GameOverReward() {
		_GameOverReward();
    }

	public void QuitGame() {
		_QuitGame();
		LoadLevelManager.Instance.StartLoadTransition(quitGameScene);
    }
}
