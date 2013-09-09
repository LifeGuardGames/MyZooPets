using UnityEngine;
using System.Collections;

//---------------------------------------------------
// ButtonOpenLanguage
// Button that opens a panel where the user can
// choose what language they would like used.
//---------------------------------------------------

public class ButtonOpenLanguages : LgButton {
	
	//---------------------------------------------------
	// ProcessClick()
	//---------------------------------------------------	
	protected override void ProcessClick() {
		ChooseLanguageUIManager.Instance.OpenUI();
	}
}