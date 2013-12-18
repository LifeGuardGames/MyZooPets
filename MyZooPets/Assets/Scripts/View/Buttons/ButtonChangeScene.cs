using UnityEngine;
using System.Collections;

//---------------------------------------------------
// ButtonChangeScene
// Generic button class that zooms in on an item and
// then changes the scene.  Note that the zoom 
// function in CameraMove does the actual loading.
//---------------------------------------------------

public class ButtonChangeScene : LgButton {
	// name of the scene to be loaded
	public string strScene;
	
	// fancy effect that plays before loading the scene
	public SceneTransition scriptTransition;
	
	// related to the camera move
	public Vector3 vOffset;			// offset of camera on the target
	public Vector3 vFinalRotation;	// how the camera should rotate
	public float fTime;				// how long the tween should last
	
	// what loading screen to show after the transition?  This is a prefab's name
	public string strLoadingScreen = "LoadingScreen";

	public bool shouldSaveSceneData; //give the option to load scene without saving partition or pet position
	public GameObject cameraGO; //needs the camera to record partition # before scene change
	public GameObject petLWF; //needs to record pet position before scene change
	
	//public CameraMove cameraMove;
	//public ZoomItem zoomItem;
	
	//---------------------------------------------------
	// ProcessClick()
	//---------------------------------------------------	
	protected override void ProcessClick() {
		// lock the click manager
		ClickManager.Instance.ClickLock();
		ClickManager.Instance.ModeLock( UIModeTypes.None );

		//Assuming that HUD is present at all scenes, so need to be hidden before scene change
		if(HUDUIManager.Instance != null)
			HUDUIManager.Instance.HidePanel();

		//Save some basic data for current scene
		RememberCurrentScene();
		
		// if there is a camera move, do it -- otherwise, just skip to the move being complete
		if ( fTime > 0 ) {
			Vector3 vFinalPos = gameObject.transform.position + vOffset;
			CameraManager.Instance.ZoomToTarget( vFinalPos, vFinalRotation, fTime, gameObject );
		}
		else
			CameraMoveDone();
	}	
	
	//---------------------------------------------------
	// CameraMoveDone()
	// Callback for when the camera is done tweening to
	// its target.
	//---------------------------------------------------	
	private void CameraMoveDone() {
		// the camera move is complete, so now let's start the transition (if it exists)
		if ( scriptTransition != null )
			scriptTransition.StartTransition( strScene, strLoadingScreen );
		else
			Debug.Log("No transition script for a scene change button!");
	}

	//---------------------------------------------------
	// RememberCurrentScene()
	// Record the pet's position and camera's partition before
	// switching to new scene.  
	//---------------------------------------------------
	private void RememberCurrentScene(){
		if(shouldSaveSceneData){
			int partition = cameraGO.GetComponent<PanToMoveCamera>().currentPartition;
			Vector3 petPos = petLWF.transform.position;
			DataManager.Instance.SceneData = new LoadSceneData(Application.loadedLevelName, petPos, partition);
		}else
			if(strScene == "MenuScene")
				//Only remove scene data if returning to menu scene
				DataManager.Instance.SceneData = null;
	}
}
