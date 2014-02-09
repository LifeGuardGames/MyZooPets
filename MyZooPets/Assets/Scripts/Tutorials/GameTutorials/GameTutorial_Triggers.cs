using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// GameTutorial_Triggers
// Tutorial to explain degrad triggers to the player.
//---------------------------------------------------

public class GameTutorial_Triggers : GameTutorial {
	// degrad trigger used for the tutorial
	DegradTrigger scriptTrigger;
	
	public GameTutorial_Triggers() : base() {	
	}	
	
	//---------------------------------------------------
	// SetMaxSteps()
	//---------------------------------------------------		
	protected override void SetMaxSteps() {
		nMaxSteps = 4;
	}
	
	//---------------------------------------------------
	// SetKey()
	//---------------------------------------------------		
	protected override void SetKey() {
		strKey = TutorialManager_Bedroom.TUT_TRIGGERS;
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
				SpawnDust();
				break;
			case 1:
				TutorialManager.Instance.StartCoroutine(ShowWellapad());
				break;
			case 2:
				TutorialManager.Instance.StartCoroutine(AttackPlayer());
				break;
			case 3:
				TeachCleanup();
				break;			
		}
	}

	//---------------------------------------------------
	// ShowWellapad()
	//---------------------------------------------------		
	private IEnumerator ShowWellapad() {
		float fWait = Constants.GetConstant<float>( "TriggerTutorialWait_PreShowWellapad" );
		yield return new WaitForSeconds(fWait);
		// highlight the fight task
		WellapadMissionController.Instance.HighlightTask("CleanRoom");
	
		// show the wellapad
		WellapadUIManager.Instance.OpenUI();
	
		// enable the close button		
		GameObject goBack = WellapadUIManager.Instance.GetScreenManager().GetBackButton();
		AddToProcessList( goBack );
		
		// listen for wellapad closing
		WellapadUIManager.Instance.OnManagerOpen += OnWellapadClosed;			
	}

	//---------------------------------------------------
	// OnWellapadClosed()
	// Callback for when the wellapad is closed.
	//---------------------------------------------------	
	private void OnWellapadClosed(object sender, UIManagerEventArgs args) {
		if (args.Opening == false) {
			// wellapad is closing, so stop listening
			WellapadUIManager.Instance.OnManagerOpen -= OnWellapadClosed;
			
			// advance to next step
			Advance();
		}
	}	
	
	//---------------------------------------------------
	// SpawnDust()
	//---------------------------------------------------		
	private void SpawnDust() {
		// immediately create the dust triger for the tutorial.  Hopefully this happens just as the calendar is closing, so the player
		// doesn't see it pop onto the screen.
		
		// NOTE - a change in the tutorial caused us to spawn the actual dust in the smoke intro tutorial, but we find/set the 
		// game object here.  However, it's possible that the game has been shut off in between, so we may need to spawn it after all...
		GameObject goTrigger = GameObject.Find( DegradationUIManager.TUT_TRIGGER );
		
		if ( goTrigger == null )
			goTrigger = DegradationUIManager.Instance.PlaceTutorialTrigger().gameObject;
			
		scriptTrigger = goTrigger.GetComponent<DegradTrigger>();
				
		Advance();
	}
	
	//---------------------------------------------------
	// AttackPlayer()
	//---------------------------------------------------		
	private IEnumerator AttackPlayer() {
		// wait a brief moment
		// float fWait = Constants.GetConstant<float>( "TriggerTutorialWait_PreAttack" );
		// yield return new WaitForSeconds( fWait );
		
		// have the dust attack the player
		TutorialManager.Instance.StartCoroutine( scriptTrigger.FireOneSkull() );
		
		// wait another brief moment	
		float fWait = Constants.GetConstant<float>( "TriggerTutorialWait_PostAttack" );
		yield return new WaitForSeconds( fWait );
		
		Advance();
	}
	
	//---------------------------------------------------
	// TeachCleanup()
	//---------------------------------------------------	
	private void TeachCleanup() {
		// show a message
		Vector3 vLoc = Constants.GetConstant<Vector3>( "TriggerPopupLoc" );
		// GameObject fingerTut = (GameObject) Resources.Load("DegradationPressTut");
		// TutorialManager.Instance.Instantiate(fingerTut, vLoc, Quaternion.identity);

        string petName = DataManager.Instance.GameData.PetInfo.PetName;
		string stringKey = GetKey() + "_" + GetStep();
		string tutMessage = String.Format(Localization.Localize(stringKey), 
			StringUtils.FormatStringPossession(petName), petName);

		Hashtable option = new Hashtable();
        option.Add(TutorialPopupFields.ShrinkBgToFitText, true);
        option.Add(TutorialPopupFields.Message, tutMessage);

		ShowPopup( Tutorial.POPUP_STD, vLoc, option:option);
	
		// spotlight the dust
		SpotlightObject( scriptTrigger.gameObject, fingerHint:true);
	
		// add the dust to clickable objects
		AddToProcessList( scriptTrigger.gameObject );
	
		// listen for callback for when the dust is cleaned	
		scriptTrigger.OnTriggerCleaned += OnDustCleaned;
	}
	
	//---------------------------------------------------
	// OnDustCleaned()
	//---------------------------------------------------	
	private void OnDustCleaned( object sender, EventArgs args ) {
		TutorialManager.Instance.StartCoroutine( CleanupFinished() );
	}
	
	//---------------------------------------------------
	// CleanupFinished()
	//---------------------------------------------------		
	private IEnumerator CleanupFinished() {
		// clean up spotlight and popup
		RemoveSpotlight();
		RemovePopup();
		RemoveFingerHint();
	
		// wait a brief moment because the player is earning points and stuff
		float fWait = Constants.GetConstant<float>( "TriggerTutorialWait_PostCleanup" );
		yield return new WaitForSeconds( fWait );	
		
		// send out completion message here, because for tutorial, the normal way won't proc
		WellapadMissionController.Instance.TaskCompleted( "CleanRoom" );
		
		// advance the tutorial
		Advance();			
	}
}
