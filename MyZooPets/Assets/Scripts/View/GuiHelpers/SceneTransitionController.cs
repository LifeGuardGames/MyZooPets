using UnityEngine;
using System.Collections;

public class SceneTransitionController : MonoBehaviour {

	public TweenToggleDemux transitionTweenToggle;
	public TweenToggleDemux loadingTweenToggle;

	public void StartTransition(){
		AudioManager.Instance.PlayClip("loadingSlideClose");
		transitionTweenToggle.Show();
	}

	public void EndTransition(){
		AudioManager.Instance.PlayClip("loadingSlideOpen");
		transitionTweenToggle.Hide();
	}

	public void StartLoadingScreen(){
		loadingTweenToggle.Show();
	}

	public void EndLoadingScreen(){
		loadingTweenToggle.Hide();
	}
}
