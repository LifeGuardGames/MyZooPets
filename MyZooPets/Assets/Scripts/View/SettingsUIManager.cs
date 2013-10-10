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
	//private TweenToggleDemux tweenNav;	// TODO move this out later, this has parent portal too
	
	// SettingsPanel holding all settings
	public GameObject settingsPanel;
	private TweenToggleDemux tweenSettings;
	
	void Start() {
		// cache the tweens for easier use
		//tweenNav = firstTimeNavPanel.GetComponent<TweenToggleDemux>();
		tweenSettings = settingsPanel.GetComponent<TweenToggleDemux>();
	}
	
	//---------------------------------------------------
	// _OpenUI()
	//---------------------------------------------------
	protected override void _OpenUI(){
		// show the panel holding all settings
		tweenSettings.Show();		
	}
	
	//---------------------------------------------------
	// _CloseUI()
	//---------------------------------------------------	
	protected override void _CloseUI(){
		// hide the panel holding all settings
		tweenSettings.Hide();		
	}
}