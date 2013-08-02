using UnityEngine;
using System;
using System.Collections;

[DoNotSerializePublic]
public class InhalerData{
    [SerializeThis]
    private bool firstTimeAdvair;// first time the player has seen the advair inhaler
                                        //(this tells us whether to show tutorial arrows in the Inhaler Game)
    [SerializeThis]
    private bool firstTimeRescue; //first time the player has seen the rescue inhaler
                                        //(this tells us whether to show tutorial arrows in the Inhaler Game)
    [SerializeThis]
    private DateTime lastInhalerPlayTime; //last time the user played the regular inhaler
    [SerializeThis]
    private bool playedInMorning; //has the user played in the morning
    [SerializeThis]
    private bool playedInAfternoon; //has the user played in the afternoon
    
    //===============Getters & Setters=================
    public bool FirstTimeAdvair{
        get{return firstTimeAdvair;}
        set{firstTimeAdvair = value;}
    }
    public bool FirstTimeRescue{
        get{return firstTimeRescue;}
        set{firstTimeRescue = value;}
    }
    public DateTime LastInhalerPlayTime{
        get{return lastInhalerPlayTime;}
        set{lastInhalerPlayTime = value;}
    }
    public bool PlayedInMorning{
        get{return playedInMorning;}
        set{playedInMorning = value;}
    }
    public bool PlayedInAfternoon{
        get{return playedInAfternoon;}
        set{playedInAfternoon = value;}
    }

    //================Initialization============
    public InhalerData(){}

    public void Init(){
        firstTimeAdvair = true;
        firstTimeRescue = true;
        lastInhalerPlayTime = DateTime.Now;
        playedInMorning = false;
        playedInAfternoon = false;
    }
}
