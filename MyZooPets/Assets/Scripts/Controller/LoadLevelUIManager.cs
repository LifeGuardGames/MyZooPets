using UnityEngine;
using System.Collections;

public class LoadLevelUIManager : Singleton<LoadLevelUIManager> {
	public SceneTransitionController transitionController;

	private string levelName = null;	// Aux to store scene to be loaded

	/// <summary>
	/// Call this to start the transition
	/// </summary>
	public void StartLoadTransition(string levelName){
	//	if(async == null){
			this.levelName = levelName;
			transitionController.StartTransition();
	//	}
	//	else{
			Debug.LogError("A level is already being loaded");
	//	}
	}

	/// <summary>
	/// Call this to initiate loading (when ending tween is done), there will be a performance hit here
	/// </summary>
	public void StartLoadCoroutine(){
		if(levelName != null){
			// UNDONE load the loading screen here
			Application.LoadLevel(levelName);	// Changing back to regular load level
		}
		else{
			Debug.LogError("No level name specified");
		}
	}

	/// <summary>
	/// Remove the transition when the new level is loaded
	/// NOTE: There will be 2 of this, loading into a new scene without deleting the persistent duplicate
	/// 	Though one will be destroyed immediately
	/// </summary>
	void OnLevelWasLoaded(){
		transitionController.EndLoadingScreen();
		levelName = null;
	}
}
