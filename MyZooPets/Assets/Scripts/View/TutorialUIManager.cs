using UnityEngine;
using System;
using System.Collections;

public class TutorialUIManager : Singleton<TutorialUIManager> {

    public GameObject calendar;
    // public GameObject challenges;
    // public GameObject diary;
    // public GameObject slotMachine;
    public GameObject realInhaler;
    public GameObject teddyInhaler;
    public GameObject backDrop; //use this giant collider for forced tutorial
    public GameObject nguiAnchor; //the anchor to spawn the backDrop;

    public NotificationUIManager notificationUIManager;
    public ClickManager clickManager;

    private const string CALENDAR_TIP_INTRO = "This calendar shows if you have been using the inhaler on your pet on time.";
    private const string CALENDAR_TIP_GREEN_STAMP = "Good Job! Green stamp means you used the inhaler on your pet on time. You can click on the green stamps to earn stars!";
    private const string CALENDAR_TIP_RED_STAMP = "Ah Oh. Red stamp means you forgot to use the inhaler on your pet on time. This is not good for the pets heath.";
    private const string CALENDAR_TIP_BONUS = "Extra stars can be collected ever 12 hours. Remember to check often!";
    private const string CALENDAR_TIP_CONCLUDE = "Now let's start the game with a new calendar";
    private const string DEGRAD_TIP1 = "Good job! You just removed an asthma trigger.";
    private const string DEGRAD_TIP2 = "Make sure you clean them up when you see them, or your pet will get sick!";
    
    void Start(){
        //use a if else if here to make sure that any tutorials not visited get called
        TutorialLogic.OnTutorialUpdated += UpdateTutorial;
        SetupTutorial();
        UpdateTutorial(null, EventArgs.Empty);
    }

    void OnDestroy(){
        TutorialLogic.OnTutorialUpdated -= UpdateTutorial;
    }

    //Event listener that activates new tutorial when an old tutorial is completed
    private void UpdateTutorial(object sender, EventArgs args){
        if(TutorialLogic.Instance.FirstTimeRealInhaler){
            realInhaler.GetComponent<TutorialHighlighting>().ShowArrow();
        }else if(TutorialLogic.Instance.FirstTimeCalendar){
            calendar.GetComponent<TutorialHighlighting>().ShowArrow();
        }else{}
    }

    //Assign OnTap event listener to game objects that are still new to the user
    private void SetupTutorial(){
        if(TutorialLogic.Instance.FirstTimeCalendar){
            calendar.GetComponent<TapItem>().OnTap += StartCalendarTutorial;
        }
        if(TutorialLogic.Instance.FirstTimeRealInhaler){
            realInhaler.GetComponent<TapItem>().OnTap += StartRealInhalerTutorial;
        }
    }

    //Use to cover up the whole screen during a mandatory tutorial
    public void BackDrop(bool isVisible){
        if(isVisible){
            GameObject go = NGUITools.AddChild(nguiAnchor, backDrop);
            go.name = "TutorialBackDrop";
            go.transform.localPosition = new Vector3(0, 0, -20); 
            go.transform.localScale = new Vector3(5000, 5000, 1); 
        }else{
            GameObject go = nguiAnchor.transform.Find("TutorialBackDrop").gameObject;
            if(go != null) Destroy(go, 0.5f);
        }
    }

    //========================Calendar Tutorial======================
    private void StartCalendarTutorial(){
        CalendarUIManager.Instance.GreenStampTutorial();
    }

    public void ShowCalendarTipGreenStamp(){
        // notificationUIManager.PopupTipWithImage(CALENDAR_TIP_GREEN_STAMP, "calendarStampCheck", 
        //     CalendarUIManager.Instance.RedExTutorial, false, true);
        notificationUIManager.TutorialMessage("help", CalendarUIManager.Instance.RedExTutorial);
    }

    public void ShowCalendarTipRedStamp(){
        // notificationUIManager.PopupTipWithImage(CALENDAR_TIP_RED_STAMP, "calendarStampEx", 
        //     ShowCalendarTipBonus, false, true);
        notificationUIManager.TutorialMessage("help", ShowCalendarTipBonus);
    }

