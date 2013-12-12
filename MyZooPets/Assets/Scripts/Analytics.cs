using UnityEngine;
using System;
using System.Collections;

public class Analytics : Singleton<Analytics>{
    public const string DIAGNOSE_RESULT_CORRECT = "correct";
    public const string DIAGNOSE_RESULT_INCORRECT = "incorrect";

    public const string PET_STATUS_OK = "ok";
    public const string PET_STATUS_SICK = "sick";
    public const string PET_STATUS_EMERGENCY = "attack";

    public const string STEP_STATUS_COMPLETE = "complete";
    public const string STEP_STATUS_QUIT = "quit";

    public const string ITEM_STATUS_BOUGHT = "bought";
    public const string ITEM_STATUS_USED = "used";
    public const string ITEM_STATUS_RECEIVED = "received";

    public const string TASK_STATUS_COMPLETE = "complete";
    public const string TASK_STATUS_FAIL = "fail";

    private static bool isCreated = false;

    public const string INHALER_CATEGORY = "MiniGame:Inhaler:";
    public const string RUNNER_CATEGORY = "MiniGame:Runner:";
    public const string DIAGNOSE_CATEGORY = "MiniGame:Diagnose:";
    public const string NINJA_CATEGORY = "MiniGame:Ninja:";

    private DateTime playTime;
    private bool isGameTimerOn = false;

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
    // public void RunnerPlayerCrashIntoTrigger(string triggerName){
    //     if(!String.IsNullOrEmpty(triggerName))
    //         GA.API.Design.NewEvent(RUNNER_CATEGORY + "TriggerCrashed:" + triggerName);
    // }

    public void RunnerPlayerItemPickUp(string itemName){
        if(!String.IsNullOrEmpty(itemName))
            GA.API.Design.NewEvent(RUNNER_CATEGORY + "ItemPickedUp:" + itemName);
    }

    public void RunnerPlayerDistanceRan(int distanceRan){
        GA.API.Design.NewEvent(RUNNER_CATEGORY + "DistanceRan", (float) distanceRan);
    }

    //==========================Inhaler Game====================================
    //Which steps in inhaler game does the kids need help the most
    public void InhalerHintRequired(int stepID){
        GA.API.Design.NewEvent(INHALER_CATEGORY + "HintRequired:" + stepID);
    }

    //Number of missed vs correct inhaler swipe sequences
    public void InhalerSwipeSequences(string stepStatus, int stepID){
        if(!String.IsNullOrEmpty(stepStatus))
            GA.API.Design.NewEvent(INHALER_CATEGORY + "Inhaler:" + stepStatus + ":" + stepID);
    }

    //==========================Diagnose track game=============================
    //Number of correct diagnose. 
    //Which symptom is the user having trouble identifying
    public void DiagnoseResult(string diagnoseResult, AsthmaStage petStatus, AsthmaStage zone){
        if(!String.IsNullOrEmpty(diagnoseResult))
            GA.API.Design.NewEvent(DIAGNOSE_CATEGORY + "Diagnose:" + diagnoseResult + ":" + 
                Enum.GetName(typeof(AsthmaStage), petStatus) + ":" + Enum.GetName(typeof(AsthmaStage), zone));
    }

    //=======================General Analytics==================================
    //Will be use in different mini games

    //Start tracking playtime 
    public void StartPlayTimeTracker(){
        playTime = DateTime.Now;
        isGameTimerOn = true;
    } 

    //Stop tracking and submit playtime
    public void EndPlayTimeTracker(){
        string levelName = "";
        switch(Application.loadedLevelName){
            case "InhalerGamePet":
                levelName = INHALER_CATEGORY;
            break;
            case "Runner":
                levelName = RUNNER_CATEGORY;
            break;
            case "DiagnoseGameTracks":
                levelName = DIAGNOSE_CATEGORY;
            break;
            case "TriggerNinja":
                levelName = NINJA_CATEGORY;
            break;
        }
        isGameTimerOn = false;
        TimeSpan timeSpentInGame = DateTime.Now - playTime; //minutes
        GA.API.Design.NewEvent(levelName + "TimeSpent:" + timeSpentInGame.Minutes);
    }

    void OnApplicationPause(bool isPaused){
        if(isPaused && isGameTimerOn)
            EndPlayTimeTracker();
    }

    //Badges unlock
    public void BadgeUnlocked(string badgeID){
        if(!String.IsNullOrEmpty(badgeID))
            GA.API.Design.NewEvent("Badge:Unlocked:" + badgeID);
    }

    //Items used or purchased
    public void ItemEvent(string itemStatus, ItemType itemType, string itemID){
        if(!String.IsNullOrEmpty(itemStatus) && !String.IsNullOrEmpty(itemID)){
            GA.API.Design.NewEvent("Items:" + itemStatus + ":" + 
                Enum.GetName(typeof(ItemType), itemType) + ":" + itemID);
        }
    }

    //Wellapad
    public void WellapadTaskEvent(string taskStatus, string missionID, string taskID){
        if(!String.IsNullOrEmpty(taskStatus) && !String.IsNullOrEmpty(missionID) && !String.IsNullOrEmpty(taskID))
            GA.API.Design.NewEvent(taskStatus + ":" + missionID + ":" + taskID);
    }
}
