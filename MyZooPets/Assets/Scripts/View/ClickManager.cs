using UnityEngine;
using System.Collections;

/// <summary>
/// Click manager.
/// All the classes that need a click to enter a certain mode will be handled here (ie. diary, trophy, inhaler game)
/// 
/// NOTE: Make sure to RELEASE THE LOCK when you exit the mode!
/// 
/// </summary>

public class ClickManager : MonoBehaviour {
	
	private bool isMobilePlatform;
	
	// All the classes that need a click input go here
//	public GameObject cameraMoveObject;
//	private CameraMove cameraMove;
	
	public GameObject diaryUIManagerObject;
	private DiaryUIManager diaryUIManager;
	
	public GameObject trophyGUIObject;
	private TrophyGUI trophyGUI;
	
	public GameObject cameraMoveObject;
	private CameraMove cameraMove;
	
	// Lock to prevent multiple clicking (diary + trophy modes at the same time)
	public static bool isLocked = false;
	
	void Start(){
		if(Application.platform == RuntimePlatform.Android ||
			Application.platform == RuntimePlatform.IPhonePlayer){
			isMobilePlatform = true;
		}
		else{
			isMobilePlatform = false;
		}
		
		// Linking script references
		diaryUIManager = diaryUIManagerObject.GetComponent<DiaryUIManager>();
		trophyGUI = trophyGUIObject.GetComponent<TrophyGUI>();
		cameraMove = cameraMoveObject.GetComponent<CameraMove>();
	}

	void Update(){
		if(!isLocked){
			if((isMobilePlatform && Input.touchCount > 0) || (!isMobilePlatform && Input.GetMouseButtonUp(0))){
				if(isMobilePlatform && (Input.GetTouch(0).phase == TouchPhase.Ended) || !isMobilePlatform){
					Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit;
					if(Physics.Raycast(myRay,out hit)){
						//Debug.Log(hit.collider.name);
						if(hit.collider.name == "room_shelf"){
							trophyGUI.TrophyClicked();
							isLocked = true;
						}
						
						else if(hit.collider.name == "room_table"){
							diaryUIManager.DiaryClicked();
							isLocked = true;
						}
						else if(hit.collider.name == "gameboy"){
							cameraMove.GameboyZoomToggle();
						}
					}
				}
			}
		}
	}
	
	public static void Lock(){
		isLocked = true;	
	}
	
	public static void ReleaseLock(){
		isLocked = false;
	}
}
