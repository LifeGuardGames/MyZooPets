using UnityEngine;
using System.Collections;

//---------------------------------------------------
// WellapadUIManager
// Manager for the Wellapad UI.
//---------------------------------------------------

public class WellapadUIManager : SingletonUI<WellapadUIManager> {
	// the actual game object of the wellapad
	private GameObject goWellapadUI;
	
	//---------------------------------------------------
	// _Start()
	//---------------------------------------------------	
	protected override void _Start() {
		// instantiate the actual wellapad object
		GameObject resourceWellapad = Resources.Load( "WellapadUI" ) as GameObject;
		goWellapadUI = LgNGUITools.AddChildWithPosition( GameObject.Find("Anchor-Center"), resourceWellapad );	

	}
	
	//---------------------------------------------------
	// _OpenUI()
	//---------------------------------------------------	
	protected override void _OpenUI(){
		//Hide other UI objects
		NavigationUIManager.Instance.HidePanel();

		// show the UI itself
		goWellapadUI.GetComponent<TweenToggleDemux>().Show();
	}
	
	//---------------------------------------------------
	// _CloseUI()
	//---------------------------------------------------	
	protected override void _CloseUI(){
		//Show other UI object
		NavigationUIManager.Instance.ShowPanel();		
		
		// hide the UI
		goWellapadUI.GetComponent<TweenToggleDemux>().Hide();
	}
}
