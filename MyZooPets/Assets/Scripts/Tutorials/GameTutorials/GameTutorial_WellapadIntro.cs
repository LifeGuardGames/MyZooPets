using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// GameTutorial_WellapadIntro
// Tutorial for showing the player what the wellapad
// is and how to access it.
//---------------------------------------------------

public class GameTutorial_WellapadIntro : GameTutorial {
	// wellapad button
	private GameObject goWellapadButton = GameObject.Find( "WellapadButton" );
	
	public GameTutorial_WellapadIntro() : base() {	
	}	
	
	//---------------------------------------------------
	// SetMaxSteps()
	//---------------------------------------------------		
	protected override void SetMaxSteps() {
		nMaxSteps = 2;
	}
	
	//---------------------------------------------------
	// SetKey()
	//---------------------------------------------------		
	protected override void SetKey() {
		strKey = TutorialManager_Bedroom.TUT_WELLAPAD;
	}
	
	//---------------------------------------------------
	// _End()
	//---------------------------------------------------		
	protected override void _End( bool bFinished ) {
	}
	
	//---------------------------------------------------
	// ProcessStep()
	//---------------------------------------------------		
	protected override void ProcessStep( int nStep ) {
		switch ( nStep ) {
			case 0:
				// start by focusingon the wellapad button
				FocusWellapadButton();
				
				break;
			
			case 1:
				WellapadMissionController.Instance.StartCoroutine( OpeningWellapad() );
			
				break;
		}
	}
	
	//---------------------------------------------------
	// OpeningWellapad()
	//---------------------------------------------------		
	private IEnumerator OpeningWellapad() {
		// destroy the spotlight we created for the button
		RemoveSpotlight();		
	
		// also, set the correct tutorial tasks in the wellapad
		WellapadMissionController.Instance.CreateTutorialMissions();	
	
		// wait a frame for the new UI elements to create themselves
		yield return 0;
		
		// highlight this task
		WellapadMissionController.Instance.HighlightTask( "DailyInhaler" );
	
		// listen for when the wellapad is closed
		WellapadUIManager.Instance.OnManagerOpen += OnWellapadClosed;		
	}

	//---------------------------------------------------
	// FocusWellapadButton()
	//---------------------------------------------------		
	private void FocusWellapadButton() {
		// begin listening for when the button is clicked
		LgButton button = goWellapadButton.GetComponent<LgButton>();
		button.OnProcessed += ButtonClicked;
		
		// the inhaler is the only object that can be clicked
		AddToProcessList( goWellapadButton );
	
		// spotlight the inhaler
		SpotlightObject( goWellapadButton, true, InterfaceAnchors.BottomLeft );
	}
	
	//---------------------------------------------------
	// ButtonClicked()
	// Callback for when the wellapad object is clicked;
	// this means we need to advance the tutorial.
	//---------------------------------------------------	
	private void ButtonClicked( object sender, EventArgs args ) {
		// stop listening for this event
		LgButton button = (LgButton) sender;
		button.OnProcessed -= ButtonClicked;
		
		// we have to allow the wellapad back button to be clicked
		GameObject goBack = WellapadUIManager.Instance.GetScreenManager().GetBackButton();
		AddToProcessList( goBack );
		
		// go to the next step
		Advance();
	}
	
	//---------------------------------------------------
	// OnWellapadClosed()
	// Callback for when the wellapad is closed.
	//---------------------------------------------------	
	private void OnWellapadClosed( object sender, UIManagerEventArgs args ) {
		if ( args.Opening == false ) {
			// wellapad is closing, so stop listening
			WellapadUIManager.Instance.OnManagerOpen -= OnWellapadClosed;
			
			// advance to next step
			Advance();
		}
	}	
}
