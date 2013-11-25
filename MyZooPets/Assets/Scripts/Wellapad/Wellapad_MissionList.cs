using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// Wellapad_MissionList
// This script controls the UI for the mission list
// that appears on the Wellapad.
//---------------------------------------------------

public class Wellapad_MissionList : MonoBehaviour {
	// the NGUI grid that the missions are put in to
	public GameObject goGrid;
	
	// prefabs for individual parts of a mission
	public GameObject prefabTitle;		// mission title
	public GameObject prefabTask;		// mission task (dynamic amount in each mission)
	public GameObject prefabReward;		// reward for completing all tasks
	
	// the number of elements in the mission list...this is used for helping to sort elements in the grid
	private int nCount = 0;
	
	//---------------------------------------------------
	// Start()
	//---------------------------------------------------		
	void Start() {
		// set up immutable task data
		DataLoader_WellapadTasks.SetupData();
		
		// create missions UI
		CreateMissions();
	}
	
	//---------------------------------------------------
	// SetNameForGrid()
	// Because it was apparently too hard for the NGUI
	// guy to just let elements in a grid appear in the
	// order they are added, we must actually rename the
	// objects in the grid numerically as they are added,
	// so they are sorted properly.
	//---------------------------------------------------		
	private void SetNameForGrid( GameObject go ) {
		go.name = "" + nCount;
		nCount += 1;
	}
	
	//---------------------------------------------------
	// OnApplicationPause()
	// Unity callback function.
	//---------------------------------------------------		
	void OnApplicationPause( bool bPaused ) {
		if ( !bPaused ) {
			// if the game is unpausing, we need to do a check to refresh the mission list	
			WellapadMissionController.Instance.RefreshCheck();
		}
	}
	
	//---------------------------------------------------
	// CreateMissions()
	// Creates the mission entries for the Wellapad.
	//---------------------------------------------------	
	private void CreateMissions() {
		// prior to creating our missions, see if we need to wipe the slate clean
		WellapadMissionController.Instance.RefreshCheck();
		
		// create our missions -- right now we just have critical and side
		CreateMission( "Critical" );
		CreateMission( "Side" );
	}
	
	//---------------------------------------------------
	// CreateMission()
	// Given a mission type, this function will add
	// entries to the Wellapad for the mission; the mission
	// title, tasks, and reward.
	//---------------------------------------------------	
	private void CreateMission( string strMissionType ) {		
		// add a title for the mission
		GameObject title = NGUITools.AddChild(goGrid, prefabTitle);
		SetNameForGrid( title );
		string strMissionTitle = GetMissionTitle( strMissionType );
		title.transform.FindChild("Title").GetComponent<UILabel>().text = strMissionTitle;			
		
		// find the available tasks for the mission and add them
		List<Data_WellapadTask> listTasks = WellapadMissionController.Instance.GetTasks( strMissionType );
		for ( int i = 0; i < listTasks.Count; i++ ) {
			Data_WellapadTask dataTask = listTasks[i];
			
			GameObject goTask = NGUITools.AddChild(goGrid, prefabTask);
			SetNameForGrid( goTask );
			
			// init this task UI with the task itself
			goTask.GetComponent<WellapadTaskUI>().Init( dataTask );
		}
		
		// add the reward for completing the mission
		GameObject goReward = NGUITools.AddChild(goGrid, prefabReward);
		SetNameForGrid( goReward );
		
		// alter the Z so that it stays above the UI
		//Vector3 vPos = goReward.transform.localPosition;
		//vPos.z = prefabReward.transform.localPosition.z;
		//goReward.transform.position = vPos;
		
		// init the reward UI
		goReward.GetComponent<WellapadRewardUI>().Init( strMissionType );			
	}
	
	//---------------------------------------------------
	// GetMissionTitle()
	// Returns the string text of the mission title for
	// the incoming mission type.
	//---------------------------------------------------		
	private string GetMissionTitle( string strMissionType ) {
		// build the key from the mission type
		string strKey = "MissionTitle_" + strMissionType;
		string strText = Localization.Localize( strKey );
		return strText;
	}
}
