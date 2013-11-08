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
	
	// the exit panels for leaving edit mode
	public TweenToggleDemux tweenExit;
	
	// the edit deco button
	public GameObject goEdit;
	private PositionTweenToggle tweenEdit;
	
	// the choose deco panel
	public GameObject goChoosePanel;
	public ChooseDecorationUI scriptChooseUI;
	
	void Start() {
		// cache the tween on the edit button for easier use
		tweenEdit = goEdit.GetComponent<PositionTweenToggle>();
		
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
		InventoryUIManager.Instance.HidePanel();
		
		// show the exit panels
		tweenExit.Show();	
		
		// hide the edit button
		HideNavButton();
		
		// hide the pet so it doesn't get in the way
		GameObject goPet = PetMovement.Instance.GetPetGameObject();
		goPet.SetActive( false );
	}
	
	//---------------------------------------------------
	// _CloseUI()
	//---------------------------------------------------	
	protected override void _CloseUI(){
		// if the choose menu was open, close it
		PositionTweenToggle tween = goChoosePanel.GetComponent<PositionTweenToggle>();
		if (tween.IsShowing)
			tween.Hide();		
		
		//Show other UI object
		NavigationUIManager.Instance.ShowPanel();	
		InventoryUIManager.Instance.ShowPanel();
		
		// hide the exit panels
		tweenExit.Hide();	
		
		// show the edit button again
		ShowNavButton();
		
		// show the pet again
		GameObject goPet = PetMovement.Instance.GetPetGameObject();
		goPet.SetActive( true );		
	}
	
	public void ShowNavButton(){
		tweenEdit.Show();
		
		// unload unused resources (since we may have instantiated some that are no longer needed)
		Resources.UnloadUnusedAssets();
	}
	
	public void HideNavButton(){
		tweenEdit.Hide();
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
		PositionTweenToggle tween = goChoosePanel.GetComponent<PositionTweenToggle>();
		if (!tween.IsShowing){
			tween.Show();
			tweenExit.Hide();
		}
		
		// update the menu based on the incoming deco node
		scriptChooseUI.UpdateItems( decoNode );
	}
	
	//---------------------------------------------------
	// CloseChooseMenu()
	// Closes the choose decoration menu -- note that
	// this does not exit deco mode.
	//---------------------------------------------------	
	public void CloseChooseMenu() {
		PositionTweenToggle tween = goChoosePanel.GetComponent<PositionTweenToggle>();
		if (!tween.IsShowing)
			Debug.Log("Something trying to close an already closed choose menu for deco edit.");
		else{
			tween.Hide();
			tweenExit.Show();
		}
		// we possibly want to Resources.UnloadUnusedAssets() here because the menu is instantiated
	}
	
	public Vector3 GetEditButtonPosition() {
		return goEdit.transform.position;	
	}
}
