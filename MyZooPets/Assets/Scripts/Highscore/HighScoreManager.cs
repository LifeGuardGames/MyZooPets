using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class HighScoreManager : Singleton<HighScoreManager> {
    public Dictionary<string, int> MinigameHighScore{
        get{
            return DataManager.Instance.GameData.HighScore.MinigameHighScore;
        }
    } 

    public void UpdateMinigameHighScore(string miniGameKey, int highScore){
        DataManager.Instance.GameData.HighScore.UpdateMinigameHighScore(miniGameKey, highScore);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
