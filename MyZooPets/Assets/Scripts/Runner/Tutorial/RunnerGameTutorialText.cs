using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RunnerGameTutorialText : Singleton<RunnerGameTutorialText> {
	public Text topText;
	public Text bottomText;
	public GameObject outroTextObject;
	private int toShow = 0;

	public bool IsVisible {
		get {
			bool visible = false;
			foreach(TweenToggleDemux toggleDemux in GetComponentsInChildren<TweenToggleDemux>()) {
				visible |= toggleDemux.IsShowing;
			}
			return visible;
		}
	}

	public void HideStage() {
		RunnerGameManager.Instance.ResumeGame();
		RunnerGameManager.Instance.SpecialInput = false;
		RunnerGameManager.Instance.AcceptInput = false;
	}

	public void ShowStage() {
		StartCoroutine(StageIEnum());
	}

	public void ShowOutro() {
		RunnerGameManager.Instance.PauseGame();
		outroTextObject.GetComponent<TweenToggleDemux>().Show();
		topText.gameObject.SetActive(false);
		bottomText.gameObject.SetActive(false);
		outroTextObject.GetComponentInChildren<Text>().text = Localization.Localize("RUNNER_TUT_OUTRO");
	}

	public void HideOutro() {
		outroTextObject.GetComponent<TweenToggleDemux>().Hide();
		StartCoroutine(UnpauseTutIEnum());
	}

	public void HideAll() {
		outroTextObject.GetComponent<TweenToggleDemux>().Hide();
		topText.gameObject.SetActive(false);
		bottomText.gameObject.SetActive(false);
		StopAllCoroutines();
	}

	private IEnumerator UnpauseTutIEnum() {
		yield return new WaitForSeconds(.3f);
		RunnerGameManager.Instance.ResumeGame();
		RunnerGameManager.Instance.AdvanceTutorial();
	}

	private IEnumerator StageIEnum() {
		if(toShow == 0) {
			topText.gameObject.SetActive(true);
			bottomText.gameObject.SetActive(false);
			topText.text = Localization.Localize("RUNNER_TUT_" + toShow);
		}
		else {
			topText.gameObject.SetActive(false);
			bottomText.gameObject.SetActive(true);
			bottomText.text = Localization.Localize("RUNNER_TUT_" + toShow);
		}
		RunnerGameManager.Instance.PauseGame();
		yield return new WaitForSeconds(.5f);
		RunnerGameManager.Instance.SpecialInput = true;
		toShow++;
	}
}
