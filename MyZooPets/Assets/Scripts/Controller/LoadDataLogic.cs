using UnityEngine;
using System.Collections;

//This class handles the timing for loading data into UI classes
//data loading in data manager is asynchronous so this class is necessary
//to provide an event based callback
public class LoadDataLogic : MonoBehaviour {
    public bool isDebugging;
    public static bool IsDataLoaded{get;set;}
    private DiaryUIManager diaryUIManager;
    private RoomGUIAnimator roomGUIAnimator;
    private EvolutionLogic evolutionLogic;
    
    //Testing Code
    private InhalerTest inhalerTest;

    void Awake(){
        if(!isDebugging){
            roomGUIAnimator = GameObject.Find("UIManager/RoomGUI").GetComponent<RoomGUIAnimator>();
            diaryUIManager = GameObject.Find ("UIManager/DiaryGUI").GetComponent<DiaryUIManager>();
            evolutionLogic = GameObject.Find("GameManager").GetComponent<EvolutionLogic>();
        }else{
            //Testing
            inhalerTest = GameObject.Find("UIManager").GetComponent<InhalerTest>();    
        }

        IsDataLoaded = false;

        //set callback for datamanager so it knows what to do when data have been
        //initialized or de serialized
        DataManager.dataLoadedCallBack = InitializeDataForUI;

        //set callback for pet hatching so it knows what to do when the pet has been 
        //hatched
        FirstTimeGUI.finishHatchCallBack = InitializeDataForUI;
    }

    //data is ready for use so initialize all UI data
    //True: dont init yet need to wait for pet to hatch, False: init
    private void InitializeDataForUI(bool firstTime){
        
        if(!isDebugging){
            if(!firstTime){ //init UI only if pet is hatched
                roomGUIAnimator.Init();
                diaryUIManager.Init();    
                evolutionLogic.Init();
                IsDataLoaded = true;
            }
        }else{
            //Testing
            inhalerTest.Init();    
        }
        
        
    }
}
