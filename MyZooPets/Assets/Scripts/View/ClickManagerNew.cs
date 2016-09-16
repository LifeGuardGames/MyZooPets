using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles the clickability of the elements (UI and world objects) in the zones
/// 
/// 1. Handles lock states for tutorials
/// </summary>
public class ClickManagerNew : MonoBehaviour {
	public enum TutorialLockState {
		None,
		WellapadButton,
		WellapadBackButton,
		InhalerButton,
		InhalerDoneWellapadBackButton,
		SmokeMonsterRoomArrow,
		FireCrystalDrag,
		FireButtonCharge,
	}

	public List<Button> allButtonsList;			// Track all the buttons here so they can be click locked

	private TutorialLockState currentTutLockState = TutorialLockState.None;

	public void SetCurrentTutLockState(TutorialLockState tutLockState){
		switch(tutLockState){
		// Set exceptions for UI via gameobject names
		case TutorialLockState.WellapadButton:
			ToggleAllUIButtons(false, "ButtonMissions");
			break;
		case TutorialLockState.WellapadBackButton:
			ToggleAllUIButtons(false, "MissionsBackButton");
			break;
		case TutorialLockState.InhalerButton:
			ToggleAllUIButtons(false);
			break;
		case TutorialLockState.InhalerDoneWellapadBackButton:
			ToggleAllUIButtons(false);
			break;
		case TutorialLockState.SmokeMonsterRoomArrow:
			ToggleAllUIButtons(false);
			break;
		case TutorialLockState.FireCrystalDrag:
			ToggleAllUIButtons(false);
			break;
		case TutorialLockState.FireButtonCharge:
			ToggleAllUIButtons(false);
			break;
		default:		// None state, turn everything on
			ToggleAllUIButtons(true);
			break;
		}
	}

	/// <summary>
	/// Turns all the buttons in allButtonsList on or off with exception
	/// </summary>
	/// <param name="isOn">Toggle everything on here</param>
	/// <param name="exceptionButtonName">If the string matches button name, toggle opposite of everything else</param>
	private void ToggleAllUIButtons(bool isOn, string exceptionButtonName = null){
		bool buttonSetCheck = false;
		foreach(Button button in allButtonsList) {
			button.enabled = isOn;
			if(exceptionButtonName != null && button.name == exceptionButtonName) {
				if(buttonSetCheck){
					// Make sure all buttons have different names
					Debug.LogError("Button shares same name for toggleCheck, should be unique: " + button.name);
				}
				else{
					// Set it the first time
					buttonSetCheck = true;
				}
			}
		}
		if(exceptionButtonName != null && !buttonSetCheck){
			Debug.LogError("Did not find exception in button list");
		}
	}
}
