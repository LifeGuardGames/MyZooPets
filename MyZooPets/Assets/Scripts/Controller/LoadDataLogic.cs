using UnityEngine;
using System.Collections;

//This class handles the timing for loading data into UI classes
//data loading in data manager is asynchronous so this class is necessary
//to provide an event based callback
public class LoadDataLogic : MonoBehaviour {
    public bool isDebug;
    public static bool IsDataLoaded{get;set;} //has data been initialized or deserialzed
    

    //Logic
    private LevelUpLogic levelUpLogic; //reference to logic
    private DegradationLogic degradationLogic; //reference to logic
    private PetMovement petMovement; //reference
    private CameraMove cameraMove; //reference 
    private Tutorial tutorial; //reference 
    private DiagnoseTimerLogic diagnoseTimerLogic; //reference
    private ClickManager clickManager;

    //UI
    private DegradationUIManager degradationUIManager; //reference to UI
    private GameObject calendar;
    private GameObject hud;
    private GameObject navigation;

    private const string ANCHOR_TOP = "UI Root (2D)/Camera/Panel/Anchor-Top/";
    private const string ANCHOR_CENTER = "UI Root (2D)/Camera/Panel/Anchor-Center/";
    private const string ANHCHOR_BOTTOMLEFT = "UI Root (2D)/Camera/Panel/Anchor-BottomLeft/";

    void Awake(){
        if(isDebug){
            DataManager.FirstTime = true; 
        }
        IsDataLoaded = false;
        
        switch(Application.loadedLevelName){
            case "NewBedRoom":
                print("in room");
                hud = GameObject.Find(ANCHOR_TOP + "HUD");
                calendar = GameObject.Find(ANCHOR_CENTER + "Calendar");
                navigation = GameObject.Find(ANHCHOR_BOTTOMLEFT + "Navigation");
                print(hud);
                clickManager = GameObject.Find ("UIManager/ClickManager").GetComponent<ClickManager>();
                levelUpLogic = GameObject.Find("GameManager/LevelUpLogic").GetComponent<LevelUpLogic>();
                degradationLogic = GameObject.Find("GameManager/DegradationLogic").GetComponent<DegradationLogic>();
                degradationUIManager = GameObject.Find("UIManager/DegradationUIManager").GetComponent<DegradationUIManager>();
                tutorial = GameObject.Find("GameManager/Tutorial").GetComponent<Tutorial>();
                // diagnoseTimerLogic = GameObject.Find("GameManager/DiagnoseTimerLogic").GetComponent<DiagnoseTimerLogic>();
                petMovement = GameObject.Find("PetMovement").GetComponent<PetMovement>();
                cameraMove = GameObject.Find("Main Camera").GetComponent<CameraMove>();

                if(!DataManager.FirstTime){ //if not first time load GUI right away
                    FirstTimeGUI.finishCheckingForFirstTime = InitializeDataForUI;
                }else{ //if first time set call back and wait for the hatching animation to finish
                    FirstTimeGUI.finishHatchCallBack = InitializeDataForUI;
                }
            break;
            // case "Yard":
            //     animator = GameObject.Find("UIManager/HUD").GetComponent<HUDAnimator>();
            //     diaryUIManager = GameObject.Find ("UIManager/DiaryGUI").GetComponent<DiaryGUI>();
            //     cameraMove = GameObject.Find("Main Camera").GetComponent<CameraMove>();
            //     InitializeDataForUI();
            // break;
            // case "InhalerGamePet":
            //     animator = GameObject.Find("UIManager/HUD").GetComponent<HUDAnimator>();
            //     InitializeDataForUI();
            // break;
            // case "InhalerGameTeddy":
            //     animator = GameObject.Find("UIManager/HUD").GetComponent<HUDAnimator>();
            //     InitializeDataForUI();
            // break;
            // case "SlotMachineGame":
            //     animator = GameObject.Find("UIManager/HUD").GetComponent<HUDAnimator>();
            //     InitializeDataForUI();
            // break;
        }
    }

    //data is ready for use so initialize all UI data
    private void InitializeDataForUI(){
        //Note: the order in which some of the classes are init matter
        //1) animator needs to be init before levelUpLogic

        switch(Application.loadedLevelName){
            case "NewBedRoom":
                print("init");
                if(DataManager.FirstTime) DataManager.FirstTime = false; //turn first time animation off
                hud.GetComponent<HUDAnimator>().Init();
                print(hud);
                hud.GetComponent<MoveTweenToggle>().Show();
                calendar.GetComponent<CalendarUIManager>().Init();
                navigation.GetComponent<MoveTweenToggle>().Show();

                clickManager.Init();
                cameraMove.Init();
                levelUpLogic.Init();
                // degradationLogic.Init();
                // degradationUIManager.Init();
                tutorial.Init();
                petMovement.Init();
                // diagnoseTimerLogic.Init();

            break;
            // case "Yard":
            //     animator.Init();
            //     diaryUIManager.Init();
            //     cameraMove.Init();

            // break;
            // case "InhalerGamePet":
            //     animator.Init();
            // break;
            // case "InhalerGameTeddy":
            //     animator.Init();
            // break;
            // case "SlotMachineGame":
            // break;
        }
       
        IsDataLoaded = true;
    }
}
