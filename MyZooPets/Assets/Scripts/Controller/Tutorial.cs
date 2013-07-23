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
            GrowShrink growShrink = calendar.GetComponent<GrowShrink>();
            calendar.GetComponent<TapItem>().OnTap += openCalendar;
            growShrink.Play();
        }
        if (DataManager.FirstTimeRealInhaler){
            realInhaler.GetComponent<TapItem>().OnTap += openRealInhaler;
        }
    }
    // For the demo.
    void TrophyDemo(){
        if (DataManager.FirstTimeShelf){
            GrowShrink growShrink = shelf.GetComponent<GrowShrink>();
            shelf.GetComponent<TapItem>().OnTap += openShelf;
            growShrink.Play();
        }
        if (DataManager.FirstTimeHelpTrophy){
            helpTrophy.GetComponent<TapItem>().OnTap += openHelpTrophy;
        }
    }

    void openCalendar(){
        ShowCalendarTip1();

        GrowShrink growShrink = calendar.GetComponent<GrowShrink>();
        growShrink.Stop();

        DataManager.FirstTimeCalendar = false;
        // added for the demo
        if (DataManager.FirstTimeRealInhaler){
            realInhaler.GetComponent<GrowShrink>().Play();
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
        GrowShrink growShrink = challenges.GetComponent<GrowShrink>();
        DataManager.FirstTimeChallenges = false;
        growShrink.Stop();
    }
    void openDiary(){
        GrowShrink growShrink = diary.GetComponent<GrowShrink>();
        DataManager.FirstTimeDiary = false;
        growShrink.Stop();
    }
    void openSlotMachine(){
        GrowShrink growShrink = slotMachine.GetComponent<GrowShrink>();
        DataManager.FirstTimeSlotMachine = false;
        growShrink.Stop();
    }
    void openRealInhaler(){
        // todo: change sprite name
        notificationUIManager.PopupTipWithImage("Use this inhaler every morning and afternoon to keep your pet healthy!", "guiPanelStatsHealth", clickManager.OpenRealInhaler, true, false);

        GrowShrink growShrink = realInhaler.GetComponent<GrowShrink>();
        growShrink.Stop();

        DataManager.FirstTimeRealInhaler = false;
    }
    void openTeddyInhaler(){
        GrowShrink growShrink = teddyInhaler.GetComponent<GrowShrink>();
        DataManager.FirstTimeTeddyInhaler = false;
        growShrink.Stop();
    }
    void openShelf(){
        GrowShrink growShrink = shelf.GetComponent<GrowShrink>();
        DataManager.FirstTimeShelf = false;
        growShrink.Stop();
        helpTrophy.GetComponent<GrowShrink>().StopAll();

        // added for the demo
        if (DataManager.FirstTimeHelpTrophy){
            helpTrophy.GetComponent<GrowShrink>().Play();
        }
    }
    void openHelpTrophy(){

        // make sure we are in trophy mode
        // todo: have a better way of checking if we are in trophy mode
        if (!ClickManager.CanRespondToTap()){ // meaning we have clicked something
            GrowShrink growShrink = helpTrophy.GetComponent<GrowShrink>();
            DataManager.FirstTimeHelpTrophy = false;
            growShrink.Stop();
        }
    }
}
