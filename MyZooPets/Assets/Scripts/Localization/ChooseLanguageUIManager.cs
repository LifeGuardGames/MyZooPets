using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// ChooseLanguageUIManager
// This UI manager is responisble for letting the
// user select their language.
//---------------------------------------------------

public class ChooseLanguageUIManager : SingletonUI<ChooseLanguageUIManager> {	
	// temp boolean to control whether or not choose language is accessible
	public bool bDisable = false;
	
	// the button that launches this manager
	public GameObject goButton;
	private MoveTweenToggle tweenButton;
	
	// panel holding all language buttons
	public GameObject goChoosePanel;
	private MoveTweenToggle tweenChoose;
	
	void Start() {
		// cache the tweens for easier use
		tweenButton = goButton.GetComponent<MoveTweenToggle>();
		tweenChoose = goChoosePanel.GetComponent<MoveTweenToggle>();
		
		// if currently disabled, destroy the button
		if ( bDisable )
			Destroy( goButton );
	}
	
	//---------------------------------------------------
	// _OpenUI()
	//---------------------------------------------------	
	protected override void _OpenUI(){
		// hide the button that opened this ui
		tweenButton.Hide();
		
		// show the panel holding all language buttons
		tweenChoose.Show();		
	}
	
	//---------------------------------------------------
	// _CloseUI()
	//---------------------------------------------------	
	protected override void _CloseUI(){
		// show the edit button again
		tweenButton.Show();
		
		// hide the panel holding all language buttons
		tweenChoose.Hide();		
	}
}