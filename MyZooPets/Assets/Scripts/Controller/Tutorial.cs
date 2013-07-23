using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {

    public GameObject calendar;
    public GameObject challenges;
    public GameObject diary;
    public GameObject slotMachine;
    public GameObject realInhaler;
    public GameObject teddyInhaler;
    public GameObject shelf;
    public GameObject helpTrophy;

    public NotificationUIManager notificationUIManager;
    public ClickManager clickManager;

    public void Start(){
        InhalerMissAndInhalerGame();
        TrophyDemo();
    }

    // For the demo.
    void InhalerMissAndInhalerGame(){
        if (DataManager.FirstTimeCalendar){
            calendar.GetComponent<TapItem>().OnTap += openCalendar;

            TutorialHighlighting highlight = calendar.GetComponent<TutorialHighlighting>();
            highlight.ShowArrow();
        }
        if (DataManager.FirstTimeRealInhaler){
            realInhaler.GetComponent<TapItem>().OnTap += openRealInhaler;
        }
    }
    // For the demo.
    void TrophyDemo(){
        if (DataManager.FirstTimeShelf){
            TutorialHighlighting highlight = shelf.GetComponent<TutorialHighlighting>();
            highlight.ShowArrow();

            shelf.GetComponent<TapItem>().OnTap += openShelf;
        }
        if (DataManager.FirstTimeHelpTrophy){
            helpTrophy.GetComponent<TapItem>().OnTap += openHelpTrophy;
        }
    }

    void openCalendar(){
        // added for the demo
        if (DataManager.FirstTimeCalendar){
            DataManager.FirstTimeCalendar = false;

            TutorialHighlighting highlight = calendar.GetComponent<TutorialHighlighting>();
            highlight.HideArrow();

            ShowCalendarTip1();

            realInhaler.GetComponent<TutorialHighlighting>().ShowArrow();
        }
    }

    void ShowCalendarTip1(){
        notificationUIManager.PopupTipWithImage("The Calendar tells you if your pet has been taking its inhaler.", "guiPanelStatsHealth", ShowCalendarTip2, true, true);
    }

    void ShowCalendarTip2(){
        notificationUIManager.PopupTipWithImage("This means your pet missed a dose.", "calendarStampEx", ShowCalendarTip3, false, true);
    }

    void ShowCalendarTip3(){
        notificationUIManager.PopupTipWithImage("This means your pet took its inhaler. Click on these to get extra points!", "calendarStampCheck", ShowCalendarTip4, false, true);
    }

    void ShowCalendarTip4(){
        notificationUIManager.PopupTipWithImage("Come back every 12 hours to get more points!", "guiPanelStarsStar", delegate(){}, false, false);
    }

    void openChallenges(){
        DataManager.FirstTimeChallenges = false;
        TutorialHighlighting highlight = challenges.GetComponent<TutorialHighlighting>();
        highlight.HideArrow();
    }

    void openDiary(){
        DataManager.FirstTimeDiary = false;
        TutorialHighlighting highlight = diary.GetComponent<TutorialHighlighting>();
        highlight.HideArrow();
    }

    void openSlotMachine(){
        DataManager.FirstTimeSlotMachine = false;
        TutorialHighlighting highlight = slotMachine.GetComponent<TutorialHighlighting>();
        highlight.HideArrow();
    }

    void openRealInhaler(){
        // todo: change sprite name
        notificationUIManager.PopupTipWithImage("Use this inhaler every morning and afternoon to keep your pet healthy!", "guiPanelStatsHealth", clickManager.OpenRealInhaler, true, false);

        TutorialHighlighting highlight = realInhaler.GetComponent<TutorialHighlighting>();
        highlight.HideArrow();

        DataManager.FirstTimeRealInhaler = false;
    }

    void openTeddyInhaler(){
        TutorialHighlighting highlight = teddyInhaler.GetComponent<TutorialHighlighting>();
        highlight.HideArrow();
        DataManager.FirstTimeTeddyInhaler = false;
    }

    void openShelf(){
        // added for the demo
        if (DataManager.FirstTimeHelpTrophy){
            DataManager.FirstTimeShelf = false;

            TutorialHighlighting highlight = shelf.GetComponent<TutorialHighlighting>();
            highlight.HideArrow();

            helpTrophy.GetComponent<TutorialHighlighting>().ShowArrow();
        }
    }
    void openHelpTrophy(){

        // make sure we are in trophy mode
        // todo: have a better way of checking if we are in trophy mode
        if (!ClickManager.CanRespondToTap()){ // meaning we have clicked something
            TutorialHighlighting highlight = helpTrophy.GetComponent<TutorialHighlighting>();
            highlight.HideArrow();
            DataManager.FirstTimeHelpTrophy = false;
        }
    }

    public void DegradTriggerClicked(){
        if (DataManager.FirstTimeDegradTrigger){
            DataManager.FirstTimeDegradTrigger = false;
            ShowDegradTip1();
        }
    }

    void ShowDegradTip1(){
        notificationUIManager.PopupTipWithImage("Good job! You just removed an asthma trigger.", "guiPanelStatsHealth", ShowDegradTip2, true, true);
    }
    void ShowDegradTip2(){
        notificationUIManager.PopupTipWithImage("Make sure you clean them up when you see them, or your pet will get sick!", "Skull", delegate(){}, false, false);
    }
}
