using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// GameTutorial_Triggers
// Tutorial to explain degrad triggers to the player.
//---------------------------------------------------

public class GameTutorialTriggers : GameTutorial{
	// degrad trigger used for the tutorial
	DegradTrigger scriptTrigger;
	
	public GameTutorialTriggers() : base(){	
	}

	protected override void SetMaxSteps(){
		maxSteps = 4;
	}

	protected override void SetKey(){
		tutorialKey = TutorialManagerBedroom.TUT_TRIGGERS;
	}
			
	protected override void _End(bool isFinished){
	}

	protected override void ProcessStep(int step){
		switch(step){
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
	
	private IEnumerator ShowWellapad(){
		float fWait = Constants.GetConstant<float>("TriggerTutorialWait_PreShowWellapad");
		yield return new WaitForSeconds(fWait);
		// highlight the fight task
		WellapadMissionController.Instance.HighlightTask("CleanRoom");
	
		// show the wellapad
		WellapadUIManager.Instance.OpenUI();
	
		// enable the close button		
		GameObject goBack = WellapadUIManager.Instance.GetScreenManager().GetBackButton();
		AddToProcessList(goBack);
		
		// listen for wellapad closing
		WellapadUIManager.Instance.OnManagerOpen += OnWellapadClosed;			
	}
	
	/// <summary>
	/// Raises the wellapad closed event.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void OnWellapadClosed(object sender, UIManagerEventArgs args){
		if(args.Opening == false){
			// wellapad is closing, so stop listening
			WellapadUIManager.Instance.OnManagerOpen -= OnWellapadClosed;
			
			// advance to next step
			Advance();
		}
	}	

	private void SpawnDust(){
		// immediately create the dust triger for the tutorial.  Hopefully this happens just as the calendar is closing, so the player
		// doesn't see it pop onto the screen.
		
		// NOTE - a change in the tutorial caused us to spawn the actual dust in the smoke intro tutorial, but we find/set the 
		// game object here.  However, it's possible that the game has been shut off in between, so we may need to spawn it after all...
		GameObject goTrigger = GameObject.Find(DegradationUIManager.TUT_TRIGGER);
		
		if(goTrigger == null)
			goTrigger = DegradationUIManager.Instance.PlaceTutorialTrigger().gameObject;
			
		scriptTrigger = goTrigger.GetComponent<DegradTrigger>();
				
		Advance();
	}
	
	//---------------------------------------------------
	// AttackPlayer()
	//---------------------------------------------------		
	private IEnumerator AttackPlayer(){
		// wait a brief moment
		// float fWait = Constants.GetConstant<float>( "TriggerTutorialWait_PreAttack" );
		// yield return new WaitForSeconds( fWait );
		
		// have the dust attack the player
		TutorialManager.Instance.StartCoroutine(scriptTrigger.FireOneSkull());
		
		// wait another brief moment	
		float postAttackWaitTime = Constants.GetConstant<float>("TriggerTutorialWait_PostAttack");
		yield return new WaitForSeconds(postAttackWaitTime);
		
		Advance();
	}

	private void TeachCleanup(){
		// show a message
		Vector3 triggerPopupLoc = Constants.GetConstant<Vector3>("TriggerPopupLoc");
		string stringKey = GetKey() + "_" + GetStep();
		string tutMessage = Localization.Localize(stringKey);
		
		Hashtable option = new Hashtable();
		option.Add(TutorialPopupFields.ShrinkBgToFitText, true);
		option.Add(TutorialPopupFields.Message, tutMessage);

		ShowPopup(Tutorial.POPUP_STD, triggerPopupLoc, useViewPort: false, option: option);
	
		// spotlight the dust
		SpotlightObject(scriptTrigger.gameObject, fingerHint: true);
	
		// add the dust to clickable objects
		AddToProcessList(scriptTrigger.gameObject);
	
		// listen for callback for when the dust is cleaned	
		scriptTrigger.OnTriggerCleaned += OnDustCleaned;
	}
	
	/// <summary>
	/// Raises the dust cleaned event.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void OnDustCleaned(object sender, EventArgs args){
		TutorialManager.Instance.StartCoroutine(CleanupFinished());
	}

	private IEnumerator CleanupFinished(){
		// clean up spotlight and popup
		RemoveSpotlight();
		RemovePopup();
		RemoveFingerHint();
	
		// wait a brief moment because the player is earning points and stuff
		float postCleanupWaitTime = Constants.GetConstant<float>("TriggerTutorialWait_PostCleanup");
		yield return new WaitForSeconds(postCleanupWaitTime);	
		
		// send out completion message here, because for tutorial, the normal way won't proc
		WellapadMissionController.Instance.TaskCompleted("CleanRoom");
		
		// advance the tutorial
		Advance();			
	}
}
