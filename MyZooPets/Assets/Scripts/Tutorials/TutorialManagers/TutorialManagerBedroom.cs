using UnityEngine;
using System;

/// <summary>
/// Responsible for managing, ordering, instantiating, etc, all tutorials
/// that happen in Bedroom scene
/// </summary>
public class TutorialManagerBedroom : TutorialManager {
	// Intro tutorials
	public const string TUT_INHALER = "FOCUS_INHALER";
	public const string TUT_SUPERWELLA_INHALER = "TUT_SUPERWELLA_INHALER";
	public const string TUT_WELLAPAD = "FOCUS_WELLAPAD";
	public const string TUT_SMOKE_INTRO = "TUT_SMOKE_INTRO";
	public const string TUT_FLAME_CRYSTAL = "TUT_FLAME_CRYSTAL";
	public const string TUT_FLAME = "TUT_FLAME";

	public const string TUT_LAST = TUT_FLAME;			// Reference to last intro tutorial

	// Misc tutorials that's not related to the intro tutorial
	public const string TUT_MOOD_DEGRADE = "TUT_TIME_DECAY";
	public const string TUT_FEED_PET = "TUT_FEED_PET";

	protected override void Awake() {
		base.Awake();
	}

	protected override void Start() {
		base.Start();
		bool isFocusWellapadTutorialDone = DataManager.Instance.GameData.Tutorial.IsTutorialFinished(TUT_WELLAPAD);
		bool isFlameTutorialDone = DataManager.Instance.GameData.Tutorial.IsTutorialFinished(TUT_FLAME);
		if(!GatingManager.Instance.HasActiveGate(1)) {
			isFlameTutorialDone = true;
			DataManager.Instance.GameData.Tutorial.ListPlayed.Add(TUT_FLAME);
        }
		// Reset the to at most introSmokeMonster if they have not completed tutorial 1 and came back
		if(isFocusWellapadTutorialDone && !isFlameTutorialDone) {
			DataManager.Instance.GameData.Tutorial.ListPlayed.Remove(TUT_SMOKE_INTRO);
			DataManager.Instance.GameData.Tutorial.ListPlayed.Remove(TUT_FLAME_CRYSTAL);
			DataManager.Instance.GameData.Tutorial.ListPlayed.Remove(TUT_FLAME);
		}

		// listen for partition changing event; used for flame tutorial
		GatingManager.Instance.OnReachedGate += OnReachedGate;

		// do the first check for tutorials
		IsPlayTutorial();
	}

	protected override bool IsPlayTutorial() {
		bool isChecking = base.IsPlayTutorial();
		if(isChecking) {
			//Tutorial 1
			TutorialPart1Check();
		}
		return isChecking;
	}

	// Everything in the first play period
	// NOTE: We took out part 2 (trigger and deco tut) for v3.0+
	private void TutorialPart1Check() {
		bool isFocusWellapadTutorialDone = DataManager.Instance.GameData.Tutorial.IsTutorialFinished(TUT_WELLAPAD);
		bool isFocusInhalerTutorialDone = DataManager.Instance.GameData.Tutorial.IsTutorialFinished(TUT_INHALER);
		bool isSuperWellaInhalerDone = DataManager.Instance.GameData.Tutorial.IsTutorialFinished(TUT_SUPERWELLA_INHALER);
		bool isSmokeIntroDone = DataManager.Instance.GameData.Tutorial.IsTutorialFinished(TUT_SMOKE_INTRO);
		bool isFlameCrystalTutorialDone = DataManager.Instance.GameData.Tutorial.IsTutorialFinished(TUT_FLAME_CRYSTAL);
		bool isFlameTutorialDone = DataManager.Instance.GameData.Tutorial.IsTutorialFinished(TUT_FLAME);

		// Play tutorials if they have not been completed yet
		if(!isFocusWellapadTutorialDone) {
			new GameTutorialWellapadIntro();
		}
		else if(!isFocusInhalerTutorialDone) {
			new GameTutorialFocusInhaler();
		}
		else if(!isSuperWellaInhalerDone) {
			new GameTutorialSuperWellaInhaler();
		}
		else if(!isSmokeIntroDone) {
			new GameTutorialSmokeIntro();
		}
		else if(isFlameCrystalTutorialDone && !isFlameTutorialDone) {
			new GameTutorialFlame();
		}
	}

	/// <summary>
	/// When the pet reaches the gate. check if flame crystal tutorial is done.
	/// This applies for existing user as well. The tutorial will be played if
	/// existing user hasn't seen flame crystal before. Only play this tutorial
	/// if there is a flame crystal in the inventory already
	/// </summary>
	private void OnReachedGate(object sender, EventArgs args) {
		bool isFlameCrystalTutorialDone = DataManager.Instance.GameData.Tutorial.IsTutorialFinished(TUT_FLAME_CRYSTAL);
		if(!isTutorialEnabled || isFlameCrystalTutorialDone) {
			return;
		}
		if(PlayPeriodLogic.Instance.IsFirstPlayPeriod()) {
			InventoryItem fireOrbItem = InventoryManager.Instance.GetItemInInventory("Usable1");
			if(fireOrbItem != null && !isFlameCrystalTutorialDone) {
				GatingManager.Instance.OnReachedGate -= OnReachedGate;
				new GameTutorialFlameCrystal();
			}
			else if(fireOrbItem == null) {  // Already complete, launch focus tutorial
				DataManager.Instance.GameData.Tutorial.ListPlayed.Add(TUT_FLAME_CRYSTAL);   // Hacky add so it passes future check
				new GameTutorialFlame();
			}
			else {
				Debug.LogError("Error with trying to reset tutorial");
			}
		}
	}
}
