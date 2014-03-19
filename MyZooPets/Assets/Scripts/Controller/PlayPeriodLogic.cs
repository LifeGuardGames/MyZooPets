using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayPeriodLogic : Singleton<PlayPeriodLogic>{
    //Return the next time the user can collect bonuses
    public DateTime NextPlayPeriod{
        get{return DataManager.Instance.GameData.Calendar.NextPlayPeriod;}
    }

    //Check if the user can play the inhaler game
    public bool CanUseRealInhaler{
        get {
			DateTime now = LgDateTime.GetTimeNow();
			bool retVal = now >= NextPlayPeriod;
			
			// special case: if we are done with the inhaler tutorial but not all tutorials, just return false
			bool tutsDone = DataManager.Instance.GameData.Tutorial.AreTutorialsFinished();
			bool inhalerTutDone = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TutorialManager_Bedroom.TUT_INHALER);
			if (!tutsDone && inhalerTutDone)
				retVal = false;
			
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
		if (time.Hour >= 12)
			return TimeFrames.Evening;
		else
			return TimeFrames.Morning;
	}

    //-----------------------------------------------
    // GetCurrentPlayPeriod()
    //
    //-----------------------------------------------
    public static DateTime GetCurrentPlayPeriod(){
        DateTime currentPlayPeriod;

        //if the time now is in the morning the current play period is 12 am
        if(LgDateTime.GetTimeNow().Hour < 12)
            currentPlayPeriod = LgDateTime.Today;
        //if the time now is in the afternoon the current play period is 12pm
        else
            currentPlayPeriod = LgDateTime.Today.AddHours(12);

        return currentPlayPeriod;
    }

    //-----------------------------------------------
    // CalculateNextPlayPeriod()
    // Based on the time now return the next reward time
    // Schedule a local notification this function is called
    //-----------------------------------------------
    public void CalculateNextPlayPeriod(){
        DateTime nextPlayPeriod;
        DateTime localNotificationFireDate;

        if(LgDateTime.GetTimeNow().Hour < 12){ 
            //next reward time at noon
            nextPlayPeriod = LgDateTime.Today.AddHours(12);
        }else{ 
            //next reward time at midnight
            nextPlayPeriod = LgDateTime.Today.AddDays(1);
        }

        bool isInhalerTutDone = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TutorialManager_Bedroom.TUT_INHALER);
        if(isInhalerTutDone){
            //register local notification.
            localNotificationFireDate = nextPlayPeriod.AddHours(7); //set notif to 7am and 7pm
            string petName = DataManager.Instance.GameData.PetInfo.PetName;
            string rawText = "NOTIFICATION_1_PRO";
            if(VersionManager.IsLite()) rawText = "NOTIFICATION_1_LITE";
            string notifText = String.Format(Localization.Localize(rawText), petName);
            LgNotificationServices.RemoveIconBadgeNumber();
            LgNotificationServices.ScheduleLocalNotification(notifText, localNotificationFireDate);
        }

        //update next playperiod in DM so it gets serialized
        DataManager.Instance.GameData.Calendar.NextPlayPeriod = nextPlayPeriod;
    }

    //-----------------------------------------------
    // CalculateCurrentPlayPeriod()
    // Reset NextPlayPeriod to the current play period
    // this is usually only use if user misses play period
    // so at the start of the game (after degradation logic)
    // we reset that play period to now
    //-----------------------------------------------
    public void CalculateCurrentPlayPeriod(){
        DataManager.Instance.GameData.Calendar.NextPlayPeriod = GetCurrentPlayPeriod(); 
    }

}
