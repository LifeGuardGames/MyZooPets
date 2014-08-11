using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// Tutorial manager.
/// Used in scenes like the yard and bedroom to keep
/// track of game tutorials.
/// </summary>
public abstract class TutorialManager : Singleton<TutorialManager>{
	// pure abstract functions ------------------
	protected abstract void _Start();	// start function
	protected abstract void _Check();	// forces the tutorial manager to do a check to see if any tutorials should be launched
	// ------------------------------------------
	
	// public on/off switch for testing while in development
	protected bool isTutorialEnabled;
	
	// tutorial that is currently active
	private GameTutorial tutorial;

	/// <summary>
	/// Whether tutorial is active in the scene
	/// </summary>
	public bool IsTutorialActive(){
		bool isActive = tutorial != null;
		return isActive;
	}

	public void SetTutorial(GameTutorial tutorial){
		// check to make sure there are not overlapping tutorials
		if(tutorial != null && this.tutorial != null){
//			Debug.LogError("Tutorial Warning: " + tutorial + " is trying to override " + this.tutorial + " ABORTING!");
			return;	
		}
	
		this.tutorial = tutorial;
		
		// if the incoming tutorial is null...
		if(tutorial == null){
			// now that the tutorial is over, force a save
			DataManager.Instance.SaveGameData();
			
			// then check for a new tutorial
			Check();
		}
	}
	
	void Awake(){
		isTutorialEnabled = Constants.GetConstant<bool>("IntroTutorialsEnabled");
	}
	
	void Start(){
		// listen for partition changing event
		CameraManager.Instance.GetPanScript().OnPartitionChanged += EnteredRoom;		
		
		_Start();
	}

	/// <summary>
	/// Entered the room. When player switches rooms.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	public void EnteredRoom(object sender, PartitionChangedArgs args){
		// do a check in case a tutorial was in a different room
		Check();
	}

	/// <summary>
	/// Checks withich tutorial should play based on certain game conditions
	/// </summary>
	protected void Check(){
		if(!isTutorialEnabled || tutorial != null)
			return;
		else
			_Check();
	}

	/// <summary>
	/// Used in scenes like the yard and bedroom to keep track of game tutorials
	/// </summary>
	public bool CanProcess(GameObject go){
		// if the gameobject is null, then tutorial doesn't care (at the moment)
		if(go == null)
			return true;
		
		// if there is no tutorial currently going on right now, the tutorial doesn't care (obviously)
		bool isActive = IsTutorialActive();
		if(!isActive)
			return true;
		
		// otherwise we have a valid object and a valid tutorial, so let's get to checkin'
		bool canProcess = tutorial.CanProcess(go);
		
		return canProcess;
	}
}
