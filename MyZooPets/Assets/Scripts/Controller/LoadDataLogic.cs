using UnityEngine;
using System.Collections;

//This class handles the timing for loading data into UI classes
//data loading in data manager is asynchronous so this class is necessary
//to provide an eventbased callback
public class LoadDateLogic : MonoBehaviour {
    public static bool IsDataLoaded{get;set;}
    private GameObject UIManager;
    private DiaryUIManager diaryUIManager;
    private RoomGUIAnimator roomGUIAnimator;

    void Awake(){
        GameObject.Find("UIManager");
        IsDataLoaded = false;
        DataManager.dataLoadedCallBack = InitializeDataForUI;
    }

    private void InitializeDataForUI(){

        IsDataLoaded = true;
    }
}
