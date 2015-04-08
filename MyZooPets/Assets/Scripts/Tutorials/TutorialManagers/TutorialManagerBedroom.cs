﻿using UnityEngine;
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
	// currently used
	public const string TUT_INHALER = "FOCUS_INHALER";
	public const string TUT_SUPERWELLA_INHALER = "TUT_SUPERWELLA_INHALER";
	public const string TUT_WELLAPAD = "FOCUS_WELLAPAD";
	public const string TUT_SMOKE_INTRO = "TUT_SMOKE_INTRO";
	public const string TUT_FLAME_CRYSTAL = "TUT_FLAME_CRYSTAL"; //new tutorial key introduced in v1.3.1
	public const string TUT_FLAME = "TUT_FLAME";
	public const string TUT_TRIGGERS = "TUT_TRIGGERS";
	public const string TUT_DECOS = "TUT_DECOS";
	public const string TUT_MOOD_DEGRADE = "TUT_TIME_DECAY";

	// tutorial that's is not related to the intro tutorial 
	public const string TUT_FEED_PET = "TUT_FEED_PET";
	
	// last tutorial
	public const string TUT_LAST = TUT_DECOS;

	protected override void Awake(){
		base.Awake();
	}

	protected override void Start(){
		base.Start();
		// listen for partition changing event; used for flame tutorial
		GatingManager.Instance.OnReachedGate += OnReachedGate;
		
		// do the first check for tutorials
		IsPlayTutorial();
	}

	protected override bool IsPlayTutorial(){
		bool isChecking = base.IsPlayTutorial();
		if(isChecking){
			//Tutorial 1
			TutorialPart1Check();

			//Tutorial 2
			TutorialPart2Check();
		}

		return isChecking;
	}

	void OnApplicationPause(bool isPaused){
		if(!isPaused){
			TutorialPart2Check();
		}
	}

	private void TutorialPart1Check(){
		bool isFocusInhalerTutorialDone = DataManager.Instance.GameData.Tutorial.IsTutorialFinished(TUT_INHALER);
		bool isFocusWellapadTutorialDone = DataManager.Instance.GameData.Tutorial.IsTutorialFinished(TUT_WELLAPAD);	
		bool isSmokeIntroDone = DataManager.Instance.GameData.Tutorial.IsTutorialFinished(TUT_SMOKE_INTRO);
		bool isFlameCrystalTutorialDone = DataManager.Instance.GameData.Tutorial.IsTutorialFinished(TUT_FLAME_CRYSTAL);
		bool isFlameTutorialDone = DataManager.Instance.GameData.Tutorial.IsTutorialFinished(TUT_FLAME);
		bool isSuperWellaInhalerDone = DataManager.Instance.GameData.Tutorial.IsTutorialFinished(TUT_SUPERWELLA_INHALER);

		//check why we need isFirstTime variable

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
		else if(isFlameCrystalTutorialDone && !isFlameTutorialDone){
			new GameTutorialFlame();
		}
	}

	public void CheckOffTutorial1DoneTime(){
		DataManager.Instance.GameData.Tutorial.Tutorial1DonePlayPeriod = PlayPeriodLogic.GetCurrentPlayPeriod();
	}

	private void TutorialPart2Check(){	// TODO this can be refactored, this is checked everytime you switch rooms....
		bool isFlameTutorialDone = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TUT_FLAME);
		bool isTriggerTutorialDone = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TUT_TRIGGERS);
		bool isDecoTutorialDone = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TUT_DECOS);
		DateTime LastTutorial1DonePlayPeriod = DataManager.Instance.GameData.Tutorial.Tutorial1DonePlayPeriod;
		
		Debug.Log("---- CHECKING PLAY PERIOD ----- " + PlayPeriodLogic.GetCurrentPlayPeriod() + " " + LastTutorial1DonePlayPeriod);

		if(PlayPeriodLogic.GetCurrentPlayPeriod() > LastTutorial1DonePlayPeriod){
			if(isFlameTutorialDone && !isTriggerTutorialDone &&
			   CameraManager.Instance.PanScript.currentPartition == 0){
//				Debug.Log("PASS");
				// play the trigger tutorial
				new GameTutorialTriggers();
			}
			else if(isFlameTutorialDone && !isDecoTutorialDone && 
			        CameraManager.Instance.PanScript.currentPartition == 0){
				// play the deco tutorial
				new GameTutorialDecorations();
			}
			else{
//				Debug.Log("FAIL");
			}
		}
	}

	/// <summary>
	/// When the pet reaches the gate. check if flame crystal tutorial is done.
	/// This applies for existing user as well. The tutorial will be played if
	/// existing user hasn't seen flame crystal before. Only play this tutorial
	/// if there is a flame crystal in the inventory already
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void OnReachedGate(object sender, EventArgs args){
		if(!isTutorialEnabled)
			return;
		bool isFlameCrystalTutorialDone = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TUT_FLAME_CRYSTAL);
		GameObject fireOrbReference = InventoryUIManager.Instance.GetFireOrbReference();

		if(fireOrbReference != null && !isFlameCrystalTutorialDone){
			GatingManager.Instance.OnReachedGate -= OnReachedGate;

			new GameTutorialFlameCrystal();
		}
	}
}
