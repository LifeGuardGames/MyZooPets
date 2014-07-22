using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Game tutorial flame. 
/// Explain to user how to use fire orb and blow fire.
/// </summary>
public class GameTutorialFlame : GameTutorial{
	GameObject fireOrbFingerHint;

	public GameTutorialFlame() : base(){		
		FireMeter.OnMeterFilled += OnMeterFilled;			// set up callback for when the player fully charges their meter
		FireMeter.OnMeterStartFilling += OnMeterStartFilling;
		PetAnimator.OnBreathEnded += OnBreathEnded;			// callback for when the pet finishes breathing fire
	}	
			
	protected override void SetMaxSteps(){
		maxSteps = 3;
	}

	protected override void SetKey(){
		tutorialKey = TutorialManagerBedroom.TUT_FLAME;
	}

	protected override void _End(bool isFinished){
	}
			
	protected override void ProcessStep(int step){
		// location of flame popups
		Vector3 flamePopupLoc = Constants.GetConstant<Vector3>("FlamePopup");
		Hashtable option = new Hashtable();

		//Tutorial popup options 
		option.Add(TutorialPopupFields.ShrinkBgToFitText, true);

		switch(step){
		case 0:
			TutorialManager.Instance.StartCoroutine(DragFireOrbHint());

			break;
		case 1:

			TutorialManager.Instance.StartCoroutine(FocusOnFlameButton());
			
			// show a little popup message telling the user to hold down the flame button
			ShowPopup(Tutorial.POPUP_STD, flamePopupLoc, useViewPort: false, option: option);
			break;
		case 2:
			string petName = DataManager.Instance.GameData.PetInfo.PetName;
			string stringKey = GetKey() + "_" + GetStep();
			string tutMessage = String.Format(Localization.Localize(stringKey), petName);
			
			option.Add(TutorialPopupFields.Message, tutMessage);
			
			// show a little popup message telling the user to let go to breath fire
			ShowPopup(Tutorial.POPUP_STD, flamePopupLoc, useViewPort: false, option: option);
			GatingManager.Instance.StartCoroutine(RemovePopupDelay());
			break;
		}
	}

	private IEnumerator DragFireOrbHint(){
		yield return new WaitForSeconds(0.5f);

		//add fire orb to the clickable list
		FireButtonUIManager.FireButtonActive += FireButtonActiveEventHandler;
		GameObject fireOrbItemReference = InventoryUIManager.Instance.GetFireOrbReference();
		GameObject fireButtonReference = FireButtonUIManager.Instance.GetFireButtonReference();
		Vector3 fireOrbItemPosition = LgNGUITools.GetScreenPosition(fireOrbItemReference);
		Vector3 fireButtonPosition = LgNGUITools.GetScreenPosition(fireButtonReference);
		
		AddToProcessList(fireOrbItemReference);

		fireOrbFingerHint = LgNGUITools.AddChildWithPosition(GameObject.Find("Anchor-BottomRight"),
		                                                  (GameObject)Resources.Load("FireOrbFingerHint"));

		// set the hint to the right spawn location
		Vector3 hintPosition = fireOrbItemPosition;
		hintPosition.z = fireOrbFingerHint.transform.localPosition.z;
		fireOrbFingerHint.transform.localPosition = hintPosition;

		
		fireButtonPosition = CameraManager.Instance.TransformAnchorPosition(fireButtonPosition, 
		                                                                    InterfaceAnchors.Center, 
		                                                                    InterfaceAnchors.BottomRight);
		fireButtonPosition.z = fireOrbFingerHint.transform.localPosition.z;

		Hashtable optional = new Hashtable();
		optional.Add("repeat", 0);
		LeanTween.moveLocal(fireOrbFingerHint, fireButtonPosition, 3f, optional);
	}

	/// <summary>
	/// When the fire button is active advance tutorial.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void FireButtonActiveEventHandler(object sender, EventArgs args){
		FireButtonUIManager.FireButtonActive -= FireButtonActiveEventHandler;

		// clean up tween from last step
		LeanTween.cancel(fireOrbFingerHint);
		GameObject.Destroy(fireOrbFingerHint);

		Advance();
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
		
		// find and spotlight the fire button
		GameObject goFlameButton = GameObject.Find(ButtonMonster.FIRE_BUTTON);
		if(goFlameButton != null){
			SpotlightObject(goFlameButton, true, InterfaceAnchors.Center, 
			                "TutorialSpotlightFlameButton", fingerHint: true, 
			                fingerHintPrefab: "PressHoldTut", fingerHintOffsetY: 0f, 
			                fingerHintFlip: true, delay: 0.5f);
			
			// add the fire button to the processable list
			// this is kind of annoying...we actually want to add the child object, because the parent object is empty...
			GameObject goButton = goFlameButton.transform.Find("ButtonParent/Button").gameObject;
			AddToProcessList(goButton);
		}
		else
			Debug.LogError("No flame button...that means the game is going to break");
	}

//	private IEnumerator TutorialEndMessage(){
//		yield return new WaitForSeconds(1f);
//
//		// since this is the last tutorial, show a little notification
//		string strKey = "TUTS_FINISHED";											// key of text to show
//		string strImage = Constants.GetConstant<string>("Tutorial_Finished");		// image to appear on notification
//		string strAnalytics = "";														// analytics tracker
//		
//		// show the standard popup
//		string petName = DataManager.Instance.GameData.PetInfo.PetName;
//		TutorialUIManager.AddStandardTutTip(NotificationPopupType.TipWithImage, 
//		                                    String.Format(Localization.Localize(strKey), 
//		              StringUtils.FormatStringPossession(petName)),
//		                                    strImage, null, true, true, strAnalytics);
//		
//		GameObject wellapadButton = (GameObject)GameObject.Find("WellapadButton");
//		if(wellapadButton != null){
//			ButtonWellapad buttonWellapadScript = wellapadButton.GetComponent<ButtonWellapad>();
//			buttonWellapadScript.SetListenersToWellapadMissionController();
//		}
//		else{
//			Debug.LogError("wellapad button can't be found: " + this);
//		}
//	}

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
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void OnMeterStartFilling(object sender, EventArgs args){
		FireMeter.OnMeterStartFilling -= OnMeterStartFilling;

		RemoveFingerHint();
	}

	/// <summary>
	/// Callback for when the pet finishes breathing fire.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void OnBreathEnded(object sender, EventArgs args){
		// unsub from callback
		PetAnimator.OnBreathEnded -= OnBreathEnded;
		
		// at this point the user has done battle with the smoke monster, so I'm going to call highlight on a bogus task here just to unhighlight everything
		WellapadMissionController.Instance.HighlightTask("");
		
		// pet began to breath fire, so advance the tut
		Advance();
	}	
}
