using UnityEngine;
using System;
using System.Collections;

//---------------------------------------------------
// InhalerData 
// Save data for Inhaler. 
// Mutable data .
//---------------------------------------------------

public class MutableDataInhaler{
    public bool IsFirstTimeRescue {get; set;} 			// First time the player has seen the rescue inhaler
                                        				// (this tells us whether to show tutorial arrows in the Inhaler Game)

	public DateTime LastestPlayPeriodUsed {get; set;}	// Last pp the user played the regular inhaler

//    public DateTime LastInhalerPlayTime {get; set;}		
//	public bool HasReceivedFireOrb {get; set;} 

    //================Initialization============
    public MutableDataInhaler(){
        Init();
    }

    public void Init(){
        IsFirstTimeRescue = true;
//		HasReceivedFireOrb = true;
		LastestPlayPeriodUsed = DateTime.MinValue;
//        LastInhalerPlayTime = LgDateTime.GetTimeNow();
    }
}
