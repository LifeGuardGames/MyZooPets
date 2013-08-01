using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[DoNotSerializePublic]
public class DegradationData{
    //Degradation Data
    [SerializeThis]
    private DateTime lastTimeUserPlayedGame; //last time that the user opened the game
    [SerializeThis]
    private List<DegradData> degradationTriggers; //list of degradation triggers that are currently in game
    [SerializeThis]
    private bool firstTimeDegradTrigger; //first time the user has clicked on an asthma trigger
    [SerializeThis]
    private bool morningTrigger; //True: spawn asthma trigger in morning, False: don't
    [SerializeThis]
    private bool afternoonTrigger; //True: spawn asthma trigger in afternoon, False: don't

    //===============Getters & Setters=================
    public DateTime LastTimeUserPlayedGame{
        get{return lastTimeUserPlayedGame;}
        set{lastTimeUserPlayedGame = value;}
    }
    public List<DegradData> DegradationTriggers{
        get{return degradationTriggers;}
        set{degradationTriggers = value;}
    }
    public bool FirstTimeDegradTrigger{
        get{return firstTimeDegradTrigger;}
        set{firstTimeDegradTrigger = value;}
    }
    public bool MorningTrigger{
        get{return morningTrigger;}
        set{morningTrigger = value;}
    }
    public bool AfternoonTrigger{
        get{return afternoonTrigger;}
        set{afternoonTrigger = value;}
    }
    public bool UseDummyData{get; set;} //initialize with test data

    //================Initialization============
    public DegradationData(){}

    public void Init(){
       lastTimeUserPlayedGame = DateTime.Now;
        degradationTriggers = new List<DegradData>();
        firstTimeDegradTrigger = true;
        morningTrigger = true;
        afternoonTrigger = true; 
    }
}
