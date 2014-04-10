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
		FireMeter.OnMeterStartFilling += OnMeterStartFilling;
		PetAnimator.OnBreathEnded += OnBreathEnded;			// callback for when the pet finishes breathing fire
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
		// RemovePopup();
	}
	
	//---------------------------------------------------
	// ProcessStep()
	//---------------------------------------------------		
	protected override void ProcessStep( int nStep ) {
		// location of flame popups
		Vector3 vPopup = Constants.GetConstant<Vector3>( "FlamePopup" );
        Hashtable option = new Hashtable();

        //Tutorial popup options 
        option.Add(TutorialPopupFields.ShrinkBgToFitText, true);

		switch ( nStep ) {
			case 0:
				// hack central...use a "surrogate" to run the coroutine since this tutorial is not a monobehaviour
				GatingManager.Instance.StartCoroutine( FocusOnFlameButton() );
			
				// show a little popup message telling the user to hold down the flame button
				ShowPopup( Tutorial.POPUP_STD, vPopup, useViewPort:false, option:option);
			
				break;
			case 1:
		        string petName = DataManager.Instance.GameData.PetInfo.PetName;
				string stringKey = GetKey() + "_" + GetStep();
				string tutMessage = String.Format(Localization.Localize(stringKey), petName);

				option.Add(TutorialPopupFields.Message, tutMessage);
				
				// show a little popup message telling the user to let go to breath fire
				ShowPopup( Tutorial.POPUP_STD, vPopup, useViewPort:false, option:option);
				GatingManager.Instance.StartCoroutine(RemovePopupDelay());
				break;
		}
	}

	private IEnumerator RemovePopupDelay(){
		yield return new WaitForSeconds(1f);
		RemovePopup();	
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
		GameObject goFlameButton = GameObject.Find(ButtonMonster.FIRE_BUTTON);
		if (goFlameButton != null){
			SpotlightObject(goFlameButton, true, InterfaceAnchors.Center, "TutorialSpotlightFlameButton",
				fingerHint:true, fingerHintPrefab:"PressHoldTut", 
				fingerHintOffsetX:-160f, delay:0.5f);
			
			// add the fire button to the processable list
			// this is kind of annoying...we actually want to add the child object, because the parent object is empty...
			GameObject goButton = goFlameButton.transform.Find( "Button" ).gameObject;
			AddToProcessList( goButton );
		}
		else
			Debug.LogError("No flame button...that means the game is going to break");
	}
	
	//---------------------------------------------------
	// OnMeterFilled()
	// Callback for when the fire meter gets to 100%.
	//---------------------------------------------------		
	private void OnMeterFilled( object sender, EventArgs args ) {
		// unsub from callback
		FireMeter.OnMeterFilled -= OnMeterFilled;
		
		// remove the spotlight so the user can see the resulting flame attack
		RemoveSpotlight();
		
		// fire meter is full, so advance the tut
		Advance();
	}

	private void OnMeterStartFilling(object sender, EventArgs args){
		FireMeter.OnMeterStartFilling -= OnMeterStartFilling;

		RemoveFingerHint();
	}
	
	//---------------------------------------------------
	// OnBreathEnded()
	// Callback for when the pet finishes breathing fire.
	//---------------------------------------------------		
	private void OnBreathEnded( object sender, EventArgs args ) {
		// unsub from callback
		PetAnimator.OnBreathEnded -= OnBreathEnded;
		
		// at this point the user has done battle with the smoke monster, so I'm going to call highlight on a bogus task here just to unhighlight everything
		WellapadMissionController.Instance.HighlightTask( "" );
		
		// pet began to breath fire, so advance the tut
		Advance();
	}	
}
