using UnityEngine;
using System.Collections;

public class SceneTransitionController : MonoBehaviour {

	public TweenToggleDemux transitionTweenToggle;
	public TweenToggleDemux loadingTweenToggle;

	public void StartTransition(){
		transitionTweenToggle.Show();
	}

	public void EndTransition(){
		transitionTweenToggle.Hide();
	}

	public void StartLoadingScreen(){
		loadingTweenToggle.Show();
	}

	public void EndLoadingScreen(){
		loadingTweenToggle.Hide();
	}
}
