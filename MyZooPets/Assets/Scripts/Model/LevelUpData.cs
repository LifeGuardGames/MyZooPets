using UnityEngine;
using System;
using System.Collections;

[DoNotSerializePublic]
public class LevelUpData{
    [SerializeThis]
    private DateTime lastLevelUpdatedTime; //last time level up meter was calculated
    [SerializeThis]
    private TimeSpan durationCum; //the total time since hatching the pet
    [SerializeThis]
    private double lastLevelUpMeter; //last calculated level up meter
    [SerializeThis]
    private double levelUpAverageCum; //cumulative average of level up meter
                                        //use this to decide what trophy to give when
                                        //leveling up
    [SerializeThis]
    private Level currentLevel; //pets current level                                    

    //===============Getters & Setters=================
    public DateTime LastLevelUpdatedTime{
        get{return lastLevelUpdatedTime;}
        set{lastLevelUpdatedTime = value;}
    }
    public TimeSpan DurationCum{
        get{return durationCum;}
        set{durationCum = value;}
    }
    public double LastLevelUpMeter{
        get{return lastLevelUpMeter;}
        set{lastLevelUpMeter = value;}
    }
    public double LevelUpAverageCum{
        get{return levelUpAverageCum;}
        set{levelUpAverageCum = value;}
    }
    public Level CurrentLevel{
        get{return currentLevel;}
        set{currentLevel = value;}
    }
    public bool UseDummyData{get; set;} //initialize with test data

    //================Initialization============
    public LevelUpData(){}

    public void Init(){
        lastLevelUpdatedTime = DateTime.Now;
        durationCum = new TimeSpan(0,0,10);
        lastLevelUpMeter = 80; //needs to be re calculated whenever health, mood are modified
        levelUpAverageCum = 80; //needs to be re calculated whenever health, mood are modified
        currentLevel = Level.Level0;
    } 
}
