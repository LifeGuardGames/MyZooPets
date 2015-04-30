﻿using UnityEngine;
using System;
using System.Collections;

//---------------------------------------------------
// ButtonChangeScene
// Generic button class that zooms in on an item and
// then changes the scene.  Note that the zoom 
// function in CameraMove does the actual loading.
//---------------------------------------------------
public class ButtonChangeScene : LgButton{
	public static EventHandler<EventArgs> OnChangeScene; //Event when changing scene on user input

	// name of the scene to be loaded
	public string strScene;
	
	// related to the camera move
	public Vector3 finalPosition;		// offset of camera on the target
	public Vector3 finalRotation;		// how the camera should rotate
	public float zoomTime = 1f;				// how long the tween should last

	public bool shouldSaveSceneData;	// give the option to load scene without saving partition or pet position
	public GameObject cameraGO; 		// needs the camera to record partition # before scene change
	public GameObject petObject; 		// needs to record pet position before scene change

	public string analyticsEvent;
	public EntranceHelperController entranceHelper;

	public bool isCheckMood = true;
	
	//---------------------------------------------------
	// ProcessClick()
	//---------------------------------------------------	
	protected override void ProcessClick(){
		if(!isCheckMood || DataManager.Instance.GameData.Stats.GetMoodState() != PetMoods.Sad){
			// lock the click manager
			ClickManager.Instance.Lock();
			if(Application.loadedLevelName != SceneUtils.YARD){
			FirstInteraction.Instance.SetString("MinigameEntrance");
			}
			//Hide other UI Objects
			//Assuming that HUD is present at all scenes, so need to be hidden before scene change
			if(HUDUIManager.Instance != null){
				HUDUIManager.Instance.HidePanel();
			}
			if(NavigationUIManager.Instance != null){
				NavigationUIManager.Instance.HidePanel();	
			}
			if(InventoryUIManager.Instance != null){
				InventoryUIManager.Instance.HidePanel();
			}
			RoomArrowsUIManager.Instance.HidePanel();

			//Sent an change scene event out, so other objects can run appropriate logic before scene change
			if(OnChangeScene != null){
				OnChangeScene(this, EventArgs.Empty);
			}

			//Save some basic data for current scene
			RememberCurrentScene();

			//record that this entrance has been used
			if(entranceHelper != null){
				entranceHelper.EntranceUsed();
			}
			
			// if there is a camera move, do it -- otherwise, just skip to the move being complete
			if(zoomTime > 0){
				CameraManager.Callback cameraDoneFunction = delegate(){
					CameraMoveDone();
				};
				CameraManager.Instance.ZoomToTarget(finalPosition, finalRotation, zoomTime, cameraDoneFunction);
			}
			else{
				CameraMoveDone();
			}
		}
		else {
			PetSpeechAI.Instance.ShowSadMessage();
		}
	}

	//---------------------------------------------------
	// CameraMoveDone()
	// Callback for when the camera is done tweening to
	// its target.
	//---------------------------------------------------
	private void CameraMoveDone(){
		// the camera move is complete, so now let's start the transition (if it exists)
		LoadLevelUIManager.Instance.StartLoadTransition(strScene);
	}

	//---------------------------------------------------
	// RememberCurrentScene()
	// Record the pet's position and camera's partition before
	// switching to new scene.  
	//---------------------------------------------------
	private void RememberCurrentScene(){
		if(shouldSaveSceneData){
			int partition = cameraGO.GetComponent<PanToMoveCamera>().currentPartition;
			Vector3 petPos = petObject.transform.position;
			DataManager.Instance.SceneData = new LoadSceneData(Application.loadedLevelName, petPos, partition);
		}
		else{
			if(strScene == SceneUtils.MENU){
				//Only remove scene data if returning to menu scene
				DataManager.Instance.SceneData = null;
			}
		}
	}
}
