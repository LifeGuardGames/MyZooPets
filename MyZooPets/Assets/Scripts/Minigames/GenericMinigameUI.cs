using UnityEngine;
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
	public MinigameGameOverController gameOverController;
	public MinigameExitConfirmController exitConfirmController;
	public GenericMinigameUIInterface minigameUIInterface;

	public string GetMinigameKey() {
		return minigameUIInterface.GetMinigameKey();
	}

	#region GameManager Calls
	public void StartUI(){
		startController.ShowPanel();
	}

	public void PauseUI(){
		pauseController.ShowPanel();
		TogglePauseButton(false);
	}

	public void GameOverUI(int score, int starCount, int coinCount, int shardCount) {
		gameOverController.PopulateAndShow(score, starCount, coinCount, shardCount);
		TogglePauseButton(false);
    }
	#endregion

	private void ExitConfirmUI() {
		exitConfirmController.ShowPanel();
    }

	private void TogglePauseButton(bool isShow) {
		minigameUIInterface.PauseToggle(isShow);
		if(isShow) {
			pauseButtonTween.Show();
		}
		else {
			pauseButtonTween.Hide();
		}
	}

	#region Calls from components, NOT from buttons
	public void OnTutorial(){
		minigameUIInterface.OnTutorial();
		TogglePauseButton(true);
    }

	public void OnResume(){
		minigameUIInterface.OnResume();
		TogglePauseButton(true);
	}

	public void OnRestart(){
		minigameUIInterface.OnRestart();
		TogglePauseButton(true);
	}

	public void OnExitGame(bool isExitDirectly){
		if(isExitDirectly){
			minigameUIInterface.QuitGame();
		}
		else{
			ExitConfirmUI();
        }
	}

	public void OnPlayAd(){
		// ...
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
