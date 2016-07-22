using UnityEngine;
using System.Collections;

public class SecretUIController : Singleton<SecretUIController> {

	public TweenToggle tween;

	void Start(){
		if (DataManager.Instance.MinigamePlayCount > 4) {
			DataManager.Instance.MinigamePlayCount = 0;
			StartCoroutine (ShowAfterOneFrame ());
		}
	}

	private IEnumerator ShowAfterOneFrame(){
		yield return 0;
		ShowPanel ();
	}

	public void ShowPanel(){
		tween.Show();
	}

	public void HidePanel(){
		tween.Hide();
	}
}
