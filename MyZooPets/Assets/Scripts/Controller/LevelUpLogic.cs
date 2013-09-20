using UnityEngine;
using System.Collections;
using System;

public class LevelUpLogic : Singleton<LevelUpLogic> {
    private static int[] levelPoints = {0, 500, 1000, 1500, 2000, 2500, 3500, 4500, 
        5500, 6500, 8500}; //points required for the nxt level

    void Start(){
        HUDAnimator.OnLevelUp += LevelUp;
    }

    void OnDestroy(){
        HUDAnimator.OnLevelUp -= LevelUp;
    }

    public int NextLevelPoints(){
        return levelPoints[(int)DataManager.Instance.Level.CurrentLevel + 1];
    }

    //Check if the pet is ready to level up
    private void LevelUp(object senders, EventArgs args){
        int nextLevelIndex = (int)DataManager.Instance.Level.CurrentLevel + 1;
        DataManager.Instance.Level.CurrentLevel = (Level)nextLevelIndex;

        BadgeLogic.Instance.CheckSeriesUnlockProgress(BadgeType.Level, nextLevelIndex, true);
    }
}
