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
	public GameObject goMissionsList;
	public GameObject goNoMissions;
	
	// the back button -- exposed for tutorials
	public GameObject goWellapadBack;
	public GameObject GetBackButton() {
		return goWellapadBack;	
	}
	
	//---------------------------------------------------
	// Start()
	//---------------------------------------------------	
	void Start() {
		// listen for reward claimed callback
		WellapadMissionController.Instance.OnRewardClaimed += OnRewardClaimed;		
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
		float fDelay = Constants.GetConstant<float>( "Wellapad_DoneDelay" );
		yield return new WaitForSeconds( fDelay );
		
		SetScreen();
	}

	//---------------------------------------------------
	// SetScreen()
	// Called when the wellapad is opening.  The wellapad
	// is an electronic device with numerous screens --
	// this function (for now) will set the proper screen.
	//---------------------------------------------------		
	public void SetScreen() {
		// for now, just check to see if the player has any outstanding missions.
		bool bActive = WellapadMissionController.Instance.HasActiveTasks();
		
		if ( bActive ) {
			// user has active tasks/missions, so show the task list
			NGUITools.SetActive( goMissionsList, true );
			NGUITools.SetActive( goNoMissions, false );
		}
		else {
			// otherwise, show the "come back later" screen
			NGUITools.SetActive( goMissionsList, false );
			NGUITools.SetActive( goNoMissions, true );			
		}
	}
}
