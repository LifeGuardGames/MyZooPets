using UnityEngine;
using System.Collections;

public class RunnerUIManager : MonoBehaviour {
    public UIPanel GameOverPanel;
    public UILabel FinalScoreLabel;
    public UILabel FinalDistanceLabel;
    public UILabel FinalCoinsLabel;

	// Use this for initialization
	void Start () {
        GameOverPanel.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ActivateGameOverPanel() {
        // Show our panel
        GameOverPanel.gameObject.SetActive(true);

        // Determine the pointss
        RunnerGameManager runnerGameManager = RunnerGameManager.GetInstance();
        ScoreManager scoreManager = runnerGameManager.ScoreManager;
        float numCoins = scoreManager.Coins;
        float numScore = scoreManager.GetScore();
        float distance = runnerGameManager.PlayerRunner.transform.position.z;

        FinalScoreLabel.text = "Score: " + numScore;
        FinalDistanceLabel.text = "Distance: " + distance;
        FinalCoinsLabel.text = "Coins: " + numCoins;
    }

    public void DeActivateGameOverPanel() {
        // Show our panel
        GameOverPanel.gameObject.SetActive(false);
    }
}
