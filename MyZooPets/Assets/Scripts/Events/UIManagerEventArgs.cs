using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//---------------------------------------------------
// UIManagerEventArgs
// Event arguments for when a UI manager opens or
// closes.
//---------------------------------------------------

public class UIManagerEventArgs : EventArgs {
	public bool Opening{get; set;}
}
