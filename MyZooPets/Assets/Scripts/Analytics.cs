using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

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

    private const string MAT_ADVERTISER_ID = "17900";
    private const string MAT_CONVERSION_KEY = "757d356bf92419b36822e14a62ebedee";

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

    //---------------------- important methods from MAT SDK --------------------
    #if UNITY_ANDROID
        // Pass the name of the plugin's dynamic library.
        // Import any functions we will be using from the MAT lib.
        // (I've listed them all here)
        [DllImport ("mobileapptracker")]
        private static extern void initNativeCode(string advertiserId, string conversionKey);

        [DllImport ("mobileapptracker")]
        private static extern void setExistingUser(bool isExisting);

        // Tracking functions
        [DllImport ("mobileapptracker")]
        private static extern int measureSession();
    #endif

     #if UNITY_IPHONE
        // Main initializer method for MAT
        [DllImport ("__Internal")]
        private static extern void initNativeCode(string advertiserId, string conversionKey);

        // Methods for setting Apple related id
        [DllImport ("__Internal")]
        private static extern void setAppleAdvertisingIdentifier(string appleAdvertisingIdentifier, bool trackingEnabled);

        [DllImport ("__Internal")] 
        private static extern void setExistingUser(bool isExisting);

        // Methods to track install, update events
        [DllImport ("__Internal")]
        private static extern void measureSession();
    #endif
    //--------------------------------------------------------------------------

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

        #if UNITY_ANDROID || UNITY_IPHONE
            //start facebook sdk
            FB.Init(OnInitComplete);

            //start MAT sdk
            initNativeCode(MAT_ADVERTISER_ID, MAT_CONVERSION_KEY);

            #if UNITY_IPHONE
                setAppleAdvertisingIdentifier(iPhone.advertisingIdentifier, iPhone.advertisingTrackingEnabled);
            #endif

            // For existing users prior to MAT SDK implementation, call setExistingUser(true) before measureSession.
            // Otherwise, existing users will be counted as new installs the first time they run your app.
            if(!DataManager.Instance.IsFirstTime)
                setExistingUser(true);

            measureSession();
        #endif
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
    public void ChangeScene(string newSceneName){
        if(!String.IsNullOrEmpty(newSceneName))
            GA.API.Design.NewEvent("SceneChanged:" + newSceneName);
    }

    //track which button is most clicked by users
    public void LgButtonClicked(string buttonName){
        if(!String.IsNullOrEmpty(buttonName))
            GA.API.Design.NewEvent("ButtonClicked:" + buttonName);
    }

    //when the user clean the triggers
    public void TriggersCleaned(String triggerID){
        if(!String.IsNullOrEmpty(triggerID) && isAnalyticsEnabled)
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

    //store short cup clicked. (from pet thought bubble)
    public void StoreItemShortCutClicked(){
        if(isAnalyticsEnabled)
            GA.API.Design.NewEvent("Button:StoreItemShortCut");
    }

}
