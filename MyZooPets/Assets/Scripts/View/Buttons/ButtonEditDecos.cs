using UnityEngine;
using System.Collections;

//---------------------------------------------------
// ButtonEditDecos
// Button that enables the user to edit the decorations
// of their home/yard.
//---------------------------------------------------

public class ButtonEditDecos : LgButton {
	
	//---------------------------------------------------
	// ProcessClick()
	//---------------------------------------------------	
	protected override void ProcessClick() {
		// if we are currently in edit deco mode, close the UI, otherwise, open it
		DecoInventoryUIManager.Instance.OpenUI();
	}
}
