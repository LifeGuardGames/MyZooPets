using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class RunnerTutorial : MinigameTutorial {
	public static string TUT_KEY = "DOCTORMATCH_TUT";
	private RunnerGameTutorialText tutorialText;
	private IEnumerator waitAdvance;

	protected override void SetMaxSteps() {
		maxSteps = 3;
	}

	protected override void SetKey() {
		tutorialKey = TUT_KEY;
	}

	protected override void _End(bool isFinished) {
		if (isFinished) {
			RunnerGameManager.Instance.NewGame();
		} else {
			if (PlayerController.OnJump != null)
				PlayerController.OnJump -= TutorialJump;
			if (PlayerController.OnDrop != null)
				PlayerController.OnDrop -= TutorialDrop;
			if (waitAdvance != null)
				RunnerGameManager.Instance.StopCoroutine(waitAdvance);
			RunnerGameManager.Instance.AcceptInput = true;
			RunnerGameManager.Instance.SpecialInput = false;
			tutorialText.StartCoroutine(tutorialText.HideAll());
		}
	}

	protected override void ProcessStep(int step) {
		switch (step) {
			case 0:
				tutorialText = GameObject.FindObjectOfType<RunnerGameTutorialText>();
				tutorialText.ShowStage();
				PlayerController.OnJump += TutorialJump;
				RunnerGameManager.Instance.AcceptInput = true;
				break;
			case 1:
				tutorialText.ShowStage();
				PlayerController.OnDrop += TutorialDrop;
				RunnerGameManager.Instance.AcceptInput = true;
				break;
			case 2:
				RunnerGameManager.Instance.AcceptInput = true;
				tutorialText.ShowOutro();
				break;
			default:
				Debug.LogError("RunnerGame tutorial has an unhandled step: " + step);
				break;
		}
	}

	private void TutorialJump(object sender, EventArgs args) {
		PlayerController.OnJump -= TutorialJump;
		tutorialText.HideStage();
		waitAdvance = WaitBeforeAdvance();
		RunnerGameManager.Instance.StartCoroutine(waitAdvance);
	}

	private void TutorialDrop(object sender, EventArgs args) {
		PlayerController.OnDrop -= TutorialDrop;
		tutorialText.HideStage();
		waitAdvance = WaitBeforeAdvance();
		RunnerGameManager.Instance.StartCoroutine(waitAdvance);
	}

	private IEnumerator WaitBeforeAdvance() {
		yield return new WaitForSeconds(1.1f);
		Advance();
	}
}
