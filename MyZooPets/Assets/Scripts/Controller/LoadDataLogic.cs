using UnityEngine;
using System.Collections;

//This class handles the timing for loading data into UI classes
//data loading in data manager is asynchronous so this class is necessary
//to provide an event based callback
public class LoadDataLogic : MonoBehaviour {
    public bool isDebug;
    public static bool IsDataLoaded{get;set;} //has data been initialized or deserialzed
    // private DiaryGUI diaryUIManager; //reference to UI
    // private CalendarGUI calendarGUI; //reference to UI
    // private ChallengesGUI challengesGUI; //reference to UI
    // private DegradationUIManager degradationUIManager; //reference to UI
    // private HUDAnimator animator; //reference to UI
    // private LevelUpLogic levelUpLogic; //reference to logic
    // private DegradationLogic degradationLogic; //reference to logic
    // private PetMovement petMovement; //reference
    // private CameraMove cameraMove; //reference 
    // private Tutorial tutorial; //reference 
    // private DiagnoseTimerLogic diagnoseTimerLogic; //reference
    // private ClickManager clickmanager;
    // private HUD hud;

    // void Awake(){
    //     if(isDebug){
    //         DataManager.FirstTime = true; 
    //     }
    //     IsDataLoaded = false;
        
    //     switch(Application.loadedLevelName){
    //         case "NewBedRoom":
    //             calendarGUI = GameObject.Find ("UIManager/CalendarGUI").GetComponent<CalendarGUI>();
    //             challengesGUI = GameObject.Find ("UIManager/ChallengesGUI").GetComponent<ChallengesGUI>();
    //             degradationUIManager = GameObject.Find("UIManager/DegradationUIManager").GetComponent<DegradationUIManager>();
    //             levelUpLogic = GameObject.Find("GameManager/LevelUpLogic").GetComponent<LevelUpLogic>();
    //             degradationLogic = GameObject.Find("GameManager/DegradationLogic").GetComponent<DegradationLogic>();
    //             tutorial = GameObject.Find("GameManager/Tutorial").GetComponent<Tutorial>();
    //             diagnoseTimerLogic = GameObject.Find("GameManager/DiagnoseTimerLogic").GetComponent<DiagnoseTimerLogic>();
    //             petMovement = GameObject.Find("PetMovement").GetComponent<PetMovement>();
    //             clickmanager = GameObject.Find ("UIManager").GetComponent<ClickManager>();
    //             animator = GameObject.Find("UIManager/HUD").GetComponent<HUDAnimator>();
    //             diaryUIManager = GameObject.Find ("UIManager/DiaryGUI").GetComponent<DiaryGUI>();
    //             cameraMove = GameObject.Find("Main Camera").GetComponent<CameraMove>();
    //             if(!DataManager.FirstTime){ //if not first time load GUI right away
    //                 FirstTimeGUI.finishCheckingForFirstTime = InitializeDataForUI;
    //             }else{ //if first time set call back and wait for the hatching animation to finish
    //                 FirstTimeGUI.finishHatchCallBack = InitializeDataForUI;
    //             }
    //         break;
    //         case "Yard":
    //             animator = GameObject.Find("UIManager/HUD").GetComponent<HUDAnimator>();
    //             diaryUIManager = GameObject.Find ("UIManager/DiaryGUI").GetComponent<DiaryGUI>();
    //             cameraMove = GameObject.Find("Main Camera").GetComponent<CameraMove>();
    //             InitializeDataForUI();
    //         break;
    //         case "InhalerGamePet":
    //             animator = GameObject.Find("UIManager/HUD").GetComponent<HUDAnimator>();
    //             InitializeDataForUI();
    //         break;
    //         case "InhalerGameTeddy":
    //             animator = GameObject.Find("UIManager/HUD").GetComponent<HUDAnimator>();
    //             InitializeDataForUI();
    //         break;
    //         case "SlotMachineGame":
    //             animator = GameObject.Find("UIManager/HUD").GetComponent<HUDAnimator>();
    //             InitializeDataForUI();
    //         break;
    //     }
    // }

    // //data is ready for use so initialize all UI data
    // private void InitializeDataForUI(){
    //     //Note: the order in which some of the classes are init matter
    //     //1) animator needs to be init before levelUpLogic

    //     switch(Application.loadedLevelName){
    //         case "NewBedRoom":
    //             if(DataManager.FirstTime) DataManager.FirstTime = false; //turn first time animation off
    //             animator.Init();
    //             diaryUIManager.Init();
    //             cameraMove.Init();
    //             calendarGUI.Init();
    //             challengesGUI.Init();
    //             levelUpLogic.Init();
    //             degradationLogic.Init();
    //             degradationUIManager.Init();
    //             tutorial.Init();
    //             petMovement.Init();
    //             clickmanager.Init();
    //             diagnoseTimerLogic.Init();

    //         break;
    //         case "Yard":
    //             animator.Init();
    //             diaryUIManager.Init();
    //             cameraMove.Init();

    //         break;
    //         case "InhalerGamePet":
    //             animator.Init();
    //         break;
    //         case "InhalerGameTeddy":
    //             animator.Init();
    //         break;
    //         case "SlotMachineGame":
    //         break;
    //     }
       
    //     IsDataLoaded = true;
    // }
}
