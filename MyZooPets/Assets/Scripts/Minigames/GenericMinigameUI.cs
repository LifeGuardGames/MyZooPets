using UnityEngine;

// Requires MinigameGenericInterface for actual scene calling
[RequireComponent(typeof(MinigameGenericInterface))]
public class GenericMinigameUI : MonoBehaviour {
	public TweenToggle pauseButtonTween;
	public MinigameStartController startController;
	public MinigamePauseController pauseController;
	public MinigameGameOverController gameOverController;
	public MinigameExitConfirmController exitConfirmController;
	public MinigameGenericInterface minigameInterface;

	public void GetMinigameKey() {
	}

	public void StartUI(){
		startController.ShowPanel();
	}

	public void PauseUI(){
		pauseController.ShowPanel();
		TogglePauseButton(false);
	}

	public void GameOverUI(){
		gameOverController.PopulateAndShow();
		TogglePauseButton(false);
    }

	public void ExitConfirmUI() {
		exitConfirmController.ShowPanel();
    }

	private void TogglePauseButton(bool isShow) {
		minigameInterface.PauseToggle(isShow);
		if(isShow) {
			pauseButtonTween.Show();
		}
		else {
			pauseButtonTween.Hide();
		}
	}

	#region Calls from components, NOT from buttons
	public void OnTutorial(){
		minigameInterface.OnTutorial();
    }

	public void OnResume(){
		minigameInterface.OnResume();
		TogglePauseButton(true);
	}

	public void OnRestart(){
		minigameInterface.OnRestart();
		TogglePauseButton(true);
	}

	public void OnExitGame(bool isExitDirectly){
		if(isExitDirectly){
			minigameInterface.QuitGame();
		}
		else{
			ExitConfirmUI();
        }
	}

	public void OnPlayAd(){
		// ...
	}
	#endregion
}
