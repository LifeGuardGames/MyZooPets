using UnityEngine;
using System;
using System.Collections;

//---------------------------------------------------
// InhalerData 
// Save data for Inhaler. 
// Mutable data .
//---------------------------------------------------

public class InhalerData{
    public bool FirstTimeRescue {get; set;} //first time the player has seen the rescue inhaler
                                        //(this tells us whether to show tutorial arrows in the Inhaler Game)
    public DateTime LastInhalerPlayTime {get; set;} //last time the user played the regular inhaler
    // public bool PlayedInMorning {get; set;} //has the user played in the morning
    // public bool PlayedInAfternoon {get; set;} //has the user played in the afternoon

    //================Initialization============
    public InhalerData(){
        Init();
    }

    public void Init(){
        FirstTimeRescue = true;
        LastInhalerPlayTime = LgDateTime.GetTimeNow();
        // PlayedInMorning = false;
        // PlayedInAfternoon = false;
    }
}
