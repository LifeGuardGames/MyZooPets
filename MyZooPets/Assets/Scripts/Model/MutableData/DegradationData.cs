using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// DegradationData 
// Save data for degradation. 
// Mutable data 
//---------------------------------------------------

public class DegradationData{
    public DateTime LastTimeUserPlayedGame {get; set;} //last time that the user opened the game
    public DateTime LastTriggerSpawnedPlayPeriod {get; set;}
    public bool IsTriggerSpawned {get; set;} //True: triggers for this play period has been spawned
	public int UncleanedTriggers {get; set;}

	/* // work in progress: supporting triggers in multiple areas (house, yard, etc)
	public Hashtable UncleanedTriggers {get; set;}
	public int GetUncleanedTriggers( string strArea ) {
		int nTriggers = 0;
		
		if ( UncleanedTriggers.ContainsKey( strArea ) )
			nTriggers = (int) UncleanedTriggers[strArea];
		else
			Debug.LogError("No uncleaned triggers found for area: " + strArea);
		
		return nTriggers;
	}
	*/

    //================Initialization============
    public DegradationData(){
        Init();
    }

    private void Init(){
        LastTimeUserPlayedGame = LgDateTime.GetTimeNow();
        LastTriggerSpawnedPlayPeriod = PlayPeriodLogic.GetCurrentPlayPeriod();
        IsTriggerSpawned = false;
		UncleanedTriggers = 0;
    }
}
