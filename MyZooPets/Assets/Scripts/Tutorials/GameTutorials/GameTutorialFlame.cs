using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Explain to user how to use fireButton and blow fire
/// </summary>
public class GameTutorialFlame : GameTutorial {
	public GameTutorialFlame() : base() {
		FireMeter.OnMeterFilled += OnMeterFilled;               // set up callback for when the player fully charges their meter
		FireMeter.OnMeterStartFilling += RemoveFingerHint;
		PetAnimationManager.OnBreathEnded += FireBlowComplete;  // callback for when the pet finishes breathing fire
	}

	protected override void SetMaxSteps() {
		maxSteps = 2;
	}

	protected override void SetKey() {
		tutorialKey = TutorialManagerBedroom.TUT_FLAME;
	}

	protected override void _End(bool isFinished) {
	}

	protected override void ProcessStep(int step) {
		switch(step) {
			case 0:
				TutorialManager.Instance.StartCoroutine(FocusOnFlameButton());
				break;
			case 1:
				RemoveRetentionPet();

				// Create a custom message with the pet's name
				string petName = DataManager.Instance.GameData.PetInfo.PetName;
				string message = GetKey() + "_" + CurrentStep;
				string tutMessage = string.Format(Localization.Localize(message), petName);

				// show a little popup message telling the user to let go to breath fire
				ShowPopup(TUTPOPUPTEXT, new Vector3(150, 219, 0), customMessage: tutMessage);
				GatingManager.Instance.StartCoroutine(RemovePopupDelay());
				break;
		}
	}

	private IEnumerator RemovePopupDelay() {
		yield return new WaitForSeconds(1f);
		RemovePopup();
	}

	/// <summary>
	/// This part of the tutorial highlights the flame
	/// button and tells the user to press and hold it.
	/// </summary>
	private IEnumerator FocusOnFlameButton() {
		yield return 0;     // wait one frame so that the flame button can appear
		GameObject fireButton = FireButtonManager.Instance.FireButtonObject;
		SpotlightObject(goTarget: fireButton, hasFingerHint: true,
						fingerState: BedroomTutFingerController.FingerState.Hold,
						fingerOffsetY: 40f, delay: 0.5f);
		AddToProcessList(fireButton);

		// show a little popup message telling the user to hold down the flame button
		ShowPopup(TUTPOPUPTEXT, new Vector3(150, 219, 0));
		ShowRetentionPet(false, new Vector3(270, 14, 0));
	}

	/// <summary>
	/// When the meter starts filling up remove hint.
	/// </summary>
	private void RemoveFingerHint(object sender, EventArgs args) {
		FireMeter.OnMeterStartFilling -= RemoveFingerHint;
		TutorialManager.Instance.StartCoroutine(RemoveFingerInTwoFrames());
    }

	private IEnumerator RemoveFingerInTwoFrames() {
		// Wait for two frames, for finger above to finish spawning
		yield return 0;
		yield return 0;
		RemoveFingerHint();
	}

	/// <summary>
	/// Callback for when the fire meter gets to 100%
	/// </summary>
	private void OnMeterFilled(object sender, EventArgs args) {
		FireMeter.OnMeterFilled -= OnMeterFilled;   // unsub from callback
		RemoveSpotlight();      // remove the spotlight so the user can see the resulting flame attack
		Advance();              // fire meter is full, so advance the tut
	}
	
	/// <summary>
	/// Callback for when the pet finishes breathing fire.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void FireBlowComplete(object sender, EventArgs args) {
		// unsub from callback
		PetAnimationManager.OnBreathEnded -= FireBlowComplete;

		// pet began to breath fire, so advance the tut
		Advance();
	}
}
