using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


//---------------------------------------------------
// NinjaManager
// Manager for the trigger ninja game.
//---------------------------------------------------

public class NinjaManager : MinigameManager<NinjaManager> {
	// testing
	private float fTime = 0;
	public float fMax;
	public int num;
	public NinjaGroupTypes eSpawnType;
	
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
			/*
			float fX = UnityEngine.Random.Range(.1f, .9f);
			
			
			string strPrefab = fX < .5f ? "NinjaTrigger" : "NinjaTriggerBomb";
			//string strPrefab = "NinjaTrigger";
			
			GameObject resource = Resources.Load(strPrefab) as GameObject;
			
			for ( float i = .1f; i <= 1f; i += .1f ) {
				Vector3 vPos = Camera.main.ViewportToWorldPoint( new Vector3( i, -.25f, 10 ) );
				GameObject go = Instantiate( resource, vPos, Quaternion.identity ) as GameObject;
				
				float fForceY = UnityEngine.Random.Range(500, 900);
				//float fForceY = 500;
				
				go.rigidbody.AddForce( new Vector3(0, fForceY, 0) );
			}
			*/
			
			SpawnGroup( num, eSpawnType );
			
			fTime = fMax;
		}
		else
			fTime -= Time.deltaTime;
	}	
	
	//---------------------------------------------------
	// SpawnGroup()
	//---------------------------------------------------	
	private void SpawnGroup( int num, NinjaGroupTypes eType ) {
		List<string> listObjects = new List<string>();
		for ( int i = 0; i < num; ++i )
			listObjects.Add("NinjaTrigger");
		
		switch ( eType ) {
			case NinjaGroupTypes.Separate:
				new SpawnGroup_Separate( listObjects );
				break;
			case NinjaGroupTypes.Clustered:
				new SpawnGroup_Cluster( listObjects );
				break;
			case NinjaGroupTypes.Meet:
				new SpawnGroup_Meet( listObjects );
				break;
			case NinjaGroupTypes.Cross:
				new SpawnGroup_Cross( listObjects );
				break;
			case NinjaGroupTypes.Split:
				new SpawnGroup_Split( listObjects );
				break;			
			default:
				Debug.Log("Unhandled group type: " + eType);
				break;
		}
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
			//Debug.Log("Touching " + go.name);
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
