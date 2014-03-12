using UnityEngine;
using System.Collections;

//---------------------------------------------------
// ButtonChangeLanguage
// Button for changing the language setting of the
// game.
//---------------------------------------------------

public class ButtonChangeLanguage : LgButton {
	
	// language key
	public string strLanguageKey;
	
	//---------------------------------------------------
	// ProcessClick()
	//---------------------------------------------------	
	protected override void ProcessClick() {
		SettingsUIManager.Instance.SetLocalization(strLanguageKey);
	}
}