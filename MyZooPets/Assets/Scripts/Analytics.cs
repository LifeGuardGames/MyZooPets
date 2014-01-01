using UnityEngine;
using System;
using System.Collections;

public class Analytics : MonoBehaviour {
    public const string DIAGNOSE_RESULT_CORRECT = "Correct";
    public const string DIAGNOSE_RESULT_INCORRECT = "Incorrect";

    public const string PET_STATUS_OK = "Ok";
    public const string PET_STATUS_SICK = "Sick";
    public const string PET_STATUS_EMERGENCY = "Attack";

    public const string STEP_STATUS_COMPLETE = "Complete";
    public const string STEP_STATUS_QUIT = "Quit";

    public const string ITEM_STATUS_BOUGHT = "Bought";
    public const string ITEM_STATUS_USED = "Used";
    public const string ITEM_STATUS_RECEIVED = "Received";

    public const string ITEM_STATS_HEALTH = "Health";
    public const string ITEM_STATS_MOOD = "Mood";

    public const string TASK_STATUS_COMPLETE = "Complete";
    public const string TASK_STATUS_FAIL = "Fail";

    private static bool isCreated = false;

    public const string INHALER_CATEGORY = "MiniGame:Inhaler:";
    public const string RUNNER_CATEGORY = "MiniGame:Runner:";
    public const string DIAGNOSE_CATEGORY = "MiniGame:Clinic:";
    public const string NINJA_CATEGORY = "MiniGame:Ninja:";

    private DateTime playTime;
    private bool isGameTimerOn = false;

    private static Analytics instance;

    //This instance creates itself if it's not in the scene.
    //Mainly for debugging purpose
    public static Analytics Instance{
        get{
            if(instance == null){

                instance = (Analytics) FindObjectOfType(typeof(Analytics));
                
                if(instance == null){
                    GameObject analyticsGO = (GameObject) Instantiate((GameObject) Resources.Load("Analytics"));
                    instance = analyticsGO.GetComponent<Analytics>();
                }
            }
            return instance;
        }
    }

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
        playTime = LgDateTime.GetTimeNow();
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
        TimeSpan timeSpentInGame = LgDateTime.GetTimeNow() - playTime; //minutes
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

    //What is the pet's health or mood when an item is used
    public void ItemEventWithPetStats(string itemID, string statsType, int statsValue){
        if(!String.IsNullOrEmpty(itemID) && !String.IsNullOrEmpty(statsType)){
            GA.API.Design.NewEvent("Items:" + ITEM_STATUS_USED + ":" + itemID + ":" +
                statsType, (float) statsValue);
        }
    }

    //Wellapad
    public void WellapadTaskEvent(string taskStatus, string missionID, string taskID){
        if(!String.IsNullOrEmpty(taskStatus) && !String.IsNullOrEmpty(missionID) && !String.IsNullOrEmpty(taskID))
            GA.API.Design.NewEvent(taskStatus + ":" + missionID + ":" + taskID);
    }

    //Gating
    public void GateUnlocked(string gateID){
        if(!String.IsNullOrEmpty(gateID))
            GA.API.Design.NewEvent("Gate:Unlocked:" + gateID);
    }

    //Tutorial completed
    public void TutorialCompleted(string tutorialID){
        if(!String.IsNullOrEmpty(tutorialID))
            GA.API.Design.NewEvent("Tutorial:Completed:" + tutorialID);
    }
}
