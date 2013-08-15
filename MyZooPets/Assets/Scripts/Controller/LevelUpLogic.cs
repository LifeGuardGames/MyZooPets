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
    private static BadgeTier awardBadge = BadgeTier.Null; //trophy awarded when leveling up
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
        return levelPoints[(int)DataManager.Instance.Level.CurrentLevel + 1];
    }

    //The trophy that is awarded at the time of level up
    public static BadgeTier AwardedBadge{
        get{return awardBadge;}
    }
    //========================================================
    void Awake(){
        timer = timeInterval;
    }
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;
		if (timer <= 0){
			timer = timeInterval;
			UpdateLevelUpAverage();
		}

        //TO DO: make a listener that listens to OnLevelUp event instead
        if(canCheckLevelUp) CanLevelUp();
	}

    //Check if the pet is ready to level up
    private void CanLevelUp(){
        int nextLevelIndex = (int)DataManager.Instance.Level.CurrentLevel + 1;
        bool canLevelUp = DataManager.Instance.Stats.Points >= levelPoints[nextLevelIndex];
        if(canLevelUp){
            canCheckLevelUp = false;
            if(DataManager.Instance.Level.LevelUpAverageCum <= OK_CARE){ //bad care
                awardBadge = BadgeTier.Bronze; 
            }else if(DataManager.Instance.Level.LevelUpAverageCum <= GOOD_CARE){ //ok care
                awardBadge = BadgeTier.Silver;
            }else{ //good care
                awardBadge = BadgeTier.Gold;
            }     
            DataManager.Instance.Level.CurrentLevel = (Level)nextLevelIndex;

            canCheckLevelUp = true;
        }
    }

    //calculate evolution meter
	private void UpdateLevelUpAverage(){
		int cumDurationSecs = (int)DataManager.Instance.Level.DurationCum.TotalSeconds;

		DateTime now = DateTime.Now;
		TimeSpan tempd = now.Subtract(DataManager.Instance.Level.LastLevelUpdatedTime);
		int timeElapsedInSecs = (int)tempd.TotalSeconds; //how many seconds since last played

		double levelUpMeter = getLevelUpMeter();

        //calculate the evo average based on the evoMeter now and the last evoMeter
		double levelUpAverageNow = (levelUpMeter + DataManager.Instance.Level.LastLevelUpMeter) / 2;

		//calculate the average evolution value, over the period of gameplay starting
        //from hatching the pet up to now
		DataManager.Instance.Level.LevelUpAverageCum = (DataManager.Instance.Level.LevelUpAverageCum * cumDurationSecs +
            levelUpAverageNow * timeElapsedInSecs) / (cumDurationSecs + timeElapsedInSecs);
		DataManager.Instance.Level.LastLevelUpdatedTime = now;
		DataManager.Instance.Level.DurationCum += tempd;
		DataManager.Instance.Level.LastLevelUpMeter = levelUpMeter;
	}

    //get the weighted evolution meter
	private static double getLevelUpMeter(){
		return 0.5 * DataManager.Instance.Stats.Mood + 0.5 * DataManager.Instance.Stats.Health;
	}
}
