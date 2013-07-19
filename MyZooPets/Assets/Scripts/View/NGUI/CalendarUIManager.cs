using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CalendarUIManager : MonoBehaviour {
    public bool isDebug; //developing option
    public Transform thisWeek; //reference to the ThisWeek gameObject
    public Transform lastWeek; //reference to the LastWeek gameObject
    public GameObject particleEffectPrefab;

    //==================Events=======================
    public static event EventHandler<EventArgs> OnCalendarClosed; //call when calendar is closed
    //===============================================

    //Class to store UI reference
    private class ThisWeekDay{
        public Transform AM {get; set;}
        public Transform PM {get; set;}

        public ThisWeekDay(Transform am, Transform pm){
            AM = am;
            PM = pm;
        }
    }

    //Class to store UI reference
    private class LastWeekDay{
        public UISprite AM {get; set;}
        public UISprite PM {get; set;}

        public LastWeekDay(UISprite am, UISprite pm){
            AM = am;
            PM = pm;
        }
    }

    private ThisWeekDay[] currentWeek = new ThisWeekDay[7]; //array of Days from this week
    private LastWeekDay[] pastWeek = new LastWeekDay[7]; //array of Days from last week
    private List<CalendarEntry> currentWeekData; //week data from DataManager
    private List<CalendarEntry> pastWeekData; //week data from DataManager

    private const string STAMP_EX = "calendarStampEx";
    private const string STAMP_CHECK = "calendarStampCheck";
    private const string HALF_STAMP_RED_BOTTOM = "calendarHalfStampRedBottom";
    private const string HALF_STAMP_RED_TOP = "calendarHalfStampRedTop";
    private const string HALF_STAMP_GREEN_BOTTOM = "calendarHalfStampGreenBottom";
    private const string HALF_STAMP_GREEN_TOP = "calendarHalfStampGreenTop";

	// Use this for initialization
	void Awake() {
	   InitWeekUIReference(true); //this week
       InitWeekUIReference(false); //last week

       if(isDebug){ //Testing code. generate dummy data for last week and this week
            List<CalendarEntry> temp = new List<CalendarEntry>();
            for(int i=0; i<7; i++){
                temp.Add(new CalendarEntry(
                    DosageRecord.Hit, DosageRecord.Miss));
            }
            temp[2].DayTime = DosageRecord.Null;
            DataManager.EntriesLastWeek = temp;
            DataManager.EntriesThisWeek = temp;
            // Init();
            CalendarClicked();
        }
	}

    void Start(){
        currentWeekData = CalendarLogic.GetCalendarEntriesThisWeek;
        pastWeekData = CalendarLogic.GetCalendarEntriesLastWeek;
    }
	
	// Update is called once per frame
	void Update () {
        //TO DO: count down timer for nxt reward collection	
	}

    //Initialize calendar when scene is ready
    // public void Init(){
        // currentWeekData = CalendarLogic.GetCalendarEntriesThisWeek;
        // pastWeekData = CalendarLogic.GetCalendarEntriesLastWeek;
    // }

    public void CalendarClicked(){
        PopulateCalendar();
        GetComponent<MoveTweenToggleDemultiplexer>().Show();
    }

    public void CalendarClosed(){
        GetComponent<MoveTweenToggleDemultiplexer>().Hide();
        if(OnCalendarClosed != null){
            OnCalendarClosed(this, EventArgs.Empty);
        }else{
            Debug.LogError("OnCalendarClosed listener is null");
        }
    }

    //Called when a checked calendar slot is clicked. Reward the player and turn the
    //slot off until the next bonus time
    public void ClaimReward(GameObject calendarSlot){
        CalendarEntry entry;
        int index = 0;
        switch(calendarSlot.transform.parent.name){
            case "Mon": index = 0; break;
            case "Tue": index = 1; break;
            case "Wed": index = 2; break;
            case "Thu": index = 3; break;
            case "Fri": index = 4; break;
            case "Sat": index = 5; break;
            case "Sun": index = 6; break;
        }
        entry = currentWeekData[index]; //get the Data reference of the button clicked
       
        //Disable further reward collection 
        if(calendarSlot.name == "AM"){ //AM
            entry.BonusCollectedDayTime = true;
        }else{ //PM
            entry.BonusCollectedNightTime = true;
        }
        calendarSlot.GetComponent<UIButton>().isEnabled = false; //turn button off

        //spawn particle effect
        GameObject prefab = NGUITools.AddChild(calendarSlot, particleEffectPrefab);
        prefab.transform.rotation = Quaternion.Euler(270, 0, 0);
        //Add reward
        CalendarLogic.ClaimReward(); 
    }
    
    //Populate the calendar based on the data stored in DataManager
    private void PopulateCalendar(){

        //Populate calendar for this week
        for(int i=0; i<currentWeekData.Count; i++){
            CalendarEntry entry = currentWeekData[i]; //Data day
            ThisWeekDay day = currentWeek[i]; //UI day

            UISprite stampSprite = day.AM.Find("Stamp").GetComponent<UISprite>();
            UIButton dayButton = day.AM.GetComponent<UIButton>();
            switch(entry.DayTime){
                case DosageRecord.Hit: //show check stamp
                    stampSprite.spriteName = STAMP_CHECK;
                    stampSprite.alpha = 1;

                    if(!entry.BonusCollectedDayTime){
                        dayButton.isEnabled = true;
                    }else{
                        dayButton.isEnabled = false;
                    }
                break;
                case DosageRecord.Miss: //show ex stamp
                    stampSprite.spriteName = STAMP_EX;
                    stampSprite.alpha = 1;
                    dayButton.isEnabled = false;
                break;
                case DosageRecord.Null: //blank
                    stampSprite.alpha = 0;
                    dayButton.isEnabled = false;
                break;
                case DosageRecord.LeaveBlank: //blank
                    stampSprite.alpha = 0;
                    dayButton.isEnabled = false;
                break;
            }

            UISprite stampSpriteNight = day.PM.Find("Stamp").GetComponent<UISprite>();
            UIButton nightButton = day.PM.GetComponent<UIButton>();
            switch(entry.NightTime){
                case DosageRecord.Hit:
                    stampSpriteNight.spriteName = STAMP_CHECK;
                    stampSpriteNight.alpha = 1;

                    if(!entry.BonusCollectedNightTime){
                        nightButton.isEnabled = true;
                    }else{
                        nightButton.isEnabled = false;
                    }
                break;
                case DosageRecord.Miss:
                    stampSpriteNight.spriteName = STAMP_EX;
                    stampSpriteNight.alpha = 1;
                    nightButton.isEnabled = false;
                break;
                case DosageRecord.Null:
                    stampSpriteNight.alpha = 0;
                    nightButton.isEnabled = false;
                break;
                case DosageRecord.LeaveBlank:
                    stampSpriteNight.alpha = 0;
                    nightButton.isEnabled = false;
                break;
            }
        }

        for(int i=0; i<pastWeekData.Count; i++){
            CalendarEntry entry = pastWeekData[i]; //Data day
            LastWeekDay day = pastWeek[i]; //UI day

            switch(entry.DayTime){
                case DosageRecord.Hit:
                    day.AM.spriteName = HALF_STAMP_GREEN_TOP;
                    day.AM.alpha = 1; //show stamp
                break;
                case DosageRecord.Miss:
                    day.AM.spriteName = HALF_STAMP_RED_TOP;
                    day.AM.alpha = 1;
                break;
                case DosageRecord.LeaveBlank:
                    day.AM.alpha = 0;
                break;
            }

            switch(entry.NightTime){
                case DosageRecord.Hit:
                    day.PM.spriteName = HALF_STAMP_GREEN_BOTTOM;
                    day.PM.alpha = 1;
                break;
                case DosageRecord.Miss:
                    day.PM.spriteName = HALF_STAMP_RED_BOTTOM;
                    day.PM.alpha = 1;
                break;
                case DosageRecord.LeaveBlank:
                    day.PM.alpha = 0;
                break;
            }
        }
    }  

    //Get all UI reference from the Hierarchy and store them in an array
    private void InitWeekUIReference(bool isThisWeek){
        Transform week = isThisWeek ? thisWeek : lastWeek;

        //loop through this week and get all child reference
        foreach(Transform day in week){
            int index = 0;
            switch(day.name){
                case "Mon": index = 0; break;
                case "Tue": index = 1; break;
                case "Wed": index = 2; break;
                case "Thu": index = 3; break;
                case "Fri": index = 4; break;
                case "Sat": index = 5; break;
                case "Sun": index = 6; break;
            }

            if(isThisWeek){
                ThisWeekDay currentDay = new ThisWeekDay(day.Find("AM"), day.Find("PM"));
                currentWeek[index] = currentDay; //store UI reference at the correct index
            }else{
                LastWeekDay pastDay = new LastWeekDay(day.Find("AM").GetComponent<UISprite>(),
                    day.Find("PM").GetComponent<UISprite>());
                pastWeek[index] = pastDay;
            }
        }
    }
}
