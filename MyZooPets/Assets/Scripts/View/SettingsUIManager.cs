using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// SettingsUIManager
// This UI manager is responisble for letting the
// user select their language.
// Also handles audio controls
//---------------------------------------------------

public class SettingsUIManager : SingletonUI<SettingsUIManager> {
	
	// the NavPanel that launches this manager
	public GameObject firstTimeNavPanel;
	private TweenToggleDemux tweenNav;
	
	// SettingsPanel holding all settings
	public GameObject settingsPanel;
	private PositionTweenToggle tweenSettings;
	
	void Start() {
		// cache the tweens for easier use
		tweenNav = firstTimeNavPanel.GetComponent<TweenToggleDemux>();
		tweenSettings = settingsPanel.GetComponent<PositionTweenToggle>();
	}
	
	//---------------------------------------------------
	// _OpenUI()
	//---------------------------------------------------
	protected override void _OpenUI(){
		// hide the panel that opened this ui
		tweenNav.Hide();

		// Hide PetSelection Area
		SelectionUIManager.Instance.HidePanel();

		
		// show the panel holding all settings
		tweenSettings.Show();		
	}
	
	//---------------------------------------------------
	// _CloseUI()
	//---------------------------------------------------	
	protected override void _CloseUI(){
		// show the panel again
		tweenNav.Show();

		//Show PetSelection Area
		SelectionUIManager.Instance.ShowPanel();
		
		// hide the panel holding all settings
		tweenSettings.Hide();		
	}
}