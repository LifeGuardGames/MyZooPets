using UnityEngine;
using System;
using System.Collections;

//---------------------------------------------------
// WellapadUIManager
// Manager for the Wellapad UI.
//---------------------------------------------------

public class WellapadUIManager : SingletonUI<WellapadUIManager> {
	public GameObject goWellapadUI; // the actual game object of the wellapad
	public GameObject messagePro;	// Switching between pro and lite versions
	public GameObject messageLite;

	private WellapadScreenUIController wellapadScreenUIController; //script that handles wellapad screen state

	//Return WellapadScreenUIController script
	public WellapadScreenUIController GetScreenManager() {
		return wellapadScreenUIController;
	}

	void Awake(){
		// instantiate the actual wellapad object
		// GameObject resourceWellapad = Resources.Load( "WellapadUI" ) as GameObject;
		// goWellapadUI = LgNGUITools.AddChildWithPosition( GameObject.Find("Anchor-Center"), resourceWellapad );

		// Change the text according to Lite version detection
		if(VersionManager.Instance.IsLite()){
			messagePro.SetActive(false);
			messageLite.SetActive(true);
		}
		else{
			messagePro.SetActive(true);
			messageLite.SetActive(false);
		}
	}

	//---------------------------------------------------
	// _Start()
	//---------------------------------------------------	
	protected override void _Start() {
		// set the tween target on the wellapad object to this object
		goWellapadUI.GetComponent<TweenToggle>().ShowTarget = gameObject;
		wellapadScreenUIController = goWellapadUI.GetComponent<WellapadScreenUIController>();

		WellapadMissionController.Instance.OnMissionsRefreshed += RefreshScreen;
		RefreshScreen();
	}
	
	//---------------------------------------------------
	// _OpenUI()
	//---------------------------------------------------	
	protected override void _OpenUI(){
		//Hide other UI objects
		NavigationUIManager.Instance.HidePanel();
		InventoryUIManager.Instance.HidePanel();
		EditDecosUIManager.Instance.HideNavButton();

		// show the UI itself
		goWellapadUI.GetComponent<TweenToggle>().Show();
	}

	//---------------------------------------------------
	// _CloseUI()
	//---------------------------------------------------	
	protected override void _CloseUI(){
		//Show other UI object
		NavigationUIManager.Instance.ShowPanel();
		InventoryUIManager.Instance.ShowPanel();
		EditDecosUIManager.Instance.ShowNavButton();
		
		// hide the UI
		goWellapadUI.GetComponent<TweenToggle>().Hide();
	}

	//---------------------------------------------------
	// RefreshScreen()
	// Sets the proper screen on the wellapad.
	//---------------------------------------------------	
	public void RefreshScreen() {
		wellapadScreenUIController.SetScreen();
	}

	private void RefreshScreen(object sender, EventArgs args){
		RefreshScreen();
	}

	
}
