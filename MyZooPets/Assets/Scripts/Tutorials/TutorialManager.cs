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
	
	// current (and only) spotlight object
	private GameObject goSpotlight;
	
	// list of objects that can be processed as input
	private List<GameObject> listCanProcess = new List<GameObject>();
	
	public GameObject goSpotTest;
	
	void Start() {
		//Debug.Log("Starting tutorial manager, running a test");
		//GameTutTest tutTest = new GameTutTest();
		
		Debug.Log("Starting tut manager, running spotlight test");
		SpotlightObject( goSpotTest );
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
	
	//---------------------------------------------------
	// SpotlightObject()
	// Puts a spotlight around the incoming object to
	// draw attention to it.
	//---------------------------------------------------	
	public void SpotlightObject( GameObject goTarget ) {
		// if the spotlight object already exists, then we want to just move it to the new location
		if ( goSpotlight != null ) {
			MoveSpotlight( goTarget );
			return;
		}
		
		//goTarget.transform.position = Camera.main.ScreenToWorldPoint( new Vector3( 512f, 384f, 20f ) );
		
		// get the proper location of the object we are going to focus on
		Vector3 vPos = Camera.main.WorldToScreenPoint( goTarget.transform.position );
		Debug.Log("Main " + vPos);
		//Vector3 vGui = CameraManager.Instance.WorldToScreen( CameraManager.Instance.cameraNGUI, vPos );
		//Debug.Log("NGUI: " + vGui);
		
		Debug.Log("What about reversing it: " + Camera.main.ScreenToWorldPoint( vPos ) );
		
		//Debug.Log("Using utility: " + UIUtility.Instance.nguiCameraWorld2Screen( goTarget.transform.position) );
		
		//Debug.Log("And the normal way: " + CameraManager.Instance.cameraNGUI.WorldToScreenPoint( goTarget.transform.position));
		
		// create the object
		GameObject goResource = Resources.Load( "TutorialSpotlight" ) as GameObject;
		goSpotlight = LgNGUITools.AddChildWithPosition( GameObject.Find("Anchor-BottomLeft"), goResource );
		vPos.z = goSpotlight.transform.position.z; // keep the default z-value of the spotlight
		//vPos.x = vPos.x * CameraManager.Instance.ratioX;
		//vPos.y = vPos.y * CameraManager.Instance.ratioY;
		goSpotlight.transform.localPosition = vPos;
		Debug.Log("Wtf: " + vPos);
		Debug.Log(goSpotlight.transform.localPosition);
		Debug.Log(goSpotlight.transform.position);
	}
	
	//---------------------------------------------------
	// MoveSpotlight()
	// Moves the spotlight object to focus on goTarget.
	//---------------------------------------------------		
	private void MoveSpotlight( GameObject goTarget ) {
		
	}
	
	//---------------------------------------------------
	// RemoveSpotlight()
	// Removes the current spotlight object.
	//---------------------------------------------------		
	public void RemoveSpotlight() {
		if ( goSpotlight == null ) {
			Debug.Log("Trying to destroy a spotlight that doesn't exist!");
			return;
		}
		
		Destroy( goSpotlight );
	}
}
