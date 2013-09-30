using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//---------------------------------------------------
// TutorialEndEventArgs
// Event arguments for when a tutorial ends.
//---------------------------------------------------

public class TutorialEndEventArgs : EventArgs {
	// did the tutorial actually get finished?
	private bool bFinished;
	public bool DidFinish() {
		return bFinished;	
	}
	
	public TutorialEndEventArgs( bool bFinished ) {
		this.bFinished = bFinished;	
	}
}