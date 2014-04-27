using UnityEngine;
using System.Collections;

public class SceneTransitionController : MonoBehaviour {

	public TweenToggleDemux transitionTweenToggle;
	public TweenToggleDemux loadingTweenToggle;

	void Start(){
//		loadingTweenToggle.gameObject.SetActive(false);
	}

	public void StartTransition(){
		transitionTweenToggle.Show();
	}

	public void EndTransition(){
		transitionTweenToggle.Hide();
	}

	public void StartLoadingScreen(){
//		loadingTweenToggle.gameObject.SetActive(true);
		loadingTweenToggle.Show();
	}

	public void EndLoadingScreen(){
		loadingTweenToggle.Hide();
	}
}
