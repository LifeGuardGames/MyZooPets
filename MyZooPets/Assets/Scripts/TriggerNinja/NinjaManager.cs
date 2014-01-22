using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


//---------------------------------------------------
// NinjaManager
// Manager for the trigger ninja game.
//---------------------------------------------------

public class NinjaManager : MinigameManager<NinjaManager> {
	// combo related
	public float fComboMaxTime;		// max time between cuts for a combo
	private float fComboTime = 0;	// time counter
	private int nCombo = 0;			// the current combo level of the player
	private int nBestCombo = 0;		// player's best combo in one run
	
	// used to count time between groups and between entries within a group
	private float fTime = 0;
	public float fMax;			// time between spawn groups
	
	// the gesture trail that follows the user's finger around
	public GestureTrail trail;
	private Vector2 trailDeltaMove;
	public Vector2 GetTrailDeltaMove(){
		return trailDeltaMove;
	}
	
	// the last position of the user's trail
	private Vector3 vLastPos = new Vector3(0,0,0);
	
	// current list of entrie to spawn triggers from
	private List<NinjaDataEntry> listCurrentEntries;
	
	void Awake(){
		//TO DO - not sure if this is the best place to set frame rate
		Application.targetFrameRate = 60;
	}
	
	//---------------------------------------------------
	// _Start()
	//---------------------------------------------------	
	protected override void _Start() {
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
		// reset variables
		fComboTime = 0;
		nCombo = 0;
		nBestCombo = 0;
		fTime = 0;
		listCurrentEntries = null;
	}
	
	//---------------------------------------------------
	// _GameOver()
	//---------------------------------------------------		
	protected override void _GameOver() {
		// send out combo task
		int nBestCombo = GetCombo_Best();
		WellapadMissionController.Instance.TaskCompleted( "Combo" + GetMinigameKey(), nBestCombo );
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
		float fDelta = Time.deltaTime;
		
		// update the player's combo
		UpdateComboTimer( fDelta );
		
		// if there is a current group of spawn entries in process...
		if ( listCurrentEntries != null && listCurrentEntries.Count > 0 ) {
			// count up
			fTime += fDelta;
			
			// if our time has surpassed the next entry's time, do it up and remove that entry
			NinjaDataEntry entry = listCurrentEntries[0];
			float fTimeEntry = entry.GetTime();
			if ( fTime >= fTimeEntry ) {
				SpawnGroup( entry );
				listCurrentEntries.RemoveAt(0);
			}
			
			// if the list of current entries is empty...null the list and reset our count so we can count down again
			if ( listCurrentEntries.Count == 0 ) {
				listCurrentEntries = null;
				fTime = fMax;
			}
		}
		else if ( fTime <= 0 ) {
			// otherwise, there is no current group and it is time to start one, so figure out which one to begin
			NinjaScoring eScore = GetScoringKey();
			NinjaData data = NinjaDataLoader.GetGroupToSpawn( NinjaModes.Classic, eScore );
			
			//Debug.Log("STARTING GROUP " + data.GetID() + " of length " + data.GetEntries().Count);
			
			// cache the list -- ALMOST FOOLED ME....use new to copy the list
			listCurrentEntries = new List<NinjaDataEntry>(data.GetEntries());
		}
		else
			fTime -= fDelta;	// otherwise, there is no group and we still need to countdown before spawning the next group
	}	
	
