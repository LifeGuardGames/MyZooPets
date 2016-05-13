using UnityEngine;
using System.Collections;

public class MinigameStartController : MonoBehaviour {
	public TweenToggleDemux panelTween;
	public GenericMinigameUI UIManager;

	#region Button calls
	public void OnTutorialButton(){
		UIManager.OnTutorial();
	}

	public void OnPlayButton(){
		// Same code flow as restart
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
