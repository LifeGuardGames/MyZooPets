using UnityEngine;
using System.Collections;

//This class handles the timing for loading data into UI classes
//data loading in data manager is asynchronous so this class is necessary
//to provide an eventbased callback
public class LoadDataLogic : MonoBehaviour {
    public bool isDebugging;
    public static bool IsDataLoaded{get;set;}
    private DiaryUIManager diaryUIManager;
    private RoomGUIAnimator roomGUIAnimator;
    
    //Testing Code
    private InhalerTest inhalerTest;

    void Awake(){
        if(!isDebugging){
            roomGUIAnimator = GameObject.Find("UIManager/RoomGUI").GetComponent<RoomGUIAnimator>();
            diaryUIManager = GameObject.Find ("UIManager/DiaryGUI").GetComponent<DiaryUIManager>();
        }else{
            //Testing
            inhalerTest = GameObject.Find("UIManager").GetComponent<InhalerTest>();    
        }

        IsDataLoaded = false;

        //set callback for datamanager so it knows what to do when data have been
        //initialized or de serialized
        DataManager.dataLoadedCallBack = InitializeDataForUI;
    }

    //data is ready for use so initialize all UI data
    //TO DO: disable splash screen here
    private void InitializeDataForUI(){
        if(!isDebugging){
            roomGUIAnimator.Init();
            diaryUIManager.Init();    
        }else{
            //Testing
            inhalerTest.Init();    
        }
        
        IsDataLoaded = true;
    }
}
