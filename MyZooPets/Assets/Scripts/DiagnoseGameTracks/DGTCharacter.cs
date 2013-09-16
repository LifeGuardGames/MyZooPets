using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// DGTCharacter
// This script is a character in the diagons game
// (tracks) that travels along the tracks and towards
// a zone.
//---------------------------------------------------

public class DGTCharacter : MonoBehaviour {	
	// list of potential sprites
	public string[] arrayOK;
	public string[] arraySick;
	public string[] arrayAttack;
	
	// the current gameobject target this character is moving towards
	private GameObject goTarget;
	
	// what type of asthma stage does this character represent?
	private AsthmaStage eStage;
	public AsthmaStage GetStage() {
		return eStage;	
	}
	
	// how many points is this character worth?
	public int nPoints;
	public int GetPointValue() {
		return nPoints;	
	}
	
	// the zone this character enters
	private GameObject goZone;
	
	// has this character been scored?
	private bool bScored = false;

	//---------------------------------------------------
	// Start()
	//---------------------------------------------------
	void Start () {
		// listen for various messages from the game manager
		DGTManager.OnSpeedChange += OnSpeedChange;			// speed change events
		DGTManager.OnStateChanged += OnGameStateChanged; 	// game state changes so the character can react appropriately
		DGTManager.OnNewGame += OnNewGame;					// new game
		
		// set this characters' attributes
		SetAttributes();
		
		// get moving to the first target
		goTarget = DGTManager.Instance.GetArrow();
		Move();	
	}
	
	//---------------------------------------------------
	// SetAttributes()
	// Sets the attributes on this characters (their sprite,
	// asthma stage, etc).
	//---------------------------------------------------	
	private void SetAttributes() {
		// set a random asthma stage
		eStage = EnumUtils.GetRandomEnum<AsthmaStage>();
		
		// pick a sprite associated with that stage -- little messy at the moment, because I think this structure may change
		string strSprite;
		string[] arraySprites;
		switch ( eStage ) {
			case AsthmaStage.OK:
				arraySprites = arrayOK;
				break;
			case AsthmaStage.Sick:
				arraySprites = arraySick;
				break;
			default: // attack
				arraySprites = arrayAttack;
				break;
		}
		int nRandom = UnityEngine.Random.Range(0, arraySprites.Length);
		strSprite = arraySprites[nRandom];
		tk2dSprite sprite = gameObject.GetComponent<tk2dSprite>();
		sprite.SetSprite( strSprite );
	}
	
	//---------------------------------------------------
	// OnDestroy()
	//---------------------------------------------------	
	void OnDestroy() {
		// stop listening to messages
		DGTManager.OnSpeedChange -= OnSpeedChange;			// speed change events	
		DGTManager.OnStateChanged -= OnGameStateChanged;	// game state changes
		DGTManager.OnNewGame -= OnNewGame;					// new game
	}
	
	//---------------------------------------------------
	// Move()
	//  Moves the character towards their current target.
	//---------------------------------------------------		
	private void Move() {
		float fSpeed = DGTManager.Instance.GetSpeed();
		
		//Change the 3 V3 to where icon should move
		Vector3[] path = new Vector3[4];
		path[0] = gameObject.transform.position;
		path[1] = goTarget.transform.position;
		path[2] = goTarget.transform.position;
		path[3] = goTarget.transform.position;
		
		Hashtable optional = new Hashtable();
		
		optional.Add("ease", LeanTweenType.linear);
		optional.Add ("onComplete", "DoneMoving");
		LeanTweenUtils.MoveAlongPathWithSpeed( gameObject, path, fSpeed, optional );
	}

	//---------------------------------------------------
	// OnTriggerEnter()
	// No longer using this because it seemed possible
	// for a character to be moving so fast that they
	// completed their movement before triggering collision.
	//---------------------------------------------------	
	/*
	void OnTriggerEnter(Collider other)
   	{
		DGTZone scriptZone = other.gameObject.GetComponent<DGTZone>();
		
		if ( scriptZone == null ) {
			Debug.Log("DGTCharacter is colliding with something other than a zone?");
			return;
		}
		
		goZone = other.gameObject;
   	}	
   	*/
	
	//---------------------------------------------------
	// OnSpeedChange()
	// When the game's track speed changes, this character
	// needs to react.
	//---------------------------------------------------
	private void OnSpeedChange(object sender, EventArgs args) {
		// if this character has been scored, we don't care about speed changes
		if ( bScored )
			return;
		
		// stop moving
		LeanTween.cancel(gameObject);
		
		// start moving again -- this will trigger the new speed changes
		Move();
	}
	
	//---------------------------------------------------
	// OnNewGame()
	// When the user restarts the game and a new game
	// begins.
	//---------------------------------------------------
	private void OnNewGame(object sender, EventArgs args) {
		// since a new game is beginning, regardless of anything, destroy ourselves
		Destroy( gameObject );
	}	
	
	//---------------------------------------------------
	// OnGameStateChanged()
	// When the game's state changes, the character may
	// want to react.
	// This function will likely change as the game/anims
	// get a little more complex.
	//---------------------------------------------------	
	private void OnGameStateChanged( object sender, GameStateArgs args ) {
		// if this character has scored, it doesn't really care about game state changes...it will take care of itself
		if ( bScored )
			return;
		
		MinigameStates eState = args.GetGameState();
		
		switch ( eState ) {
			case MinigameStates.GameOver:
				// stop them in their tracks
				LeanTween.cancel( gameObject );
				break;
			case MinigameStates.Paused:
				// stop them in their tracks
				LeanTween.pause( gameObject );
				break;
			case MinigameStates.Playing:
				// resume their movement
				LeanTween.resume( gameObject );
				break;
		}
	}
	
	//---------------------------------------------------
	// DoneMoving()
	// This character has finished a lean tween.  Based
	// on where the character moved to, either score or
	// pick the next target.
	//---------------------------------------------------	
	private void DoneMoving() {
		if ( goTarget.GetComponent<DGTZone>() != null ) {
			// score the character
			ScoreCharacter();
		
			// then destroy them
			Destroy( gameObject );
		}
		else {
			// move the character to the currently selected zone
			goTarget = DGTManager.Instance.GetSelectedZone();
			
			Move();
		}
	}
	
	//---------------------------------------------------
	// ScoreCharacter()
	// goCharacter has reached goZone -- now the manager
	// should score this interaction.
	//---------------------------------------------------		
	public void ScoreCharacter() {
		// set this to avoid updating on speed once the character has been scored
		bScored = true;
		
		if ( goTarget == null || goTarget.GetComponent<DGTZone>() == null ) {
			Debug.Log("Character unable to score because of null component");
			return;
		}
		
		// update the wave countdown
		// it's important this happens BEFORE anything else or the game may get into a weird state
		DGTManager.Instance.UpdateWaveCountdown(-1);		
		
		DGTZone scriptZone = goTarget.GetComponent<DGTZone>();
		
		// score the interaction -- for now, if the character type matches the zone type, award some points
		AsthmaStage eZoneStage = scriptZone.GetStage();
		AsthmaStage eCharStage = GetStage();

		if ( eZoneStage == eCharStage ) {
			// character was sent to the right zone -- get some points!
			int nVal = GetPointValue();
			DGTManager.Instance.UpdateScore( nVal );
		}
		else {
			// character was sent to wrong zone...lose a life!
			DGTManager.Instance.UpdateLives( -1 );
		}
	}	
}
