using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;

/// <summary>
/// Tutorial for showing the player what the wellapad is and how to access it
/// </summary>
public class GameTutorialWellapadIntro : GameTutorial{
	// wellapad button
	private GameObject missionButton = GameObject.Find("ButtonMissions");
	private GameObject retentionMinipet;
	private Vector3 minipetOldColliderSize;
	private Button button;
    private UnityAction buttonAction;

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
		retentionMinipet = GameObject.Find("Pebble");

		minipetOldColliderSize = retentionMinipet.GetComponent<BoxCollider>().size;

		// spotlight the minipet
		SpotlightObject(retentionMinipet, false,
		                fingerHint: true, fingerHintPrefab: "BedroomTutFingerPressDelay", focusOffsetY: 50f, fingerHintFlip: true, delay: 2f);

		retentionMinipet.GetComponent<MiniPet>().OnTutorialMinipetClicked += RetentionPetClicked1;
	}

	// Minipet introduction	TODO finish
	private IEnumerator FocusOnRetentionMinipet2(){
		yield return new WaitForSeconds(1f);

		// Keep the spotlight from last step so only show finger
		ShowFingerHint(retentionMinipet, flipX: true);

		// Show popup message
		Vector3 popupLoc = Constants.GetConstant<Vector3>("MinipetPopupLoc");
		ShowPopup(TUTPOPUPTEXT, popupLoc);
		
		ShowRetentionPet(false, new Vector3(208, -177, -160));
		retentionMinipet.GetComponent<BoxCollider>().size = new Vector3(18, 14, 1);	// increase the collider size
		retentionMinipet.GetComponent<MiniPet>().OnTutorialMinipetClicked += RetentionPetClicked2;
	}

	private void RetentionPetClicked1(object sender, EventArgs args){
		retentionMinipet.GetComponent<MiniPet>().OnTutorialMinipetClicked -= RetentionPetClicked1;
		//		RemoveSpotlight();
		RemoveFingerHint();
		Advance();
	}
	
	private void RetentionPetClicked2(object sender, EventArgs args){
		retentionMinipet.GetComponent<MiniPet>().OnTutorialMinipetClicked -= RetentionPetClicked2;
		retentionMinipet.GetComponent<BoxCollider>().size = minipetOldColliderSize;
		RemoveSpotlight();
		RemoveFingerHint();
		RemovePopup();
		RemoveRetentionPet();
		Advance();
	}

	/////////

	private void FocusWellapadButton(){
		// begin listening for when the button is clicked
		button = missionButton.GetComponent<Button>();
		buttonAction = new UnityAction(ButtonClicked);
		button.onClick.AddListener(buttonAction);
		Debug.Log("Added listener");

		// spotlight the wellapad
		SpotlightObject(missionButton, true, 
			fingerHint: true, fingerHintPrefab: "BedroomTutFingerPressDelay", fingerHintFlip: true, delay: 0.5f);

		TutorialManager.Instance.StartCoroutine(CreateWellapadButtonTutMessage());
		ShowRetentionPet(false, new Vector3(208, -177, -160));
	}

	//using this to deplay ShowPopup call for 0.5 seconds
	private IEnumerator CreateWellapadButtonTutMessage(){
		yield return new WaitForSeconds(0.5f);

		// the wellapad is the only object that can be clicked
		// only allow the button to be clicked after all the tutorial components
		// fade in
		AddToProcessList(missionButton);

		// show popup message
		Vector3 popupLoc = Constants.GetConstant<Vector3>("WellapadPopupLoc");
		ShowPopup(TUTPOPUPTEXT, popupLoc);
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

	//---------------------------------------------------
	// ButtonClicked()
	// Callback for when the wellapad object is clicked;
	// this means we need to advance the tutorial.
	//---------------------------------------------------	
	private void ButtonClicked(){
		button.onClick.RemoveListener(buttonAction);
		Debug.Log("Removed listener");

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
