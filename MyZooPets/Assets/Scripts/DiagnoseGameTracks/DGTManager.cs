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
	public float fMaxSpeed;
	private float fCurrentSpeed;
	public float GetSpeed( bool bMax = false ) {
		float fSpeed = bMax ? fMaxSpeed : fCurrentSpeed;
		
		return fSpeed;	
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
		// the location is in viewport coordinates, so translate it
		Vector3 vRealLoc = CameraManager.Instance.cameraMain.ViewportToWorldPoint( vSpawnLocation );
		return vRealLoc;	
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

	private int numOfCorrectDiagnose; //keep track of the number of correct diagnose
	
	//=======================Events========================
	public static EventHandler<EventArgs> OnSpeedChange; 	// when the game speed changes
	//=====================================================

	//---------------------------------------------------
	// _Start()
	//---------------------------------------------------	
	protected override void _Start() {		
		// sign up for callbacks
		DGTCharacter.OnCharacterScored += CharacterScored;	// character scoring
	}	
	
	//---------------------------------------------------
	// _OnDestroy()
	//---------------------------------------------------	
	protected override void _OnDestroy() {
		// stop listening for callbacks
		DGTCharacter.OnCharacterScored -= CharacterScored;	// character scoring	
	}
	
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
		ResetWaveCountdown();

		// set num of correct diagnose for each new game
		numOfCorrectDiagnose = 0;
		
		// if the play hasn't played the tutorial yet, start it
		if ( TutorialOn() && ( IsTutorialOverride() || !DataManager.Instance.GameData.Tutorial.ListPlayed.Contains( DGTTutorial.TUT_KEY ) ) )
			StartTutorial();		
		
		// set the spawn timer to 0
		// NOTE this must come after the tutorial starts because setting spawn timer to 0 immediately spawns a character
		SetSpawnTimer( 0 );
	}

	//---------------------------------------------------
	// _GameOver()
	//---------------------------------------------------		
	protected override void _GameOver(){
		BadgeLogic.Instance.CheckSeriesUnlockProgress(BadgeType.PatientNumber, numOfCorrectDiagnose, true);
	}
	
	//---------------------------------------------------
	// GetMinigameKey()
	//---------------------------------------------------	
	protected override string GetMinigameKey() {
		return "Clinic";	
	}	
	
	// //---------------------------------------------------
	// // HasCutscene()
	// //---------------------------------------------------		
	// protected override bool HasCutscene() {
	// 	return true;
	// }	
	
	//---------------------------------------------------
	// SetSelectedZone()
	// Sets the currently selected zone to the incoming
	// goZone.
	//---------------------------------------------------		
	public void SetSelectedZone( GameObject goZone ) {
		if ( goZone == goSelectedZone )
			return;
		
		// if a tutorial is happening, just set the speed to starting speed...this is in case the track was stopped
		// because the player didn't switch the tracks quick enough
		if ( IsTutorialRunning() )
			SetTrackSpeed( fStartingSpeed );
		
		// the zone changed, so play a sound (only if there was a valid zone before though)
		if ( goSelectedZone != null )
			AudioManager.Instance.PlayClip( "clinicSwitchTracks" );		
		
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
	}	
	
	//---------------------------------------------------
	// CharacterScored()
	// When a character reaches a zone (regardless of if
	// it's the correct zone).
	//---------------------------------------------------		
	public void CharacterScored( object sender, CharacterScoredEventArgs args ) {	
		// if the game is over, don't do anything
		if ( GetGameState() == MinigameStates.GameOver )
			return;
		
		// get relevant variables from the args
		DGTCharacter characterScored = args.character;
		DGTZone zoneTarget = args.zone;
		
		// get the asthma stages of the character and variable
		AsthmaStage eZoneStage = zoneTarget.GetStage();
		AsthmaStage eCharStage = characterScored.GetStage();

		if ( eZoneStage == eCharStage )
			CharacterScored_Right( characterScored, zoneTarget);
		else 
			CharacterScored_Wrong( characterScored, zoneTarget );	
		
		// regardless of whether the character scored right or wrong, show a poof FX
		Vector3 vPosFX = characterScored.gameObject.transform.position;
		ParticleUtils.CreateParticle( "ClinicZonePuff", vPosFX );
	}
	
	//---------------------------------------------------
	// CharacterScored_Right()
	// Called whenever a character reaches the right
	// zone.
	//---------------------------------------------------		
	private void CharacterScored_Right( DGTCharacter character, DGTZone zone) {
		// character was sent to the right zone -- get some points!
		int nVal = character.GetPointValue();
		UpdateScore( nVal );
		
		// set sound
		string strSound = "clinicCorrect";
		
		// update the wave count (if it's not a tutorial)
		if ( IsTutorialRunning() == false ){
			UpdateWaveCountdown(-1);
			numOfCorrectDiagnose++; //increment if not tutorial
		}
		
		// every X points the player gets an additional life
		int nExtraLife = Constants.GetConstant<int>( "Clinic_ExtraLife" );
		int nScore = GetScore();
		if ( nScore % nExtraLife == 0 ) {
			strSound = "PointSingle";		// set the sound to something different
			UpdateLives( 1 );
		}
		
		// play appropriate sound
		AudioManager.Instance.PlayClip( strSound );

		// Analytics
		Analytics.Instance.DiagnoseResult(Analytics.DIAGNOSE_RESULT_CORRECT, 
			character.GetStage(), zone.GetStage());
	}
	
	//---------------------------------------------------
	// CharacterScored_Wrong()
	// Called whenever a character reaches the wrong zone.
	//---------------------------------------------------		
	private void CharacterScored_Wrong( DGTCharacter character, DGTZone zone ) {
		// character was sent to wrong zone...lose a life!
		UpdateLives( -1 );
		
		// play an incorrect sound
		AudioManager.Instance.PlayClip( "clinicWrong" );

		//Analytics
		Analytics.Instance.DiagnoseResult(Analytics.DIAGNOSE_RESULT_INCORRECT,
			character.GetStage(), zone.GetStage());
		
		// also slow down the game, if this didn't cause us to have a game over
		MinigameStates eState = GetGameState();
		if ( eState == MinigameStates.Playing )
			SlowGameDown();
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
	// ResetWaveCountdown()
	//---------------------------------------------------		
	private void ResetWaveCountdown() {
		SetWaveCountdown( nCharactersPerWave );
	}
	
	//---------------------------------------------------
	// NextWave()
	//---------------------------------------------------		
	private void NextWave() {
		// reset the countdown
		ResetWaveCountdown();
		
		// increases the speed of the game
		ChangeTrackSpeed( fSpeedIncrease );
		
		// increase the speed of the spawn timer
		ChangeSpawnRate( fSpawnChange );
	}
	
	//---------------------------------------------------
	// UpdateWaveCountdown()
	//---------------------------------------------------		
	private void UpdateWaveCountdown( int num ) {
		int nNew = nWaveCountdown + num;
		SetWaveCountdown( nNew );
	}
	
	//---------------------------------------------------
	// ChangeTrackSpeed()
	// Changes the speed of the characters/track.
	//---------------------------------------------------		
	private void ChangeTrackSpeed( float fChange) {
		float fCurSpeed = GetSpeed();
		float fNewSpeed = fCurSpeed + fChange;	
		
		// track speed is increasing, so play a sound
		// NOTE currently assumes track speed can only increase
		AudioManager.Instance.PlayClip( "clinicSpeedUp" );
		
		// set the speed
		SetTrackSpeed( fNewSpeed );
	}
	private void SetTrackSpeed( float fNewSpeed, bool bPausing = false ) {
		fCurrentSpeed = fNewSpeed;
		
		if ( fCurrentSpeed < fStartingSpeed && !bPausing )
			fCurrentSpeed = fStartingSpeed;
		
		// send out a message to all things on the track letting them know their speed needs to change
       if( OnSpeedChange != null )
            OnSpeedChange(this, EventArgs.Empty);		
	}
	public void StopTrack() {
		SetTrackSpeed( 0, true );	
	}
	
	//---------------------------------------------------
	// SlowGameDown()
	// This function is called when the player incorrectly
	// diagnoses a pet, so we slow the game down to help
	// them out.
	//---------------------------------------------------	
	private void SlowGameDown() {
		// roll back the speed x 2
		ChangeTrackSpeed( fSpeedIncrease * -2 );
		
		// also roll back the spawn timer
		ChangeSpawnRate( fSpawnChange * -2 );
		
		// also reset the wave countdown so the game doesn't speed up right away
		ResetWaveCountdown();
	}
	
	//---------------------------------------------------
	// ChangeSpawnSpeed()
	// Changees the rate at which characters spawn.
	//---------------------------------------------------	
	private void ChangeSpawnRate( float fChange ) {
		fSpawnRate += fChange;
		
		// set a floor & ceiling
		if ( fSpawnRate < 1 )
			fSpawnRate = 1;
		else if ( fSpawnRate > fStartSpawnRate )
			fSpawnRate = fStartSpawnRate;
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
		// if a tutorial is going on, and the stage stack is empty, it means we need to wait to spawn more characters
		DGTTutorial tutorial = GetTutorial() as DGTTutorial;
		if ( IsTutorialRunning() && tutorial.ShouldWait() )
			return;
		
		// first, reset our spawn timer
		SetSpawnTimer( fSpawnRate );
		
		// now, create a character
		Vector3 vLoc = GetSpawnLocation();
		GameObject goChar = Instantiate( prefabCharacter, vLoc, Quaternion.identity ) as GameObject;
		
		// if there is something in the stages stack, it means that the newly spawned character should have a specific type
		if ( IsTutorialRunning() ) {
			AsthmaStage eStage = tutorial.GetStageToSpawn();
			DGTCharacter script = goChar.GetComponent<DGTCharacter>();
			script.SetAttributes( eStage );
		}
	}
	
	//---------------------------------------------------
	// UpdateSpawnTimer()
	//---------------------------------------------------		
	private void UpdateSpawnTimer( float fTime ) {
		float fNew = fSpawnTimer + fTime;
		SetSpawnTimer( fNew );
	}
	
	//---------------------------------------------------
	// IsZoneLocked()
	// A zone may be locked (user can't select it) if
	// the tutorial is going on.
	//---------------------------------------------------		
	public bool IsZoneLocked( AsthmaStage eZoneStage ) {
		bool bLocked = false;
		
		if ( IsTutorialRunning() ) {
			DGTTutorial tutorial = GetTutorial() as DGTTutorial;
			AsthmaStage eCurrentStage = tutorial.GetCurrentStage();
			bLocked = eCurrentStage != eZoneStage;
		}
		
		return bLocked;
	}
	
	//---------------------------------------------------
	// GetReward()
	//---------------------------------------------------		
	public override int GetReward( MinigameRewardTypes eType ) {
		// for now, just use the standard way
		return GetStandardReward( eType );
	}
	
	//---------------------------------------------------
	// StartTutorial()
	// Tutorial code	
	//---------------------------------------------------
	private void StartTutorial() {
		// set our tutorial
		SetTutorial( new DGTTutorial() );
	}	
}
