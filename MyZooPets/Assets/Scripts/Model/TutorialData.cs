using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialData{
    public bool FirstTimeCalendar {get; set;} //First time clicking on calendar
    public bool FirstTimeRealInhaler {get; set;}
    public bool FirstTimeDegradTrigger {get; set;}
    public List<string> ListPlayed {get; set;}	// list of tutorials that have been played	
	
    //================Initialization============
    public TutorialData(){}

    public void Init(){
        FirstTimeCalendar = true;
        FirstTimeRealInhaler = true;
        FirstTimeDegradTrigger = true;
		
		ListPlayed = new List<string>();
		
		// testing
		//ListPlayed.Add( TutorialManager_Bedroom.TUT_TRIGGERS );
    }
}
