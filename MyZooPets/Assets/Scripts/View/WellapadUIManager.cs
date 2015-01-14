using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Wellapad user interface manager.
/// </summary>

public class WellapadUIManager : SingletonUI<WellapadUIManager> {
	public TweenToggle wellapadTweenParent;

	private WellapadScreenUIController wellapadScreenUIController; //script that handles wellapad screen state

	//Return WellapadScreenUIController script
	public WellapadScreenUIController GetScreenManager() {
		return wellapadScreenUIController;
	}
	
	protected override void Start() {
		// set the tween target on the wellapad object to this object
		wellapadTweenParent.ShowTarget = gameObject;
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
		wellapadTweenParent.Show();
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
