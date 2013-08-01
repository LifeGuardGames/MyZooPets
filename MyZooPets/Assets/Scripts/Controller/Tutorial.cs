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
        if (DataManager.Instance.Tutorial.FirstTimeCalendar){
            calendar.GetComponent<TapItem>().OnTap += openCalendar;

            TutorialHighlighting highlight = calendar.GetComponent<TutorialHighlighting>();
            highlight.ShowArrow();
        }
        if (DataManager.Instance.Tutorial.FirstTimeRealInhaler){
            realInhaler.GetComponent<TapItem>().OnTap += openRealInhaler;
        }
    }
    // For the demo.
    void TrophyDemo(){
        if (DataManager.Instance.Tutorial.FirstTimeShelf){
            TutorialHighlighting highlight = shelf.GetComponent<TutorialHighlighting>();
            highlight.ShowArrow();

            shelf.GetComponent<TapItem>().OnTap += openShelf;
        }
        if (DataManager.Instance.Tutorial.FirstTimeHelpTrophy){
            helpTrophy.GetComponent<TapItem>().OnTap += openHelpTrophy;
        }
    }

    void openCalendar(){
        // added for the demo
        if (DataManager.Instance.Tutorial.FirstTimeCalendar){
            DataManager.Instance.Tutorial.FirstTimeCalendar = false;

            TutorialHighlighting highlight = calendar.GetComponent<TutorialHighlighting>();
            highlight.HideArrow();

            ShowCalendarTip1();

            realInhaler.GetComponent<TutorialHighlighting>().ShowArrow();
        }
    }

    void ShowCalendarTip1(){
        notificationUIManager.PopupTipWithImage("The Calendar tells you if your pet has been taking its inhaler.", "calendarIcon", ShowCalendarTip2, true, true);
    }

    void ShowCalendarTip2(){
        notificationUIManager.PopupTipWithImage("This means your pet missed a dose.", "calendarStampEx", ShowCalendarTip3, false, true);
    }

    void ShowCalendarTip3(){
        notificationUIManager.PopupTipWithImage("This means your pet took its inhaler. Click on these to get extra points!", "calendarStampCheck", ShowCalendarTip4, false, true);
    }

    void ShowCalendarTip4(){
        notificationUIManager.PopupTipWithImage("Come back every 12 hours to get more points!", "calendarIcon", delegate(){}, false, false);
    }

    void openChallenges(){
        DataManager.Instance.Tutorial.FirstTimeChallenges = false;
        TutorialHighlighting highlight = challenges.GetComponent<TutorialHighlighting>();
        highlight.HideArrow();
    }

    void openDiary(){
        DataManager.Instance.Tutorial.FirstTimeDiary = false;
        TutorialHighlighting highlight = diary.GetComponent<TutorialHighlighting>();
        highlight.HideArrow();
    }

    void openSlotMachine(){
        DataManager.Instance.Tutorial.FirstTimeSlotMachine = false;
        TutorialHighlighting highlight = slotMachine.GetComponent<TutorialHighlighting>();
        highlight.HideArrow();
    }

    void openRealInhaler(){
        notificationUIManager.PopupTipWithImage("Use this inhaler every morning and afternoon to keep your pet healthy!", "advairPurple", clickManager.OpenRealInhaler, true, false);

        TutorialHighlighting highlight = realInhaler.GetComponent<TutorialHighlighting>();
        highlight.HideArrow();

        DataManager.Instance.Tutorial.FirstTimeRealInhaler = false;
    }

    void openTeddyInhaler(){
        TutorialHighlighting highlight = teddyInhaler.GetComponent<TutorialHighlighting>();
        highlight.HideArrow();
        DataManager.Instance.Tutorial.FirstTimeTeddyInhaler = false;
    }

    void openShelf(){
        // added for the demo
        if (DataManager.Instance.Tutorial.FirstTimeHelpTrophy){
            DataManager.Instance.Tutorial.FirstTimeShelf = false;

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
            DataManager.Instance.Tutorial.FirstTimeHelpTrophy = false;
        }
    }

    public void DegradTriggerClicked(){
        if (DataManager.Instance.Tutorial.FirstTimeDegradTrigger){
            DataManager.Instance.Tutorial.FirstTimeDegradTrigger = false;
            ShowDegradTip1();
        }
    }

    void ShowDegradTip1(){
        notificationUIManager.PopupTipWithImage("Good job! You just removed an asthma trigger.", "guiPanelStatsHealth", ShowDegradTip2, true, true);
    }
    void ShowDegradTip2(){
        notificationUIManager.PopupTipWithImage("Make sure you clean them up when you see them, or your pet will get sick!", "Skull", delegate(){}, false, true); // disappear immediately when done, because the level up message should pop up right away
    }
}
