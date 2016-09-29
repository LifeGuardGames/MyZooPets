using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Game tutorial flame. 
/// Explain to user how to use fire orb and blow fire.
/// </summary>
public class GameTutorialFlame : GameTutorial{

	public GameTutorialFlame() : base(){		
		FireMeter.OnMeterFilled += OnMeterFilled;					// set up callback for when the player fully charges their meter
		FireMeter.OnMeterStartFilling += RemoveFingerHint;
		PetAnimationManager.OnBreathEnded += FireBlowComplete;			// callback for when the pet finishes breathing fire
	}	
			
	protected override void SetMaxSteps(){
		maxSteps = 2;
	}

	protected override void SetKey(){
		tutorialKey = TutorialManagerBedroom.TUT_FLAME;
	}

	protected override void _End(bool isFinished){
	}
			
	protected override void ProcessStep(int step){
		// location of flame popups
		Vector3 flamePopupLoc = Constants.GetConstant<Vector3>("FlamePopup");
		switch(step){
		case 0:
			TutorialManager.Instance.StartCoroutine(FocusOnFlameButton());
			// show a little popup message telling the user to hold down the flame button
			ShowPopup(TUTPOPUPTEXT, flamePopupLoc);
			ShowRetentionPet(false, new Vector3(270, -186, -160));
			break;
		case 1:
			RemoveRetentionPet();

			// Create a custom message with the pet's name
			string petName = DataManager.Instance.GameData.PetInfo.PetName;
			string message = GetKey() + "_" + CurrentStep;
			string tutMessage = string.Format(Localization.Localize(message), petName);
			
			// show a little popup message telling the user to let go to breath fire
			ShowPopup(TUTPOPUPTEXT, flamePopupLoc, customMessage: tutMessage);
			GatingManager.Instance.StartCoroutine(RemovePopupDelay());

			// Check off the time that tutorial1 is done for next play period
			TutorialManagerBedroom tutorialManager = GameObject.Find("TutorialManager").GetComponent<TutorialManagerBedroom>();
			tutorialManager.CheckOffTutorial1DoneTime();
			break;
		}
	}

	/// <summary>
	/// Removes the popup delay.
	/// </summary>
	/// <returns>The popup delay.</returns>
	private IEnumerator RemovePopupDelay(){
		yield return new WaitForSeconds(1f);
		RemovePopup();	
	}

	/// <summary>
	/// Focuses on the flame button.
	/// This part of the tutorial highlights the flame
	/// button and tells the user to press and hold it.
	/// </summary>
	/// <returns>The on flame button.</returns>
	private IEnumerator FocusOnFlameButton(){
		// wait one frame so that the flame button can appear
		yield return 0;

		GameObject fireButton = FireButtonManager.Instance.FireButtonObject;

		SpotlightObject(fireButton, isGUI: true, hasFingerHint: true,
		                fingerState: BedroomTutFingerController.FingerState.Hold,
						spotlightOffsetY: 100f, fingerOffsetX: 0f, fingerOffsetY: -40f,
		                fingerHintFlip: true, delay: 0.5f);

		AddToProcessList(fireButton);
	}

	/// <summary>
	/// Raises the meter filled event.
	/// Callback for when the fire meter gets to 100%
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void OnMeterFilled(object sender, EventArgs args){
		// unsub from callback
		FireMeter.OnMeterFilled -= OnMeterFilled;
		
		// remove the spotlight so the user can see the resulting flame attack
		RemoveSpotlight();
		
		// fire meter is full, so advance the tut
		Advance();
	}

	/// <summary>
	/// When the meter starts filling up remove hint.
	/// </summary>
	private void RemoveFingerHint(object sender, EventArgs args){
		FireMeter.OnMeterStartFilling -= RemoveFingerHint;
		RemoveFingerHint();
	}

	/// <summary>
	/// Callback for when the pet finishes breathing fire.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void FireBlowComplete(object sender, EventArgs args){
		// unsub from callback
		PetAnimationManager.OnBreathEnded -= FireBlowComplete;
		
		// pet began to breath fire, so advance the tut
		Advance();
	}	
}
