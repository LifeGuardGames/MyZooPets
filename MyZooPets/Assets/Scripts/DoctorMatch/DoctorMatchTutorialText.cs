using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DoctorMatchTutorialText : Singleton<DoctorMatchTutorialText> {
	public GameObject introTextObject;
	public GameObject stageTextObject;
	private int toShow = 0;
	private float showTime = 0;
	private float minShow = 1.5f;

	public IEnumerator HideAll(){
		StopAllCoroutines();
		Debug.Log("Called");
		introTextObject.GetComponent<TweenToggleDemux>().Hide();
		stageTextObject.GetComponent<TweenToggleDemux>().Hide();
		yield return new WaitForSeconds(.5f); //If they are still in the process of showing, this must wait for them to appear
		introTextObject.GetComponent<TweenToggleDemux>().Hide();
		stageTextObject.GetComponent<TweenToggleDemux>().Hide();
	}

	public void ShowIntro() {
		DoctorMatchManager.Instance.DisableZones();
		introTextObject.GetComponent<TweenToggleDemux>().Show();
		introTextObject.GetComponentInChildren<Text>().text = Localization.Localize("DOCTOR_TUT_INTRO");
		showTime = Time.time;
	}

	public void HideIntro() {
		if (Time.time - showTime < minShow) {
			showTime -= minShow;
			return;
		}
		introTextObject.GetComponent<TweenToggleDemux>().Hide();
		ShowStage(1f);
	}

	public void ShowStage(float timeBeforeDisplay) {
		StartCoroutine(StageIEnum());
		showTime = Time.time;
	}

	public void HideStage() {
		if (Time.time - showTime < minShow) {
			showTime -= minShow;
			return;
		}
		stageTextObject.GetComponent<TweenToggleDemux>().Hide();
		DoctorMatchManager.Instance.EnableZones();
		DoctorMatchManager.Instance.SpawnFinger(toShow - 1);
	}

	private IEnumerator StageIEnum() {
		DoctorMatchManager.Instance.DisableZones();
		yield return new WaitForSeconds(.5f);
		stageTextObject.GetComponent<TweenToggleDemux>().Show();
		stageTextObject.GetComponentInChildren<Text>().text = Localization.Localize("DOCTOR_TUT_" + toShow);
		toShow++;
	}
}
