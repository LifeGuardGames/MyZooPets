using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Tutorial to alert the user to use their inhaler.
/// </summary>
public class GameTutorialFocusInhaler : GameTutorial{
	// inhaler object
	private GameObject goInhaler = GameObject.Find("Entrance_Inhaler");
	
	public GameTutorialFocusInhaler() : base(){	
	}

	protected override void SetMaxSteps(){
		maxSteps = 2;
	}

	protected override void SetKey(){
		tutorialKey = TutorialManagerBedroom.TUT_INHALER;
	}

	protected override void _End(bool isFinished){
		// note that this tutorial never actually ends, because it just goes into the inhaler game
	}

	protected override void ProcessStep(int step){
		switch(step){
		case 0:
			// the start of the focus inhaler tutorial
			FocusInhaler();
			ShowRetentionPet(true, new Vector3(-281, -86, -160));
			break;
			
		case 1:
			// destroy the spotlight we created for the inhaler
			RemoveSpotlight();
			RemoveFingerHint();
			RemovePopup();
			RemoveRetentionPet();
			break;
		}
	}

	private void FocusInhaler(){
		// begin listening for when the inhaler is clicked
		LgWorldButton button = goInhaler.GetComponent<LgWorldButton>();
		button.OnProcessed += InhalerClicked;
		
		// the inhaler is the only object that can be clicked
		AddToProcessList(goInhaler);
	
		// spotlight the inhaler
		SpotlightObject(goInhaler, hasFingerHint: true, fingerHintFlip: true);

		TutorialManager.Instance.StartCoroutine(CreateFocusInhalerTutMessage());
	}

	private IEnumerator CreateFocusInhalerTutMessage(){
		yield return new WaitForSeconds(0.5f);

		// Create a custom message with the pet's name
		string tutKey = GetKey() + "_" + CurrentStep;
		string petName = DataManager.Instance.GameData.PetInfo.PetName;
		string message = string.Format(Localization.Localize(tutKey), StringUtils.FormatStringPossession(petName));
		
		Vector3 popupLoc = Constants.GetConstant<Vector3>("InhalerPopupLoc");
		
		ShowPopup(TUTPOPUPTEXT, popupLoc, customMessage: message);
	}

	/// <summary>
	/// Callback for when the Inhaler object is clicked clicked.
	/// This means we need to advnace the tutorial
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void InhalerClicked(object sender, EventArgs args){
		// stop listening for this event
		LgWorldButton button = (LgWorldButton)sender;
		button.OnProcessed -= InhalerClicked;
		
		// go to the next step
		Advance();
	}
}
