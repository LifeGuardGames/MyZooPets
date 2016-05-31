using UnityEngine;
using System;
using System.Collections;

public class DoctorMatchTutorial : MinigameTutorial {
	public static string TUT_KEY = "DOCTORMATCH_TUT";
	private GameObject[] buttons;

	protected override void SetMaxSteps() {
		maxSteps = 4;
	}

	protected override void SetKey() {
		tutorialKey = TUT_KEY;
	}

	protected override void _End(bool isFinished) {
		DoctorMatchManager.Instance.NewGame();
		//DoctorMatchManager.Instance.BarFinger();
	}

	protected override void ProcessStep(int step) {
		switch (step) {
			case 0:
				DoctorMatchManager.Instance.SpawnFinger(0);
				DoctorMatchManager.Instance.assemblyLineController.SpawnTutorialSet(0);
				break;
			case 1:
				DoctorMatchManager.Instance.SpawnFinger(1);
				DoctorMatchManager.Instance.assemblyLineController.SpawnTutorialSet(1);
				break;
			case 2:
				DoctorMatchManager.Instance.SpawnFinger(2);
				DoctorMatchManager.Instance.assemblyLineController.SpawnTutorialSet(2);
				break;
			case 3:
				//DoctorMatchManager.Instance.StartMode(4)
				//assembyLine.SpawnRandomSet
			default:
				Debug.LogError("DoctorMatch tutorial has an unhandled step: " + step);
				break;
		}
	}
}
