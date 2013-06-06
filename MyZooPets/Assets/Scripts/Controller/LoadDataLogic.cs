using UnityEngine;
using System.Collections;

//This class handles the timing for loading data into UI classes
//data loading in data manager is asynchronous so this class is necessary
//to provide an eventbased callback
public class LoadDataLogic : MonoBehaviour {
    public static bool IsDataLoaded{get;set;}
    private DiaryUIManager diaryUIManager;
    private RoomGUIAnimator roomGUIAnimator;
    private EvolutionLogic evolutionLogic;

    void Awake(){
        roomGUIAnimator = GameObject.Find("UIManager/RoomGUI").GetComponent<RoomGUIAnimator>();
        diaryUIManager = GameObject.Find ("UIManager/DiaryGUI").GetComponent<DiaryUIManager>();
        evolutionLogic = GameObject.Find("GameManager").GetComponent<EvolutionLogic>();
		IsDataLoaded = false;
        DataManager.dataLoadedCallBack = InitializeDataForUI;
    }

    private void InitializeDataForUI(){
		roomGUIAnimator.Init();
		diaryUIManager.Init ();
        evolutionLogic.Init();
        IsDataLoaded = true;
    }
}
