using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// LgLwfCutscene
// Script on an LWF cutscene.
//---------------------------------------------------

public class LgLwfCutscene : LWFAnimator {
	// cutscene to play -- must match the movie clip name
	// currently not used -- implement if we do more cutscenes
	public string strCutscene;
	
	//=======================Events========================
	public static EventHandler<EventArgs> OnCutsceneDone;
	//=====================================================		
	
	//---------------------------------------------------
	// ClipFinished()
	// Callback for when a cutscene finishes playing.
	//---------------------------------------------------	
	protected override void ClipFinished() {
		if( OnCutsceneDone != null )
    		OnCutsceneDone(this, EventArgs.Empty);			
	}
}
