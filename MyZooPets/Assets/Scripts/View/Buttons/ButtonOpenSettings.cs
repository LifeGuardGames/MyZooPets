using UnityEngine;
using System.Collections;

//---------------------------------------------------
// ButtonOpenLanguage
// Button that opens a panel where the user can
// choose what language they would like used.
//---------------------------------------------------

public class ButtonOpenSettings : LgWorldButton {
	
	//---------------------------------------------------
	// ProcessClick()
	//---------------------------------------------------	
	protected override void ProcessClick() {
		SettingsUIManager.Instance.OpenUI();
	}
}