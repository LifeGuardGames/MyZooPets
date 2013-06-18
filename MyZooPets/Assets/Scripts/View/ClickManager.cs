using UnityEngine;
using System.Collections;

/// <summary>
/// Click manager.
/// All the classes that need a click to enter a certain mode will be handled here (ie. diary, trophy, inhaler game)
/// 
/// NOTE: When entering a mode, lock click and mode, when done transitioning, unlock click
///       When exiting a mode, unlock click and mode after finish transitioning
/// 
/// </summary>

public class ClickManager : MonoBehaviour {
	private bool isMobilePlatform;
	
	// All the classes that need a click input go here
	public GameObject diaryUIManagerObject;
	private DiaryGUI diaryUIManager;
	
	public GameObject trophyGUIObject;
	private TrophyGUI trophyGUI;
	
	public GameObject cameraMoveObject;
	private CameraMove cameraMove;
	
	public static bool isClickLocked;	// Lock to prevent multiple clicking (diary + trophy modes at the same time)
	public static bool isModeLocked;	// Lock to prevent clicking other objects when zoomed into a mode (clicking diary in trophy more)
	
	void Start(){
		if(Application.platform == RuntimePlatform.Android ||
			Application.platform == RuntimePlatform.IPhonePlayer){
			isMobilePlatform = true;
		}
		else{
			isMobilePlatform = false;
		}

		isClickLocked = false;
		isModeLocked = false;
		
		// Linking script references
		diaryUIManager = diaryUIManagerObject.GetComponent<DiaryGUI>();
		trophyGUI = trophyGUIObject.GetComponent<TrophyGUI>();
		cameraMove = cameraMoveObject.GetComponent<CameraMove>();
	}

	void Update(){
		//Debug.Log(isClickLocked + " " + isModeLocked);
		if(!isClickLocked && !isModeLocked){
			if((isMobilePlatform && Input.touchCount > 0) || (!isMobilePlatform && Input.GetMouseButtonUp(0))){
				if(isMobilePlatform && (Input.GetTouch(0).phase == TouchPhase.Ended) || !isMobilePlatform){
					Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit;
					if(Physics.Raycast(myRay,out hit)){
						//Debug.Log(hit.collider.name);
						if(hit.collider.name == "room_shelf"){
							trophyGUI.TrophyClicked();
							ClickLock();
							ModeLock();
						}
						else if(hit.collider.name == "room_table"){
							diaryUIManager.DiaryClicked();
							ClickLock();
							ModeLock();
						}
						else if(hit.collider.name == "gameboy"){
							cameraMove.GameboyZoomToggle();
							ClickLock();
							ModeLock();
						}
					}
				}
			}
		}
	}
	
	// Disable clicking when transitioning between modes
	public static void ClickLock(){
		isClickLocked = true;	
	}
	
	// Enable clicking after the transitioning is done, usually called from LeanTween callback
	public static void ReleaseClickLock(){
		isClickLocked = false;
	}
	
	// Disable clicking other objects inside a mode (ie, cant click shelf when in trophy mode)
	public static void ModeLock(){
		isModeLocked = true;
	}
	
	// Enable clicking other objects after completed exiting a mode
	public static void ReleaseModeLock(){
		isModeLocked = false;
	}
}
