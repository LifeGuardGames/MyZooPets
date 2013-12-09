using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// DegradationData 
// Save data for degradation. Mutable data 
//---------------------------------------------------

public class DegradationData{
    public DateTime LastTimeUserPlayedGame {get; set;} //last time that the user opened the game
    public List<DegradData> DegradationTriggers {get; set;} //list of degradation triggers that are currently in game
    public bool FirstTimeDegradTrigger {get; set;} //first time the user has clicked on an asthma trigger
    public bool MorningTrigger {get; set;} //True: spawn asthma trigger in morning, False: don't
    public bool AfternoonTrigger {get; set;} //True: spawn asthma trigger in afternoon, False: don't
	public int UncleanedTriggers {get; set;}

    //================Initialization============
    public DegradationData(){}

    public void Init(){
        LastTimeUserPlayedGame = DateTime.Now;
        DegradationTriggers = new List<DegradData>();
        FirstTimeDegradTrigger = true;
        MorningTrigger = true;
        AfternoonTrigger = true; 
		UncleanedTriggers = 0;
    }
}
