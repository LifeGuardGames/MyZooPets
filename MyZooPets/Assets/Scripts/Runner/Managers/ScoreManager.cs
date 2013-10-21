/* Sean Duane
 * ScoreManager.cs
 * 8:26:2013   14:38
 * Description:
 * Keeps track of the various 'score' elements of the game.
 * The distance covered is involed in the final score.
 * Score of items gathered is then added to this distance covered for the final score.
 * Also keeps track of coins.
 * This is usually dispalyed via the UI.
 */

using UnityEngine;
using System.Collections;

public class ScoreManager : Singleton<ScoreManager> {
    public int ScorePerIncrement = 10;
    public float ScoreDistance = 10.0f;
    public UILabel ScoreLabel = null;
    public UILabel CoinLabel = null;
    public UILabel DistanceLabel = null;
	
	// if X seconds elapse without picking up a coin, the streak is over
	public float fCoinStreakTime;
	
	// coin streak countdown
	private float mfCoinStreakCountdown;
	private int mnCoinStreak;
	
	public int GetCoinStreak() {
		return mnCoinStreak;
	}
	private void SetCoinStreak( int num ) {
		mnCoinStreak = num;	
	}
	private void ChangeCoinStreak( int change ) {
		int nStreak = GetCoinStreak();
		SetCoinStreak( nStreak+change );
	}
	private float GetCoinStreakCountdown() {
		return mfCoinStreakCountdown;	
	}
	private void SetCoinStreakCountdown( float num ) {
		mfCoinStreakCountdown = num;	
	}
	private void ChangeCoinStreakCountdown( float change ) {
		float fCountdown = GetCoinStreakCountdown();
		SetCoinStreakCountdown( fCountdown + change );
	}	
	
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
        PlayerController playerController = PlayerController.Instance;
        float distanceTraveled = playerController.transform.position.x;

        if (ScoreLabel != null) {
            int newDistanceScore = (int)(distanceTraveled / ScoreDistance) * ScorePerIncrement;
            SetDistancePoints(newDistanceScore);
        }

        if (DistanceLabel != null) {
            DistanceLabel.text = Localization.Localize( "RUNNER_DISTANCE" ) + distanceTraveled.ToString("F1");
        }
		
		// update coin streak countdown (if it's not 0)
		if ( mfCoinStreakCountdown > 0 ) {
			ChangeCoinStreakCountdown( -Time.deltaTime );
		
			// if the countdown ran out, our streak is reset
			if ( mfCoinStreakCountdown <= 0 )
				SetCoinStreak( 0 );
		}
	}

    public void Reset() {
        mPlayerDistancePoints = 0;
        mPlayerCoins = 0;
        mPlayerPoints = 0;
		SetCoinStreakCountdown( 0 );
		SetCoinStreak( 0 );
    }

    public void AddCoins(int inNumCoinsToAdd) {
		// the player picked up a coin, so increment their streak and reset the countdown
		SetCoinStreakCountdown( fCoinStreakTime );
		ChangeCoinStreak( 1 );
		
        mPlayerCoins += inNumCoinsToAdd;
        if (CoinLabel != null)
            CoinLabel.text = Localization.Localize( "RUNNER_COINS" ) + mPlayerCoins;
    }

    public void AddPoints(int inNumPointsToAdd) {
        mPlayerPoints += inNumPointsToAdd;
        if (ScoreLabel != null)
            ScoreLabel.text = Localization.Localize( "RUNNER_SCORE" ) + (mPlayerDistancePoints + mPlayerPoints);
    }

    public void SetDistancePoints(int inDistancePoints) {
        mPlayerDistancePoints = inDistancePoints;
        if (ScoreLabel != null)
            ScoreLabel.text = Localization.Localize( "RUNNER_SCORE" ) + (mPlayerDistancePoints + mPlayerPoints);
    }

    public float GetScore() {
        return (mPlayerDistancePoints + mPlayerPoints);
    }
}
