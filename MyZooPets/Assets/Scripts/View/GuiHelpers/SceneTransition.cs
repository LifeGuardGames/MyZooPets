using UnityEngine;
using System.Collections;

// Uses toggle callback to do this
public class SceneTransition : MonoBehaviour {

	public string levelName;
	public GameObject optionalLoadingScreen;
	public GameObject transitionToggleObject;	// move tween toggle / demux

	// Use this for initialization
	void Start(){
		if(optionalLoadingScreen != null){
			optionalLoadingScreen.SetActive(false);
		}

		transitionToggleObject.SetActive(false);
	}
	
	public void StartTransition(){
		if (!string.IsNullOrEmpty(levelName))
		{
			transitionToggleObject.SetActive(true);
			
			// Check for any tween toggles
			TweenToggleDemux demux = transitionToggleObject.GetComponent<TweenToggleDemux>();
			TweenToggle toggle = transitionToggleObject.GetComponent<TweenToggle>();
			if(demux != null){
				demux.ShowTarget = gameObject;
				demux.isShowFinishedCallback = true;
				demux.ShowFunctionName = "ShowPictureAndLoad";
				demux.Show();
			}
			else if(toggle != null){
				toggle.ShowTarget = gameObject;
				toggle.ShowFunctionName = "ShowPictureAndLoad";
				toggle.Show();
			}
			else{
				Debug.LogError("No toggle detected");
			}
		}
	}
	
	public void StartTransition(string strLevel) {
		print ("CALLING start transition : " + strLevel);
		levelName = strLevel;	
		StartTransition();
	}

	// Call this with callback from toggle
	public void ShowPictureAndLoad(){
		if(optionalLoadingScreen != null){
			optionalLoadingScreen.SetActive(true);
		}
		print ("Loading level : " + levelName);
		Application.LoadLevel(levelName);
	}
}
