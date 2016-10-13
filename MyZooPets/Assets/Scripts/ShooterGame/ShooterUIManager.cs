using UnityEngine;

public class ShooterUIManager : Singleton<ShooterUIManager> {
	public MovingSky sun;
	public MovingSky moon;
	public Transform posSky;
	public Transform posBottom;
	public TextureListAlphaTween dayTween;
	public TextureListAlphaTween nightTween;

	public GameObject fingerPos;
	public GameObject FingerPos {
		get { return fingerPos; }
	}

	public void Quit() {
		LeanTween.cancel(sun.gameObject);
		LeanTween.cancel(moon.gameObject);
	}

	public void Reset() {
		sun.inSky = true;
		moon.inSky = false;
		sun.transform.position = posSky.position;
		moon.transform.position = posBottom.position;
		dayTween.InstantShow();
		nightTween.InstantHide();
	}

	// changes the sun to moon or moon to sun and then sets off the next transition once it is complete
	public void StartTimeTransition() {
		if(!ShooterGameManager.Instance.inTutorial) {
			if(!ShooterGameManager.Instance.isGameOver) {
				if(ShooterGameManager.Instance.waveNum == 0) {
					GameObject tutorialFinger = (GameObject)Resources.Load("ShooterPressTut");
					GameObject fingerUI = GameObjectUtils.AddChildGUI(GameObject.Find("Canvas"), tutorialFinger);
					fingerPos = fingerUI;
					RectTransform rect = fingerUI.GetComponent<RectTransform>();
					rect.anchorMax = new Vector2(1f, 0);
					rect.anchorMin = new Vector2(1f, 0);
					rect.anchoredPosition = new Vector2(-100f, 100f);
				}
				if(sun.GetComponent<MovingSky>().inSky) {
					LeanTween.move(sun.gameObject, posBottom.position, 2.0f).setOnComplete(EndTimeTransition).setEase(LeanTweenType.easeInQuad);
					nightTween.Show();
					dayTween.Hide();
				}
				else {
					LeanTween.move(moon.gameObject, posBottom.position, 2.0f).setOnComplete(EndTimeTransition).setEase(LeanTweenType.easeInQuad);
					dayTween.Show();
					nightTween.Hide();
				}
			}
		}
		else {
			LeanTween.move(sun.gameObject, posBottom.position, 2.0f).setOnComplete(TutChange).setEase(LeanTweenType.easeInQuad);
			nightTween.Show();
			dayTween.Hide();
		}
	}

	public void TutChange() {
		// if its the tutorial go to next step
		if(ShooterGameManager.Instance.inTutorial) {
			LeanTween.move(moon.gameObject, posSky.position, 2.0f).setEase(LeanTweenType.easeOutQuad);
		}
	}

	// Finishes the time transition and starts new wave
	private void EndTimeTransition() {
		MovingSky sunScript = sun.GetComponent<MovingSky>();
		MovingSky moonScript = moon.GetComponent<MovingSky>();

		if(sunScript.inSky == true) {
			LeanTween.move(moon.gameObject, posSky.position, 2.0f).setOnComplete(ShooterGameManager.Instance.BeginNewWave).setEase(LeanTweenType.easeOutQuad);
			moonScript.inSky = true;
			sunScript.inSky = false;
		}
		else {
			LeanTween.move(sun.gameObject, posSky.position, 2.0f).setOnComplete(ShooterGameManager.Instance.BeginNewWave).setEase(LeanTweenType.easeOutQuad);
			sunScript.inSky = true;
			moonScript.inSky = false;
		}
	}
}
