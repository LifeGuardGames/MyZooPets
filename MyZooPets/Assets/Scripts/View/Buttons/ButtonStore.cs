using UnityEngine;
using System.Collections;

//---------------------------------------------------
// ButtonStore
// Button that opens the store UI.
//---------------------------------------------------

public class ButtonStore : LgWorldButton {
	
	//---------------------------------------------------
	// ProcessClick()
	//---------------------------------------------------	
	protected override void ProcessClick() {
		StoreUIManager.Instance.OpenUI();
	}
}
