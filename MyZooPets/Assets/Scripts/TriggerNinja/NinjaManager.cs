using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


//---------------------------------------------------
// NinjaManager
// Manager for the trigger ninja game.
//---------------------------------------------------

public class NinjaManager : MinigameManager<NinjaManager> {
	
	// the y threshold after which objects get destroyed
	private float fFloor;
	public float GetFloor() {
		return fFloor;	
	}
	
	// testing
	private float fTime = 0;
	
	//---------------------------------------------------
	// _Start()
	//---------------------------------------------------	
	protected override void _Start() {
		// set the floor -- this varies from device to device
		// we get the floor by finding the y value of the 0,-.5 position of the viewport
		Vector3 vFloor = Camera.main.ViewportToWorldPoint( new Vector3(0, -.5f, 10) );
		fFloor = vFloor.y;
	}	
	
	//---------------------------------------------------
	// _OnDestroy()
	//---------------------------------------------------	
	protected override void _OnDestroy() {
	}
	
	//---------------------------------------------------
	// _NewGame()
	//---------------------------------------------------	
	protected override void _NewGame() {	
	}
	
	//---------------------------------------------------
	// GetMinigameKey()
	//---------------------------------------------------	
	protected override string GetMinigameKey() {
		return "Ninja";	
	}	
	
	//---------------------------------------------------
	// HasCutscene()
	//---------------------------------------------------		
	protected override bool HasCutscene() {
		return false;
	}	
	
	//---------------------------------------------------
	// _Update()
	//---------------------------------------------------
	protected override void _Update () {
		if ( fTime <= 0 ) {
			float fX = UnityEngine.Random.Range(.1f, .9f);
			Vector3 vPos = Camera.main.ViewportToWorldPoint( new Vector3( fX, -.25f, 10 ) );
			
			string strPrefab = fX < .5f ? "NinjaTrigger" : "NinjaTriggerBomb";
			//string strPrefab = "NinjaTrigger";
			
			GameObject resource = Resources.Load(strPrefab) as GameObject;
			GameObject go = Instantiate( resource, vPos, Quaternion.identity ) as GameObject;
			
			float fForceY = UnityEngine.Random.Range(500, 900);
			//float fForceY = 500;
			
			go.rigidbody.AddForce( new Vector3(0, fForceY, 0) );
			
			fTime = 1;
		}
		else
			fTime -= Time.deltaTime;
	}	
	
	//---------------------------------------------------
	// OnDrag()
	//---------------------------------------------------	
	void OnDrag( DragGesture gesture ) 
	{
		// check is playing
		if ( GetGameState() != MinigameStates.Playing )
			return;
		
		// current gesture phase (Started/Updated/Ended)
		//ContinuousGesturePhase phase = gesture.Phase;
		
		GameObject go = gesture.Selection;
		if ( go ) {
			Debug.Log("Touching " + go.name);
			NinjaTrigger trigger = go.GetComponent<NinjaTrigger>();
			
			// if the trigger is null, check the parent...a little hacky, but sue me!
			if ( trigger == null )
				trigger = go.transform.parent.gameObject.GetComponent<NinjaTrigger>();
				
			if ( trigger )
				trigger.OnCut();
		}
		
		// Drag/displacement since last frame
		//Vector2 deltaMove = gesture.DeltaMove;
		
		// Total drag motion from initial to current position
		//Vector2 totalMove = gesture.TotalMove;
	}
	
	
	
	////////----------------- Tutorial code	
	/*private void StartTutorial() {
		// set our tutorial
		SetTutorial( new DGTTutorial() );
	}	*/
}
