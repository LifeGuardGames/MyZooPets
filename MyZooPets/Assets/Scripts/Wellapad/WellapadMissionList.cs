using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// Wellapad_MissionList
// This script controls the UI for the mission list
// that appears on the Wellapad.
//---------------------------------------------------

public class WellapadMissionList : MonoBehaviour {
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
		// before doing anything check to see if we need to refresh our tasks
		// WellapadMissionController.Instance.RefreshCheck();
		
		WellapadMissionController.Instance.OnMissionsRefreshed += OnMissionsRefreshed; 
		// create missions UI
		StartCoroutine( DisplayMissions() );
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
		go.name = nCount + "_WellapadTask";
		nCount += 1;
	}
	
	//---------------------------------------------------
	// CreateMissions()
	// Creates the mission entries for the Wellapad.
	//---------------------------------------------------	
	private void CreateMissions() {		
		List<string> listCurrentMissions = WellapadMissionController.Instance.GetCurrentMissions();
		
		// create our missions
		foreach ( string strMission in listCurrentMissions )
			CreateMission( strMission );
		
		goGrid.GetComponent<UIGrid>().Reposition();
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
		
		// add the reward for completing the mission
		GameObject goReward = NGUITools.AddChild(goGrid, prefabReward);
		SetNameForGrid( goReward );
		
		// find the available tasks for the mission and add them
		List<WellapadTask> listTasks = WellapadMissionController.Instance.GetTasks( strMissionType );
		for ( int i = 0; i < listTasks.Count; i++ ) {
			WellapadTask task = listTasks[i];
			
			GameObject goTask = NGUITools.AddChild(goGrid, prefabTask);
			SetNameForGrid( goTask );
			
			// init this task UI with the task itself
			goTask.GetComponent<WellapadTaskUI>().Init( task );
		}
		
		
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
	
	//---------------------------------------------------
	// OnMissionsRefreshed()
	// When the user's current missions expire and must
	// be refreshed, this function will take care of the
	// UI behind it.
	//---------------------------------------------------		
	private void OnMissionsRefreshed( object sender, EventArgs args ) {
		StartCoroutine( DisplayMissions() );
	}
	
	//---------------------------------------------------
	// DisplayMissions()
	//---------------------------------------------------		
	private IEnumerator DisplayMissions() {
		// reset the count for our grid labeling
		nCount = 0;
		
		// destroy all children in the grid
		foreach (Transform child in goGrid.transform) {
			Destroy( child.gameObject );
		}
		
		// wait a frame so the objects actually get destroyed
		yield return 0;
		
		// and create the missions anew
		CreateMissions();
		
		// reposition the elements since we just added a bunch
		goGrid.GetComponent<UIGrid>().Reposition();
	}
}
