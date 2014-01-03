using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayPeriodLogic : Singleton<PlayPeriodLogic>{
    //Return the next time the user can collect bonuses
    public DateTime NextPlayPeriod{
        get{return DataManager.Instance.GameData.Calendar.NextPlayPeriod;}
		// set{DataManager.Instance.GameData.Calendar.NextPlayPeriod = value;}
    }

    //Check if the user can play the inhaler game
    public bool CanUseRealInhaler{
        get {
			DateTime now = LgDateTime.GetTimeNow();
			bool retVal = now >= NextPlayPeriod;
			
            return retVal;
        }
    } 
	
    //-----------------------------------------------
    // GetTimeFrame()
    // Given a time, will return whether or not that
	// time is considered morning or evening.  All
	// parts of the code should use this to determine
	// what time of day it is.
    //-----------------------------------------------	
	public static TimeFrames GetTimeFrame( DateTime time ) {
		if ( time.Hour > 12 )
			return TimeFrames.Evening;
		else
			return TimeFrames.Morning;
	}

    //-----------------------------------------------
    // CalculateNextPlayPeriod()
    // Based on the time now return the next reward time
    //-----------------------------------------------
    public void CalculateNextPlayPeriod(){
        DateTime nextPlayPeriod;
        if(LgDateTime.GetTimeNow().Hour < 12){ 
            //next reward time at noon
            nextPlayPeriod = LgDateTime.Today.AddHours(12);
        }else{ 
            //next reward time at midnight
            nextPlayPeriod = LgDateTime.Today.AddDays(1);
        }

        DataManager.Instance.GameData.Calendar.NextPlayPeriod = nextPlayPeriod;
    }

    public void CalculateCurrentPlayPeriod(){
        DateTime currentPlayPeriod;
        if(LgDateTime.GetTimeNow().Hour < 12){ 
            //next reward time at noon
            currentPlayPeriod = LgDateTime.Today;
        }else{ 
            //next reward time at midnight
            currentPlayPeriod = LgDateTime.Today.AddHours(12);
        }

        DataManager.Instance.GameData.Calendar.NextPlayPeriod = currentPlayPeriod;
    }
}
