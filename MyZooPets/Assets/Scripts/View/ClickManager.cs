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
	private Vector3 destinationPoint;

	// All the classes that need a click input go here
	public GameObject diaryUIManagerObject;
	private DiaryGUI diaryUIManager;

	public GameObject calendarGUIObject;
	private CalendarGUI calendarGUI;

	public GameObject challengesGUIObject;
	private ChallengesGUI challengesGUI;

	public GameObject trophyGUIObject;
	private TrophyGUI trophyGUI;

	public GameObject cameraMoveObject;
	private CameraMove cameraMove;

	public GameObject rotateInRoomObject;
	private RotateInRoom rotateInRoom;
	
	
	public GameObject petsprite;

	public static bool isClickLocked;	// Lock to prevent multiple clicking (diary + trophy modes at the same time)
	public static bool isModeLocked;	// Lock to prevent clicking other objects when zoomed into a mode (clicking diary in trophy more)

	public void init(){
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
		calendarGUI = calendarGUIObject.GetComponent<CalendarGUI>();
		challengesGUI = challengesGUIObject.GetComponent<ChallengesGUI>();
		diaryUIManager = diaryUIManagerObject.GetComponent<DiaryGUI>();
		trophyGUI = trophyGUIObject.GetComponent<TrophyGUI>();
		cameraMove = cameraMoveObject.GetComponent<CameraMove>();
		rotateInRoom = rotateInRoomObject.GetComponent<RotateInRoom>();
		petsprite = GameObject.Find("SpritePet");
		destinationPoint = petsprite.transform.position;

		// Init swipe listener.
		SwipeDetection.OnSwipeDetected += OnSwipeDetected;
	}

	void OnSwipeDetected(Swipe s){
		switch (s){
			case Swipe.Up:
				print("Swipe.Up");
			break;
			case Swipe.Down:
				print("Swipe.Down");
			break;
			case Swipe.Left:
				print("Swipe.Left");
				rotateInRoom.RotateLeft();
			break;
			case Swipe.Right:
				print("Swipe.Right");
				rotateInRoom.RotateRight();
			break;
		}
	}

	void Update(){
		if(!LoadDataLogic.IsDataLoaded) return; //return if not finish loading
		
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
						// else if(hit.collider.name == "room_table"){
						else if(hit.collider.name == "Book"){
							diaryUIManager.DiaryClicked();
							ClickLock();
							ModeLock();
						}
						else if(hit.collider.name == "Laptop"){
							challengesGUI.DiaryClicked();
							ClickLock();
							ModeLock();
						}
						else if(hit.collider.name == "Calendar"){
							calendarGUI.DiaryClicked();
							ClickLock();
							ModeLock();
						}
						else if(hit.collider.name == "gameboy"){
							cameraMove.GameboyZoomToggle();
							ClickLock();
							ModeLock();
						}
						else if(hit.collider.name == "PetHead"){
							// todo
							print("Pet Head");
							// ClickLock();
							// ModeLock();
						}
						else if(hit.collider.name == "PetTummy"){
							// todo
							print("Pet Tummy");
							// ClickLock();
							// ModeLock();
						}
						else if(hit.collider.name =="ColliderPlane"){
							destinationPoint = hit.point;
						}
					}
				}
			}
		}
		//Move pet to clicked position
		petsprite.transform.position = Vector3.MoveTowards(petsprite.transform.position,destinationPoint,5f * Time.deltaTime);

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
