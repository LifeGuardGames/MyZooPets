using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// WellapadScreenUIManager
// The wellapad is an electronic device with many
// screens.  This script decides which screens to
// show and hide.
//---------------------------------------------------

public class WellapadScreenUIController : MonoBehaviour {
	// screens of the wellapad (as game objects)
	public GameObject goWellapadScreen;
	public GameObject goWellapadBack; // the back button -- exposed for tutorials

	private string missionDonePrefabName;
	private string missionListPrefabName = "MissionList";
	private GameObject missionListGO;
	private GameObject missionDoneGO;

	public GameObject GetBackButton() {
		return goWellapadBack;	
	}

	void Awake(){
		missionDonePrefabName = VersionManager.IsLite() ? "MissionDoneLite" : "MissionDonePro";
	}	
	//---------------------------------------------------
	// Start()
	//---------------------------------------------------	
	void Start() {
		// listen for reward claimed callback
		WellapadMissionController.Instance.OnRewardClaimed += OnRewardClaimed;		
	}

	//---------------------------------------------------
	// SetScreen()
	// Called when the wellapad is opening.  The wellapad
	// is an electronic device with numerous screens --
	// this function (for now) will set the proper screen.
	//---------------------------------------------------		
	public void SetScreen() {
		// for now, just check to see if the player has any outstanding missions.
		bool hasActiveTasks = WellapadMissionController.Instance.HasActiveTasks();

		foreach(Transform child in goWellapadScreen.transform){
			child.gameObject.SetActive(false);
			Destroy(child.gameObject);
		}
		
		if(hasActiveTasks){
			// user has active tasks/missions, so show the task list
			if(missionListGO == null){
				GameObject missionListPrefab = (GameObject) Resources.Load(missionListPrefabName);
				missionListGO = LgNGUITools.AddChildWithPosition(goWellapadScreen, missionListPrefab);
			}

			missionListGO.GetComponent<WellapadMissionUIController>().DisplayMissions();
		}else{
			// otherwise, show the "come back later" screen
			if(missionDoneGO == null){
				GameObject missionDonePrefab = (GameObject) Resources.Load(missionDonePrefabName);
				missionDoneGO = LgNGUITools.AddChildWithPosition(goWellapadScreen, missionDonePrefab);
			}
		}
	}

	//---------------------------------------------------
	// OnRewardClaimed()
	// Callback for when the user claims a wellapad reward.
	//---------------------------------------------------	
	private void OnRewardClaimed( object sender, EventArgs args ) {
		StartCoroutine( SetScreenDelay() );
	}	

	//---------------------------------------------------
	// SetScreenDelay()
	// In order to make the transition from the missions
	// to the done screen more appealing, I creating this
	// function to kick off the set screen on a delay.
	//---------------------------------------------------		
	private IEnumerator SetScreenDelay() {
		float fDelay = Constants.GetConstant<float>("Wellapad_DoneDelay");
		yield return new WaitForSeconds(fDelay);
		
		SetScreen();
	}
}
