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
using UnityEngine.UI;
public class ScoreManager : Singleton<ScoreManager>{
	public Text distanceText;
	public Text coinsText;
	public Text comboText;

	public int scorePerIncrement = 3;

	public float scoreDistance = 10.0f; //every 10 unit in distance traveled equals to 10 score points
	public float coinStreakTime; // if X seconds elapse without picking up a coin, the streak is over

	private int lastDistancePoints=0;
	private int mPlayerDistancePoints = 0; //points calculated from distance
	private int mPlayerPoints = 0; //points accumulated in the game (getting item or hitting trigger)
	private int mPlayerCoins = 0; //coins collected in the game
	private int distanceTraveled = 0;
	private float coinStreakCountdown; 
	private int coinStreak; //counting how many coins are picked up in a row

	private float comboMod=1;



	public int Streak{
		get{
			return coinStreak;
		}
	}
	public int Coins{ 
		get{ 
			return mPlayerCoins; 
		} 
	}
	public float Combo {
		get {
			return comboMod;
		}
	}
	/// <summary>
	/// Gets the score. Which is just the distance score + the number of coins collected
	/// </summary>
	/// <value>The score.</value>
	public int Score{ 
		get{
			return mPlayerDistancePoints + mPlayerPoints;
		}
	}

	public void IncrementCombo(float increment){
		comboMod += increment;
	}

	/// <summary>
	/// Gets the distance traveled by the player.
	/// </summary>
	/// <value>The distance.</value>
	public int Distance{ 
		get{ 
			return distanceTraveled; 
		} 
	}
    
	/// <summary>
	/// Gets the coin streak.
	/// </summary>
	/// <returns>The coin streak.</returns>
	public int GetCoinStreak(){
		return coinStreak;
	}

	void Awake(){
		Reset();
	}

	void Update(){
		if(RunnerGameManager.Instance.IsPaused || RunnerGameManager.Instance.isGameOver) {
			return;
		}
		else {
			PlayerController playerController = PlayerController.Instance;
			distanceTraveled = (int)playerController.transform.position.x;

			//calculate the new points from the distance traveled
			int newDistancePoints = (int)(distanceTraveled / scoreDistance) * scorePerIncrement;
			SetDistancePoints(newDistancePoints);

			// update coin streak countdown (if it's not 0)
			if(coinStreakCountdown > 0) {
				ChangeCoinStreakCountdown(-Time.deltaTime);

				// if the countdown ran out, our streak is reset
				if(coinStreakCountdown <= 0)
					SetCoinStreak(0);
			}
			distanceText.text = Distance.ToString() + " M";
			coinsText.text = Coins.ToString();
			comboText.text = "x" + Combo.ToString("F1");
		}
	}

	/// <summary>
	/// Reset all score variables to zero.
	/// </summary>
	public void Reset(){
		mPlayerDistancePoints = 0;
		lastDistancePoints=0;

		mPlayerPoints = 0;
		mPlayerCoins = 0;
		distanceTraveled = 0;
		SetCoinStreakCountdown(0);
		SetCoinStreak(0);
		AddCoins(0);
		ResetCombo();
	}

	/// <summary>
	/// Reset all score variables to zero.
	/// </summary>
	public void ResetCombo(){
		comboMod=1;
	}

	/// <summary>
	/// Adds the coins.
	/// </summary>
	/// <param name="inNumCoinsToAdd">number coins to add.</param>
	public void AddCoins(int numOfCoinsToAdd){
		SetCoinStreakCountdown(coinStreakTime);
		ChangeCoinStreak(1);

		// the player picked up a coin, so increment their streak and reset the countdown
		// can't go below 0 coins -- this sounds silly, but coins right now is the new "points"
		int pointsToAdd = (int) Mathf.Floor((numOfCoinsToAdd) * comboMod);
		mPlayerCoins = Mathf.Max(mPlayerCoins + pointsToAdd, 0);
		RunnerGameManager.Instance.UpdateScore(pointsToAdd);
	}
		
	public void SetDistancePoints(int distancePoints){
		lastDistancePoints = mPlayerDistancePoints;
		mPlayerDistancePoints = distancePoints;
		int deltaDistancePoints = (distancePoints-lastDistancePoints);
		RunnerGameManager.Instance.UpdateScore(deltaDistancePoints);
	}

	private void ChangeCoinStreak(int change){
		int nStreak = GetCoinStreak();
		SetCoinStreak(nStreak + change);
	}

	private void SetCoinStreak(int num){
		coinStreak = num; 
	}

	//---------------------------------------------------
	//Coin streak count down functions

	private void SetCoinStreakCountdown(float num){
		coinStreakCountdown = num;    
	}

	private float GetCoinStreakCountdown(){
		return coinStreakCountdown;   
	}

	private void ChangeCoinStreakCountdown(float change){
		float fCountdown = GetCoinStreakCountdown();
		SetCoinStreakCountdown(fCountdown + change);
	}
	//---------------------------------------------------

}
