using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour {
    public int ScorePerIncrement = 10;
    public float ScoreDistance = 10.0f;
    public UILabel ScoreLabel = null;
    public UILabel CoinLabel = null;
    public UILabel DistanceLabel = null;

    private int mPlayerDistancePoints = 0;
    private int mPlayerCoins = 0;
    private int mPlayerPoints = 0;

    public float Coins { get { return mPlayerCoins; } }
    public float PlayerPoints { get { return mPlayerDistancePoints; } }
    public float DistancePoints { get { return mPlayerPoints; } }

	// Use this for initialization
    void Start() {
        AddCoins(0);
	}
	
	// Update is called once per frame
	void Update() {
        PlayerRunner playerRunner = RunnerGameManager.GetInstance().PlayerRunner;
        if (playerRunner != null) {
            float distanceTraveled = playerRunner.transform.position.z;

            if (ScoreLabel != null) {
                int newDistanceScore = (int)(distanceTraveled / ScoreDistance) * ScorePerIncrement;
                SetDistancePoints(newDistanceScore);
            }

            if (DistanceLabel != null) {
                DistanceLabel.text = "Distance: " + distanceTraveled.ToString("F1");
            }
        }
	}

    public void Reset() {
        mPlayerDistancePoints = 0;
        mPlayerCoins = 0;
        mPlayerPoints = 0;
    }

    public void AddCoins(int inNumCoinsToAdd) {
        mPlayerCoins += inNumCoinsToAdd;
        if (CoinLabel != null)
            CoinLabel.text = "Coins: " + mPlayerCoins;
    }

    public void AddPoints(int inNumPointsToAdd) {
        mPlayerPoints += inNumPointsToAdd;
        if (ScoreLabel != null)
            ScoreLabel.text = "Score: " + (mPlayerDistancePoints + mPlayerPoints);
    }

    public void SetDistancePoints(int inDistancePoints) {
        mPlayerDistancePoints = inDistancePoints;
        if (ScoreLabel != null)
            ScoreLabel.text = "Score: " + (mPlayerDistancePoints + mPlayerPoints);
    }

    public float GetScore() {
        return (mPlayerDistancePoints + mPlayerPoints);
    }
}
