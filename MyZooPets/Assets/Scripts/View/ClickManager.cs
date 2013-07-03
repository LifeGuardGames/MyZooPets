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

	public GameObject petsprite;
	private NotificationUIManager notificationUIManager;

	public static bool isClickLocked;	// Lock to prevent multiple clicking (diary + trophy modes at the same time)
	public static bool isModeLocked;	// Lock to prevent clicking other objects when zoomed into a mode (clicking diary in trophy more)

	public void Init(){
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
		petsprite = GameObject.Find("SpritePet");
		notificationUIManager = GameObject.Find("NotificationUIManager").GetComponent<NotificationUIManager>();
		destinationPoint = petsprite.transform.position;

		AssignOnTapEvents();

	}

	// assigning methods that get called when these individual objects get called in the scene
	void AssignOnTapEvents(){
		// GameObject.Find("Book").GetComponent<TapItem>().OnTap += OnTapBook;
		GameObject.Find("Laptop").GetComponent<TapItem>().OnTap += OnTapLaptop;
		GameObject.Find("Calendar").GetComponent<TapItem>().OnTap += OnTapCalendar;
		GameObject.Find("SlotMachine").GetComponent<TapItem>().OnTap += OnTapSlotMachine;
		GameObject.Find("RealInhaler").GetComponent<TapItem>().OnTap += OnTapRealInhaler;
		GameObject.Find("TeddyInhaler").GetComponent<TapItem>().OnTap += OnTapTeddyInhaler;
		GameObject.Find("Shelf").GetComponent<TapItem>().OnTap += OnTapShelf;
		GameObject.Find("HelpTrophy").GetComponent<TapItem>().OnTap += OnTapHelpTrophy;
	}

	public static bool CanRespondToTap(){
		if (LoadDataLogic.IsDataLoaded){
			if(!isClickLocked && !isModeLocked){
				return true;
			}
		}
		return false;
	}

//	void OnTapBook(){ //TODO CHANGE FUNCTION NAME, no longer book but button
//		if (CanRespondToTap()){
//			diaryUIManager.DiaryClicked();
//			ClickLock();
//			ModeLock();
//		}
//	}
	void OnTapLaptop(){
		if (CanRespondToTap()){
			challengesGUI.ChallengesClicked();
			ClickLock();
			ModeLock();
		}
	}
	void OnTapCalendar(){
		if (CanRespondToTap()){
			calendarGUI.CalendarClicked();
			ClickLock();
			ModeLock();
		}
	}
	void OnTapSlotMachine(){
		if (CanRespondToTap()){
			cameraMove.SlotMachineZoomToggle();
			ClickLock();
			ModeLock();
		}
	}
	void OnTapRealInhaler(){
		if (CanRespondToTap()){
			if (CalendarLogic.HasCheckedCalendar){
				if (CalendarLogic.IsThereMissDosageToday){
					cameraMove.RealInhalerZoomToggle();
					ClickLock();
					ModeLock();
				}
				else {
					notificationUIManager.PopupNotification(
						"I don't need this right now.",
						delegate(){}
					);
				}
			}
			else {
				notificationUIManager.PopupNotification(
					"I don't know if I need this now. Open calendar?",
					calendarGUI.CalendarClicked,
					delegate(){}
				);
			}
		}
	}
	void OnTapTeddyInhaler(){
		if (CanRespondToTap()){
			cameraMove.TeddyInhalerZoomToggle();
			ClickLock();
			ModeLock();
		}
	}
	void OnTapShelf(){
		if (CanRespondToTap()){
			trophyGUI.TrophyClicked();
			ClickLock();
			ModeLock();
		}
	}
	void OnTapHelpTrophy(){
        // make sure we are in trophy mode
        // todo: have a better way of checking if we are in trophy mode
        if (!ClickManager.CanRespondToTap()){ // meaning we have clicked something

	        notificationUIManager.PopupNotification("Level up to get more trophies!",
	            delegate(){
	            	// nothing
	            });

			ClickLock();
			ModeLock();
		}
	}




	void Update(){
// 		if(!LoadDataLogic.IsDataLoaded) return; //return if not finish loading

// 		//Debug.Log(isClickLocked + " " + isModeLocked);
// 		if(!isClickLocked && !isModeLocked){
// 			if((isMobilePlatform && Input.touchCount > 0) || (!isMobilePlatform && Input.GetMouseButtonUp(0))){
// 				if(isMobilePlatform && (Input.GetTouch(0).phase == TouchPhase.Ended) || !isMobilePlatform){
// 					Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
// 					RaycastHit hit;
// 					if(Physics.Raycast(myRay,out hit)){
// 						//Debug.Log(hit.collider.name);
// 						// if(hit.collider.name == "room_shelf"){
// 						// 	trophyGUI.TrophyClicked();
// 						// 	ClickLock();
// 						// 	ModeLock();
// 						// }
// 						// // else if(hit.collider.name == "room_table"){
// 						// else if(hit.collider.name == "Book"){
// 						// 	diaryUIManager.DiaryClicked();
// 						// 	ClickLock();
// 						// 	ModeLock();
// 						// }
// 						// else if(hit.collider.name == "Laptop"){
// 						// 	challengesGUI.DiaryClicked();
// 						// 	ClickLock();
// 						// 	ModeLock();
// 						// }
// 						// else if(hit.collider.name == "Calendar"){
// 						// 	calendarGUI.DiaryClicked();
// 						// 	ClickLock();
// 						// 	ModeLock();
// 						// }
// 						// else if(hit.collider.name == "gameboy"){
// 						// 	cameraMove.GameboyZoomToggle();
// 						// 	ClickLock();
// 						// 	ModeLock();
// 						// }
// 						// else if(hit.collider.name == "PetHead"){
// 						// 	// todo
// 						// 	print("Pet Head");
// 						// 	// ClickLock();
// 						// 	// ModeLock();
// 						// }
// 						// else if(hit.collider.name == "PetTummy"){
// 						// 	// todo
// 						// 	print("Pet Tummy");
// 						// 	// ClickLock();
// 						// 	// ModeLock();
// 						// }
// 						// else if(hit.collider.name =="ColliderPlane"){
// 						//
// 						//Disabled clicking on lower screen. Temp Solution
// 						if (hit.collider.name =="ColliderPlane"){
// 							if(Input.GetTouch(0).position.y > 110)
// //							print (Input.GetTouch(0).position.y);
// //							print (Input.mousePosition.y);
// 							destinationPoint = hit.point;
// 						}
// 					}
// 				}
// 			}
// 		}
// 		//Move pet to clicked position
// 		petsprite.transform.position = Vector3.MoveTowards(petsprite.transform.position,destinationPoint,5f * Time.deltaTime);

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
