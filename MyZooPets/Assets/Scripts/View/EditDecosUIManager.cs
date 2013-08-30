using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EditDecosUIManager : SingletonUI<EditDecosUIManager> {	
	// the exit button
	public MoveTweenToggle tweenExit;
	
	// the choose deco panel
	public GameObject goChoosePanel;
	public ChooseDecorationUI scriptChooseUI;
	
	protected override void _OpenUI(){
		//Hide other UI objects
		NavigationUIManager.Instance.HidePanel();
		
		// show the exit button
		tweenExit.Show();		
	}

	protected override void _CloseUI(){
		// if the choose menu was open, close it
		MoveTweenToggle tween = goChoosePanel.GetComponent<MoveTweenToggle>();
		if ( tween.IsShowing() )
			tween.Hide();		
		
		//Show other UI object
		NavigationUIManager.Instance.ShowPanel();	
		
		// hide the exit button
		tweenExit.Hide();	
	}
	
	public void UpdateChooseMenu( DecorationTypes eType ) {
		// if the menu is not showing, show it
		MoveTweenToggle tween = goChoosePanel.GetComponent<MoveTweenToggle>();
		if ( !tween.IsShowing() )
			tween.Show();
		
		scriptChooseUI.UpdateItems( eType );
	}
	
	public void CloseChooseMenu() {
		MoveTweenToggle tween = goChoosePanel.GetComponent<MoveTweenToggle>();
		if ( !tween.IsShowing() )
			Debug.Log("Something trying to close an already closed choose menu for deco edit.");
		else
			tween.Hide();
	}
}