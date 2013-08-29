using UnityEngine;
using System.Collections;

//---------------------------------------------------
// ButtonBadges
// Button that opens the badge UI.
//---------------------------------------------------

public class ButtonBadges : LgButton {
	
	//---------------------------------------------------
	// ProcessClick()
	//---------------------------------------------------	
	protected override void ProcessClick() {
		BadgeUIManager.Instance.OpenUI();
	}
}
