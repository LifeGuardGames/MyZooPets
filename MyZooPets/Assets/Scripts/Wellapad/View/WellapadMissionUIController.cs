using UnityEngine;
using System;
using System.Collections.Generic;

//---------------------------------------------------
// Wellapad_MissionList
// This script controls the UI for the mission list
// that appears on the Wellapad.
//---------------------------------------------------

public class WellapadMissionUIController : MonoBehaviour {
	// the NGUI grid that the missions are put in to
	public GameObject goGrid;
	public GameObject prefabTask;		// mission task (dynamic amount in each mission)
	
	// the number of elements in the mission list...this is used for helping to sort elements in the grid
	private int nCount = 0;

	//---------------------------------------------------
	// Start()
	//---------------------------------------------------		
	void Start() {		
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
		List<string> currentMissions = WellapadMissionController.Instance.GetCurrentTasks();
		// create our missions
		if(currentMissions.Count > 0)
			foreach (string strMission in currentMissions)
				CreateMission(strMission);
	}
	
	//---------------------------------------------------
	// CreateMission()
	// Given a mission type, this function will add
	// entries to the Wellapad for the mission; the mission
	// reward, and tasks.
	//---------------------------------------------------	
	private void CreateMission(string missionType){	
		// find the available tasks for the mission and add them
		MutableDataWellapadTask task = WellapadMissionController.Instance.GetTask(missionType);
	
		GameObject goTask = GameObjectUtils.AddChild(goGrid, prefabTask);
		SetNameForGrid( goTask );
			
		// init this task UI with the task itself
		goTask.GetComponent<WellapadTaskUI>().Init(task);
	}
	
	//---------------------------------------------------
	// GetMissionTitle()
	// Returns the string text of the mission title for
	// the incoming mission type.
	//---------------------------------------------------		
	private string GetMissionTitle( string missionType ) {
		// build the key from the mission type
		string strKey = "MissionTitle_" + missionType;
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
		// StartCoroutine( DisplayMissions() );
		DisplayMissions();
	}
	
	//---------------------------------------------------
	// DisplayMissions()
	//---------------------------------------------------		
	public void DisplayMissions() {
		// reset the count for our grid labeling
		nCount = 0;

		// destroy all children in the grid
		foreach (Transform child in goGrid.transform) {
			child.gameObject.SetActive(false);
			Destroy(child.gameObject);
		}

		// and create the missions anew
		CreateMissions();
	
	//	StartCoroutine(GridRepo());
	}

//	private IEnumerator GridRepo(){
//		yield return 0; 
//		goGrid.GetComponent<UIGrid>().Reposition();
	//}
}
