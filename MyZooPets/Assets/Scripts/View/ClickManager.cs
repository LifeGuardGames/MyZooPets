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

	public GameObject inventoryUIObject;

	public GameObject navigationUIObject;

	public GameObject challengesGUIObject;
	// private ChallengesGUI challengesGUI;

	public GameObject trophyGUIObject;
	private TrophyGUI trophyGUI;

	public GameObject cameraMoveObject;
	private CameraMove cameraMove;

	public NotificationUIManager notificationUIManager;

	public static bool isClickLocked;	// Lock to prevent multiple clicking (diary + trophy modes at the same time)
	public static bool isModeLocked;	// Lock to prevent clicking other objects when zoomed into a mode (clicking diary in trophy more)
    bool trophyMessageShowing = false;

    void Awake(){
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
		if(trophyGUIObject != null)
			trophyGUI = trophyGUIObject.GetComponent<TrophyGUI>();
		if(cameraMoveObject != null)
			cameraMove = cameraMoveObject.GetComponent<CameraMove>();
    }

    void Start(){
		AssignOnTapEvents();

		NoteUIManager.OnNoteClosed += OnNoteClosed;
		StoreUIManager.OnStoreClosed += OnStoreClosed;
		CalendarUIManager.OnCalendarClosed += OnCalendarClosed;
		TrophyGUI.OnTrophyClosed += OnTrophyClosed;
    }

	//Clean all even listeners
	void OnDestroy(){
		NoteUIManager.OnNoteClosed -= OnNoteClosed;
		StoreUIManager.OnStoreClosed -= OnStoreClosed;
		CalendarUIManager.OnCalendarClosed -= OnCalendarClosed;
		TrophyGUI.OnTrophyClosed -= OnTrophyClosed;
	}

	// assigning methods that get called when these individual objects get called in the scene
	void AssignOnTapEvents(){
		switch(Application.loadedLevelName){
			case "NewBedRoom":
				GameObject.Find("GO_Laptop").GetComponent<TapItem>().OnTap += OnTapLaptop;
				GameObject.Find("GO_Calendar").GetComponent<TapItem>().OnTap += OnTapCalendar;
				GameObject.Find("GO_SlotMachine").GetComponent<TapItem>().OnTap += OnTapSlotMachine;
				GameObject.Find("GO_RealInhaler").GetComponent<TapItem>().OnTap += OnTapRealInhaler;
				GameObject.Find("GO_TeddyInhaler").GetComponent<TapItem>().OnTap += OnTapTeddyInhaler;
				GameObject.Find("GO_Shelf").GetComponent<TapItem>().OnTap += OnTapShelf;
				GameObject.Find("GO_HelpTrophy").GetComponent<TapItem>().OnTap += OnTapHelpTrophy;
			break;
			case "Yard":
			break;
		}
	}

	public static bool CanRespondToTap(){
		if(!isClickLocked && !isModeLocked){
			return true;
		}
		return false;
	}

	//===========Note================
	public void OnClickNote(){
		if(CanRespondToTap()){
			noteUIManager.NoteClicked();
			cameraMove.ZoomToggle(ZoomItem.Pet); //zoom into pet

			ClickLock();
			ModeLock();

			//Hide other UI objects
			navigationUIObject.GetComponent<MoveTweenToggleDemultiplexer>().Hide();
			inventoryUIObject.GetComponent<MoveTweenToggle>().Hide();
		}
	}
	private void OnNoteClosed(object sender, EventArgs e){
		ClickLock();
		cameraMove.ZoomOutMove();

		//Show other UI object
		navigationUIObject.GetComponent<MoveTweenToggleDemultiplexer>().Show();
		inventoryUIObject.GetComponent<MoveTweenToggle>().Show();
	}
	//==============================

	//==============Store=================
	public void OnClickStore(){
		if(CanRespondToTap()){
			storeUIManager.StoreClicked();
			ClickLock();
			ModeLock();

			//Hide other UI objects
			navigationUIObject.GetComponent<MoveTweenToggleDemultiplexer>().Hide();
		}
	}
	private void OnStoreClosed(object sender, EventArgs e){
		ReleaseClickLock();
		ReleaseModeLock();

		//Show other UI object
		navigationUIObject.GetComponent<MoveTweenToggleDemultiplexer>().Show();
	}
	//==================================

	//===========Calendar============
	void OnTapCalendar(){
		if (CanRespondToTap()){
			calendarUIManager.CalendarClicked();
			ClickLock();
			ModeLock();

			//Hide other UI objects
			navigationUIObject.GetComponent<MoveTweenToggleDemultiplexer>().Hide();
			inventoryUIObject.GetComponent<MoveTweenToggle>().Hide();
		}
	}
	private void OnCalendarClosed(object sender, EventArgs e){
		ReleaseClickLock();
		ReleaseModeLock();
		//Show other UI object
		navigationUIObject.GetComponent<MoveTweenToggleDemultiplexer>().Show();
		inventoryUIObject.GetComponent<MoveTweenToggle>().Show();
	}
	//==========================

	//=================Shelf=====================
	private void OnTapShelf(){
		if (CanRespondToTap()){
			trophyGUI.TrophyClicked();
			ClickLock();
			ModeLock();

			//Hide other UI objects
			navigationUIObject.GetComponent<MoveTweenToggleDemultiplexer>().Hide();
			hudUIObject.GetComponent<MoveTweenToggleDemultiplexer>().Hide();
		}
	}
	private void OnTrophyClosed(object senders, EventArgs e){
		ClickLock();
		cameraMove.ZoomOutMove();

		//Show other UI Objects
		navigationUIObject.GetComponent<MoveTweenToggleDemultiplexer>().Show();
		hudUIObject.GetComponent<MoveTweenToggleDemultiplexer>().Show();
	}
	//=========================================

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
			cameraMove.ZoomToggle(ZoomItem.SlotMachine);
			ClickLock();
			ModeLock();
		}
	}
	void OnTapRealInhaler(){
		if (CanRespondToTap()){
			if (DataManager.FirstTimeRealInhaler){
				return; // taken care of in Tutorial.cs
			}
			if (CalendarLogic.CanUseRealInhaler){
				OpenRealInhaler();
			}
			else {
				notificationUIManager.PopupNotificationOneButton(
					"I don't need this right now.",
					delegate(){}
				);
			}
		}
	}

	// also called in tutorial as a callback
	public void OpenRealInhaler(){
		cameraMove.ZoomToggle(ZoomItem.RealInhaler);
		ClickLock();
		ModeLock();

		//Hide other UI Objects
		navigationUIObject.GetComponent<MoveTweenToggleDemultiplexer>().Hide();
		hudUIObject.GetComponent<MoveTweenToggleDemultiplexer>().Hide();
	}

	void OnTapTeddyInhaler(){
		if (CanRespondToTap()){
			// cameraMove.TeddyInhalerZoomToggle();
			cameraMove.ZoomToggle(ZoomItem.PracticeInhaler);
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
