using UnityEngine;
using System.Collections;

public class LoadLevelUIManager : Singleton<LoadLevelUIManager> {
	public ThreadPriority threadPriority;
	public SceneTransitionController transitionController;

	private string levelName = null;	// Aux to store scene to be loaded
	private AsyncOperation async = null;	// Aux for keeping track of current loading coroutine
	
	/// <summary>
	/// Call this to start the transition
	/// </summary>
	/// <param name="levelName">Level to be loaded</param>
	/// <param name="loadingScreen">Loading screen to use</param>
	public void StartLoadTransition(string levelName){
		Application.backgroundLoadingPriority = threadPriority;

		if(async == null){
			this.levelName = levelName;
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
			Application.LoadLevel(levelName);	// Changing back to regular load level
//			StartCoroutine(Load());
		}
		else{
			Debug.LogError("No level name specified");
		}
	}
//	private IEnumerator Load(){
//
//		// Save the play period information before you change scenes
//		if(Application.loadedLevelName == SceneUtils.BEDROOM || Application.loadedLevelName == SceneUtils.YARD){
//			PlayPeriodLogic.Instance.SetLastPlayPeriod();
//		}
//
//#if UNITY_EDITOR
//		Debug.LogWarning("ASYNC LOAD STARTED - " + "DO NOT EXIT PLAY MODE UNTIL SCENE LOADS... UNITY WILL CRASH");
//#endif
//		async = Application.LoadLevelAsync(levelName);
//		async.allowSceneActivation = true;
//		yield return async;
//	}

	/// <summary>
	/// This is tells unity when the scene can be loaded, either immediately / when the loading is finished
	/// </summary>
	public void ActivateScene(){
		async.allowSceneActivation = true;
	}

	/// <summary>
	/// Remove the transition when the new level is loaded
	/// NOTE: There will be 2 of this, loading into a new scene without deleting the persistent duplicate
	/// 	Though one will be destroyed immediately
	/// </summary>
	void OnLevelWasLoaded(){
		transitionController.EndLoadingScreen();
		async = null;
	}

//	void OnGUI(){
//		if(GUI.Button(new Rect(100, 100, 100, 100), "Load scene")){
//			StartLoadTransition("test");
//		}
//	}
}
