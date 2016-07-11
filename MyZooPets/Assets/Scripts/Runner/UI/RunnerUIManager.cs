/* 
 * Description:
 * A centralized place to keep the more custom UI.
 * Right now doesnt do much more than give buttons the ability to show/hide certain panels.
 * Any runner specific UI code should probably exist here.
 */

using UnityEngine;
using System.Collections;

public class RunnerUIManager : Singleton<RunnerUIManager>{
	//    public UIPanel GameOverPanel;
	//    public UILabel FinalScoreLabel;
	//    public UILabel FinalDistanceLabel;
	//    public UILabel FinalCoinsLabel;

	// // Use this for initialization
	// void Start () {
	//        GameOverPanel.gameObject.SetActive(false);
	// }
	
	// // Update is called once per frame
	// void Update () {
	
	// }

	//    public void ActivateGameOverPanel() {
	//        // Show our panel
	//        GameOverPanel.gameObject.SetActive(true);

	//        // Determine the pointss
	//        float numCoins = ScoreManager.Instance.Coins;
	//        float numScore = ScoreManager.Instance.GetScore();
	//        float distance = PlayerController.Instance.transform.position.x;

	//        FinalScoreLabel.text = Localization.Localize( "RUNNER_SCORE" ) + numScore;
	//        FinalDistanceLabel.text = Localization.Localize( "RUNNER_DISTANCE" ) + distance.ToString("F1");
	//        FinalCoinsLabel.text = Localization.Localize( "RUNNER_COINS" ) + numCoins;
	//    }

	//    public void DeActivateGameOverPanel() {
	//        // Show our panel
	//        GameOverPanel.gameObject.SetActive(false);
	//    }
}
