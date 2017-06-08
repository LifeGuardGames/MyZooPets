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
		if(DataManager.Instance.GameData.MinGames.mode == MiniGameModes.None) { 
			minigameOpening.text = Localization.Localize(UIManager.GetMinigameKey() + "_OPENING");
		}
		else if (DataManager.Instance.GameData.MinGames.mode == MiniGameModes.Time) {
			minigameOpening.text = Localization.Localize(UIManager.GetMinigameKey() + "_TIME");
		}
		else if(DataManager.Instance.GameData.MinGames.mode == MiniGameModes.Life) {
			minigameOpening.text = Localization.Localize(UIManager.GetMinigameKey() + "_LIFE");
		}
		else if(DataManager.Instance.GameData.MinGames.mode == MiniGameModes.Speed) {
			minigameOpening.text = Localization.Localize(UIManager.GetMinigameKey() + "_SPEED");
		}
	}

	#region Button calls
	public void OnTutorialButton(){
		UIManager.OnTutorial();
		HidePanel();
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
