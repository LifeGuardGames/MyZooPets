using UnityEngine;
using System.Collections;
using System;

public class LevelUpLogic : Singleton<LevelUpLogic> {
    //Move this to xml. 
    private static int[] levelPoints = {0, 750, 1000, 1500, 2000, 2500, 3500, 4500, 
        5500, 6500, 8500}; //points required for the nxt level


    public int NextLevelPoints(){
        return levelPoints[(int)DataManager.Instance.GameData.Level.CurrentLevel + 1];
    }

}
