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
				// start by focusing on the wellapad button
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
	private void FocusWellapadButton(){
		// begin listening for when the button is clicked
		LgButton button = goWellapadButton.GetComponent<LgButton>();
		button.OnProcessed += ButtonClicked;
		
		// the wellapad is the only object that can be clicked
		AddToProcessList(goWellapadButton);

		string tutKey = GetKey() + "_" + GetStep();
		string tutMessage = Localization.Localize(tutKey);

		// show popup message
		Vector3 vLoc = Constants.GetConstant<Vector3>("WellapadPopupLoc");

		Hashtable option = new Hashtable();
		option.Add(TutorialPopupFields.ShrinkBgToFitText, true);
		option.Add(TutorialPopupFields.Message, tutMessage);

		ShowPopup(Tutorial.POPUP_STD, vLoc, option:option);
	
		// spotlight the wellapad
		SpotlightObject(goWellapadButton, true, InterfaceAnchors.BottomLeft, 
			fingerHint:true, fingerHintFlip:true, delay:0.5f);
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

		// clean message and spotlight
		RemoveSpotlight();
		RemoveFingerHint();
		RemovePopup();
		
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
