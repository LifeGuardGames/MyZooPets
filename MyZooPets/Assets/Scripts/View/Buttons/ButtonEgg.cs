using UnityEngine;
using System.Collections;

//---------------------------------------------------
// ButtonEgg
// Button for the egg that appears when users first
// hatch their pet.
//---------------------------------------------------

public class ButtonEgg : LgButton {
	
	//---------------------------------------------------
	// ProcessClick()
	//---------------------------------------------------	
	protected override void ProcessClick() {
		// FirstTimeNGUI.Instance.OpenUI();
        CustomizationUIManager.Instance._OpenUI();
	}

}