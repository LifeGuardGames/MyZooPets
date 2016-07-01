using UnityEngine;

public class DoctorMatchTutorial : MinigameTutorial {
	private DoctorMatchTutorialText tutorialText;

	protected override void SetKey() {
		tutorialKey = "DOCTORMATCH_TUT";
	}

	protected override void SetMaxSteps() {
		maxSteps = 4;
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
