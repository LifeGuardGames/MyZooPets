using UnityEngine;

public class MinigameContinueController : MonoBehaviour {
	public TweenToggleDemux panelTween;
	public GenericMinigameUI UIManager;

	#region Button calls
	public void OnPlayAdContinueButton() {
		UIManager.OnPlayAd();
		HidePanel();
	}

	public void OnExitButton() {
		UIManager.OnContinueRejected();
		HidePanel();
	}
	#endregion

	public void ShowPanel() {
		panelTween.Show();
	}

	public void HidePanel() {
		panelTween.Hide();
	}
}
