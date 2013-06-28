using UnityEngine;
using System.Collections;

//This class handles the timing for loading data into UI classes
//data loading in data manager is asynchronous so this class is necessary
//to provide an event based callback
public class LoadDataLogic : MonoBehaviour {
    public static bool IsDataLoaded{get;set;} //has data been initialized or deserialzed
    private DiaryGUI diaryUIManager; //reference to UI
    private CalendarGUI calendarGUI; //reference to UI
    private ChallengesGUI challengesGUI; //reference to UI
    private DegradationGUI degradationUIManager; //reference to UI
    private RoomGUIAnimator roomGUIAnimator; //reference to UI
    private EvolutionLogic evolutionLogic; //reference to logic
    private DegradationLogic degradationLogic; //reference to logic
    private PetMovement petMovement; //reference to ...logic?
    private CameraMove cameraMove; //reference to ...logic?
    private Tutorial tutorial; //reference to... logic?
    private ClickManager clickmanager;

    void Awake(){
        IsDataLoaded = false;
        roomGUIAnimator = GameObject.Find("UIManager/RoomGUI").GetComponent<RoomGUIAnimator>();
        
        //different things to load for different scenes
        if(Application.loadedLevelName == "NewBedRoom"){
            diaryUIManager = GameObject.Find ("UIManager/DiaryGUI").GetComponent<DiaryGUI>();
            calendarGUI = GameObject.Find ("UIManager/CalendarGUI").GetComponent<CalendarGUI>();
            challengesGUI = GameObject.Find ("UIManager/ChallengesGUI").GetComponent<ChallengesGUI>();
            degradationUIManager = GameObject.Find("UIManager/DegradationGUI").GetComponent<DegradationGUI>();
            evolutionLogic = GameObject.Find("GameManager").GetComponent<EvolutionLogic>();
            degradationLogic = GameObject.Find("GameManager").GetComponent<DegradationLogic>();
            tutorial = GameObject.Find("GameManager").GetComponent<Tutorial>();
            petMovement = GameObject.Find("PetMovement").GetComponent<PetMovement>();
            cameraMove = GameObject.Find("Main Camera").GetComponent<CameraMove>();
            clickmanager = GameObject.Find ("UIManager").GetComponent<ClickManager>();

            if(!DataManager.FirstTime){ //if not first time load GUI right away
                InitializeDataForUI();
            }else{ //if first time set call back and wait for the hatching animation to finish
                FirstTimeGUI.finishHatchCallBack = InitializeDataForUI;
            }
        }else if(Application.loadedLevelName == "Yard"){
            InitializeDataForUI();
        }
    }

    //data is ready for use so initialize all UI data
    private void InitializeDataForUI(){
        roomGUIAnimator.Init();
        if(Application.loadedLevelName == "NewBedRoom"){
            if(DataManager.FirstTime) DataManager.FirstTime = false; //turn first time animation off
            diaryUIManager.Init();
            calendarGUI.Init();
            challengesGUI.Init();
            evolutionLogic.Init();
            degradationLogic.Init();
            degradationUIManager.Init();
            tutorial.Init();
            petMovement.Init();
            cameraMove.Init();
            clickmanager.Init();
        }else if(Application.loadedLevelName == "Yard"){

        }
        IsDataLoaded = true;
		
		//hatching gives 500 stars
        //DataManager.AddStars(500);
    }
}
