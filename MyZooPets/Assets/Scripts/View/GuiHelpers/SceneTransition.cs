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
	private GameObject goLoadScreen;	

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
	public void StartTransition( string strScene, string strLoadingPrefab = "" ){
		gameObject.SetActive(true);
		
		// wow, this is dumb...thanks for not updating public default variables, Unity
		if ( string.IsNullOrEmpty(strLoadingPrefab) )
			strLoadingPrefab = DataLoadScene.GetRandomLoadScreen();
		
		// load the loading prefab (it will start inactive)
		GameObject goLoading = Resources.Load( strLoadingPrefab ) as GameObject;
		goLoadScreen = LgNGUITools.AddChildWithPositionAndScale( transform.parent.gameObject, goLoading );
		
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

		// StartCoroutine(Load());
	}

	// private IEnumerator Load(){
	// 	AsyncOperation async = Application.LoadLevelAsync(strScene);

	// 	while(!async.isDone){
	// 		print(async.progress);
	// 		yield return null;
	// 	}
	// }
	
	//---------------------------------------------------
	// TransitionDone()
	// Callback for when the transition is done tweening.
	//---------------------------------------------------	
	private void TransitionDone() {
		// show the loading screen (if it exists)
		if(goLoadScreen != null) {
			goLoadScreen.SetActive(true);
			
			// init the loading screen script
			LoadingScreen script = goLoadScreen.GetComponent<LoadingScreen>();
			if ( script )
				script.Init( strScene );
			else
				Debug.LogError("No loading screen on " + goLoadScreen);
		}	
	}
}

