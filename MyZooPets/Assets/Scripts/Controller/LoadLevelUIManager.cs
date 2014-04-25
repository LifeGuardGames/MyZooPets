using UnityEngine;
using System.Collections;

public class LoadLevelUIManager : Singleton<LoadLevelUIManager> {

	public SceneTransitionController transitionController;

	private string levelName = null;	// Aux to store scene to be loaded
	private string loadingScreen = null;	// Aux to store loading screen to be loaded from resources
	private AsyncOperation async = null;	// Aux for keeping track of current loading coroutine
	
	/// <summary>
	/// Call this to start the transition
	/// </summary>
	/// <param name="levelName">Level to be loaded</param>
	/// <param name="loadingScreen">Loading screen to use</param>
	public void StartLoadTransition(string levelName, string loadingScreen){
		if(async == null){
			this.levelName = levelName;
			Debug.Log(levelName);
			this.loadingScreen = loadingScreen;
			transitionController.StartTransition();
		}
		else{
			Debug.LogError("A level is already being loaded");
		}
	}

	/// <summary>
	/// Call this to initiate loading (when ending tween is done), there will be a performance hit here
	/// </summary>
	public void StartLoadCoroutine(){
		if(levelName != null){
			// UNDONE load the loading screen here
			StartCoroutine(Load());
		}
		else{
			Debug.LogError("No level name specified");
		}
	}
	private IEnumerator Load(){
		Debug.LogWarning("ASYNC LOAD STARTED - " + "DO NOT EXIT PLAY MODE UNTIL SCENE LOADS... UNITY WILL CRASH");
		async = Application.LoadLevelAsync(levelName);
		async.allowSceneActivation = true;
		yield return async;
	}

	/// <summary>
	/// This is tells unity when the scene can be loaded, either immediately / when the loading is finished
	/// </summary>
	public void ActivateScene(){
		async.allowSceneActivation = true;
	}
}
