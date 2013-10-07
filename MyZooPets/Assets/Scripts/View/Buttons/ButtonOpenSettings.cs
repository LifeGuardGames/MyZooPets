using UnityEngine;
using System.Collections;

//---------------------------------------------------
// ButtonOpenLanguage
// Button that opens a panel where the user can
// choose what language they would like used.
//---------------------------------------------------

public class ButtonOpenSettings : LgButton {
	
	//---------------------------------------------------
	// ProcessClick()
	//---------------------------------------------------	
	protected override void ProcessClick() {
		print ("SFSDF");	// DOESNT GET CALLED THE SECOND TIME
		SettingsUIManager.Instance.OpenUI();
	}
}