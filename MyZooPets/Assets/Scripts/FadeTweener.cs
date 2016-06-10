using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Replacement class for NGUITweenAlpha needs fleshing out
/// </summary>
public class FadeTweener : MonoBehaviour {

	public float tweenIncrement;
	public Image blackScreen;
	public Text fadeOutText;
	private Color c;


	public void FadeText() {
		StartCoroutine(FadeOutTextTimer());
	}

	IEnumerator FadeOutTextTimer() {
		yield return new WaitForSeconds(tweenIncrement);
		c = fadeOutText.color;
		c.a += tweenIncrement;
		fadeOutText.color = c;
		if(fadeOutText.color.a <= 1) {
			StartCoroutine(FadeOutTextTimer());
		}
	}



	public void FadeImage() {
		StartCoroutine(FadeOutImageTimer());
	}

	IEnumerator FadeOutImageTimer() {
		yield return new WaitForSeconds(tweenIncrement);
		c = blackScreen.color;
		c.a += tweenIncrement;
		blackScreen.GetComponent<Image>().color = c;
		if(blackScreen.GetComponent<Image>().color.a <= 1) {
			StartCoroutine(FadeOutImageTimer());
		}
	}

	public void FadeUpImageScreen() {
		StartCoroutine(FadeUpImageUP());
	}

	IEnumerator FadeUpImageUP() {
		yield return new WaitForSeconds(tweenIncrement);
		c = blackScreen.GetComponent<Image>().color;
		c.a -= tweenIncrement;
		blackScreen.GetComponent<Image>().color = c;
		if(blackScreen.GetComponent<Image>().color.a > 0) {
			StartCoroutine(FadeUpImageUP());
		}
		else if(blackScreen.GetComponent<Image>().color.a <= 1) {
			blackScreen.gameObject.SetActive(false);
		}
	}
}
