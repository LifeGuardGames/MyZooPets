using UnityEngine;
using System.Collections;

//---------------------------------------------------
// SceneTranstion
// This script should be on all gameobjects that
// are transitions; it controls the actual graphical
// portion of the transitions.
//---------------------------------------------------

public class SceneTransition : MonoBehaviour {
	// toggles that could be a part of the transition
	public TweenToggleDemux demux;
	public TweenToggle toggle;
	
	// scene to load
	private string strScene;
	
	// loading screen to show when the scene is loading
	public GameObject goLoadScreen;	

	//---------------------------------------------------
	// Start()
	//---------------------------------------------------
	void Start(){
		// transitions are inactive by default
		//gameObject.SetActive(false);
		// JOE commented this out....because somehow I think Start() is called after the object is set active, so it was just deactivating
		// it again.  But how did it work before then...
	}
	
	//---------------------------------------------------
	// StartTransition()
	// Begins fancy transition.  Will send a callback to
	// goCaller when the transition is complete.
	//---------------------------------------------------
	public void StartTransition( string strScene ){
		gameObject.SetActive(true);
		
		// cache scene to load
		this.strScene = strScene;
		
		// Check for any tween toggles
		if(demux != null){
			demux.ShowTarget = gameObject;
			demux.isShowFinishedCallback = true;
			demux.ShowFunctionName = "TransitionDone";
			demux.Show();
		}
		else if(toggle != null){
			toggle.ShowTarget = gameObject;
			toggle.ShowFunctionName = "TransitionDone";
			toggle.Show();
		}
		else{
			Debug.LogError("No toggle detected");
		}
	}
	
	//---------------------------------------------------
	// TransitionDone()
	// Callback for when the transition is done tweening.
	//---------------------------------------------------	
	private void TransitionDone() {
		// show the loading screen (if it exists)
		if(goLoadScreen != null)
			goLoadScreen.SetActive(true);

		Debug.Log("Loading level: " + strScene);
		
		// load the scene
		Application.LoadLevel( strScene );		
	}	
}
