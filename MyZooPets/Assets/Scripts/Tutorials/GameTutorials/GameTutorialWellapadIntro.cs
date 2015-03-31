using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Tutorial for showing the player what the wellapad is and how to access it
/// </summary>
public class GameTutorialWellapadIntro : GameTutorial{
	// wellapad button
	private GameObject goWellapadButton = GameObject.Find("WellapadButton");
	private GameObject retentionMinipet;

	public GameTutorialWellapadIntro() : base(){	
	}

	protected override void SetMaxSteps(){
		maxSteps = 4;
	}

	protected override void SetKey(){
		tutorialKey = TutorialManagerBedroom.TUT_WELLAPAD;
	}

	protected override void _End(bool isFinished){
	}

	protected override void ProcessStep(int step){
		switch(step){
		case 0:
			TutorialManager.Instance.StartCoroutine(FocusOnRetentionMinipet1());
			break;
		case 1: // TODO integrate pebble second click here
			TutorialManager.Instance.StartCoroutine(FocusOnRetentionMinipet2());
			break;
		case 2:
			// start by focusing on the wellapad button
			FocusWellapadButton();
			break;
			
		case 3:
			TutorialManager.Instance.StartCoroutine(OpeningWellapad());
			break;
		}
	}

	// Prompt click on minipet
	private IEnumerator FocusOnRetentionMinipet1(){
		yield return 0;
		retentionMinipet = GameObject.Find("MiniPetPebbleDemon");

		// spotlight the minipet
		SpotlightObject(retentionMinipet, false, InterfaceAnchors.Center,
		                fingerHint: true, fingerHintPrefab: "PressTutWithDelay", focusOffsetY: 60f, fingerHintFlip: true, delay: 2f);

		retentionMinipet.GetComponent<MiniPet>().OnTutorialMinipetClicked += RetentionPetClicked1;
	}

	// Minipet introduction	TODO finish
	private IEnumerator FocusOnRetentionMinipet2(){
		yield return new WaitForSeconds(1f);

		// Keep the spotlight from last step so only show finger
		ShowFingerHint(retentionMinipet, flipX: true);

		string tutKey = GetKey() + "_" + GetStep();
		string tutMessage = Localization.Localize(tutKey);
		
		// Show popup message
		Vector3 popupLoc = Constants.GetConstant<Vector3>("MinipetPopupLoc");
		Hashtable option = new Hashtable();
		option.Add(TutorialPopupFields.ShrinkBgToFitText, true);
		option.Add(TutorialPopupFields.Message, tutMessage);
		ShowPopup(Tutorial.POPUP_STD, popupLoc, option: option);
		
		ShowRetentionPet(false, new Vector3(208, -177, -160));

		retentionMinipet.GetComponent<MiniPet>().OnTutorialMinipetClicked += RetentionPetClicked2;
	}

	/////////

	private void FocusWellapadButton(){
		// begin listening for when the button is clicked
		LgButton button = goWellapadButton.GetComponent<LgButton>();
		button.OnProcessed += ButtonClicked;
		
		// spotlight the wellapad
		SpotlightObject(goWellapadButton, true, InterfaceAnchors.BottomLeft, 
			fingerHint: true, fingerHintPrefab: "PressTutWithDelay", fingerHintFlip: true, delay: 0.5f);

		TutorialManager.Instance.StartCoroutine(CreateWellapadButtonTutMessage());
		ShowRetentionPet(false, new Vector3(208, -177, -160));
	}

	//using this to deplay ShowPopup call for 0.5 seconds
	private IEnumerator CreateWellapadButtonTutMessage(){
		yield return new WaitForSeconds(0.5f);

		// the wellapad is the only object that can be clicked
		// only allow the button to be clicked after all the tutorial components
		// fade in
		AddToProcessList(goWellapadButton);

		string tutKey = GetKey() + "_" + GetStep();
		string tutMessage = Localization.Localize(tutKey);

		// show popup message
		Vector3 popupLoc = Constants.GetConstant<Vector3>("WellapadPopupLoc");

		Hashtable option = new Hashtable();
		option.Add(TutorialPopupFields.ShrinkBgToFitText, true);
		option.Add(TutorialPopupFields.Message, tutMessage);
		ShowPopup(Tutorial.POPUP_STD, popupLoc, option: option);
	}

	/////////

	private IEnumerator OpeningWellapad(){
		// destroy the spotlight we created for the button
		RemoveSpotlight();		
		
		// also, set the correct tutorial tasks in the wellapad
		WellapadMissionController.Instance.CreateTutorialPart1Missions();	
		
		// wait a frame for the new UI elements to create themselves
		yield return 0;
		
		// listen for when the wellapad is closed
		WellapadUIManager.Instance.OnManagerOpen += OnWellapadClosed;
	}

	private void RetentionPetClicked1(object sender, EventArgs args){
		retentionMinipet.GetComponent<MiniPet>().OnTutorialMinipetClicked -= RetentionPetClicked1;
//		RemoveSpotlight();
		RemoveFingerHint();
		Advance();
	}

	private void RetentionPetClicked2(object sender, EventArgs args){
		retentionMinipet.GetComponent<MiniPet>().OnTutorialMinipetClicked -= RetentionPetClicked2;
		RemoveSpotlight();
		RemoveFingerHint();
		RemovePopup();
		RemoveRetentionPet();
		Advance();
	}

	//---------------------------------------------------
	// ButtonClicked()
	// Callback for when the wellapad object is clicked;
	// this means we need to advance the tutorial.
	//---------------------------------------------------	
	private void ButtonClicked(object sender, EventArgs args){
		// stop listening for this event
		LgButton button = (LgButton)sender;
		button.OnProcessed -= ButtonClicked;
		
		// we have to allow the wellapad back button to be clicked
		GameObject goBack = WellapadUIManager.Instance.GetScreenManager().GetBackButton();
		AddToProcessList(goBack);

		// clean message and spotlight
		RemoveSpotlight();
		RemoveFingerHint();
		RemovePopup();
		RemoveRetentionPet();
		
		// go to the next step
		Advance();
	}
	
	//---------------------------------------------------
	// OnWellapadClosed()
	// Callback for when the wellapad is closed.
	//---------------------------------------------------	
	private void OnWellapadClosed(object sender, UIManagerEventArgs args){
		if(args.Opening == false){
			// wellapad is closing, so stop listening
			WellapadUIManager.Instance.OnManagerOpen -= OnWellapadClosed;
			
			// advance to next step
			Advance();
		}
	}	
}
