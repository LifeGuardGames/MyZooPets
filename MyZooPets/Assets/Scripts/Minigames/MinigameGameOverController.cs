using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Managed by GenericMinigameUI
/// </summary>
public class MinigameGameOverController : MonoBehaviour {
	public TweenToggleDemux panelTween;
	public GenericMinigameUI UIManager;
	public Text scoreText;
	public Text starText;
	public Text coinText;
	public Text shardText;

	public TweenToggle restartButtonToggle;
	public TweenToggle quitButtonToggle;

	void Awake() {
		RewardManager.OnAllRewardsDone += ShowButtons;
	}

	void OnDestroy() {
		RewardManager.OnAllRewardsDone -= ShowButtons;
	}

	public void PopulateAndShow(int score, int starCount, int coinCount, int shardCount){
		scoreText.text = score.ToString();
        starText.text = starCount.ToString();
		coinText.text = coinCount.ToString();
		shardText.text = shardCount.ToString();
		ShowPanel();

		// Actually reward the player
		UIManager.OnReward();
	}

	#region Button calls
	public void OnAdButton(){
		UIManager.OnPlayAd();
	}

	public void OnExitButton(){
		UIManager.OnExitGame(true);
    }

	public void OnRestartButton(){
		HideButton();
		HidePanel();
		UIManager.OnRestart();
    }
	#endregion

	public void ShowPanel(){
		panelTween.Show();
	}

	public void HidePanel(){
		panelTween.Hide();
	}

	public void ShowButtons(object sender, System.EventArgs args) {
		restartButtonToggle.Show();
		quitButtonToggle.Show();
		InventoryUIManager.Instance.HidePanel();
    }

	public void HideButton() {
		restartButtonToggle.Hide();
		quitButtonToggle.Hide();
	}
}
