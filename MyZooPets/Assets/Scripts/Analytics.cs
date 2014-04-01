﻿using UnityEngine;
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

    public const string INHALER_CATEGORY = "MiniGame:Inhaler:";
    public const string RUNNER_CATEGORY = "MiniGame:Runner:";
    public const string DIAGNOSE_CATEGORY = "MiniGame:Clinic:";
    public const string NINJA_CATEGORY = "MiniGame:Ninja:";

    private static bool isCreated = false;
    private static Analytics instance;
    // private DateTime playTime;
    // private bool isGameTimerOn = false;
    private bool isAnalyticsEnabled = false;

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
        //make persistent
        if(isCreated){
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        isCreated = true;

        //Get constants and check if analytics event should be sent
        isAnalyticsEnabled = Constants.GetConstant<bool>("AnalyticsEnabled");

        //start facebook sdk
        FB.Init(OnInitComplete);
    }

    private void OnInitComplete(){
        //tell facebook this is the first install.
        //i think facebook sdk only sends the first ever publish install so we
        //don't have to do the first install check ourself
        FB.PublishInstall(PublishComplete);
    }

    private void PublishComplete(FBResult result){
        Debug.Log("publish response: " + result.Text);
    }

    //=========================Runner Game======================================
    //Where did the user die most often in the runner game?
    public void RunnerPlayerDied(string levelComponentName){
        if(!String.IsNullOrEmpty(levelComponentName) && isAnalyticsEnabled)
            GA.API.Design.NewEvent(RUNNER_CATEGORY + "Dead:" + levelComponentName);
    }

    //Which triggers does the user have difficulty recognizing as bad triggers?
    //Number of times crashed into trigger. 
    public void RunnerPlayerCrashIntoTrigger(string triggerName){
        if(!String.IsNullOrEmpty(triggerName) && isAnalyticsEnabled)
            GA.API.Design.NewEvent(RUNNER_CATEGORY + "TriggerCrashed:" + triggerName);
    }

    public void RunnerPlayerItemPickUp(string itemName){
        if(!String.IsNullOrEmpty(itemName) && isAnalyticsEnabled)
            GA.API.Design.NewEvent(RUNNER_CATEGORY + "ItemPickedUp:" + itemName);
    }

    public void RunnerPlayerDistanceRan(int distanceRan){
        if(isAnalyticsEnabled)
            GA.API.Design.NewEvent(RUNNER_CATEGORY + "DistanceRan", (float) distanceRan);
    }

    //==========================Inhaler Game====================================
    //Which steps in inhaler game does the kids need help the most
    public void InhalerHintRequired(int stepID){
        if(isAnalyticsEnabled)
            GA.API.Design.NewEvent(INHALER_CATEGORY + "HintRequired:" + stepID);
    }

    //Number of missed vs correct inhaler swipe sequences
    public void InhalerSwipeSequences(string stepStatus, int stepID){
        if(!String.IsNullOrEmpty(stepStatus) && isAnalyticsEnabled)
            GA.API.Design.NewEvent(INHALER_CATEGORY + "Inhaler:" + stepStatus + ":" + stepID);
    }

    //==========================Diagnose track game=============================
    //Number of correct diagnose. 
    //Which symptom is the user having trouble identifying
    public void DiagnoseResult(string diagnoseResult, AsthmaStage petStatus, AsthmaStage zone){
        if(!String.IsNullOrEmpty(diagnoseResult) && isAnalyticsEnabled)
            GA.API.Design.NewEvent(DIAGNOSE_CATEGORY + "Diagnose:" + diagnoseResult + ":" + 
                Enum.GetName(typeof(AsthmaStage), petStatus) + ":" + Enum.GetName(typeof(AsthmaStage), zone));
    }

    //=======================General Analytics==================================
    //Will be use in different mini games
    public void PetColorChosen(string petColor){
        if(!String.IsNullOrEmpty(petColor) && isAnalyticsEnabled)
            GA.API.Design.NewEvent("PetColorChosen:" + petColor);
    }

    // //record when a user changes to another scene. Can be used to track how many
    // //times user plays mini game 
    // public void ChangeSceneButtonClicked(string miniGameCatName){
    //     if(!String.IsNullOrEmpty(miniGameCatName))
    //         GA.API.Design.NewEvent(miniGameCatName + "StartGame");
    // }

    //when the user clean the triggers
    public void TriggersCleaned(String triggerID){
        if(!String.IsNullOrEmpty(triggerID))
            GA.API.Design.NewEvent("TriggersCleaned:" + triggerID);
    }

    //Badges unlock
    public void BadgeUnlocked(string badgeID){
        if(!String.IsNullOrEmpty(badgeID) && isAnalyticsEnabled)
            GA.API.Design.NewEvent("Badge:Unlocked:" + badgeID);
    }

    //Items used or purchased
    public void ItemEvent(string itemStatus, ItemType itemType, string itemID){
        if(!String.IsNullOrEmpty(itemStatus) && !String.IsNullOrEmpty(itemID) && isAnalyticsEnabled){
            GA.API.Design.NewEvent("Items:" + itemStatus + ":" + 
                Enum.GetName(typeof(ItemType), itemType) + ":" + itemID);
        }
    }

    //What is the pet's health or mood when an item is used
    public void ItemEventWithPetStats(string itemID, string statsType, int statsValue){
        if(!String.IsNullOrEmpty(itemID) && !String.IsNullOrEmpty(statsType) && isAnalyticsEnabled){
            GA.API.Design.NewEvent("Items:" + ITEM_STATUS_USED + ":" + itemID + ":" +
                statsType, (float) statsValue);
        }
    }

    //Wellapad
    public void WellapadTaskEvent(string taskStatus, string missionID, string taskID){
        if(!String.IsNullOrEmpty(taskStatus) && !String.IsNullOrEmpty(missionID) && 
            !String.IsNullOrEmpty(taskID) && isAnalyticsEnabled)
            GA.API.Design.NewEvent("Wellapad:Task:" + taskStatus + ":" + missionID + ":" + taskID);
    }

    //Wellapad xp reward claim
    public void ClaimWellapadBonusXP(){
        if(isAnalyticsEnabled)
            GA.API.Design.NewEvent("Wellapad:Collect:BonusXP");
    }

    //Gating
    public void GateUnlocked(string gateID){
        if(!String.IsNullOrEmpty(gateID) && isAnalyticsEnabled)
            GA.API.Design.NewEvent("Gate:Unlocked:" + gateID);
    }

    //Tutorial completed
    public void TutorialCompleted(string tutorialID){
        if(!String.IsNullOrEmpty(tutorialID) && isAnalyticsEnabled)
            GA.API.Design.NewEvent("Tutorial:Completed:" + tutorialID);
    }

    //Flame unlocked
    public void FlameUnlocked(string flameID){
        if(!String.IsNullOrEmpty(flameID) && isAnalyticsEnabled)
            GA.API.Design.NewEvent("Flame:Unlocked:" + flameID);
    }

    //Pet level up
    public void LevelUnlocked(Level levelID){
        if(isAnalyticsEnabled)
            GA.API.Design.NewEvent("PetLevel:Unlocked:" + levelID.ToString());
    }

    public void ZeroHealth(){
        if(isAnalyticsEnabled)
            GA.API.Design.NewEvent("ZeroHealth");
    }

    public void TriggerHitPet(){
        if(isAnalyticsEnabled)
            GA.API.Design.NewEvent("TriggerHitPet");
    }


}
