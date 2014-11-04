using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Wellapad user interface manager.
/// </summary>

public class WellapadUIManager : SingletonUI<WellapadUIManager> {
//	public GameObject goWellapadUI; // the actual game object of the wellapad

	private WellapadScreenUIController wellapadScreenUIController; //script that handles wellapad screen state

	//Return WellapadScreenUIController script
	public WellapadScreenUIController GetScreenManager() {
		return wellapadScreenUIController;
	}
	
	protected override void Start() {
		// set the tween target on the wellapad object to this object
		GetComponent<TweenToggle>().ShowTarget = gameObject;
		wellapadScreenUIController = GetComponent<WellapadScreenUIController>();

		WellapadMissionController.Instance.OnMissionsRefreshed += RefreshScreen;
		RefreshScreen();
	}

	protected override void _OpenUI(){
		//Hide other UI objects
		NavigationUIManager.Instance.HidePanel();
		InventoryUIManager.Instance.HidePanel();
		RoomArrowsUIManager.Instance.HidePanel();

		// show the UI itself
		GetComponent<TweenToggle>().Show();

//		bool hasActiveTasks = WellapadMissionController.Instance.HasActiveTasks();
	}

	//---------------------------------------------------
	// _CloseUI()
	//---------------------------------------------------	
	protected override void _CloseUI(){
		//Show other UI object
		NavigationUIManager.Instance.ShowPanel();
		InventoryUIManager.Instance.ShowPanel();
		RoomArrowsUIManager.Instance.ShowPanel();
		
		// hide the UI
		GetComponent<TweenToggle>().Hide();
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
