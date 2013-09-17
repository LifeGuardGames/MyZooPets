using UnityEngine;
using System.Collections;
using System;

public class LevelUpLogic : Singleton<LevelUpLogic> {
    private static int[] levelPoints = {0, 500, 1000, 1500, 2000, 2500, 3500, 4500, 
        5500, 6500, 8500}; //points required for the nxt level

    //=========================API============================
    //The point requirement for next level up
    public int NextLevelPoints(){
        return levelPoints[(int)DataManager.Instance.Level.CurrentLevel + 1];
    }

    //========================================================
    void Start(){
        HUDAnimator.OnLevelUp += LevelUp;
    }

    void OnDestroy(){
        HUDAnimator.OnLevelUp -= LevelUp;
    }

    //Check if the pet is ready to level up
    private void LevelUp(object senders, EventArgs args){
        int nextLevelIndex = (int)DataManager.Instance.Level.CurrentLevel + 1;
        DataManager.Instance.Level.CurrentLevel = (Level)nextLevelIndex;

        BadgeLogic.Instance.CheckSeriesUnlockProgress(BadgeType.Level, nextLevelIndex, true);
    }

 //    //calculate evolution meter
	// private void UpdateLevelUpAverage(){
	// 	int cumDurationSecs = (int)DataManager.Instance.Level.DurationCum.TotalSeconds;

	// 	DateTime now = DateTime.Now;
	// 	TimeSpan tempd = now.Subtract(DataManager.Instance.Level.LastLevelUpdatedTime);
	// 	int timeElapsedInSecs = (int)tempd.TotalSeconds; //how many seconds since last played

	// 	double levelUpMeter = getLevelUpMeter();

 //        //calculate the evo average based on the evoMeter now and the last evoMeter
	// 	double levelUpAverageNow = (levelUpMeter + DataManager.Instance.Level.LastLevelUpMeter) / 2;

	// 	//calculate the average evolution value, over the period of gameplay starting
 //        //from hatching the pet up to now
	// 	DataManager.Instance.Level.LevelUpAverageCum = (DataManager.Instance.Level.LevelUpAverageCum * cumDurationSecs +
 //            levelUpAverageNow * timeElapsedInSecs) / (cumDurationSecs + timeElapsedInSecs);
	// 	DataManager.Instance.Level.LastLevelUpdatedTime = now;
	// 	DataManager.Instance.Level.DurationCum += tempd;
	// 	DataManager.Instance.Level.LastLevelUpMeter = levelUpMeter;
	// }

 //    //get the weighted evolution meter
	// private static double getLevelUpMeter(){
	// 	return 0.5 * DataManager.Instance.Stats.Mood + 0.5 * DataManager.Instance.Stats.Health;
	// }
}
