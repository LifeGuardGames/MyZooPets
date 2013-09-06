using UnityEngine;
using System.Collections;

public class CalendarTutorialHelper : Singleton<CalendarTutorialHelper>{
    public GameObject calendarHintArrowPrefab;
    private GameObject calendarPanel;
    private GameObject greenStampHintArrow; 
    private GameObject redStampHintArrow;
    private Transform morning;
    private Transform night;

    private const string RED_EX_DOWN = "calendarButtonRedExDown";
    private const string GREEN_CHECK_DOWN = "calendarButtonGreenCheckDown";
    private const string GREEN_CHECK = "calendarButtonGreenCheck";
    private const string GRAY_CHECK = "calendarButtonGrayCheck";
    private const string RED_EX = "calendarButtonRedEx";

     //Make the necessary modification to set up for tutorial
    public void SetUpForTutorial(Transform morning, Transform night, GameObject calendarPanel){
        this.morning = morning;
        this.night = night;
        this.calendarPanel = calendarPanel;

        UIButtonMessage morningButtonMessage = this.morning.GetComponent<UIButtonMessage>();
        UIButtonMessage nightButtonMessage = this.night.GetComponent<UIButtonMessage>();

        //Set button click handler to be handle by the tutorial helper
        morningButtonMessage.target = gameObject;
        morningButtonMessage.functionName = "TutorialRewardClaim";
        nightButtonMessage.target = gameObject;
        nightButtonMessage.functionName = "TutorialRewardClaim";

        //Add UIPanel to morning and night, so their Z index can be modified without
        //affecting other UI components. Need to refresh gameObject by using SetActive
        morning.gameObject.AddComponent<UIPanel>();
        morning.gameObject.SetActive(false);
        morning.gameObject.SetActive(true);
        night.gameObject.AddComponent<UIPanel>();

        //Set the finish target to TutorialUIManager
        this.calendarPanel.GetComponent<TweenToggleDemux>().isShowFinishedCallback = true;
        this.calendarPanel.GetComponent<TweenToggleDemux>().ShowTarget = TutorialUIManager.Instance.gameObject;
        this.calendarPanel.GetComponent<TweenToggleDemux>().ShowFunctionName = "StartCalendarTutorial";
    }

    //Black out everything. Only shows the green stamp
    public void SetUpGreenStampTip(){
        //Switch button to green stamp
        UIImageButton morningButton = morning.GetComponent<UIImageButton>();
        morningButton.normalSprite = GREEN_CHECK;
        morningButton.hoverSprite = GREEN_CHECK;
        morningButton.pressedSprite = GREEN_CHECK_DOWN;
        morningButton.isEnabled = false;
        morningButton.isEnabled = true;

        //Display hint arrow
        greenStampHintArrow = NGUITools.AddChild(morning.gameObject, calendarHintArrowPrefab);
        greenStampHintArrow.transform.localPosition = new Vector3(-80, 23, 0);
        greenStampHintArrow.transform.localEulerAngles = new Vector3(0, 
            calendarHintArrowPrefab.transform.localEulerAngles.y, 0);

        //Bring green stamp above the back drop
        morning.localPosition = new Vector3(morning.localPosition.x, morning.localPosition.y, -21); 
    }

    //Black out everything. Only shows the red stamp
    public void SetUpRedExTip(){
        //Switch button to red stamp
        UIImageButton nightButton = night.GetComponent<UIImageButton>();
        nightButton.normalSprite = RED_EX;
        nightButton.hoverSprite = RED_EX;
        nightButton.pressedSprite = RED_EX_DOWN;
        nightButton.isEnabled = false;
        nightButton.isEnabled = true;

        //Remove green stamp tutorial
        if(greenStampHintArrow != null) Destroy(greenStampHintArrow);
        morning.localPosition = new Vector3(morning.localPosition.x, morning.localPosition.y, 0);

        //Display hint arrow
        redStampHintArrow = NGUITools.AddChild(night.gameObject, calendarHintArrowPrefab);
        redStampHintArrow.transform.localPosition = new Vector3(-80, 22, 0);
        redStampHintArrow.transform.localEulerAngles = new Vector3(0, 
            calendarHintArrowPrefab.transform.localEulerAngles.y, 0);

       //Bring red stamp above the back drop
        night.localPosition = new Vector3(night.localPosition.x, night.localPosition.y, -21); 
    }

    //Black out everything.
    public void SetUpBonusTip(){
       //Bring red stamp below the back drop
        if(redStampHintArrow != null) Destroy(redStampHintArrow);
        night.localPosition = new Vector3(night.localPosition.x, night.localPosition.y, 0);

        //Bring gray stamp above
        morning.localPosition = new Vector3(morning.localPosition.x, morning.localPosition.y, -21);
    }

    //Reset calendar to original after tutorial is finished
    public void CleanUpTutorial(){
        //Reset button message handler to CalendarUIManager
        UIButtonMessage morningButtonMessage = this.morning.GetComponent<UIButtonMessage>();
        UIButtonMessage nightButtonMessage = this.night.GetComponent<UIButtonMessage>();

        morningButtonMessage.target = CalendarUIManager.Instance.gameObject;
        morningButtonMessage.functionName = "ClaimReward";
        nightButtonMessage.target = CalendarUIManager.Instance.gameObject;
        nightButtonMessage.functionName = "ClaimReward";

        //Reset the Z index of the buttons so they are the same as other buttons
        morning.localPosition = new Vector3(morning.localPosition.x, morning.localPosition.y, 0);
        night.localPosition = new Vector3(night.localPosition.x, night.localPosition.y, 0);

        //Destroy unused UIPanel
        Destroy(morning.GetComponent<UIPanel>());
        Destroy(night.GetComponent<UIPanel>());
        
        //Reset the finish target
        calendarPanel.GetComponent<TweenToggleDemux>().isShowFinishedCallback = false;
        calendarPanel.GetComponent<TweenToggleDemux>().ShowTarget = null;
        calendarPanel.GetComponent<TweenToggleDemux>().ShowFunctionName = "";

        //Clean up hint arrow if still there
        if(greenStampHintArrow != null) Destroy(greenStampHintArrow);
        if(redStampHintArrow != null) Destroy(redStampHintArrow);

        //Erase tutorial data and redraw calendar
        CalendarLogic.Instance.ResetWeekAfterTutorialFinish();

        //Destroy tutorial helper since it's not needed in the game anymore 
        Destroy(gameObject, 0.5f); 
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
            CalendarLogic.Instance.ClaimReward(calendarSlot.transform.position);
        }else{
            if(button.normalSprite == RED_EX) Destroy(redStampHintArrow);

            //shake the calendar slot
            Hashtable optional = new Hashtable();
            optional.Add("ease", LeanTweenType.punch);
            LeanTween.moveX(calendarSlot, 0.03f, 0.5f, optional);
        }
    }
}