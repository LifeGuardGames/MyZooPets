using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[DoNotSerializePublic]
public class TutorialData{
    [SerializeThis]
    private bool firstTimeCalendar; //first time clicking on calendar
    [SerializeThis]
    private bool firstTimeChallenges; //first time clicking on challenges
    [SerializeThis]
    private bool firstTimeDiary;
    [SerializeThis]
    private bool firstTimeSlotMachine;
    [SerializeThis]
    private bool firstTimeRealInhaler;
    [SerializeThis]
    private bool firstTimeTeddyInhaler;
    [SerializeThis]
    private bool firstTimeShelf;
    [SerializeThis]
    private bool firstTimeHelpTrophy; 
    [SerializeThis]
    private bool firstTimeDegradTrigger;
	
    [SerializeThis]
    private List<string> listPlayed;	// list of tutorials that have been played	


    //===============Getters & Setters=================
    public bool FirstTimeCalendar{
        get{return firstTimeCalendar;}
        set{firstTimeCalendar = value;}
    }
    public bool FirstTimeChallenges{
        get{return firstTimeChallenges;}
        set{firstTimeChallenges = value;}
    }
    public bool FirstTimeDiary{
        get{return firstTimeDiary;}
        set{firstTimeDiary = value;}
    }
    public bool FirstTimeSlotMachine{
        get{return firstTimeSlotMachine;}
        set{firstTimeSlotMachine = value;}
    }
    public bool FirstTimeRealInhaler{
        get{return firstTimeRealInhaler;}
        set{firstTimeRealInhaler = value;}
    }
    public bool FirstTimeTeddyInhaler{
        get{return firstTimeTeddyInhaler;}
        set{firstTimeTeddyInhaler = value;}
    }
    public bool FirstTimeShelf{
        get{return firstTimeShelf;}
        set{firstTimeShelf = value;}
    }
    public bool FirstTimeHelpTrophy{
        get{return firstTimeHelpTrophy;}
        set{firstTimeHelpTrophy = value;}
    }    
    public bool FirstTimeDegradTrigger{
        get{return firstTimeDegradTrigger;}
        set{firstTimeDegradTrigger = value;}
    }
	
    //===============Getters & Setters=================
    public List<string> ListPlayed {
        get{return listPlayed;}
        set{listPlayed = value;}
    }
	
    //================Initialization============
    public TutorialData(){}

    public void Init(){
        firstTimeCalendar = true;
        firstTimeChallenges = true;
        firstTimeDiary = true;
        firstTimeSlotMachine = true;
        firstTimeRealInhaler = true;
        firstTimeTeddyInhaler = true;
        firstTimeShelf = true;
        firstTimeHelpTrophy = true; 
        firstTimeDegradTrigger = true;
		
		listPlayed = new List<string>();
    }
}
