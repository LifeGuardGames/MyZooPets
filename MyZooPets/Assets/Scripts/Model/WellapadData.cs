﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// WellapadData
// Save data for all things Wellapad related.
//---------------------------------------------------

public class WellapadData {		
	// when the wellapad missions were created; used to determine if a refresh is necessary
	public DateTime DateMissionsCreated { get; set; }
	
	// list of unlocked tasks the user may receive
	public List<string> TasksUnlocked {get; set;}
	
	// list of current tasks/missions the user has
	public Dictionary< string, Mission > CurrentTasks {get; set;}
	
	//---------------------------------------------------
	// ResetMissions()
	// Wipes out save data for the player's mission.  Used
	// when the missions have expired.
	//---------------------------------------------------		
	public void ResetMissions() {
		CurrentTasks = new Dictionary< string, Mission >();
	}
	
    //Populate with dummy data
    public void Init(){
        TasksUnlocked = new List<string>();
		TasksUnlocked.Add("PlayClinic");
		
		ResetMissions();
		
		// testing
		DateMissionsCreated = DateTime.Now;
		Dictionary<string, bool> test = new Dictionary<string, bool>();
		test["FightMonster"] = false;
		CurrentTasks["Side"] = new Mission("Side", test);
		
		
    }	
}
