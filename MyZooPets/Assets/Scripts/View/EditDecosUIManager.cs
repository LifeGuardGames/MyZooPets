using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// EditDecosUIManager()
// This UI manager is responisble for the edit
// decoration mode/system.
//---------------------------------------------------

public class EditDecosUIManager : SingletonUI<EditDecosUIManager> {	
	// temp boolean to control whetehr or not edit mode is accessible
	public bool bDisableEditMode = false;
	
	// the exit button for leaving edit mode
	public MoveTweenToggle tweenExit;
	
	// the edit deco button
	public GameObject goEdit;
	private MoveTweenToggle tweenEdit;
	
	// the choose deco panel
	public GameObject goChoosePanel;
	public ChooseDecorationUI scriptChooseUI;
	
	void Start() {
		// cache the tween on the edit button for easier use
		tweenEdit = goEdit.GetComponent<MoveTweenToggle>();
		
		// if edit mode is currently disabled, destroy the button
		if ( bDisableEditMode )
			Destroy( goEdit );
	}
	
	//---------------------------------------------------
	// _OpenUI()
	//---------------------------------------------------	
	protected override void _OpenUI(){
		//Hide other UI objects
		NavigationUIManager.Instance.HidePanel();
		
		// show the exit button
		tweenExit.Show();	
		
		// hide the edit button
		tweenEdit.Hide();
	}
	
	//---------------------------------------------------
	// _CloseUI()
	//---------------------------------------------------	
	protected override void _CloseUI(){
		// if the choose menu was open, close it
		MoveTweenToggle tween = goChoosePanel.GetComponent<MoveTweenToggle>();
		if ( tween.IsShowing() )
			tween.Hide();		
		
		//Show other UI object
		NavigationUIManager.Instance.ShowPanel();	
		
		// hide the exit button
		tweenExit.Hide();	
		
		// show the edit button again
		tweenEdit.Show();
		
		// unload unused resources (since we may have instantiated some that are no longer needed)
		Resources.UnloadUnusedAssets();
	}
	
	//---------------------------------------------------
	// UpdateChooseMenu()
	// Called when a decoration node is selected, this
	// function kicks off the process of displaying a 
	// menu for showing the user which decorations they
	// may place.
	//---------------------------------------------------	
	public void UpdateChooseMenu( DecorationNode decoNode ) {
		// if the menu is not showing, show it
		MoveTweenToggle tween = goChoosePanel.GetComponent<MoveTweenToggle>();
		if ( !tween.IsShowing() )
			tween.Show();
		
		// update the menu based on the incoming deco node
		scriptChooseUI.UpdateItems( decoNode );
	}
	
	//---------------------------------------------------
	// CloseChooseMenu()
	// Closes the choose decoration menu -- note that
	// this does not exit deco mode.
	//---------------------------------------------------	
	public void CloseChooseMenu() {
		MoveTweenToggle tween = goChoosePanel.GetComponent<MoveTweenToggle>();
		if ( !tween.IsShowing() )
			Debug.Log("Something trying to close an already closed choose menu for deco edit.");
		else
			tween.Hide();
		
		// we possibly want to Resources.UnloadUnusedAssets() here because the menu is instantiated
	}
	
	public Vector3 GetEditButtonPosition() {
		return goEdit.transform.position;	
	}
}