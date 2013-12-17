using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// TutorialManager_Bedroom
// Tutorial manager for the bedroom.  Responsible for
// managing, ordering, instantiating, etc, all tutorials
// that happen in this room.
//---------------------------------------------------

public class TutorialManager_Bedroom : TutorialManager {
	// tutorial consts
	
	// old and unused
	public const string TUT_INTRO = "IntroNotification";
	public const string TUT_CALENDAR = "FOCUS_CALENDAR";
	
	// currently used
	public const string TUT_INHALER = "FOCUS_INHALER";
	public const string TUT_WELLAPAD = "FOCUS_WELLAPAD";
	public const string TUT_CLAIM_FIRST = "CLAIM_FIRST_REWARD";
	public const string TUT_SMOKE_INTRO = "TUT_SMOKE_INTRO";
	public const string TUT_FLAME = "TUT_FLAME";
	public const string TUT_TRIGGERS = "TUT_TRIGGERS";
	public const string TUT_DECOS = "TUT_DECOS";
	
	// last tutorial
	public const string TUT_LAST = "FOCUS_NINJA";
	
	//---------------------------------------------------
	// _Start()
	//---------------------------------------------------	
	protected override void _Start() {
		// listen for partition changing event; used for flame tutorial
		GatingManager.Instance.OnReachedGate += OnReachedGate;
		
		// do the first check for tutorials
		Check();
	}
	
	//---------------------------------------------------
	// _Check()
	//---------------------------------------------------		
	protected override void _Check() {
		// old and unused
		//bool bIntro = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains( TUT_INTRO );
		//bool bFocusCalendar = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains( TUT_CALENDAR );
		
		bool bFocusInhaler = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains( TUT_INHALER );
		bool bFocusWellapad = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains( TUT_WELLAPAD );	
		//bool bClaimFirst = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains( TUT_CLAIM_FIRST ); 
		bool bTriggers = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains( TUT_TRIGGERS );
		bool bSmokeIntro = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains( TUT_SMOKE_INTRO );
		bool bDecos = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains( TUT_DECOS );
		bool bFlameTut = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains( TUT_FLAME );
		
		// these tutorials occur in quick succession
		if ( !bFocusWellapad ) {
			// start by highlighting the wellapad button
			new GameTutorial_WellapadIntro();
		}
		else if ( !bFocusInhaler )  {
			// next check to see if the focus inhaler tutorial should display
			new GameTutorial_FocusInhaler();
		}			
		else if ( !bSmokeIntro ) {
			// play the smoke monster intro tutorial
			new GameTutorial_SmokeIntro();
		}
		else if ( bFlameTut && !bTriggers && CameraManager.Instance.GetPanScript().currentPartition == 1 ) {
			// play the trigger tutorial
			new GameTutorial_Triggers();
		}		
		else if ( bFlameTut && !bDecos && CameraManager.Instance.GetPanScript().currentPartition == 1 ) {
			// play the deco tutorial
			new GameTutorial_Decorations();
		}
	}
	
	//---------------------------------------------------
	// IntroDone()
	// Callback for when the intro notification is clicked.
	//---------------------------------------------------	
	private void IntroDone() {
		// mark the tut as viewed
		DataManager.Instance.GameData.Tutorial.ListPlayed.Add( TUT_INTRO );	
		
		// and check to see what the next tut should be
		Check();
	}
	
	//---------------------------------------------------
	// OnReachedGate()
	//---------------------------------------------------	
	public void OnReachedGate( object sender, EventArgs args ) {
		if ( !bOn )
			return;
		
		bool bFlameTut = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains( TUT_FLAME );
		
		// if the player reached a gated room and has not yet seen the flame tutorial, start it
		if ( !bFlameTut ) {
			// unsub from callback
			GatingManager.Instance.OnReachedGate -= OnReachedGate;
		
			// start the tut
			new GameTutorial_Flame();
		}
	}
}
