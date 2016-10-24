using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Wellapad user interface manager
/// NOTE: This will open up with the fire crystal ui manager as well
/// </summary>
public class WellapadUIManager : SingletonUI<WellapadUIManager>{
	public TweenToggle wellapadTweenParent;

	private WellapadScreenUIController wellapadScreenUIController; //script that handles wellapad screen state

	protected override void Awake(){
		eModeType = UIModeTypes.Wellapad;
	}

	//Return WellapadScreenUIController script
	public WellapadScreenUIController GetScreenManager(){
		return wellapadScreenUIController;
	}
	
	protected override void Start(){
		// set the tween target on the wellapad object to this object
		wellapadTweenParent.ShowTarget = gameObject;
		wellapadScreenUIController = GetComponent<WellapadScreenUIController>();

		WellapadMissionController.Instance.OnMissionsRefreshed += RefreshScreen;
		RefreshScreen();
	}

	/// <summary>
	/// NOTE: Called from Fire Crystal UI Manager!!!
	/// </summary>
	protected override void _OpenUI(){
		//Hide other UI objects
		NavigationUIManager.Instance.HidePanel();
		InventoryUIManager.Instance.HidePanel();
		RoomArrowsUIManager.Instance.HidePanel();

		// Open the fire crystal UI manager together  with special settings
		FireCrystalUIManager.Instance.isLockModeInClickmanager = false;	// Temporary unlock
		FireCrystalUIManager.Instance.isIgnoreTweenLockOnClose = true;
		FireCrystalUIManager.Instance.OpenUI();

		// show the UI itself
		wellapadTweenParent.Show();
		RefreshScreen();
	}

	public void CloseUIProperly(){
		FireCrystalUIManager.Instance.CloseUIBasedOnScene();
	}

	/// <summary>
	/// NOTE: Called from Fire Crystal UI Manager!!!
	/// </summary>
	protected override void _CloseUI(){
		//Show other UI objects
		if(!ClickManager.Instance.IsStackContainsType(UIModeTypes.EditDecos)){	// If in deco mode dont show these
			NavigationUIManager.Instance.ShowPanel();
			InventoryUIManager.Instance.ShowPanel(false);
		}
		RoomArrowsUIManager.Instance.ShowPanel();

		// Close the fire crystal UI manager together with special settings
		FireCrystalUIManager.Instance.CloseUI();
		FireCrystalUIManager.Instance.isLockModeInClickmanager = true; // Set this back immediately after close
		FireCrystalUIManager.Instance.isIgnoreTweenLockOnClose = false;

		// hide the UI
		wellapadTweenParent.Hide();
	}

	//---------------------------------------------------
	// RefreshScreen()
	// Sets the proper screen on the wellapad.
	//---------------------------------------------------	
	public void RefreshScreen(){
		wellapadScreenUIController.SetScreen();
	}

	private void RefreshScreen(object sender, EventArgs args){
		RefreshScreen();
	}
}
