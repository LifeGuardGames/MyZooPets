using UnityEngine;
using UnityEngine.UI;

public class MinigameStartController : MonoBehaviour {
	public TweenToggleDemux panelTween;
	public GenericMinigameUI UIManager;
	public Text minigameTitle;
	public Text minigameOpening;

	public void Start() {
		
	}

	#region Button calls
	public void OnTutorialButton(){
		UIManager.OnTutorial();
	}

	public void OnPlayButton(){
		// Same code flow as restart
		UIManager.OnRestart();
	}

	public void OnExitButton() {
		UIManager.OnExitGame(true);
	}
	#endregion

	public void ShowPanel(){
		panelTween.Show();
	}

	public void HidePanel(){
		panelTween.Hide();
	}
}
