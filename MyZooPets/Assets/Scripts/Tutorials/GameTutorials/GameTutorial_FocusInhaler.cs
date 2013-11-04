using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// GameTutorial_FocusInhaler
// Tutorial to alert the user to use their inhaler.
//---------------------------------------------------

public class GameTutorial_FocusInhaler : GameTutorial {
	// inhaler object
	private GameObject goInhaler = GameObject.Find( "GO_RealInhaler" );
	
	public GameTutorial_FocusInhaler() : base() {	
	}	
	
	//---------------------------------------------------
	// SetMaxSteps()
	//---------------------------------------------------		
	protected override void SetMaxSteps() {
		nMaxSteps = 1;
	}
	
	//---------------------------------------------------
	// SetKey()
	//---------------------------------------------------		
	protected override void SetKey() {
		strKey = TutorialManager_Bedroom.TUT_INHALER;
	}
	
	//---------------------------------------------------
	// _End()
	//---------------------------------------------------		
	protected override void _End( bool bFinished ) {
		// destroy the spotlight we created for the inhaler
		RemoveSpotlight();
		
		// destroy the popup we created
		RemovePopup();
	}
	
	//---------------------------------------------------
	// ProcessStep()
	//---------------------------------------------------		
	protected override void ProcessStep( int nStep ) {
		switch ( nStep ) {
			case 0:
				// the start of the focus inhaler tutorial
				FocusInhaler();
			
				// show a little popup message
				Vector3 vLoc = Constants.GetConstant<Vector3>( "InhalerPopupLoc" );
				ShowPopup( Tutorial.POPUP_STD, vLoc );
				break;
		}
	}
	
	//---------------------------------------------------
	// FocusInhaler()
	//---------------------------------------------------		
	private void FocusInhaler() {
		// begin listening for when the inhaler is clicked
		LgButton button = goInhaler.GetComponent<LgButton>();
		button.OnProcessed += InhalerClicked;
		
		// the inhaler is the only object that can be clicked
		AddToProcessList( goInhaler );
	
		// spotlight the inhaler
		SpotlightObject( goInhaler );
	}
	
	//---------------------------------------------------
	// InhalerClicked()
	// Callback for when the inhaler object is clicked;
	// this means we need to advance the tutorial.
	//---------------------------------------------------	
	private void InhalerClicked( object sender, EventArgs args ) {
		// stop listening for this event
		LgButton button = (LgButton) sender;
		button.OnProcessed -= InhalerClicked;
		
		// go to the next step
		Advance();
	}
}
