using UnityEngine;
using System.Collections;

//This class handles the timing for loading data into UI classes
//data loading in data manager is asynchronous so this class is necessary
//to provide an event based callback
public class LoadDataLogic : MonoBehaviour {
    public static bool IsDataLoaded{get;set;} //has data been initialized or deserialzed
    private DiaryGUI diaryUIManager; //reference to UI
    private RoomGUIAnimator roomGUIAnimator; //reference to UI
    private EvolutionLogic evolutionLogic; //reference to logic

    void Awake(){
        roomGUIAnimator = GameObject.Find("UIManager/RoomGUI").GetComponent<RoomGUIAnimator>();
        diaryUIManager = GameObject.Find ("UIManager/DiaryGUI").GetComponent<DiaryGUI>();
        evolutionLogic = GameObject.Find("GameManager").GetComponent<EvolutionLogic>();

        IsDataLoaded = false;

        if(!DataManager.FirstTime){ //if not first time load GUI right away
            InitializeDataForUI();
        }else{ //if first time set call back and wait for the hatching animation to finish
            FirstTimeGUI.finishHatchCallBack = InitializeDataForUI;    
        }
    }

    //data is ready for use so initialize all UI data
    //True: dont init yet need to wait for pet to hatch, False: init
    private void InitializeDataForUI(){
        if(DataManager.FirstTime) DataManager.FirstTime = false; //turn first time animation off
        roomGUIAnimator.Init();
        diaryUIManager.Init();    
        evolutionLogic.Init();
        IsDataLoaded = true;
    }
}
