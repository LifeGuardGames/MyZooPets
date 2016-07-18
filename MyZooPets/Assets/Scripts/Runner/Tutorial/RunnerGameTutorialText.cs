using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RunnerGameTutorialText : Singleton<RunnerGameTutorialText>{
	public GameObject stageTextObject;
	public Text topText;
	public Text bottomText;
	public GameObject outroTextObject;
	public GameObject itemTextObject;
	private int toShow = 0;

	public bool IsVisible{
		get{
			bool visible = false;
			foreach(TweenToggleDemux toggleDemux in GetComponentsInChildren<TweenToggleDemux>()){
				visible |= toggleDemux.IsShowing;
			}
			return visible;
		}
	}

	public void HideStage(){
		stageTextObject.GetComponent<TweenToggleDemux>().Hide();
		RunnerGameManager.Instance.ResumeGame();
		RunnerGameManager.Instance.SpecialInput = false;
		RunnerGameManager.Instance.AcceptInput = false;
	}

	public void ShowStage(){
		StartCoroutine(StageIEnum());
	}

	public void ShowOutro(){
		RunnerGameManager.Instance.PauseGame();
		outroTextObject.GetComponent<TweenToggleDemux>().Show();
		outroTextObject.GetComponentInChildren<Text>().text = Localization.Localize("RUNNER_TUT_OUTRO");
	}

	public void HideOutro(){
		outroTextObject.GetComponent<TweenToggleDemux>().Hide();
		StartCoroutine(UnpauseTutIEnum());
	}

	public void HideAll(){
		/*outroTextObject.GetComponent<TweenToggleDemux>().
		stageTextObject.GetComponent<TweenToggleDemux>().hideImmediately();
		itemTextObject.GetComponent<TweenToggleDemux>().hideImmediately();
		outroTextObject.GetComponent<TweenToggleDemux>().hideImmediately=true;
		stageTextObject.GetComponent<TweenToggleDemux>().hideImmediately=true;
		itemTextObject.GetComponent<TweenToggleDemux>().hideImmediately=true;*/
		Debug.Log("HIDE");
		outroTextObject.GetComponent<TweenToggleDemux>().Hide();
		stageTextObject.GetComponent<TweenToggleDemux>().Hide();
		itemTextObject.GetComponent<TweenToggleDemux>().Hide();


		StopAllCoroutines();
	}

	public void ShowItem(string toDisplay){
		RunnerGameManager.Instance.PauseGame();
		itemTextObject.GetComponent<TweenToggleDemux>().Show();
		itemTextObject.GetComponentInChildren<Text>().text = toDisplay;
	}

	public void HideItem(){
		itemTextObject.GetComponent<TweenToggleDemux>().Hide();
		StartCoroutine(UnpauseItemIEnum());
	}

	private IEnumerator UnpauseItemIEnum(){
		yield return new WaitForSeconds(.3f);
		RunnerGameManager.Instance.ResumeGame();
	}

	private IEnumerator UnpauseTutIEnum(){
		yield return new WaitForSeconds(.3f);
		RunnerGameManager.Instance.ResumeGame();
		RunnerGameManager.Instance.AdvanceTutorial();
	}

	private IEnumerator StageIEnum(){
		Debug.Log("SHOW");
		stageTextObject.GetComponent<TweenToggleDemux>().Show();
		if(toShow == 0){
			topText.text = Localization.Localize("RUNNER_TUT_" + toShow);
		}
		else{
			topText.text = "";
			bottomText.text = Localization.Localize("RUNNER_TUT_" + toShow);
		}
		RunnerGameManager.Instance.PauseGame();
		yield return new WaitForSeconds(.5f);
		RunnerGameManager.Instance.SpecialInput = true;
		toShow++;
	}

}
