using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// DGTTutorial
// Tutorial for the clinic game.
//---------------------------------------------------

public class DGTTutorial : MinigameTutorial {
	// constants
	private const int NUM_TO_SPAWN = 3;		// number of characters to spawn per tutorial step
	
	// stack of pre-determined character types to be spawned; used in tutorial
	private Stack<AsthmaStage> stackStages = new Stack<AsthmaStage>();	
	
	// how many pets have scored during this step of the tutorial?
	private int nScored;
	
	// the current stage this tutorial is highlighting
	private AsthmaStage eCurrentStage;
	private void SetCurrentStage( AsthmaStage eStage ) {
		eCurrentStage = eStage;	
	}
	public AsthmaStage GetCurrentStage() {
		return eCurrentStage;	
	}
	
	public DGTTutorial() {
		// set the max steps for this tutorial
		nMaxSteps = 3;
		
		// set the key
		strKey = "DGT_TUT";
		
		// listen for character scoring
		DGTCharacter.OnCharacterScored += CharacterScored;
	}
	
	//---------------------------------------------------
	// _End()
	//---------------------------------------------------	
	protected override void _End() {
		// stop listen for character scoring
		DGTCharacter.OnCharacterScored -= CharacterScored;	
	}
	
	//---------------------------------------------------
	// ProcessStep()
	//---------------------------------------------------		
	protected override void ProcessStep( int nStep ) {
		// show the proper tutorial message
		ShowMessage();
		
		switch ( nStep ) {
			case 0:				
				// send healthy pets out
				QueueCharacters( AsthmaStage.OK );
				break;
			case 1:
				// send sick pets out
				QueueCharacters( AsthmaStage.Sick );
				break;	
			case 2:
				// send very sick pets out
				QueueCharacters( AsthmaStage.Attack );
				break;				
			case 3:
				// this part of the tutorial is just text			
				break;
			default:
				Debug.Log("Clinic tutorial has an unhandled step: " + nStep );
				break;
		}		
	}
	
	//---------------------------------------------------
	// QueueCharacters()
	// Based on the current stage for this tutorial,
	// queue up a bunch of those characters on the stack.
	// The DGTManager will use this stack when spawning
	// characters.
	//---------------------------------------------------		
	private void QueueCharacters( AsthmaStage eStage ) {
		// reset # of pets scored
		nScored = 0;		
		
		// set the current stage
		SetCurrentStage( eStage );
		
		// queue up the characters
		for ( int i = 0; i < NUM_TO_SPAWN; ++i )
			stackStages.Push( eStage );		
	}
	
	//---------------------------------------------------
	// CharacterScored()
	// When a character reaches a zone (regardless of if
	// it's the correct zone).
	//---------------------------------------------------		
	public void CharacterScored( object sender, EventArgs args ) {
		// increment pets scored
		nScored++;
		
		// if the number of spawned pets scored, time to go to the next step
		if ( nScored == NUM_TO_SPAWN )
			Advance();
	}	
	
	//---------------------------------------------------
	// ShouldWait()
	// Returns true if the stack is currently empty.
	//---------------------------------------------------		
	public bool ShouldWait() {
		bool bWait = stackStages.Count == 0;
		return bWait;
	}
	
	//---------------------------------------------------
	// GetStageToSpawn()
	// Pops the stack to determine what kind of character
	// should be spawned.
	//---------------------------------------------------	
	public AsthmaStage GetStageToSpawn() {
		AsthmaStage eStage = AsthmaStage.OK;
		
		if ( stackStages.Count > 0 )
			eStage = stackStages.Pop();
		else
			Debug.Log("Clinic tutorial trying to pop a stage that doesn't exist.");
		
		return eStage;
	}
}
