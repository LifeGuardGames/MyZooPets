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
	
	public GameTutorialWellapadIntro() : base(){	
	}

	protected override void SetMaxSteps(){
		maxSteps = 2;
	}

	protected override void SetKey(){
		tutorialKey = TutorialManagerBedroom.TUT_WELLAPAD;
	}

	protected override void _End(bool isFinished){
	}

	protected override void ProcessStep(int step){
		switch(step){
		case 0:
			// start by focusing on the wellapad button
			FocusWellapadButton();
			break;
			
		case 1:
			TutorialManager.Instance.StartCoroutine(OpeningWellapad());
			break;
		}
	}
	
	//---------------------------------------------------
	// OpeningWellapad()
	//---------------------------------------------------		
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
	
	private void FocusWellapadButton(){
		// begin listening for when the button is clicked
		LgButton button = goWellapadButton.GetComponent<LgButton>();
		button.OnProcessed += ButtonClicked;
		
		// spotlight the wellapad
		SpotlightObject(goWellapadButton, true, InterfaceAnchors.BottomLeft, 
			fingerHint: true, fingerHintPrefab: "PressTutWithDelay", fingerHintFlip: true, delay: 2f);

		TutorialManager.Instance.StartCoroutine(CreateWellapadButtonTutMessage());
	}

	//using this to deplay ShowPopup call for 2 seconds
	private IEnumerator CreateWellapadButtonTutMessage(){
		yield return new WaitForSeconds(2f);

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
