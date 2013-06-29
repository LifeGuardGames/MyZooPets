using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {

    public GameObject calendar;
    public GameObject challenges;
    public GameObject diary;
    public GameObject slotMachine;
    public GameObject realInhaler;
    public GameObject teddyInhaler;

    public void Init(){
        InhalerMissAndInhalerGame();
    }

    // not used right now
    void HighlightAll(){
        if (DataManager.FirstTimeCalendar){
            GrowShrink growShrink = calendar.GetComponent<GrowShrink>();
            calendar.GetComponent<TapItem>().OnTap += openCalendar;
            growShrink.Play();
        }
        if (DataManager.FirstTimeChallenges){
            GrowShrink growShrink = challenges.GetComponent<GrowShrink>();
            challenges.GetComponent<TapItem>().OnTap += openChallenges;
            growShrink.Play();
        }
        if (DataManager.FirstTimeDiary){
            GrowShrink growShrink = diary.GetComponent<GrowShrink>();
            diary.GetComponent<TapItem>().OnTap += openDiary;
            growShrink.Play();
        }
        if (DataManager.FirstTimeSlotMachine){
            GrowShrink growShrink = slotMachine.GetComponent<GrowShrink>();
            slotMachine.GetComponent<TapItem>().OnTap += openSlotMachine;
            growShrink.Play();
        }
        if (DataManager.FirstTimeRealInhaler){
            GrowShrink growShrink = realInhaler.GetComponent<GrowShrink>();
            realInhaler.GetComponent<TapItem>().OnTap += openRealInhaler;
            growShrink.Play();
        }
        if (DataManager.FirstTimeTeddyInhaler){
            GrowShrink growShrink = teddyInhaler.GetComponent<GrowShrink>();
            teddyInhaler.GetComponent<TapItem>().OnTap += openTeddyInhaler;
            growShrink.Play();
        }
    }

    // For the demo.
    void InhalerMissAndInhalerGame(){
        if (DataManager.FirstTimeCalendar){
            GrowShrink growShrink = calendar.GetComponent<GrowShrink>();
            calendar.GetComponent<TapItem>().OnTap += openCalendar;
            growShrink.Play();
        }
        if (DataManager.FirstTimeRealInhaler){
            GrowShrink growShrink = realInhaler.GetComponent<GrowShrink>();
            realInhaler.GetComponent<TapItem>().OnTap += openRealInhaler;
        }
    }

    void openCalendar(){
        if (ClickManager.CanRespondToTap()){
            GrowShrink growShrink = calendar.GetComponent<GrowShrink>();
            DataManager.FirstTimeCalendar = false;
            growShrink.Stop();

            // added for the demo
            if (DataManager.FirstTimeRealInhaler){
                realInhaler.GetComponent<GrowShrink>().Play();
            }
        }
    }
    void openChallenges(){
        if (ClickManager.CanRespondToTap()){
            GrowShrink growShrink = challenges.GetComponent<GrowShrink>();
            DataManager.FirstTimeChallenges = false;
            growShrink.Stop();
        }
    }
    void openDiary(){
        if (ClickManager.CanRespondToTap()){
            GrowShrink growShrink = diary.GetComponent<GrowShrink>();
            DataManager.FirstTimeDiary = false;
            growShrink.Stop();
        }
    }
    void openSlotMachine(){
        if (ClickManager.CanRespondToTap()){
            GrowShrink growShrink = slotMachine.GetComponent<GrowShrink>();
            DataManager.FirstTimeSlotMachine = false;
            growShrink.Stop();
        }
    }
    void openRealInhaler(){
        if (ClickManager.CanRespondToTap()){
            GrowShrink growShrink = realInhaler.GetComponent<GrowShrink>();
            DataManager.FirstTimeRealInhaler = false;
            growShrink.Stop();
        }
    }
    void openTeddyInhaler(){
        if (ClickManager.CanRespondToTap()){
            GrowShrink growShrink = teddyInhaler.GetComponent<GrowShrink>();
            DataManager.FirstTimeTeddyInhaler = false;
            growShrink.Stop();
        }
    }
}
