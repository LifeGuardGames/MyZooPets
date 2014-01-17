using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// EditDecosUIManager()
// This UI manager is responisble for the edit
// decoration mode/system.
//---------------------------------------------------


public class NodeSelectedArgs : EventArgs {
	public GameObject Node{get; set;}
}

public class EditDecosUIManager : SingletonUI<EditDecosUIManager> {	
	//------------ Event Handlers ----------------------------------
	public event EventHandler<NodeSelectedArgs> OnNodeSelected;		// when a deco node is selected
	//--------------------------------------------------------------
	
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
	
	// "saved" deco node for when the user is in the choose menu and opens the shop
	private DecorationNode nodeSaved;
	public bool IsNodeSaved() {
		bool bSaved = nodeSaved != null;
		return bSaved;
	}
	
	void Start() {
		eModeType = UIModeTypes.EditDecos;
		
		// cache the tween on the edit button for easier use
		tweenEdit = goEdit.GetComponent<PositionTweenToggle>();
		
		// if edit mode is currently disabled, destroy the button
		if ( bDisableEditMode )
			Destroy( goEdit );
		
		// listen for partition change event
		CameraManager.Instance.GetPanScript().OnPartitionChanging += OnPartitionChanging;
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
		
		// clear any saved node
		nodeSaved = null;
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
	// OpenShop()
	// Opens the store leading to decorations for the
	// current category the playing is trying to place.
	// This is a little messy/complicated, because we are
	// basically faking the deco UI closing and the
	// shop UI opening.  It's not legit because all the
	// tweening and demux make it diffcult to do legitly.
	//---------------------------------------------------	
	private void OpenShop() {
		// save the node the player was trying to use
		nodeSaved = scriptChooseUI.GetNode();
		
		// close this UI and show the edit decos button
		CloseChooseMenu_( false );
		tweenEdit.Show();
		
		// push the shop mode type onto the click manager stack
		ClickManager.Instance.Lock( UIModeTypes.Store );
	
		// open the shop
		StoreUIManager.OnShortcutModeEnd += ReopenChooseMenu;	
		StoreUIManager.Instance.OpenToSubCategory( "Decorations", true );
		
		// open the specific sub category in the shop
		string strCat = nodeSaved.GetDecoType().ToString();
		StoreUIManager.Instance.CreateSubCategoryItemsTab( strCat, StoreUIManager.Instance.colors[(int)nodeSaved.GetDecoType()]);
	}
	
	//---------------------------------------------------
	// ReopenChooseMenu()
	// This function is called from the store UI when the
	// store closes and the user had opened the store
	// from the deco system.
	//---------------------------------------------------	
	private void ReopenChooseMenu(object sender, EventArgs args) {
		// hide the edit button
		tweenEdit.Hide();
		
		// update the menu
		UpdateChooseMenu( nodeSaved );	
		
		// pop the mode we pushed earlier from the click manager
		ClickManager.Instance.ReleaseLock();

		StoreUIManager.OnShortcutModeEnd -= ReopenChooseMenu;
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
		
		// send out a callback for deco nodes to update their highlight state
		if ( OnNodeSelected != null ) {
			NodeSelectedArgs args = new NodeSelectedArgs();
			args.Node = decoNode.gameObject;
			OnNodeSelected( this, args );
		}
	}
	
	//---------------------------------------------------
	// CloseChooseMenu()
	// Closes the choose decoration menu -- note that
	// this does not exit deco mode.  The _ version of
	// this function is because "SendMessage" does not
	// like default parameters...
	//---------------------------------------------------
	public void CloseChooseMenu() {
		CloseChooseMenu_();	
		
		// send out a callback for deco nodes to update their highlight state (the menu is closing so none should be highlighted)
		if ( OnNodeSelected != null ) {
			NodeSelectedArgs args = new NodeSelectedArgs();
			OnNodeSelected( this, args );
		}		
	}
	
	public void CloseChooseMenu_( bool bShowExit = true ) {
		PositionTweenToggle tween = goChoosePanel.GetComponent<PositionTweenToggle>();
		if (!tween.IsShowing)
			Debug.LogError("Something trying to close an already closed choose menu for deco edit.");
		else{
			tween.Hide();
			
			if ( bShowExit )
				tweenExit.Show();
		}
		// we possibly want to Resources.UnloadUnusedAssets() here because the menu is instantiated
	}
	
	//---------------------------------------------------
	// GetEditButtonPosition()
	// Returns the position of the edit button.  Used for
	// tutorials.
	//---------------------------------------------------		
	public Vector3 GetEditButtonPosition() {
		return goEdit.transform.position;	
	}
	
	//---------------------------------------------------
	// GetTutorialEntry()
	// Returns the special tutorial entry in the choose
	// menu.  Used for tutorials.
	//---------------------------------------------------		
	public GameObject GetTutorialEntry() {
		GameObject goEntry = scriptChooseUI.GetTutorialEntry();
		return goEntry;
	}
	
	//---------------------------------------------------
	// GetChooseScript()
	// Returns the choose menu script.  Used for tutorials.
	//---------------------------------------------------		
	public ChooseDecorationUI GetChooseScript() {
		return scriptChooseUI;
	}
	
	//---------------------------------------------------
	// GetClickLockExceptions()
	// Edit decos UI actually allows moving.
	//---------------------------------------------------
	protected override List<ClickLockExceptions> GetClickLockExceptions() {
		List<ClickLockExceptions> listExceptions = new List<ClickLockExceptions>();
		listExceptions.Add( ClickLockExceptions.Moving );
		
		return listExceptions;
	}	
	
	//---------------------------------------------------
	// OnPartitionChanging()
	// When the player is changing rooms.
	//---------------------------------------------------	
	public void OnPartitionChanging( object sender, PartitionChangedArgs args ) {
		// if the user is changing rooms in deco mode, close the choose deco UI if it is open
		PositionTweenToggle tween = goChoosePanel.GetComponent<PositionTweenToggle>();
		if ( tween.IsShowing )
			CloseChooseMenu();
	}
}
