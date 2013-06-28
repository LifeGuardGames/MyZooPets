using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {

    public GameObject calendar;
    public GameObject challenges;
    public GameObject diary;
    public GameObject slotMachine;

    public void Init(){
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
    }

    void openCalendar(){
        GrowShrink growShrink = calendar.GetComponent<GrowShrink>();
        DataManager.FirstTimeCalendar = false;
        growShrink.Stop();
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
}
