using UnityEngine;
using System.Collections;

public class MinigamePauseController : MonoBehaviour {
	public TweenToggleDemux panelTween;
	public GenericMinigameUI UIManager;

	#region Button calls
	public void OnResumeButton(){
		UIManager.OnResume();
	}

	public void OnRestartButton(){
		UIManager.OnRestart();
	}

	public void OnExitButton(){
		UIManager.OnExitGame(false);
	}
	#endregion

	public void ShowPanel(){
		panelTween.Show();
	}

	public void HidePanel(){
		panelTween.Hide();
	}
}
