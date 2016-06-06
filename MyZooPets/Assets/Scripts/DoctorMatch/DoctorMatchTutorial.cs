﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class DoctorMatchTutorial : MinigameTutorial {
	public static string TUT_KEY = "DOCTORMATCH_TUT";
	private DoctorMatchTutorialText tutorialText;

	protected override void SetMaxSteps() {
		maxSteps = 4;
	}

	protected override void SetKey() {
		tutorialKey = TUT_KEY;
	}

	protected override void _End(bool isFinished) {
		if (isFinished) {
			DoctorMatchManager.Instance.NewGame();
		} else {
			tutorialText.StartCoroutine(tutorialText.HideAll());
		}
	}

	protected override void ProcessStep(int step) {
		switch (step) {
			case 0: 
				tutorialText = GameObject.FindObjectOfType<DoctorMatchTutorialText>();
				tutorialText.ShowIntro();
				DoctorMatchManager.Instance.assemblyLineController.SpawnTutorialSet(0);
				break;
			case 1:
				DoctorMatchManager.Instance.assemblyLineController.SpawnTutorialSet(1);
				tutorialText.ShowStage(1f);
				break;
			case 2:
				DoctorMatchManager.Instance.assemblyLineController.SpawnTutorialSet(2);
				tutorialText.ShowStage(1f);
				break;
			case 3:
				DoctorMatchManager.Instance.assemblyLineController.PopulateQueue(false,7);
				tutorialText.ShowStage(1f);
				break;
			default:
				Debug.LogError("DoctorMatch tutorial has an unhandled step: " + step);
				break;
		}
	}
}
