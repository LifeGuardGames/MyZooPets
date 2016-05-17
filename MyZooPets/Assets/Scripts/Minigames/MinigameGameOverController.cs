using UnityEngine;
using System.Collections;

public class MinigameGameOverController : MonoBehaviour {
	public TweenToggleDemux panelTween;
	public GenericMinigameUI UIManager;

	public void PopulateAndShow(){
		// ...
		ShowPanel();
	}

	#region Button calls
	public void OnAdButton(){
		UIManager.OnPlayAd();
	}

	public void OnExitButton(){
		UIManager.OnExitGame(true);
	}

	public void OnRestartButton(){
		UIManager.OnRestart();
	}
	#endregion

	public void ShowPanel(){
		panelTween.Show();
	}

	public void HidePanel(){
		panelTween.Hide();
	}
}
