using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// GameTutorial_Flame
// Tutorial to explain to the user how to blow fire
// at the smoke monster.
//---------------------------------------------------

public class GameTutorial_Flame : GameTutorial {
	
	public GameTutorial_Flame() : base() {		
		FireMeter.OnMeterFilled += OnMeterFilled;			// set up callback for when the player fully charges their meter
		PetAnimator.OnBreathStarted += OnBreathStarted;		// callback for when the pet starts to actually breath fire
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
		strKey = TutorialManager_Bedroom.TUT_FLAME;
	}
	
	//---------------------------------------------------
	// _End()
	//---------------------------------------------------		
	protected override void _End( bool bFinished ) {
		// clean up various things this tutorial created
		RemovePopup();
		RemoveSpotlight();
	}
	
	//---------------------------------------------------
	// ProcessStep()
	//---------------------------------------------------		
	protected override void ProcessStep( int nStep ) {
		// location of flame popups
		Vector3 vPopup = Constants.GetConstant<Vector3>( "FlamePopup" );
		
		switch ( nStep ) {
			case 0:
				// hack central...use a "surrogate" to run the coroutine since this tutorial is not a monobehaviour
				GatingManager.Instance.StartCoroutine( FocusOnFlameButton() );
			
				// show a little popup message telling the user to hold down the flame button
				ShowPopup( Tutorial.POPUP_LONG, vPopup );
			
				break;
		case 1:
				// show a little popup message telling the user to let go to breath fire
				ShowPopup( Tutorial.POPUP_LONG, vPopup );
			
				break;
		}
	}
	
	//---------------------------------------------------
	// FocusOnFlameButton()
	// This part of the tutorial highlights the flame
	// button and tells the user to press and hold it.
	//---------------------------------------------------		
	private IEnumerator FocusOnFlameButton() {
		// wait one frame so that the flame button can appear
		yield return 0;
		
		// find and spotlight the fire button
		GameObject goFlameButton = GameObject.Find( ButtonMonster.FIRE_BUTTON );
		SpotlightObject( goFlameButton, true, InterfaceAnchors.Center, "TutorialSpotlightFlameButton" );
		
		// add the fire button to the processable list
		// this is kind of annoying...we actually want to add the child object, because the parent object is empty...
		GameObject goButton = goFlameButton.transform.Find( "Button" ).gameObject;
		AddToProcessList( goButton );
	}
	
	//---------------------------------------------------
	// OnMeterFilled()
	// Callback for when the fire meter gets to 100%.
	//---------------------------------------------------		
	private void OnMeterFilled( object sender, EventArgs args ) {
		// unsub from callback
		FireMeter.OnMeterFilled -= OnMeterFilled;
		
		// fire meter is full, so advance the tut
		Advance();
	}
	
	//---------------------------------------------------
	// OnBreathStarted()
	// Callback for when the pet beginst to breath fire.
	//---------------------------------------------------		
	private void OnBreathStarted( object sender, EventArgs args ) {
		// unsub from callback
		PetAnimator.OnBreathStarted -= OnBreathStarted;
		
		// pet began to breath fire, so advance the tut
		Advance();
	}	
}
