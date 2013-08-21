using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Click manager.
/// All the classes that need a click to enter a certain mode will be handled here (ie. diary, badge, inhaler game)
///
///
/// NOTE: When entering a mode, lock click and mode, when done transitioning, unlock click
///       When exiting a mode, unlock click and mode after finish transitioning
///
/// </summary>

public class ClickManager : MonoBehaviour {

	// All the classes that need a click input go here

	//===========Objects and components in NGUI==========
	public GameObject hudUIManagerObject;
	private HUDUIManager hudUIManager;

	public GameObject calendarUIManagerObject;
	private CalendarUIManager calendarUIManager;

	public GameObject storeUIManagerObject;
	private StoreUIManager storeUIManager;

	public GameObject noteUIManagerObject;
	private NoteUIManager noteUIManager;

	public GameObject inventoryUIManagerObject;
	private InventoryUIManager inventoryUIManager;

	public GameObject navigationUIManagerObject;
	private NavigationUIManager navigationUIManager;

	public GameObject badgeUIManagerObject;
	private BadgeUIManager badgeUIManager;

	public GameObject UILoadScreen;
	//=================================================

	//==============GameObjects and components in the world==============
	public GameObject GOCalendarObject;
	public GameObject GOSlotMachineObject;
	public GameObject GORealInhalerObject;
	public GameObject GOTeddyInhalerObject;
	public GameObject GOBadgeObject;
	public GameObject GODojoObject; 
	public GameObject GOYardSignObject; 

	public GameObject cameraMoveObject;
	private CameraMove cameraMove;
	//===============================================

    public static GameObject UIRoot; // this is used to add a collider to the UIRoot to stop non-GUI elements from being clicked when GUI menus are active

	public static bool isClickLocked;	// Lock to prevent multiple clicking (diary + trophy modes at the same time)
	public static bool isModeLocked;	// Lock to prevent clicking other objects when zoomed into a mode (clicking diary in trophy more)

    bool trophyMessageShowing = false;

    void Awake(){
		isClickLocked = false;
		isModeLocked = false;

		// Linking script references
		if(hudUIManagerObject != null)
			hudUIManager = hudUIManagerObject.GetComponent<HUDUIManager>();
		if(calendarUIManagerObject != null)
			calendarUIManager = calendarUIManagerObject.GetComponent<CalendarUIManager>();
		if(noteUIManagerObject != null)
			noteUIManager = noteUIManagerObject.GetComponent<NoteUIManager>();
		if(storeUIManagerObject != null)
			storeUIManager = storeUIManagerObject.GetComponent<StoreUIManager>();
		if(navigationUIManagerObject != null)
			navigationUIManager = navigationUIManagerObject.GetComponent<NavigationUIManager>();
		if(badgeUIManagerObject != null)
			badgeUIManager = badgeUIManagerObject.GetComponent<BadgeUIManager>();
		if(inventoryUIManagerObject != null)
			inventoryUIManager = inventoryUIManagerObject.GetComponent<InventoryUIManager>();

		if(cameraMoveObject != null)
			cameraMove = cameraMoveObject.GetComponent<CameraMove>();
    }

    void Start(){
		AssignOnTapEvents();

		NoteUIManager.OnNoteClosed += OnNoteClosed;
		StoreUIManager.OnStoreClosed += OnStoreClosed;
		CalendarUIManager.OnCalendarClosed += OnCalendarClosed;
		BadgeUIManager.OnBadgeBoardClosed += OnBadgeBoardClosed;
    }

	//Clean all event listeners and static references
	void OnDestroy(){
		NoteUIManager.OnNoteClosed -= OnNoteClosed;
		StoreUIManager.OnStoreClosed -= OnStoreClosed;
		CalendarUIManager.OnCalendarClosed -= OnCalendarClosed;
		BadgeUIManager.OnBadgeBoardClosed -= OnBadgeBoardClosed;
		UIRoot = null;
	}

	// If set to true (GUI menus are active), add a collider in UIRoot to stop user from clicking on anything else.
	// If set to false, deactivate the collider.
	public static void SetActiveGUIModeLock(bool GUIActive){
		if (UIRoot == null){
			UIRoot = GameObject.Find("UI Root (2D)");
		}
		BoxCollider col = UIRoot.collider as BoxCollider;
		if (UIRoot.collider == null){
			col = UIRoot.AddComponent<BoxCollider>();
			col.center = new Vector3(0,0,50); // so this collider is behind all actual GUI elements and won't interfere with them
			col.size = new Vector3(3000, 3000, 1); // this should be big enough to account for all different resolutions
		}
		col.enabled = GUIActive;
	}

	// assigning methods that get called when these individual objects get called in the scene
	void AssignOnTapEvents(){
		switch(Application.loadedLevelName){
			case "NewBedRoom":
				GameObject.Find("GO_Laptop").GetComponent<TapItem>().OnTap += OnTapLaptop;
				GOCalendarObject.GetComponent<TapItem>().OnTap += OnTapCalendar;
				GOSlotMachineObject.GetComponent<TapItem>().OnTap += OnTapSlotMachine;
				GORealInhalerObject.GetComponent<TapItem>().OnTap += OnTapRealInhaler;
				GOTeddyInhalerObject.GetComponent<TapItem>().OnTap += OnTapTeddyInhaler;
				GOBadgeObject.GetComponent<TapItem>().OnTap += OnTapBadgeBoard;
				GODojoObject.GetComponent<TapItem>().OnTap += OnTapDojoDoor;
				GOYardSignObject.GetComponent<TapItem>().OnTap += OnTapYardSign;
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
			SetActiveGUIModeLock(true);

			//Hide other UI objects
			navigationUIManager.HidePanel();
			inventoryUIManager.HidePanel();

			GA.API.Design.NewEvent("UserTouch:Note");
		}
	}
	private void OnNoteClosed(object sender, EventArgs e){
		ClickLock();
		cameraMove.ZoomOutMove();

		//Show other UI object
		navigationUIManager.ShowPanel();
		inventoryUIManager.ShowPanel();
	}
	//==============================

