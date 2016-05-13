using UnityEngine;
using System.Collections;

public class GenericMinigameUI : MonoBehaviour {
	public MinigameStartController startController;
	public MinigamePauseController pauseController;
	public MinigameGameOverController gameOverController;

	public void StartUI(){
		startController.ShowPanel();
	}

	public void PauseUI(){
		pauseController.ShowPanel();
	}

	public void GameOverUI(){
		gameOverController.PopulateAndShow();
	}

	#region Calls from components, NOT from buttons
	public void OnTutorial(){
		// ...
	}

	public void OnResume(){
		// ...
	}

	public void OnRestart(){
		// ...
	}

	public void OnExitGame(bool isExitDirectly){
		if(isExitDirectly){
			// ...
		}
		else{
			// ...
		}
	}

	public void OnPlayAd(){
		// ...
	}
	#endregion
}
