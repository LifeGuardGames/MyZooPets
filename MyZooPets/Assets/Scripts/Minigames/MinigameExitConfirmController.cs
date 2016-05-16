using UnityEngine;

public class MinigameExitConfirmController : MonoBehaviour {
	public TweenToggleDemux panelTween;
	public GenericMinigameUI UIManager;

	#region Button calls
	public void OnClosePanelButton() {
		HidePanel();
    }

	public void OnExitButton() {
		UIManager.OnExitGame(true);
	}
	#endregion

	public void ShowPanel() {
		panelTween.Show();
	}

	public void HidePanel() {
		panelTween.Hide();
	}
}
