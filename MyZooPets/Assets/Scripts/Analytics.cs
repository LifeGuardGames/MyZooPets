using UnityEngine;
using System;
using System.Collections;

public class Analytics : Singleton<Analytics>{
    public const string DIAGNOSE_RESULT_CORRECT = "correct";
    public const string DIAGNOSE_RESULT_INCORRECT = "incorrect";

    public const string PET_STATUS_OK = "ok";
    public const string PET_STATUS_SICK = "sick";
    public const string PET_STATUS_EMERGENCY = "emergency";

    public const string STEP_STATUS_CORRECT = "correct";
    public const string STEP_STATUS_INCORRECT = "incorrect";
    public const string STEP_STATUS_QUIT = "quit";

    private static bool isCreated = false;

    private const string INHALER_CATEGORY = "MiniGame:Inhaler:";
    private const string RUNNER_CATEGORY = "MiniGame:Runner:";
    private const string DIAGNOSE_CATEGORY = "MiniGame:Diagnose:";

    private DateTime playTime;

    void Awake(){
        if(isCreated){
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        isCreated = true;
    }

    //=========================Runner Game======================================
    //Where did the user die most often in the runner game?
    public void RunnerPlayerDied(string levelComponentName){
        if(!String.IsNullOrEmpty(levelComponentName))
            GA.API.Design.NewEvent(RUNNER_CATEGORY + "Dead:" + levelComponentName);
    }

    //Which triggers does the user have difficulty recognizing as bad triggers?
    //Number of times crashed into trigger. 
    public void RunnerPlayerCrashIntoTrigger(string triggerName){
        if(!String.IsNullOrEmpty(triggerName))
            GA.API.Design.NewEvent(RUNNER_CATEGORY + "TriggerCrashed:" + triggerName);
    }

    //==========================Inhaler Game====================================
    //Which steps in inhaler game does the kids need help the most
    public void InhalerHintRequired(string stepID){
        if(!String.IsNullOrEmpty(stepID))
            GA.API.Design.NewEvent(INHALER_CATEGORY + "HintRequired:" + stepID);
    }

    //Number of missed vs correct inhaler swipe sequences
    public void InhalerSwipeSequences(string stepStatus, string stepID){
        if(!String.IsNullOrEmpty(stepStatus) && !String.IsNullOrEmpty(stepID))
            GA.API.Design.NewEvent(INHALER_CATEGORY + "Inhaler:" + stepStatus + ":" + stepID);
    }

    //==========================Diagnose track game=============================
    //Number of correct diagnose. 
    //Which symptom is the user having trouble identifying
    public void DiagnoseResult(string diagnoseResult, string petStatus){
        if(!String.IsNullOrEmpty(diagnoseResult) && !String.IsNullOrEmpty(petStatus))
            GA.API.Design.NewEvent(DIAGNOSE_CATEGORY + "Diagnose:" + diagnoseResult + ":" + petStatus);
    }

    //=======================General Analytics==================================
    //Will be use in different mini games

    //Start tracking playtime 
    public void StartPlayTimeTracker(){
        playTime = DateTime.Now;
    } 

    //Stop tracking and submit playtime
    public void EndPlayTimeTracker(string miniGameName){
        TimeSpan timeSpentInGame = DateTime.Now - playTime; //minutes
        if(!String.IsNullOrEmpty(miniGameName))
            GA.API.Design.NewEvent(miniGameName + "TimeSpent:" + timeSpentInGame);
    }

    //Badges unlock
    public void BadgeUnlocked(string badgeName){
        if(!String.IsNullOrEmpty(badgeName))
            GA.API.Design.NewEvent("Badge:Unlocked:" + badgeName);
    }

    
}
