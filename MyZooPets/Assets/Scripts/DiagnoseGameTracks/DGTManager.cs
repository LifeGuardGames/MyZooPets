using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


//---------------------------------------------------
// DGTManager
// Manager for the diagnose game (tracks).
//---------------------------------------------------

public class DGTManager : MinigameManager<DGTManager> {
	
	// object that points to the selected zone
	public GameObject goArrow;
	public GameObject GetArrow() {
		return goArrow;
	}
	
	// speed of the tracks
	public float fStartingSpeed;
	private float fCurrentSpeed;
	public float GetSpeed() {
		return fCurrentSpeed;	
	}
	
	// gameplay variables
	public float fStartSpawnRate;	// a new character is created every X seconds
	public int nCharactersPerWave;	// every X characters, the difficulty increases
	public float fSpeedIncrease;	// when a new wave happens, the speed increases by X
	public float fSpawnChange;		// when a new wave happens, characters spawn faster
	
	// rate at which characters spawn
	private float fSpawnRate;		// a new character is created every X seconds	
	
	// characters are spawned here
	public Vector3 vSpawnLocation;	
	private Vector3 GetSpawnLocation() {
		return vSpawnLocation;	
	}
	
	// prefab for character to be created
	public GameObject prefabCharacter;
	
	// when this reaches 0, a new wave occurs
	private int nWaveCountdown;
	
	// when this reaches 0, a new character is spawned
	private float fSpawnTimer;
	
	// the zone that starts off selected
	public GameObject goStartingZone;
	
	// the zone that is currently selected
	private GameObject goSelectedZone;
	
	//=======================Events========================
	public static EventHandler<EventArgs> OnSpeedChange; //when the game speed changes
	//=====================================================
	
	//---------------------------------------------------
	// _NewGame()
	//---------------------------------------------------	
	protected override void _NewGame() {
		// set our selected zone to the starting zone
		SetSelectedZone( goStartingZone );
		
		// set our starting speed
		fCurrentSpeed = fStartingSpeed;
		
		// set the starting spawn rate
		fSpawnRate = fStartSpawnRate;
		
		// set the wave countdown
		SetWaveCountdown( nCharactersPerWave );
		
		// set the spawn timer to 0
		SetSpawnTimer( 0 );
	}
	
	//---------------------------------------------------
	// SetSelectedZone()
	// Sets the currently selected zone to the incoming
	// goZone.
	//---------------------------------------------------		
	public void SetSelectedZone( GameObject goZone ) {
		if ( goZone == goSelectedZone )
			return;
		
		// change the zone
		goSelectedZone = goZone;
		
		// update the graphics indicating which zone is selected
		UpdatePathVisuals();
	}
	
	//---------------------------------------------------
	// GetSelectedZone()
	//---------------------------------------------------	
	public GameObject GetSelectedZone() {
		return goSelectedZone;	
	}
	
	//---------------------------------------------------
	// UpdatePathVisuals()
	// Updates various sprites on the screen to indicate
	// which path the characters will move towards.
	//---------------------------------------------------	
	private void UpdatePathVisuals() {
		// for now, just point the arrow at the selected zone
		SpriteUtils.PointTo( goArrow, goSelectedZone, 0 );
	}
	
	//---------------------------------------------------
	// _Update()
	//---------------------------------------------------
	protected override void _Update () {
		
		// update spawn timer for spawning new characters
		UpdateSpawnTimer(-Time.deltaTime);
		
		if (Input.GetKeyDown ("space")) {
			Debug.Log("Let's turn up the heat!");
			ChangeTrackSpeed( fSpeedIncrease );
		}
	}	
	
	//---------------------------------------------------
	// SetWaveCountdown()
	//---------------------------------------------------		
	private void SetWaveCountdown( int num ) {
		nWaveCountdown = num;	
		
		// if the wave countdown reaches 0, the game is going to get harder!
		if ( nWaveCountdown == 0 )
			NextWave();
	}
	
	//---------------------------------------------------
	// NextWave()
	//---------------------------------------------------		
	private void NextWave() {
		// reset the countdown
		SetWaveCountdown( nCharactersPerWave );
		
		// increases the speed of the game
		ChangeTrackSpeed( fSpeedIncrease );
		
		// increase the speed of the spawn timer
		ChangeSpawnRate( fSpawnChange );
	}
	
	//---------------------------------------------------
	// UpdateWaveCountdown()
	//---------------------------------------------------		
	public void UpdateWaveCountdown( int num ) {
		int nNew = nWaveCountdown + num;
		SetWaveCountdown( nNew );
	}
	
	//---------------------------------------------------
	// ChangeTrackSpeed()
	// Changes the speed of the characters/track.
	//---------------------------------------------------		
	private void ChangeTrackSpeed( float fChange ) {
		fCurrentSpeed += fChange;	
		
		// send out a message to all things on the track letting them know their speed needs to change
       if( OnSpeedChange != null )
            OnSpeedChange(this, EventArgs.Empty);	
	}
	
	//---------------------------------------------------
	// ChangeSpawnSpeed()
	// Changees the rate at which characters spawn.
	//---------------------------------------------------	
	private void ChangeSpawnRate( float fChange ) {
		fSpawnRate += fChange;
		
		// set a floor
		if ( fSpawnRate < 1 )
			fSpawnRate = 1;
	}
	
	//---------------------------------------------------
	// SetSpawnTimer()
	//---------------------------------------------------		
	private void SetSpawnTimer( float fTime ) {
		fSpawnTimer = fTime;
		
		if ( fSpawnTimer <= 0 )
			SpawnCharacter();
	}
	
	//---------------------------------------------------
	// SpawnCharacter()
	//---------------------------------------------------		
	private void SpawnCharacter() {
		// first, reset our spawn timer
		SetSpawnTimer( fSpawnRate );
		
		// now, create a character
		Vector3 vLoc = GetSpawnLocation();
		Instantiate( prefabCharacter, vLoc, Quaternion.identity );
	}
	
	//---------------------------------------------------
	// UpdateSpawnTimer()
	//---------------------------------------------------		
	private void UpdateSpawnTimer( float fTime ) {
		float fNew = fSpawnTimer + fTime;
		SetSpawnTimer( fNew );
	}
}
