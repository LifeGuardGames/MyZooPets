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
		UIManager.OnRestart();
		HidePanel();
    }
	#endregion

	public void ShowPanel(){
		panelTween.Show();
	}

	public void HidePanel(){
		panelTween.Hide();
	}
}
