using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeTweener : MonoBehaviour {

	public float tweenIncrement;
	public Image blackScreen;
	private Color c;

	public void BlackOutScreen() {
		StartCoroutine(BlackOutTimer());
	}

	IEnumerator BlackOutTimer() {
		yield return new WaitForSeconds(0.1f);
		c = blackScreen.color;
		c.a += tweenIncrement;
		blackScreen.GetComponent<Image>().color = c;
		if(blackScreen.GetComponent<Image>().color.a <= 1) {
			StartCoroutine(BlackOutTimer());
		}
	}

	public void LightsUpScreen() {
		StartCoroutine(LightsUP());
	}

	IEnumerator LightsUP() {
		yield return new WaitForSeconds(0.1f);
		c = blackScreen.GetComponent<Image>().color;
		c.a -= tweenIncrement;
		blackScreen.GetComponent<Image>().color = c;
		if(blackScreen.GetComponent<Image>().color.a > 0) {
			StartCoroutine(LightsUP());
		}
		else if(blackScreen.GetComponent<Image>().color.a <= 1) {
			blackScreen.gameObject.SetActive(false);
		}
	}
}
