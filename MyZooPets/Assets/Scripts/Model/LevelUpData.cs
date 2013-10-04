using UnityEngine;
using System;
using System.Collections;

public class LevelUpData{
    public DateTime LastLevelUpdatedTime {get; set;} //last time level up meter was calculated
    public double LastLevelUpMeter {get; set;} //last calculated level up meter
    public double LevelUpAverageCum {get; set;}//cumulative average of level up meter
                                        //use this to decide what trophy to give when
                                        //leveling up
    public Level CurrentLevel {get; set;} //pets current level                                    

    //================Initialization============
    public LevelUpData(){}

    public void Init(){
        LastLevelUpdatedTime = DateTime.Now;
        LastLevelUpMeter = 80; //needs to be re calculated whenever health, mood are modified
        LevelUpAverageCum = 80; //needs to be re calculated whenever health, mood are modified
        CurrentLevel = Level.Level0;
    } 
}