    public void ShowCalendarTipBonus(){
        // notificationUIManager.PopupTipWithImage(CALENDAR_TIP_BONUS, "calendarIcon", 
        //     ShowCalendarTipConclude, false, true);
        notificationUIManager.TutorialMessage("help", ShowCalendarTipConclude);
    }

    /*
        1)Conclude the calendar tutorial. 
        2)Turn this tutorial off in DataManager. 
        3)Unregister the tap item listener. 
        4)Show Arrow for Real Inhaler.
    */
    public void ShowCalendarTipConclude(){
        notificationUIManager.PopupTipWithImage(CALENDAR_TIP_CONCLUDE, "calendarIcon",
            CalendarUIManager.Instance.ResetAfterTutorialFinish, false, true);

        TutorialLogic.Instance.FirstTimeCalendar = false;
        calendar.GetComponent<TapItem>().OnTap -= StartCalendarTutorial;
        calendar.GetComponent<TutorialHighlighting>().HideArrow();
        realInhaler.GetComponent<TutorialHighlighting>().ShowArrow();
    }

    //============Trigger tutorial=================
    public void StartDegradTriggerTutorial(){
        if (TutorialLogic.Instance.FirstTimeDegradTrigger){
            ShowDegradTipIntro();
        }
    }

    private void ShowDegradTipIntro(){
        notificationUIManager.PopupTipWithImage(DEGRAD_TIP1, "guiPanelStatsHealth", 
            ShowDegradTipConclude, true, true);
    }
    
    private void ShowDegradTipConclude(){
        // disappear immediately when done, because the level up message should pop up right away
        notificationUIManager.PopupTipWithImage(DEGRAD_TIP2, "Skull", null, false, true); 
        TutorialLogic.Instance.FirstTimeDegradTrigger = false;
    }

    //==============Inhaler tutorial=================
    private void StartRealInhalerTutorial(){
        notificationUIManager.PopupTipWithImage("Use this inhaler every morning and afternoon to keep your pet healthy!", "advairPurple", clickManager.OpenRealInhaler, true, false);

        TutorialHighlighting highlight = realInhaler.GetComponent<TutorialHighlighting>();
        highlight.HideArrow();

        DataManager.Instance.Tutorial.FirstTimeRealInhaler = false;
    }

    private void StartTeddyInhalertutorial(){
        TutorialHighlighting highlight = teddyInhaler.GetComponent<TutorialHighlighting>();
        highlight.HideArrow();
        DataManager.Instance.Tutorial.FirstTimeTeddyInhaler = false;
    }
    
    // private void openChallenges(){
    //     DataManager.Instance.Tutorial.FirstTimeChallenges = false;
    //     TutorialHighlighting highlight = challenges.GetComponent<TutorialHighlighting>();
    //     highlight.HideArrow();
    // }

    // void openDiary(){
    //     DataManager.Instance.Tutorial.FirstTimeDiary = false;
    //     TutorialHighlighting highlight = diary.GetComponent<TutorialHighlighting>();
    //     highlight.HideArrow();
    // }

    // void openSlotMachine(){
    //     DataManager.Instance.Tutorial.FirstTimeSlotMachine = false;
    //     TutorialHighlighting highlight = slotMachine.GetComponent<TutorialHighlighting>();
    //     highlight.HideArrow();
    // }

    

    // void openShelf(){
    //     // added for the demo
    //     if (DataManager.Instance.Tutorial.FirstTimeHelpTrophy){
    //         DataManager.Instance.Tutorial.FirstTimeShelf = false;

    //         TutorialHighlighting highlight = shelf.GetComponent<TutorialHighlighting>();
    //         highlight.HideArrow();

    //         helpTrophy.GetComponent<TutorialHighlighting>().ShowArrow();
    //     }
    // }
    // void openHelpTrophy(){

    //     // make sure we are in trophy mode
    //     // todo: have a better way of checking if we are in trophy mode
    //     if (!ClickManager.CanRespondToTap()){ // meaning we have clicked something
    //         TutorialHighlighting highlight = helpTrophy.GetComponent<TutorialHighlighting>();
    //         highlight.HideArrow();
    //         DataManager.Instance.Tutorial.FirstTimeHelpTrophy = false;
    //     }
    // }

   
}
