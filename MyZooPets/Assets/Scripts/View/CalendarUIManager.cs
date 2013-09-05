using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CalendarUIManager : SingletonUI<CalendarUIManager> {
    public bool isDebug; //developing option
    public GameObject calendarTutorialHelperPrefab;
	public GameObject calendarPanel;
	public Transform thisWeek; //reference to the ThisWeek gameObject
    public Transform lastWeek; //reference to the LastWeek gameObject
    public UILabel rewardLabel;

    //Class to store UI reference
    private struct ThisWeekDay{
        public Transform AM {get; set;}
        public Transform PM {get; set;}

        public ThisWeekDay(Transform am, Transform pm){
            AM = am;
            PM = pm;
        }
    }
    //Class to store UI reference
    private struct LastWeekDay{
        public UISprite AM {get; set;}
        public UISprite PM {get; set;}

        public LastWeekDay(UISprite am, UISprite pm){
            AM = am;
            PM = pm;
        }
    }

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

    private ThisWeekDay[] currentWeek = new ThisWeekDay[7]; //array of Days from this week
    private LastWeekDay[] pastWeek = new LastWeekDay[7]; //array of Days from last week
    private List<CalendarEntry> currentWeekData; //week data from DataManager
    private List<CalendarEntry> pastWeekData; //week data from DataManager
    private int numberOfGreenStamps; //keep track of the green checks so we know when the user
                                //has collected all the rewards
    private bool timerActive; //True: run count down timer, False: don't run
    private float countDownTime; //time till the next reward

	// Use this for initialization
	void Awake() {
	   InitWeekUIReference(true); //this week
       InitWeekUIReference(false); //last week
       timerActive = false;
	}

    void Start(){
        CalendarLogic.OnCalendarReset += ResetCalendar;

        //Instantiate CalendarTutorialHelper
        if(TutorialLogic.Instance.FirstTimeCalendar){
            GameObject tutorialHelper = (GameObject)Instantiate(calendarTutorialHelperPrefab);
            tutorialHelper.name = "CalendarTutorialHelper";
            tutorialHelper.transform.parent = transform.parent;

            CalendarTutorialHelper.Instance.SetUpForTutorial(currentWeek[6].AM, currentWeek[6].PM, calendarPanel);
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

    protected override void _OpenUI(){
		//Hide other UI objects
		NavigationUIManager.Instance.HidePanel();
		InventoryUIManager.Instance.HidePanel();
		
		BGController.Instance.Show("blue");
		
        CalendarLogic.Instance.CalendarOpened();
        calendarPanel.GetComponent<TweenToggleDemux>().Show();
    }

    protected override void _CloseUI(){		
		//Show other UI object
		NavigationUIManager.Instance.ShowPanel();
		InventoryUIManager.Instance.ShowPanel();
		
		BGController.Instance.Hide();
		
        calendarPanel.GetComponent<TweenToggleDemux>().Hide();
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
            CalendarLogic.Instance.ClaimReward(calendarSlot.transform.position);

            numberOfGreenStamps--; //keep track of the rewards claimed
            if(numberOfGreenStamps == 0){ //all rewards have been claimed
                CalendarLogic.Instance.IsRewardClaimed = true;
                ResetTimer();
            }
            GA.API.Design.NewEvent("UserTouch:Calendar:Green");
        }else{ //No bonuses for blank for red ex
            //shake the calendar slot
            Hashtable optional = new Hashtable();
            optional.Add("ease", LeanTweenType.punch);
            LeanTween.moveX(calendarSlot, 0.01f, 0.5f, optional);
            if(button.normalSprite == RED_EX){
                GA.API.Design.NewEvent("UserTouch:Calendar:Red");
            }else{
                GA.API.Design.NewEvent("UserTouch:Calendar:Gray");
            }
        }
    }

    private void ResetTimer(){
        TimeSpan timeSpan = CalendarLogic.Instance.NextPlayPeriod - DateTime.Now;
        countDownTime = (float) timeSpan.TotalSeconds;
        //Next Reward timer
        if(CalendarLogic.Instance.IsRewardClaimed){
            //next reward time
            timerActive = true;
            
        }else{
            //claim reward now!!!!
            rewardLabel.text = "NOW!!";
            timerActive = false;
        }
    }

    private void SetSpriteOfUIImageButton(UIImageButton imageButton, string sprite){
        imageButton.normalSprite = sprite;
        imageButton.hoverSprite = sprite;
        imageButton.pressedSprite = sprite; 
    }
	
    //Populate the calendar based on the data stored in DataManager
    private void ResetCalendar(object sender, EventArgs args){
        currentWeekData = CalendarLogic.Instance.GetCalendarEntriesThisWeek;
        pastWeekData = CalendarLogic.Instance.GetCalendarEntriesLastWeek;
        numberOfGreenStamps = CalendarLogic.Instance.GreenStampCount;

        ResetTimer();
        //Populate calendar for this week
        for(int i=0; i<currentWeekData.Count; i++){
            CalendarEntry entry = currentWeekData[i]; //Data day
            ThisWeekDay day = currentWeek[i]; //UI day

            UIImageButton morningButton = day.AM.GetComponent<UIImageButton>();
            switch(entry.DayTime){
                case DosageRecord.Hit: //show check stamp
                    if(!entry.BonusCollectedDayTime){
                        morningButton.normalSprite = GREEN_CHECK;
                        morningButton.hoverSprite = GREEN_CHECK;
                        morningButton.pressedSprite = GREEN_CHECK_DOWN;
                    }else{
                        SetSpriteOfUIImageButton(morningButton, GRAY_CHECK);
                    }
                break;
                case DosageRecord.Miss: //show ex stamp
                    morningButton.normalSprite = RED_EX;
                    morningButton.hoverSprite = RED_EX;
                    morningButton.pressedSprite = RED_EX_DOWN;
                break;
                case DosageRecord.Null: //blank
                    SetSpriteOfUIImageButton(morningButton, BLANK);
                break;
                case DosageRecord.LeaveBlank: //blank
                    SetSpriteOfUIImageButton(morningButton, BLANK);
                break;
            }
            morningButton.enabled = false; // Tell it to redraw
			morningButton.enabled = true;

            UIImageButton nightButton = day.PM.GetComponent<UIImageButton>();
            switch(entry.NightTime){
                case DosageRecord.Hit:
                    if(!entry.BonusCollectedNightTime){
                        nightButton.normalSprite = GREEN_CHECK;
                        nightButton.hoverSprite = GREEN_CHECK;
                        nightButton.pressedSprite = GREEN_CHECK_DOWN;
                    }else{
                        SetSpriteOfUIImageButton(nightButton, GRAY_CHECK);
                    }
                break;
                case DosageRecord.Miss:
                    nightButton.normalSprite = RED_EX;
                    nightButton.hoverSprite = RED_EX;
                    nightButton.pressedSprite = RED_EX_DOWN;
                break;
                case DosageRecord.Null:
                    SetSpriteOfUIImageButton(nightButton, BLANK);
                break;
                case DosageRecord.LeaveBlank:
                    SetSpriteOfUIImageButton(nightButton, BLANK);
                break;
            }
			nightButton.enabled = false; // Tell it to redraw
			nightButton.enabled = true;
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
				OpenUI();				
			}
			if(GUI.Button (new Rect(10, 70, 100, 50), "hide")){
				CloseUI();				
			}
		}
	}
}