	//==============Store=================
	public void OnClickStore(){
		if(CanRespondToTap()){
			storeUIManager.StoreClicked();
			ClickLock();
			ModeLock();
			SetActiveGUIModeLock(true);

			//Hide other UI objects
			navigationUIManager.HidePanel();

			GA.API.Design.NewEvent("UserTouch:Store");
		}
	}
	private void OnStoreClosed(object sender, EventArgs e){
		ReleaseClickLock();
		ReleaseModeLock();
		SetActiveGUIModeLock(false);

		//Show other UI object
		navigationUIManager.ShowPanel();
	}
	//==================================

	//===========Calendar============
	void OnTapCalendar(){
		if (CanRespondToTap()){
			calendarUIManager.CalendarClicked();
			ClickLock();
			ModeLock();
			SetActiveGUIModeLock(true);

			//Hide other UI objects
			navigationUIManager.HidePanel();
			inventoryUIManager.HidePanel();

			GA.API.Design.NewEvent("UserTouch:Calendar");
		}
	}
	private void OnCalendarClosed(object sender, EventArgs e){
		ReleaseClickLock();
		ReleaseModeLock();
		SetActiveGUIModeLock(false);
		//Show other UI object
		navigationUIManager.ShowPanel();
		inventoryUIManager.ShowPanel();
	}
	//==========================

	//=================Badge Board=====================
	private void OnTapBadgeBoard(){
		if (CanRespondToTap()){
			badgeUIManager.BadgeBoardClicked();
			cameraMove.ZoomToggle(ZoomItem.BadgeBoard);
			ClickLock();
			ModeLock();

			//Hide other UI objects
			navigationUIManager.HidePanel();
			hudUIManager.HidePanel();
			inventoryUIManager.HidePanel();

			GA.API.Design.NewEvent("UserTouch:BadgeBoard");
		}
	}
	private void OnBadgeBoardClosed(object senders, EventArgs e){
		ReleaseClickLock();
		ReleaseModeLock();

		cameraMove.ZoomOutMove();

		//Show other UI Objects
		navigationUIManager.ShowPanel();
		hudUIManager.ShowPanel();
		inventoryUIManager.ShowPanel();
	}
	//=========================================

	//========================Dojo Door================
	private void OnTapDojoDoor(){
		if(CanRespondToTap()){
			// GODojoObject.GetComponent<DojoUIManager>().DojoDoorClicked();
			GODojoObject.GetComponent<Animator>().SetBool("Open", true);
			cameraMove.ZoomToggle(ZoomItem.Dojo);
			ClickLock();
			ModeLock();

			//Hide other UI objects
			navigationUIManager.HidePanel();
			hudUIManager.HidePanel();
			inventoryUIManager.HidePanel();

			GA.API.Design.NewEvent("UserTouch:Dojo");

			UILoadScreen.SetActive(true);
			Application.LoadLevel("Dojo");
		}
	}
	// private void OnDojoDoorClosed(object senders, EventArgs e){
	// 	GODojoObject.GetComponent<Animator>().SetBool("Open", false);
	// 	ClickLock();
	// 	cameraMove.ZoomOutMove();

	// 	//Show other UI objects
	// 	navigationUIManager.ShowPanel();
	// 	hudUIManager.ShowPanel();
	// 	inventoryUIManager.ShowPanel();
	// }
	//=======================

	//====================Yard Sign========================
	private void OnTapYardSign(){
		//UILoadScreen.SetActive(true);
		//Application.LoadLevel("Yard");
	}


	//=========================================
	void OnTapLaptop(){
		if (CanRespondToTap()){
			//NOTE: logic not implemented so leaving it out for now
			// challengesGUI.ChallengesClicked();
			// ClickLock();
			// ModeLock();
			GA.API.Design.NewEvent("UserTouch:Computer");
		}
	}

	void OnTapSlotMachine(){
		if (CanRespondToTap()){
			cameraMove.ZoomToggle(ZoomItem.SlotMachine);
			ClickLock();
			ModeLock();
			GA.API.Design.NewEvent("UserTouch:SlotMachine");
		}
	}
	void OnTapRealInhaler(){
		if (CanRespondToTap()){
			if (DataManager.Instance.Tutorial.FirstTimeRealInhaler){
				return; // taken care of in Tutorial.cs
			}
			if (CalendarLogic.CanUseRealInhaler){
				OpenRealInhaler();
			}
			else {
				NotificationUIManager.Instance.EnqueuePopupNotificationOneButton(
					"I don't need this right now.",
					delegate(){}
				);
			}
			GA.API.Design.NewEvent("UserTouch:RealInhaler");
		}
	}

	// also called in tutorial as a callback
	public void OpenRealInhaler(){
		cameraMove.ZoomToggle(ZoomItem.RealInhaler);
		ClickLock();
		ModeLock();

		//Hide other UI Objects
		navigationUIManager.HidePanel();
		hudUIManager.HidePanel();
	}

	public void OnTapTeddyInhaler(){
		if (CanRespondToTap()){
			// cameraMove.TeddyInhalerZoomToggle();
			cameraMove.ZoomToggle(ZoomItem.PracticeInhaler);
			ClickLock();
			ModeLock();
			GA.API.Design.NewEvent("UserTouch:TeddyInhaler");
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
		SetActiveGUIModeLock(false);
	}
}
