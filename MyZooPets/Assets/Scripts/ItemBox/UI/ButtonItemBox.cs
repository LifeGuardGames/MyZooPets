using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// ButtonItemBox
// Opens the item box UI.
//---------------------------------------------------

public class ButtonItemBox : LgButton {	
	// logic script for this item box
	public ItemBoxLogic scriptLogic;
	
	//---------------------------------------------------
	// ProcessClick()
	//---------------------------------------------------		
	protected override void ProcessClick() {
		// open the box
		scriptLogic.OpenBox();
		
		// the box was opened...destroy ourselves
		Destroy(gameObject);
	}
}
