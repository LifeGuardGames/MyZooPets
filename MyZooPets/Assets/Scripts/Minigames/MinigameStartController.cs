using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Managed by GenericMinigameUI
/// </summary>
public class MinigameStartController : MonoBehaviour {
	public TweenToggleDemux panelTween;
	public GenericMinigameUI UIManager;
	public Text minigameTitle;
	public Text minigameOpening;

	public void Start() {
		minigameTitle.text = Localization.Localize(UIManager.GetMinigameKey() + "_TITLE");
		minigameOpening.text = Localization.Localize(UIManager.GetMinigameKey() + "_OPENING");
	}

	#region Button calls
	public void OnTutorialButton(){
		UIManager.OnTutorial();
	}

	public void OnPlayButton(){
		// Same code flow as restart
		UIManager.OnRestart();
		HidePanel();
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
