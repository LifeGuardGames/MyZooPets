using UnityEngine;
using System.Collections;

//---------------------------------------------------
// ButtonNote
// Button that opens the note UI.
//---------------------------------------------------

public class ButtonNote : LgButton {
	
	//---------------------------------------------------
	// ProcessClick()
	//---------------------------------------------------	
	protected override void ProcessClick() {
		NoteUIManager.Instance.OpenUI();
	}
}
