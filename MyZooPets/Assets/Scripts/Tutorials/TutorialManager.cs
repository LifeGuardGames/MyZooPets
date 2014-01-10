using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

//---------------------------------------------------
// TutorialManager
// Used in scenes like the yard and bedroom to keep
// track of game tutorials.
//---------------------------------------------------

public abstract class TutorialManager : Singleton<TutorialManager> {
	// pure abstract functions ------------------
	protected abstract void _Start();	// start function
	protected abstract void _Check();		// forces the tutorial manager to do a check to see if any tutorials should be launched
	// ------------------------------------------
	
	// public on/off switch for testing while in development
	protected bool bOn;
	
	// tutorial that is currently active
	private GameTutorial tutorial;
	public bool IsTutorialActive() {
		bool bActive = tutorial != null;
		return bActive;
	}
	public void SetTutorial( GameTutorial tutorial ) {
		// check to make sure there are not overlapping tutorials
		if ( tutorial != null && this.tutorial != null ) {
			Debug.LogError("Tutorial Warning: " + tutorial + " is trying to override " + this.tutorial + " ABORTING!");
			return;	
		}
	
		this.tutorial = tutorial;
		
		// if the incoming tutorial is null...
		if ( tutorial == null ) {
			// now that the tutorial is over, force a save
			DataManager.Instance.SaveGameData();
			
			// then check for a new tutorial
			Check();
		}
	}
	
	void Awake() {
		bOn = Constants.GetConstant<bool>( "IntroTutorialsEnabled" );
	}
	
	void Start() {
		//Debug.Log("Starting tutorial manager, running a test");
		//GameTutTest tutTest = new GameTutTest();
		
		//Debug.Log("Starting tut manager, running spotlight test");
		//SpotlightObject( goSpotTest );
		
		// listen for partition changing event
		CameraManager.Instance.GetPanScript().OnPartitionChanged += EnteredRoom;		
		
		_Start();
	}
	
	//---------------------------------------------------
	// EnteredRoom()
	// When the player switches rooms.
	//---------------------------------------------------	
	public void EnteredRoom( object sender, PartitionChangedArgs args ) {
		// do a check in case a tutorial was in a different room
		Check();
	}
	
	//---------------------------------------------------
	// Check()
	// Checks which tutorial should play based on certain
	// game conditions.
	//---------------------------------------------------	
	protected void Check() {
		if ( !bOn || tutorial != null )
			return;
		else
			_Check();
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
		bool bCanProcess = tutorial.CanProcess( go );
		
		return bCanProcess;
	}
}