	//---------------------------------------------------
	// SpawnGroup()
	//---------------------------------------------------		
	private void SpawnGroup( NinjaDataEntry entry ) {
		// create the proper list of objects to spawn
		int nCount = entry.GetTriggers();
		int nBombs = entry.GetBombs();
		List<string> listObjects = new List<string>();
		for ( int i = 0; i < nCount; ++i ) 
			listObjects.Add("NinjaTrigger");
		
		for ( int i = 0; i < nBombs; ++i )
			listObjects.Add("NinjaBomb");
		
		// shuffle the list so everything is nice and mixed up
		listObjects.Shuffle();
		
		// create the proper object based off the entry's pattern
		NinjaPatterns eType = entry.GetPattern();
		switch ( eType ) {
			case NinjaPatterns.Separate:
				new SpawnGroup_Separate( listObjects );
				break;
			case NinjaPatterns.Clustered:
				new SpawnGroup_Cluster( listObjects );
				break;
			case NinjaPatterns.Meet:
				new SpawnGroup_Meet( listObjects );
				break;
			case NinjaPatterns.Cross:
				new SpawnGroup_Cross( listObjects );
				break;
			case NinjaPatterns.Split:
				new SpawnGroup_Split( listObjects );
				break;			
			default:
				Debug.LogError("Unhandled group type: " + eType);
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
		
		// update the ninja gesture cut trail
		UpdateTrail( gesture );
		
		GameObject go = gesture.Selection;
		if ( go ) {
			//Debug.Log("Touching " + go.name);
			NinjaTrigger trigger = go.GetComponent<NinjaTrigger>();
			
			// if the trigger is null, check the parent...a little hacky, but sue me!
			if ( trigger == null )
				trigger = go.transform.parent.gameObject.GetComponent<NinjaTrigger>();
				
			if ( trigger )
				trigger.OnCut( gesture.Position );
		}
	}
	
	//---------------------------------------------------
	// UpdateTrail()
	// Updates the trail renderer that follows the
	// user's finger/mouse around.
	// I'm not sure how/why FingerGestures only calls
	// drag events on this class; otherwise I would have
	// had the trail itself take these messages...
	//---------------------------------------------------	
	private void UpdateTrail( DragGesture gesture ) {
   		ContinuousGesturePhase ePhase = gesture.Phase;
		
	    // the screen position the user's finger is at currently
	    Vector2 vPos = gesture.Position;
	    
	    // based on phase of the gesture, call certain functions
	    switch ( ePhase ) {
	    	case ContinuousGesturePhase.Started:
	    		trail.DragStarted( vPos );
	    		break;
	    	case ContinuousGesturePhase.Updated:
	    		trail.DragUpdated( vPos );
	    		break;		
	    	case ContinuousGesturePhase.Ended:
	    		trail.DragEnded();
	    		break;
		}
		
		// save the last position for use with displaying combo
		vLastPos = vPos;
		
		trailDeltaMove = gesture.DeltaMove;
	}
	
	//---------------------------------------------------
	// GetScoringKey()
	// Returns the current scoring key, based on the
	// player's current score.  This function is a little
	// hacky.
	//---------------------------------------------------	
	private NinjaScoring GetScoringKey() {
		int nScore = GetScore();
		NinjaScoring eScore;
		
		if ( nScore == 0 )
			eScore = NinjaScoring.Start_1;
		else if ( nScore > 0 && nScore < 3 )
			eScore = NinjaScoring.Start_2;
		else if ( nScore == 3 )
			eScore = NinjaScoring.Start_3;
		else if ( nScore > 3 && nScore < 25 )
			eScore = NinjaScoring.Med;
		else
			eScore = NinjaScoring.High;
		
		//Debug.Log("Current score key: " + eScore);
		
		return eScore;
	}
	
	//---------------------------------------------------
	// GetCombo()
	// Returns the player's current combo level.
	//---------------------------------------------------		
	public int GetCombo() {
		return nCombo;	
	}
	public int GetCombo_Best() {
		return nBestCombo;	
	}
	
	//---------------------------------------------------
	// IncreaseCombo()
	// Increases the player's combo level by num.
	//---------------------------------------------------	
	public void IncreaseCombo( int num ) {
		// increase the combo
		int nCombo = GetCombo();
		nCombo += num;
		SetCombo( nCombo );
		
		// by default, increasing the combo resets the countdown before the combo expires
		fComboTime = GetMaxComboTime();
	}
	
	//---------------------------------------------------
	// SetCombo()
	// Sets the player's combo level to num.
	//---------------------------------------------------	
	private void SetCombo( int num ) {
		nCombo = num;	
	}
	private void SetCombo_Best( int num ) {
		nBestCombo = num;	
	}
	
	//---------------------------------------------------
	// GetMaxComboTime()
	// Returns the max time between successful cuts before
	// the player's combo expires.
	//---------------------------------------------------	
	private float GetMaxComboTime() {
		return fComboMaxTime;
	}	
	
	//---------------------------------------------------
	// UpdateComboTimer()
	// Takes care of updating the combo timer.
	//---------------------------------------------------	
	private void UpdateComboTimer( float fDelta ) {
		// if the player doesn't have a combo going, don't bother
		int nCombo = GetCombo();
		if ( nCombo <= 0 )
			return;
		
		// update the time
		fComboTime -= fDelta;
		
		// if the time has expired, end the current combo
		if ( fComboTime <= 0 )
			OnComboEnd();
	}
	
	//---------------------------------------------------
	// OnComboEnd()
	//---------------------------------------------------	
	private void OnComboEnd() {
		// give the player an additional point for each level of their combo
		int nCombo = GetCombo();
		if ( nCombo > 2 ) {
			UpdateScore( nCombo );
				        	
			
		}

		// get the right text for combo
		string strText = Localization.Localize("NINJA_COMBO");
		strText = StringUtils.Replace( strText, StringUtils.NUM, nCombo );
		
		// get the position of where to spawn the floaty text -- the last place the user's finger was (using this for now)
		Vector3 vPos = CameraManager.Instance.TransformAnchorPosition( vLastPos, InterfaceAnchors.BottomLeft, InterfaceAnchors.Center );
		//vPos.y *= CameraManager.Instance.GetRatioDifference();
		//vPos.x *= CameraManager.Instance.GetRatioDifference();

		// set up the hashtable full of options
        Hashtable option = new Hashtable();
        option.Add("parent", GameObject.Find("Anchor-Center"));
        option.Add("text", strText);
		option.Add("prefab", "NinjaComboFloatyText");
		option.Add("position", vPos);
        option.Add("textSize", Constants.GetConstant<int>("Ninja_ComboTextSize"));
		
		// spawn floaty text
        FloatyUtil.SpawnFloatyText(option);			
		
		// if the current combo was better than their best, update it
		int nBest = GetCombo_Best();
		if ( nCombo > nBest )
			SetCombo_Best( nCombo );
		
		// reset the combo down to 0
		SetCombo( 0 );	
		
		//Debug.Log("Combo ended: " + nCombo);
	}
	
	//---------------------------------------------------
	// GetReward()
	//---------------------------------------------------		
	public override int GetReward( MinigameRewardTypes eType ) {
		// for now, just use the standard way
		return GetStandardReward( eType );
	}	
}
