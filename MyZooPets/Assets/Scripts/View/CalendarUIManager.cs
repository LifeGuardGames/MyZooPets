using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CalendarUIManager : SingletonUI<CalendarUIManager> {
	public static event EventHandler<EventArgs> OnCalendarClosed; //call when calendar is closed

    public bool isDebug; //developing option
	public GameObject calendarPanel;
	public Transform thisWeek; //reference to the ThisWeek gameObject
    public Transform lastWeek; //reference to the LastWeek gameObject
    public UILabel rewardLabel;
    public GameObject calendarHintArrow;

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
    private CalendarLogic calendarLogic; //reference

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
        if(TutorialLogic.Instance.FirstTimeCalendar) SetUpForTutorial();
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
		
        calendarLogic.CalendarOpened();
        calendarPanel.GetComponent<MoveTweenToggleDemultiplexer>().Show();
    }

    protected override void _CloseUI(){		
		//Show other UI object
		NavigationUIManager.Instance.ShowPanel();
		InventoryUIManager.Instance.ShowPanel();
		
        calendarPanel.GetComponent<MoveTweenToggleDemultiplexer>().Hide();
        //if(D.Assert(OnCalendarClosed != null, "OnCalendarClosed has no listeners"))
        //    OnCalendarClosed(this, EventArgs.Empty);
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

    //==================Tutorial===========================
    private GameObject greenStampHintArrow; //temp reference to hint arrow.
    private GameObject redStampHintArrow;
    private Transform day;
    private Transform night;

    //Make the necessary modification to set up for tutorial
    private void SetUpForTutorial(){
        day = currentWeek[6].AM;
        night = currentWeek[6].PM;
        day.GetComponent<UIButtonMessage>().functionName = "TutorialRewardClaim";
        night.GetComponent<UIButtonMessage>().functionName = "TutorialRewardClaim";

        //Set the finish target to TutorialUIManager
        calendarPanel.GetComponent<MoveTweenToggleDemultiplexer>().isShowFinishedCallback = true;
        calendarPanel.GetComponent<MoveTweenToggleDemultiplexer>().ShowTarget = TutorialUIManager.Instance.gameObject;
        calendarPanel.GetComponent<MoveTweenToggleDemultiplexer>().ShowFunctionName = "StartCalendarTutorial";
    }

    //Black out everything. Only shows the green stamp
    public void SetUpGreenStampTip(){
        //Display hint arrow
        greenStampHintArrow = NGUITools.AddChild(day.gameObject, calendarHintArrow);
        greenStampHintArrow.transform.localPosition = new Vector3(-136, 23, 0);
        greenStampHintArrow.transform.localEulerAngles = new Vector3(0, 
            calendarHintArrow.transform.localEulerAngles.y, 0);

        //Bring green stamp above the back drop
        day.localPosition = new Vector3(day.localPosition.x, day.localPosition.y, -21); 
    }

    //Black out everything. Only shows the red stamp
    public void SetUpRedExTip(){
        //Remove green stamp tutorial
        if(greenStampHintArrow != null) Destroy(greenStampHintArrow);
        day.localPosition = new Vector3(day.localPosition.x, day.localPosition.y, -6);

        //Display hint arrow
        redStampHintArrow = NGUITools.AddChild(currentWeek[6].PM.gameObject, calendarHintArrow);
        redStampHintArrow.transform.localPosition = new Vector3(-136, 22, 0);
        redStampHintArrow.transform.localEulerAngles = new Vector3(0, 
            calendarHintArrow.transform.localEulerAngles.y, 0);

       //Bring red stamp above the back drop
        night.localPosition = new Vector3(night.localPosition.x, night.localPosition.y, -21); 
    }

    //Black out everything.
    public void SetUpBonusTip(){
       //Bring red stamp below the back drop
        if(redStampHintArrow != null) Destroy(redStampHintArrow);
        night.localPosition = new Vector3(night.localPosition.x, night.localPosition.y, -6);

        //Bring gray stamp above
        day.localPosition = new Vector3(day.localPosition.x, day.localPosition.y, -21);
 
    }

    //Reset calendar to original after tutorial is finished
    public void CleanUpTutorial(){
        //Erase all tutorial data
        day.GetComponent<UIButtonMessage>().functionName = "ClaimReward";
        night.GetComponent<UIButtonMessage>().functionName = "ClaimReward";

        //Reset the finish target
        calendarPanel.GetComponent<MoveTweenToggleDemultiplexer>().isShowFinishedCallback = false;
        calendarPanel.GetComponent<MoveTweenToggleDemultiplexer>().ShowTarget = null;
        calendarPanel.GetComponent<MoveTweenToggleDemultiplexer>().ShowFunctionName = "";

        //Clean up hint arrow if still there
        if(greenStampHintArrow != null) Destroy(greenStampHintArrow);
        if(redStampHintArrow != null) Destroy(redStampHintArrow);

        //Erase tutorial data
        calendarLogic.ResetWeekAfterTutorialFinish();
    }

    //Use only during tutorial to prompt tips when green or red stamps are clicked
    public void TutorialRewardClaim(GameObject calendarSlot){
        UIImageButton button = calendarSlot.GetComponent<UIImageButton>();
        if(button.normalSprite == GREEN_CHECK){

            //Disable green stamp hint
            Destroy(greenStampHintArrow);
            button.normalSprite = GRAY_CHECK;
            button.hoverSprite = GRAY_CHECK;
            button.pressedSprite = GRAY_CHECK;
            button.isEnabled = false;
            button.isEnabled = true;

            //Reward
            calendarLogic.ClaimReward(calendarSlot.transform.position);
        }else{
            if(button.normalSprite == RED_EX) Destroy(redStampHintArrow);

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

            UIImageButton dayButton = day.AM.GetComponent<UIImageButton>();
            switch(entry.DayTime){
                case DosageRecord.Hit: //show check stamp
                    if(!entry.BonusCollectedDayTime){
                        dayButton.normalSprite = GREEN_CHECK;
                        dayButton.hoverSprite = GREEN_CHECK;
                        dayButton.pressedSprite = GREEN_CHECK_DOWN;
                    }else{
                        dayButton.normalSprite = GRAY_CHECK;
                        dayButton.hoverSprite = GRAY_CHECK;
                        dayButton.pressedSprite = GRAY_CHECK;
                    }
                break;
                case DosageRecord.Miss: //show ex stamp
                    dayButton.normalSprite = RED_EX;
                    dayButton.hoverSprite = RED_EX;
                    dayButton.pressedSprite = RED_EX_DOWN;
                break;
                case DosageRecord.Null: //blank
                    dayButton.normalSprite = BLANK;
                    dayButton.hoverSprite = BLANK;
                    dayButton.pressedSprite = BLANK;
                break;
                case DosageRecord.LeaveBlank: //blank
                    dayButton.normalSprite = BLANK;
                    dayButton.hoverSprite = BLANK;
                    dayButton.pressedSprite = BLANK;
                break;
            }
            dayButton.enabled = false; // Tell it to redraw
			dayButton.enabled = true;

            UIImageButton nightButton = day.PM.GetComponent<UIImageButton>();
            switch(entry.NightTime){
                case DosageRecord.Hit:
                    if(!entry.BonusCollectedNightTime){
                        nightButton.normalSprite = GREEN_CHECK;
                        nightButton.hoverSprite = GREEN_CHECK;
                        nightButton.pressedSprite = GREEN_CHECK_DOWN;
                    }else{
                        nightButton.normalSprite = GRAY_CHECK;
                        nightButton.hoverSprite = GRAY_CHECK;
                        nightButton.pressedSprite = GRAY_CHECK;
                    }
                break;
                case DosageRecord.Miss:
                    nightButton.normalSprite = RED_EX;
                    nightButton.hoverSprite = RED_EX;
                    nightButton.pressedSprite = RED_EX_DOWN;
                break;
                case DosageRecord.Null:
                    nightButton.normalSprite = BLANK;
                    nightButton.hoverSprite = BLANK;
                    nightButton.pressedSprite = BLANK;
                break;
                case DosageRecord.LeaveBlank:
                    nightButton.normalSprite = BLANK;
                    nightButton.hoverSprite = BLANK;
                    nightButton.pressedSprite = BLANK;
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
