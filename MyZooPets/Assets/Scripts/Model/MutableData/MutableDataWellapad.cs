using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// WellapadData
// Save data for all things Wellapad related.
// Mutable data
//---------------------------------------------------

public class MutableDataWellapad {	
	// when the wellapad missions were created; used to determine if a refresh is necessary
	public DateTime DateMissionsCreated { get; set; }
	
	// list of unlocked tasks the user may receive
	public List<string> TasksUnlocked {get; set;}
	
	// list of current tasks/missions the user has
	public Dictionary< string, MutableDataWellapadTask > CurrentTasks {get; set;}
	
	public MutableDataWellapad(){
		Init();
	}

	//---------------------------------------------------
	// ResetMissions()
	// Wipes out save data for the player's mission.  Used
	// when the missions have expired.
	//---------------------------------------------------		
	public void ResetMissions() {
		CurrentTasks = new Dictionary< string, MutableDataWellapadTask >();
	}
	
    //Populate with dummy data
    private void Init(){
		ResetMissions();
    }	
}
