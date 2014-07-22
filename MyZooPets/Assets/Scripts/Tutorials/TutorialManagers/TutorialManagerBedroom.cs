using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Tutorial manager bedroom.
/// Responsible for
/// managing, ordering, instantiating, etc, all tutorials
/// that happen in this room.
/// </summary>
public class TutorialManagerBedroom : TutorialManager{
	// old and unused
	public const string TUT_INTRO = "IntroNotification";
	public const string TUT_CALENDAR = "FOCUS_CALENDAR";
	
	// currently used
	public const string TUT_INHALER = "FOCUS_INHALER";
	public const string TUT_SUPERWELLA_INHALER = "TUT_SUPERWELLA_INHALER";
	public const string TUT_WELLAPAD = "FOCUS_WELLAPAD";
	public const string TUT_CLAIM_FIRST = "CLAIM_FIRST_REWARD";
	public const string TUT_SMOKE_INTRO = "TUT_SMOKE_INTRO";
	public const string TUT_FLAME = "TUT_FLAME";
	public const string TUT_TRIGGERS = "TUT_TRIGGERS";
	public const string TUT_DECOS = "TUT_DECOS";

	// tutorial that's is not related to the intro tutorial 
	public const string TUT_FEED_PET = "TUT_FEED_PET";
	
	// last tutorial
	// TODO: need to be reviewed
	public const string TUT_LAST = TUT_DECOS;
//	public const string TUT_LAST = TUT_FLAME;
	
	protected override void _Start(){
		// listen for partition changing event; used for flame tutorial
		GatingManager.Instance.OnReachedGate += OnReachedGate;
		
		// do the first check for tutorials
		Check();
	}

	protected override void _Check(){
		//Tutorial 1
		TutorialPart1Check();

		//Tutorial 2
		TutorialPart2Check();
	}

	private void TutorialPart1Check(){
		bool isFocusInhalerTutorialDone = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TUT_INHALER);
		bool isFocusWellapadTutorialDone = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TUT_WELLAPAD);	
		bool isSmokeIntroDone = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TUT_SMOKE_INTRO);
		bool isSuperWellaInhalerDone = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TUT_SUPERWELLA_INHALER);

		if(!isFocusWellapadTutorialDone){
			// start by highlighting the wellapad button
			new GameTutorialWellapadIntro();
		}
		else if(!isFocusInhalerTutorialDone){
			// next check to see if the focus inhaler tutorial should display
			new GameTutorialFocusInhaler();
		}
		else if(!isSuperWellaInhalerDone){
			new GameTutorialSuperWellaInhaler();
		}
		else if(!isSmokeIntroDone){
			// play the smoke monster intro tutorial
			new GameTutorialSmokeIntro();
		}
		else{}
	}

	private void TutorialPart2Check(){
		bool isFlameTutorialDone = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TUT_FLAME);
		bool isTriggerTutorialDone = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TUT_TRIGGERS);
		bool isDecoTutorialDone = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TUT_DECOS);
		DateTime nextPlayPeriod = PlayPeriodLogic.Instance.NextPlayPeriod;

		if(LgDateTime.GetTimeNow() >= nextPlayPeriod){
			if(isFlameTutorialDone && !isTriggerTutorialDone &&
			   CameraManager.Instance.GetPanScript().currentPartition == 0){
				// play the trigger tutorial
				new GameTutorialTriggers();
			}
			else if(isFlameTutorialDone && !isDecoTutorialDone && 
			        CameraManager.Instance.GetPanScript().currentPartition == 0){
				// play the deco tutorial
				new GameTutorialDecorations();
			}
			else{}
		}
	}

	/// <summary>
	/// Intros the done.
	/// Callback for when the intro notification is clicked.
	/// </summary>
	private void IntroDone(){
		// mark the tut as viewed
		DataManager.Instance.GameData.Tutorial.ListPlayed.Add(TUT_INTRO);	
		
		// and check to see what the next tut should be
		Check();
	}

	public void OnReachedGate(object sender, EventArgs args){
		if(!isTutorialEnabled)
			return;
		
		bool isFlameTutorialDone = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TUT_FLAME);
		
		// if the player reached a gated room and has not yet seen the flame tutorial, start it
		if(!isFlameTutorialDone){
			// unsub from callback
			GatingManager.Instance.OnReachedGate -= OnReachedGate;
		
			// start the tut
			new GameTutorialFlame();
		}
	}
}
