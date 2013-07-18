using UnityEngine;
using System.Collections;

//This class handles the timing for loading data into UI classes
//data loading in data manager is asynchronous so this class is necessary
//to provide an event based callback
public class LoadDataLogic : MonoBehaviour {
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
    private GameObject inventory;

    private const string ANCHOR_TOP = "UI Root (2D)/Camera/Panel/Anchor-Top/";
    private const string ANCHOR_CENTER = "UI Root (2D)/Camera/Panel/Anchor-Center/";
    private const string ANCHOR_BOTTOMLEFT = "UI Root (2D)/Camera/Panel/Anchor-BottomLeft/";
    private const string ANCHOR_BOTTOMRIGHT = "UI Root (2D)/Camera/Panel/Anchor-BottomRight/";

    void Awake(){
        switch(Application.loadedLevelName){
            case "NewBedRoom":
                hud = GameObject.Find(ANCHOR_TOP + "HUD");
                calendar = GameObject.Find(ANCHOR_CENTER + "Calendar");
                navigation = GameObject.Find(ANCHOR_BOTTOMLEFT + "Navigation");
                inventory = GameObject.Find(ANCHOR_BOTTOMRIGHT + "Inventory");

                clickManager = GameObject.Find ("UIManager/ClickManager").GetComponent<ClickManager>();
                levelUpLogic = GameObject.Find("GameManager/LevelUpLogic").GetComponent<LevelUpLogic>();
                degradationLogic = GameObject.Find("GameManager/DegradationLogic").GetComponent<DegradationLogic>();
                degradationUIManager = GameObject.Find("UIManager/DegradationUIManager").GetComponent<DegradationUIManager>();
                tutorial = GameObject.Find("GameManager/Tutorial").GetComponent<Tutorial>();
                // diagnoseTimerLogic = GameObject.Find("GameManager/DiagnoseTimerLogic").GetComponent<DiagnoseTimerLogic>();
                petMovement = GameObject.Find("PetMovement").GetComponent<PetMovement>();
                cameraMove = GameObject.Find("Main Camera").GetComponent<CameraMove>();

            break;
            case "Yard":
                hud = GameObject.Find(ANCHOR_TOP + "HUD");
                navigation = GameObject.Find(ANCHOR_BOTTOMLEFT + "Navigation");
                inventory = GameObject.Find(ANCHOR_BOTTOMRIGHT + "Inventory");

                clickManager = GameObject.Find ("UIManager/ClickManager").GetComponent<ClickManager>();
                cameraMove = GameObject.Find("Main Camera").GetComponent<CameraMove>();
            break;
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

    void Start(){
        Invoke("InitializeDataForUI", 0.5f);
    }

    //Data is ready for use so initialize all UI data
    private void InitializeDataForUI(){
        switch(Application.loadedLevelName){
            case "NewBedRoom":
                hud.GetComponent<HUDAnimator>().Init();
                hud.GetComponent<MoveTweenToggle>().Show();
                calendar.GetComponent<CalendarUIManager>().Init();
                navigation.GetComponent<MoveTweenToggle>().Show();
                inventory.GetComponent<MoveTweenToggle>().Show();

                clickManager.Init();
                cameraMove.Init();
                levelUpLogic.Init();
                degradationLogic.Init();
                degradationUIManager.Init();
                tutorial.Init();
                petMovement.Init();
                // diagnoseTimerLogic.Init();

            break;
            case "Yard":
                hud.GetComponent<HUDAnimator>().Init();
                hud.GetComponent<MoveTweenToggle>().Show();
                navigation.GetComponent<MoveTweenToggle>().Show();
                inventory.GetComponent<MoveTweenToggle>().Show();

                clickManager.Init();
                cameraMove.Init();
            break;
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
