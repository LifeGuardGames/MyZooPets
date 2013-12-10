﻿using UnityEngine;
using System.Collections;

//---------------------------------------------------
// WellapadUIManager
// Manager for the Wellapad UI.
//---------------------------------------------------

public class WellapadUIManager : SingletonUI<WellapadUIManager> {
	// the actual game object of the wellapad
	private GameObject goWellapadUI;
	private WellapadScreenManager scriptScreenManager;
	
	//---------------------------------------------------
	// _Start()
	//---------------------------------------------------	
	protected override void _Start() {
		// instantiate the actual wellapad object
		GameObject resourceWellapad = Resources.Load( "WellapadUI" ) as GameObject;
		goWellapadUI = LgNGUITools.AddChildWithPosition( GameObject.Find("Anchor-Center"), resourceWellapad );	
		
		scriptScreenManager = goWellapadUI.GetComponent<WellapadScreenManager>();
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
		
		scriptScreenManager.SetScreen();
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
