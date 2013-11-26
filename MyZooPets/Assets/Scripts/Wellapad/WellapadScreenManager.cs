using UnityEngine;
using System.Collections;

//---------------------------------------------------
// WellapadScreenManager
// The wellapad is an electronic device with many
// screens.  This script decides which screens to
// show and hide.
//---------------------------------------------------

public class WellapadScreenManager : MonoBehaviour {
	// screens of the wellapad (as game objects)
	public GameObject goMissionsList;
	public GameObject goNoMissions;

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
