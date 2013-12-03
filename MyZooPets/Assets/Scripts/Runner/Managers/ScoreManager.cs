/* 
 * ScoreManager.cs
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
    public int scorePerIncrement = 3;
    public float scoreDistance = 10.0f; //every 10 unit in distance traveled equals to 10 score points
    public float fCoinStreakTime; // if X seconds elapse without picking up a coin, the streak is over
	
    private int mPlayerDistancePoints = 0; //points calculated from distance
    private int mPlayerPoints = 0; //points accumulated in the game (getting item or hitting trigger)
    private int mPlayerCoins = 0; //coins collected in the game
    private int distanceTraveled = 0;

    private float mfCoinStreakCountdown; //
    private int mnCoinStreak; //counting how many coins are picked up in a row

    public int Coins{ 
        get{ 
            return mPlayerCoins; 
        } 
    }

    public int Score{ 
        get{
            return mPlayerDistancePoints + mPlayerPoints;
        }
    }

    public int Distance{ 
        get{ 
            return distanceTraveled; 
        } 
    }
    
    public int GetCoinStreak() {
        return mnCoinStreak;
    }

    void Awake(){
        Reset();
    }

	void Update(){
        if(!RunnerGameManager.Instance.GameRunning) return;

        PlayerController playerController = PlayerController.Instance;
        distanceTraveled = (int)playerController.transform.position.x;

        //calculate the new points from the distance traveled
        int newDistancePoints = (int)(distanceTraveled / scoreDistance) * scorePerIncrement;
        SetDistancePoints(newDistancePoints);

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
        mPlayerPoints = 0;
        mPlayerCoins = 0;
        distanceTraveled = 0;
        SetCoinStreakCountdown(0);
        SetCoinStreak(0);
        AddCoins(0);
    }

    public void AddCoins(int inNumCoinsToAdd){
        SetCoinStreakCountdown(fCoinStreakTime);
        ChangeCoinStreak(1);

		// the player picked up a coin, so increment their streak and reset the countdown
        mPlayerCoins += inNumCoinsToAdd;
    }

    public void AddPoints(int inNumPointsToAdd){
		// score can't go below 0
        mPlayerPoints = Mathf.Max( mPlayerPoints + inNumPointsToAdd, 0 );
    }

    public void SetDistancePoints(int inDistancePoints){
        mPlayerDistancePoints = inDistancePoints;
    }

    private void ChangeCoinStreak( int change ) {
        int nStreak = GetCoinStreak();
        SetCoinStreak( nStreak+change );
    }

    private void SetCoinStreak( int num ) {
        mnCoinStreak = num; 
    }

    //---------------------------------------------------
    //Coin streak count down functions

    private void SetCoinStreakCountdown( float num ) {
        mfCoinStreakCountdown = num;    
    }

    private float GetCoinStreakCountdown() {
        return mfCoinStreakCountdown;   
    }

    private void ChangeCoinStreakCountdown( float change ) {
        float fCountdown = GetCoinStreakCountdown();
        SetCoinStreakCountdown( fCountdown + change );
    }
    //---------------------------------------------------
}
