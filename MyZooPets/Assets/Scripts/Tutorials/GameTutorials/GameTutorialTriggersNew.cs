using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// GameTutorial_Triggers
// Tutorial to explain degrad triggers to the player.
//---------------------------------------------------

public class GameTutorialTriggersNew : GameTutorial{
	// degrad trigger used for the tutorial
	DegradTrigger scriptTrigger;
	
	public GameTutorialTriggersNew() : base(){	
	}
	
	protected override void SetMaxSteps(){
		maxSteps = 2;
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
			TeachCleanup();
			break;
		}
	}
	
	private void SpawnDust(){
		GameObject goTrigger = GameObject.Find(DegradationUIManager.TUT_TRIGGER);
		
		if(goTrigger == null){
			goTrigger = DegradationUIManager.Instance.PlaceTutorialTrigger().gameObject;
		}
		
		scriptTrigger = goTrigger.GetComponent<DegradTrigger>();
		
		Advance();
	}
	
	private void TeachCleanup(){
		// show a message
		Vector3 triggerPopupLoc = Constants.GetConstant<Vector3>("TriggerPopupLoc");
		ShowPopup(TUTPOPUPTEXT, triggerPopupLoc);
		
		// spotlight the dust
		SpotlightObject(scriptTrigger.gameObject, hasFingerHint: true, fingerHintFlip: true);
		
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
		RoomArrowsUIManager.Instance.ShowRightArrow();
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
