using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CalendarUIManager : Singleton<CalendarUIManager> {
    public bool isDebug; //developing option
    public Transform thisWeek; //reference to the ThisWeek gameObject
    public Transform lastWeek; //reference to the LastWeek gameObject
    public UILabel rewardLabel;
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
    private int numberOfGreenStamps; //keep track of the green checks so we know when the user
                                //has collected all the rewards
    private bool timerActive; //True: run count down timer, False: don't run
    private float countDownTime; //time till the next reward
    private CalendarLogic calendarLogic; //reference

    //sprite name in atlas
    private const string BLANK = "calendarButtonBlank";
    private const string RED_EX_DOWN = "calendarButtonRedExDown";
    private const string RED_EX = "calendarButtonRedEx";
    private const string GREEN_CHECK_DOWN = "calendarButtonGreenCheckDown";
    private const string GREEN_CHECK = "calendarButtonGreenCheck";
    private const string GRAY_CHECK = "calendarButtonGrayCheck";
    private const string HALF_STAMP_RED_BOTTOM = "calendarHalfStampRedBottom";
    private const string HALF_STAMP_RED_TOP = "calendarHalfStampRedTop";
    private const string HALF_STAMP_GREEN_BOTTOM = "calendarHalfStampGreenBottom";
    private const string HALF_STAMP_GREEN_TOP = "calendarHalfStampGreenTop";

	// Use this for initialization
	void Awake() {
	   InitWeekUIReference(true); //this week
       InitWeekUIReference(false); //last week
       timerActive = false;
        calendarLogic = GameObject.Find("GameManager/CalendarLogic").GetComponent<CalendarLogic>();
	}

    void Start(){
        CalendarLogic.OnCalendarReset += ResetCalendar;

        //check if in tutorial phase. special handler needed for sample data
        //ThisWeekDay[3] sample data. set special button handler
        if(TutorialLogic.Instance.FirstTimeCalendar){
            SetUpForTutorial();
        }
    }

    void OnDestroy(){
        CalendarLogic.OnCalendarReset -= ResetCalendar;
    }
        
	// Update is called once per frame
	void Update () {
        if(timerActive){
            countDownTime -= Time.deltaTime;
            if(countDownTime <= 0){
                timerActive = false; 
                return;
            }
            TimeSpan interval = TimeSpan.FromSeconds(countDownTime);
            string[] split = interval.ToString().Split('.');
            rewardLabel.text = split[0]; 
        }
	}

    public void CalendarClicked(){
        calendarLogic.CalendarOpened();
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
        UIImageButton button = calendarSlot.GetComponent<UIImageButton>();

        if(button.normalSprite == GREEN_CHECK){ //bonuses for green check
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
            button.normalSprite = GRAY_CHECK;
            button.hoverSprite = GRAY_CHECK;
            button.pressedSprite = GRAY_CHECK;
            button.isEnabled = false;
            button.isEnabled = true;

            //Add reward
            calendarLogic.ClaimReward(calendarSlot.transform.position);

            numberOfGreenStamps--; //keep track of the rewards claimed
            if(numberOfGreenStamps == 0){ //all rewards have been claimed
                calendarLogic.IsRewardClaimed = true;
                ResetTimer();
            }
        }else{ //No bonuses for blank for red ex
            //shake the calendar slot
            Hashtable optional = new Hashtable();
            optional.Add("ease", LeanTweenType.punch);
            LeanTween.moveX(calendarSlot, 0.01f, 0.5f, optional);
        }
    }

    //==================Tutorial===========================
    //Make the necessary modification to set up for tutorial
    private void SetUpForTutorial(){
        currentWeek[6].AM.GetComponent<UIButtonMessage>().functionName = "TutorialRewardClaim";
        currentWeek[6].PM.GetComponent<UIButtonMessage>().functionName = "TutorialRewardClaim";
    }

    public void GreenStampTutorial(){
        // TutorialUIManager.Instance.BackDrop(true);

        // Transform day = currentWeek[6].AM;
        // day.localPosition = new Vector3(day.localPosition.x,
        //     day.localPosition.y, -15);
    }

    public void RedExTutorial(){

    }

    //Reset calendar to original after tutorial is finished
    public void ResetAfterTutorialFinish(){
        //erase all tutorial data
        currentWeek[6].AM.GetComponent<UIButtonMessage>().functionName = "ClaimReward";
        currentWeek[6].PM.GetComponent<UIButtonMessage>().functionName = "ClaimReward";
        calendarLogic.ResetWeekAfterTutorialFinish();
    }

    //Use only during tutorial to prompt tips when green or red stamps are clicked
    public void TutorialRewardClaim(GameObject calendarSlot){
        UIImageButton button = calendarSlot.GetComponent<UIImageButton>();
        if(button.normalSprite == GREEN_CHECK){
            calendarLogic.ClaimReward(calendarSlot.transform.position);
            TutorialUIManager.Instance.ShowCalendarTipGreenStamp();
        }else{
            TutorialUIManager.Instance.ShowCalendarTipRedStamp();
            //shake the calendar slot
            Hashtable optional = new Hashtable();
            optional.Add("ease", LeanTweenType.punch);
            LeanTween.moveX(calendarSlot, 0.03f, 0.5f, optional);
        }
    }
    //===================================================

    private void ResetTimer(){
        TimeSpan timeSpan = calendarLogic.NextPlayPeriod - DateTime.Now;
        countDownTime = (float) timeSpan.TotalSeconds;
        //Next Reward timer
        if(calendarLogic.IsRewardClaimed){
            //next reward time
            timerActive = true;
            
        }else{
            //claim reward now!!!!
            rewardLabel.text = "NOW!!";
            timerActive = false;
        }
    }
	
    //Populate the calendar based on the data stored in DataManager
    private void ResetCalendar(object sender, EventArgs args){
        currentWeekData = calendarLogic.GetCalendarEntriesThisWeek;
        pastWeekData = calendarLogic.GetCalendarEntriesLastWeek;
        numberOfGreenStamps = calendarLogic.GreenStampCount;

        ResetTimer();
        //Populate calendar for this week
        for(int i=0; i<currentWeekData.Count; i++){
            CalendarEntry entry = currentWeekData[i]; //Data day
            ThisWeekDay day = currentWeek[i]; //UI day

            UIImageButton dayImageButton = day.AM.GetComponent<UIImageButton>();
            switch(entry.DayTime){
                case DosageRecord.Hit: //show check stamp
                    dayImageButton.normalSprite = GREEN_CHECK;
                    dayImageButton.hoverSprite = GREEN_CHECK;
                    dayImageButton.pressedSprite = GREEN_CHECK_DOWN;
                break;
                case DosageRecord.Miss: //show ex stamp
                    dayImageButton.normalSprite = RED_EX;
                    dayImageButton.hoverSprite = RED_EX;
                    dayImageButton.pressedSprite = RED_EX_DOWN;
                break;
                case DosageRecord.Null: //blank
                    dayImageButton.normalSprite = BLANK;
                    dayImageButton.hoverSprite = BLANK;
                    dayImageButton.pressedSprite = BLANK;
                break;
                case DosageRecord.LeaveBlank: //blank
                    dayImageButton.normalSprite = BLANK;
                    dayImageButton.hoverSprite = BLANK;
                    dayImageButton.pressedSprite = BLANK;
                break;
            }
            dayImageButton.enabled = false; // Tell it to redraw
			dayImageButton.enabled = true;

            UIImageButton nightImageButton = day.PM.GetComponent<UIImageButton>();
            switch(entry.NightTime){
                case DosageRecord.Hit:
                    nightImageButton.normalSprite = GREEN_CHECK;
                    nightImageButton.hoverSprite = GREEN_CHECK;
                    nightImageButton.pressedSprite = GREEN_CHECK_DOWN;
                break;
                case DosageRecord.Miss:
                    nightImageButton.normalSprite = RED_EX;
                    nightImageButton.hoverSprite = RED_EX;
                    nightImageButton.pressedSprite = RED_EX_DOWN;
                break;
                case DosageRecord.Null:
                    nightImageButton.normalSprite = BLANK;
                    nightImageButton.hoverSprite = BLANK;
                    nightImageButton.pressedSprite = BLANK;
                break;
                case DosageRecord.LeaveBlank:
                    nightImageButton.normalSprite = BLANK;
                    nightImageButton.hoverSprite = BLANK;
                    nightImageButton.pressedSprite = BLANK;
                break;
            }
			nightImageButton.enabled = false; // Tell it to redraw
			nightImageButton.enabled = true;
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
	
	void OnGUI(){
		if(isDebug){
			if(GUI.Button (new Rect(10, 10, 100, 50), "show")){
				CalendarClicked();				
			}
			if(GUI.Button (new Rect(10, 70, 100, 50), "hide")){
				CalendarClosed();				
			}
		}
	}
}
