using UnityEngine;
using System.Collections;

//---------------------------------------------------
// WellapadUIManager
// Manager for the Wellapad UI.
//---------------------------------------------------

public class WellapadUIManager : SingletonUI<WellapadUIManager> {
	public GameObject wellapadButton; //wellapad button in navigation

	private GameObject goWellapadUI; // the actual game object of the wellapad
	private WellapadScreenManager wellapadScreenManager; //script that handles wellapad screen state

	//Return WellapadScreenManager script
	public WellapadScreenManager GetScreenManager() {
		return wellapadScreenManager;	
	}
	
	//---------------------------------------------------
	// _Start()
	//---------------------------------------------------	
	protected override void _Start() {
		// instantiate the actual wellapad object
		GameObject resourceWellapad = Resources.Load( "WellapadUI" ) as GameObject;
		goWellapadUI = LgNGUITools.AddChildWithPosition( GameObject.Find("Anchor-Center"), resourceWellapad );	
		
		// set the tween target on the wellapad object to this object
		goWellapadUI.GetComponent<TweenToggle>().ShowTarget = gameObject;
		
		wellapadScreenManager = goWellapadUI.GetComponent<WellapadScreenManager>();
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
		
		// set the right screen
		RefreshScreen();
	}
	
	//---------------------------------------------------
	// RefreshScreen()
	// Sets the proper screen on the wellapad.
	//---------------------------------------------------	
	public void RefreshScreen() {
		wellapadScreenManager.SetScreen();
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
}
