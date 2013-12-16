using UnityEngine;
using System.Collections;

//---------------------------------------------------
// ScoreUIManager
// This is strictly a view controller, for displaying
// information to the player
//---------------------------------------------------

public class ScoreUIManager : Singleton<ScoreUIManager> {
    public UILabel coinLabel;
    public UILabel distanceLabel;
    public UILabel scoreLabel;
    public GameObject hudPanel;

    void Awake(){
        hudPanel.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        if(!RunnerGameManager.Instance.GameRunning) return;
        UpdateCoins();
        UpdateDistance();
        UpdateScore();
	}

    public void Show(){
        hudPanel.SetActive(true);
    }

    public void UpdateCoins(){
        coinLabel.text = Localization.Localize("RUNNER_COINS") + ScoreManager.Instance.Coins;
    }

    public void UpdateDistance(){
        distanceLabel.text = Localization.Localize("RUNNER_DISTANCE") + ScoreManager.Instance.Distance;
    }

    public void UpdateScore(){
        // scoreLabel.text = Localization.Localize("RUNNER_SCORE") + ScoreManager.Instance.Score;
    }
}
