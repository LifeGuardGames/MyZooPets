using UnityEngine;
using System.Collections;
using System;

//Calculates evolution meter every 30 secs
//Decides when pet hits evolution stage
public class LevelUpLogic : MonoBehaviour {
	private float timer = 0;
	private float timeInterval = 30f;
    private bool canCheckLevelUp = true; //use this to prohibit update from checking
                                        //level up too many times
    private static TrophyTier awardTrophy = TrophyTier.Null; //trophy awarded when leveling up
    private static int[] levelPoints = {0, 500, 1000, 1500, 2000, 2500, 3500, 4500, 
        5500, 6500, 8500}; //points required for the nxt level
    private const int OK_CARE = 30;
    private const int GOOD_CARE = 70;

    //=========================API============================
    // //initialize level up tracking timer
    // public void Init () {
    //     // timer = timeInterval;
    // }

    //The point requirement for next level up
    public static int NextLevelPoints(){
        return levelPoints[(int)DataManager.CurrentLevel + 1];
    }

    //The trophy that is awarded at the time of level up
    public static TrophyTier AwardedTrophy{
        get{return awardTrophy;}
    }
    //========================================================
    void Awake(){
        timer = timeInterval;
    }
	// Update is called once per frame
	void Update () {
        if(!LoadDataLogic.IsDataLoaded) return;

		timer -= Time.deltaTime;
		if (timer <= 0){
			timer = timeInterval;
			UpdateLevelUpAverage();
		}

        if(canCheckLevelUp) CanLevelUp();
	}

    //Check if the pet is ready to level up
    private void CanLevelUp(){
        int nextLevelIndex = (int)DataManager.CurrentLevel + 1;
        bool canLevelUp = DataManager.Points >= levelPoints[nextLevelIndex];
        if(canLevelUp){
            canCheckLevelUp = false;
            if(DataManager.LevelUpAverageCum <= OK_CARE){ //bad care
                awardTrophy = TrophyTier.Bronze; 
            }else if(DataManager.LevelUpAverageCum <= GOOD_CARE){ //ok care
                awardTrophy = TrophyTier.Silver;
            }else{ //good care
                awardTrophy = TrophyTier.Gold;
            }     
            DataManager.CurrentLevel = (Level)nextLevelIndex;

            canCheckLevelUp = true;
        }
    }

    //calculate evolution meter
	private void UpdateLevelUpAverage(){
		int cumDurationSecs = (int)DataManager.DurationCum.TotalSeconds;

		DateTime now = DateTime.Now;
		TimeSpan tempd = now.Subtract(DataManager.LastLevelUpdatedTime);
		int timeElapsedInSecs = (int)tempd.TotalSeconds; //how many seconds since last played

		double levelUpMeter = getLevelUpMeter();

        //calculate the evo average based on the evoMeter now and the last evoMeter
		double levelUpAverageNow = (levelUpMeter + DataManager.LastLevelUpMeter) / 2;

		//calculate the average evolution value, over the period of gameplay starting
        //from hatching the pet up to now
		DataManager.LevelUpAverageCum = (DataManager.LevelUpAverageCum * cumDurationSecs +
            levelUpAverageNow * timeElapsedInSecs) / (cumDurationSecs + timeElapsedInSecs);
		DataManager.LastLevelUpdatedTime = now;
		DataManager.DurationCum += tempd;
		DataManager.LastLevelUpMeter = levelUpMeter;
	}

    //get the weighted evolution meter
	private static double getLevelUpMeter(){
		return 0.5 * DataManager.Mood + 0.5 * DataManager.Health;
	}
}
