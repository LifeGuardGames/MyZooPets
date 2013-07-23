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

    // not used right now
    // void HighlightAll(){
    //     if (DataManager.FirstTimeCalendar){
    //         GrowShrink growShrink = calendar.GetComponent<GrowShrink>();
    //         calendar.GetComponent<TapItem>().OnTap += openCalendar;
    //         growShrink.Play();
    //     }
    //     if (DataManager.FirstTimeChallenges){
    //         GrowShrink growShrink = challenges.GetComponent<GrowShrink>();
    //         challenges.GetComponent<TapItem>().OnTap += openChallenges;
    //         growShrink.Play();
    //     }
    //     if (DataManager.FirstTimeDiary){
    //         GrowShrink growShrink = diary.GetComponent<GrowShrink>();
    //         diary.GetComponent<TapItem>().OnTap += openDiary;
    //         growShrink.Play();
    //     }
    //     if (DataManager.FirstTimeSlotMachine){
    //         GrowShrink growShrink = slotMachine.GetComponent<GrowShrink>();
    //         slotMachine.GetComponent<TapItem>().OnTap += openSlotMachine;
    //         growShrink.Play();
    //     }
    //     if (DataManager.FirstTimeRealInhaler){
    //         GrowShrink growShrink = realInhaler.GetComponent<GrowShrink>();
    //         realInhaler.GetComponent<TapItem>().OnTap += openRealInhaler;
    //         growShrink.Play();
    //     }
    //     if (DataManager.FirstTimeTeddyInhaler){
    //         GrowShrink growShrink = teddyInhaler.GetComponent<GrowShrink>();
    //         teddyInhaler.GetComponent<TapItem>().OnTap += openTeddyInhaler;
    //         growShrink.Play();
    //     }
    // }

    // For the demo.
    void InhalerMissAndInhalerGame(){
        if (DataManager.FirstTimeCalendar){
            calendar.GetComponent<TapItem>().OnTap += openCalendar;

            TutorialHighlighting highlight = calendar.GetComponent<TutorialHighlighting>();
            highlight.ShowArrow();

            // GrowShrink growShrink = calendar.GetComponent<GrowShrink>();
            // growShrink.Play();
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

            // GrowShrink growShrink = shelf.GetComponent<GrowShrink>();
            // growShrink.Play();
            shelf.GetComponent<TapItem>().OnTap += openShelf;
        }
        if (DataManager.FirstTimeHelpTrophy){
            helpTrophy.GetComponent<TapItem>().OnTap += openHelpTrophy;
        }
    }

    void openCalendar(){
        ShowCalendarTip1();

        TutorialHighlighting highlight = calendar.GetComponent<TutorialHighlighting>();
        highlight.HideArrow();
        // GrowShrink growShrink = calendar.GetComponent<GrowShrink>();
        // growShrink.Stop();

        DataManager.FirstTimeCalendar = false;
        // added for the demo
        if (DataManager.FirstTimeRealInhaler){
            // realInhaler.GetComponent<GrowShrink>().Play();
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
        // GrowShrink growShrink = challenges.GetComponent<GrowShrink>();
        // growShrink.Stop();
    }
    void openDiary(){
        DataManager.FirstTimeDiary = false;
        TutorialHighlighting highlight = diary.GetComponent<TutorialHighlighting>();
        highlight.HideArrow();
        // GrowShrink growShrink = diary.GetComponent<GrowShrink>();
        // growShrink.Stop();
    }
    void openSlotMachine(){
        DataManager.FirstTimeSlotMachine = false;
        TutorialHighlighting highlight = slotMachine.GetComponent<TutorialHighlighting>();
        highlight.HideArrow();
        // GrowShrink growShrink = slotMachine.GetComponent<GrowShrink>();
        // growShrink.Stop();
    }
    void openRealInhaler(){
        // todo: change sprite name
        notificationUIManager.PopupTipWithImage("Use this inhaler every morning and afternoon to keep your pet healthy!", "guiPanelStatsHealth", clickManager.OpenRealInhaler, true, false);

        TutorialHighlighting highlight = realInhaler.GetComponent<TutorialHighlighting>();
        highlight.HideArrow();
        // GrowShrink growShrink = realInhaler.GetComponent<GrowShrink>();
        // growShrink.Stop();

        DataManager.FirstTimeRealInhaler = false;
    }
    void openTeddyInhaler(){
        TutorialHighlighting highlight = teddyInhaler.GetComponent<TutorialHighlighting>();
        highlight.HideArrow();
        DataManager.FirstTimeTeddyInhaler = false;
        // GrowShrink growShrink = teddyInhaler.GetComponent<GrowShrink>();
        // growShrink.Stop();
    }
    void openShelf(){
        TutorialHighlighting highlight = shelf.GetComponent<TutorialHighlighting>();
        highlight.HideArrow();

        DataManager.FirstTimeShelf = false;

        // GrowShrink growShrink = shelf.GetComponent<GrowShrink>();
        // growShrink.Stop();

        // helpTrophy.GetComponent<GrowShrink>().StopAll();

        // added for the demo
        if (DataManager.FirstTimeHelpTrophy){
            // helpTrophy.GetComponent<GrowShrink>().Play();
            helpTrophy.GetComponent<TutorialHighlighting>().ShowArrow();
        }
    }
    void openHelpTrophy(){

        // make sure we are in trophy mode
        // todo: have a better way of checking if we are in trophy mode
        if (!ClickManager.CanRespondToTap()){ // meaning we have clicked something
            // GrowShrink growShrink = helpTrophy.GetComponent<GrowShrink>();
            // growShrink.Stop();
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
