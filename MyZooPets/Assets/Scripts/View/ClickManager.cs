using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Click manager.
/// All the classes that need a click to enter a certain mode will be handled here (ie. diary, trophy, inhaler game)
///	
///
/// NOTE: When entering a mode, lock click and mode, when done transitioning, unlock click
///       When exiting a mode, unlock click and mode after finish transitioning
///
/// </summary>

public class ClickManager : MonoBehaviour {
	private bool isMobilePlatform;
	private Vector3 destinationPoint;

	// All the classes that need a click input go here
	// public GameObject diaryUIManagerObject;
	// private DiaryGUI diaryUIManager;
	public GameObject hudUIObject;

	public GameObject calendarUIObject;
	private CalendarUIManager calendarUIManager;

	public GameObject storeUIObject;
	private StoreUIManager storeUIManager;

	public GameObject noteUIObject;
	private NoteUIManager noteUIManager;

	public GameObject navigationUIObject;

	public GameObject challengesGUIObject;
	private ChallengesGUI challengesGUI;

	public GameObject trophyGUIObject;
	private TrophyGUI trophyGUI;

	public GameObject cameraMoveObject;
	private CameraMove cameraMove;

	public NotificationUIManager notificationUIManager;
	private GameObject petsprite;

	public static bool isClickLocked;	// Lock to prevent multiple clicking (diary + trophy modes at the same time)
	public static bool isModeLocked;	// Lock to prevent clicking other objects when zoomed into a mode (clicking diary in trophy more)
    bool trophyMessageShowing = false;

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
		if(calendarUIObject != null) 
			calendarUIManager = calendarUIObject.GetComponent<CalendarUIManager>();
		if(noteUIObject != null)
			noteUIManager = noteUIObject.GetComponent<NoteUIManager>();
		if(storeUIObject != null)
			storeUIManager = storeUIObject.GetComponent<StoreUIManager>();
		// challengesGUI = challengesGUIObject.GetComponent<ChallengesGUI>();
		// diaryUIManager = diaryUIManagerObject.GetComponent<DiaryGUI>();
		// trophyGUI = trophyGUIObject.GetComponent<TrophyGUI>();
		if(cameraMoveObject != null)
			cameraMove = cameraMoveObject.GetComponent<CameraMove>();
		petsprite = GameObject.Find("SpritePet");
		destinationPoint = petsprite.transform.position;

		AssignOnTapEvents();

	}

	// assigning methods that get called when these individual objects get called in the scene
	void AssignOnTapEvents(){
		switch(Application.loadedLevelName){
			case "NewBedRoom":
				GameObject.Find("Laptop").GetComponent<TapItem>().OnTap += OnTapLaptop;
				GameObject.Find("Calendar").GetComponent<TapItem>().OnTap += OnTapCalendar;
				GameObject.Find("SlotMachine").GetComponent<TapItem>().OnTap += OnTapSlotMachine;
				GameObject.Find("RealInhaler").GetComponent<TapItem>().OnTap += OnTapRealInhaler;
				GameObject.Find("TeddyInhaler").GetComponent<TapItem>().OnTap += OnTapTeddyInhaler;
				GameObject.Find("Shelf").GetComponent<TapItem>().OnTap += OnTapShelf;
				GameObject.Find("HelpTrophy").GetComponent<TapItem>().OnTap += OnTapHelpTrophy;
			break;
			case "Yard":
			break;
		}
	}

	public static bool CanRespondToTap(){
		if (LoadDataLogic.IsDataLoaded){
			if(!isClickLocked && !isModeLocked){
				return true;
			}
		}
		return false;
	}

	//===========Note================
	public void OnClickNote(){
		if(CanRespondToTap()){
			noteUIManager.NoteClicked();
			NoteUIManager.OnNoteClosed += OnNoteClosed;
			cameraMove.ZoomToggle(ZoomItem.Pet); //zoom into pet
	
			ClickLock();
			ModeLock();

			//Hide other UI objects
			navigationUIObject.GetComponent<MoveTweenToggle>().Hide();
		}
	}
	private void OnNoteClosed(object sender, EventArgs e){
		cameraMove.ZoomOutMove();
		ReleaseClickLock();
		ReleaseModeLock();

		//Show other UI object
		navigationUIObject.GetComponent<MoveTweenToggle>().Show();
	}
	//==============================

	//==============Store=================
	public void OnClickStore(){
		if(CanRespondToTap()){
			storeUIManager.StoreClicked();
			StoreUIManager.OnStoreClosed += OnStoreClosed;
			ClickLock();
			ModeLock();

			//Hide other UI objects
			navigationUIObject.GetComponent<MoveTweenToggle>().Hide();
		}
	}
	private void OnStoreClosed(object sender, EventArgs e){
		ReleaseClickLock();
		ReleaseModeLock();

		//Show other UI object
		navigationUIObject.GetComponent<MoveTweenToggle>().Show();
	}
	//==================================

	//===========Calendar============
	void OnTapCalendar(){
		if (CanRespondToTap()){
			calendarUIManager.CalendarClicked();
			CalendarUIManager.OnCalendarClosed += OnCalendarClosed;
			ClickLock();
			ModeLock();

			//Hide other UI objects
			navigationUIObject.GetComponent<MoveTweenToggle>().Hide();
		}
	}
	private void OnCalendarClosed(object sender, EventArgs e){
		ReleaseClickLock();
		ReleaseModeLock();

		//Show other UI object
		navigationUIObject.GetComponent<MoveTweenToggle>().Show();
	}
	//==========================

	void OnTapLaptop(){
		if (CanRespondToTap()){
			//NOTE: logic not implemented so leaving it out for now
			// challengesGUI.ChallengesClicked();
			// ClickLock();
			// ModeLock();
		}
	}

	void OnTapSlotMachine(){
		if (CanRespondToTap()){
			// cameraMove.SlotMachineZoomToggle();
			cameraMove.ZoomToggle(ZoomItem.SlotMachine);
			ClickLock();
			ModeLock();
		}
	}
	void OnTapRealInhaler(){
		if (CanRespondToTap()){
			// if (CalendarLogic.HasCheckedCalendar){
				if (CalendarLogic.CanUseRealInhaler){
					// cameraMove.RealInhalerZoomToggle();
					cameraMove.ZoomToggle(ZoomItem.RealInhaler);
					ClickLock();
					ModeLock();

					//Hide other UI Objects
					navigationUIObject.GetComponent<MoveTweenToggle>().Hide();
					hudUIObject.GetComponent<MoveTweenToggle>().Hide();
				}
				else {
					notificationUIManager.PopupNotificationOneButton(
						"I don't need this right now.",
						delegate(){}
					);
				}
			// }
			// else {
			// 	notificationUIManager.PopupNotification(
			// 		"I don't know if I need this now. Open calendar?",
			// 		calendarGUI.CalendarClicked,
			// 		delegate(){}
			// 	);
			// }
		}
	}
	void OnTapTeddyInhaler(){
		if (CanRespondToTap()){
			// cameraMove.TeddyInhalerZoomToggle();
			cameraMove.ZoomToggle(ZoomItem.PracticeInhaler);
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

        	if (trophyMessageShowing == false){
	        	trophyMessageShowing = true;
		        notificationUIManager.PopupNotificationOneButton(
		        	"Level up to get more trophies!",
		            delegate(){
		            	trophyMessageShowing = false;
	            	},
	            	"OK"
	            );
        	}

			ClickLock();
			ModeLock();
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
