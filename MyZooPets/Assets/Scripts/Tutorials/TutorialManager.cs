using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

//---------------------------------------------------
// TutorialManager
// Used in scenes like the yard and bedroom to keep
// track of game tutorials.
//---------------------------------------------------

public class TutorialManager : Singleton<TutorialManager> {
	// tutorial that is currently active
	private GameTutorial tutorial;
	public bool IsTutorialActive() {
		bool bActive = tutorial != null;
		return bActive;
	}
	public void SetTutorial( GameTutorial tutorial ) {
		// check to make sure there are not overlapping tutorials
		if ( tutorial != null && this.tutorial != null ) {
			Debug.Log("Tutorial Warning: " + tutorial + " is trying to override " + this.tutorial + " ABORTING!");
			return;	
		}
		
		this.tutorial = tutorial;
	}
	
	// list of objects that can be processed as input
	private List<GameObject> listCanProcess = new List<GameObject>();
	
	void Start() {
		//Debug.Log("Starting tutorial manager, running a test");
		//GameTutTest tutTest = new GameTutTest();
	}
	
	//---------------------------------------------------
	// CanProcess()
	// Used in scenes like the yard and bedroom to keep
	// track of game tutorials.
	//---------------------------------------------------	
	public bool CanProcess( GameObject go ) {
		// if the gameobject is null, then tutorial doesn't care (at the moment)
		if ( go == null )
			return true;
		
		// if there is no tutorial currently going on right now, the tutorial doesn't care (obviously)
		bool bActive = IsTutorialActive();
		if ( !bActive )
			return true;
		
		// otherwise we have a valid object and a valid tutorial, so let's get to checkin'
		bool bCanProcess = listCanProcess.Contains( go );
		
		return bCanProcess;
	}
}
