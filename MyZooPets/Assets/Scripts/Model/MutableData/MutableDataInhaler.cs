using UnityEngine;
using System;
using System.Collections;

//---------------------------------------------------
// InhalerData 
// Save data for Inhaler. 
// Mutable data .
//---------------------------------------------------

public class MutableDataInhaler{
    public bool FirstTimeRescue {get; set;} //first time the player has seen the rescue inhaler
                                        //(this tells us whether to show tutorial arrows in the Inhaler Game)
    public DateTime LastInhalerPlayTime {get; set;} //last time the user played the regular inhaler

    //================Initialization============
    public MutableDataInhaler(){
        Init();
    }

    public void Init(){
        FirstTimeRescue = true;
        LastInhalerPlayTime = LgDateTime.GetTimeNow();
    }
}
