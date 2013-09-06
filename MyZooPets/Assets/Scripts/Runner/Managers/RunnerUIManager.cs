/* Sean Duane
 * RunnerUIManager.cs
 * 8:26:2013   14:38
 * Description:
 * A centralized place to keep the more custom UI.
 * Right now doesnt do much more than give buttons the ability to show/hide certain panels.
 * Any runner specific UI code should probably exist here.
 */

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

        FinalScoreLabel.text = Localization.Localize( "RUNNER_SCORE" ) + numScore;
        FinalDistanceLabel.text = Localization.Localize( "RUNNER_DISTANCE" ) + distance.ToString("F1");
        FinalCoinsLabel.text = Localization.Localize( "RUNNER_COINS" ) + numCoins;
    }

    public void DeActivateGameOverPanel() {
        // Show our panel
        GameOverPanel.gameObject.SetActive(false);
    }
}
