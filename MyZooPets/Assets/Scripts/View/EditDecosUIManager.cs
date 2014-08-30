using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// EditDecosUIManager()
// This UI manager is responisble for the edit
// decoration mode/system.
//---------------------------------------------------
public class NodeSelectedArgs : EventArgs{
	public GameObject Node{ get; set; }
}

public class EditDecosUIManager : SingletonUI<EditDecosUIManager>{	
	//------------ Event Handlers ----------------------------------
	public event EventHandler<NodeSelectedArgs> OnNodeSelected;		// when a deco node is selected
	//--------------------------------------------------------------

	public bool IsDisableEditMode = false; // temp boolean to control whetehr or not edit mode is accessible
	public TweenToggleDemux tweenExit; // the exit panels for leaving edit mode
	public GameObject goEdit; // the edit deco button
	public GameObject shopButton; // The shop button
	public GameObject goChoosePanel; // the choose deco panel
	public ChooseDecorationUIController chooseDecorationScript;

	private PositionTweenToggle tweenEdit;
	private DecorationNode nodeSaved; // "saved" deco node for when the user is in the choose menu and opens the shop

	void Start(){
		eModeType = UIModeTypes.EditDecos;
		
		// cache the tween on the edit button for easier use
		tweenEdit = goEdit.GetComponent<PositionTweenToggle>();
		
		// if edit mode is currently disabled, destroy the button
		if(IsDisableEditMode)
			Destroy(goEdit);
		
		// listen for partition change event
		CameraManager.Instance.GetPanScript().OnPartitionChanging += OnPartitionChanging;
	}

	public bool IsNodeSaved(){
		bool isSaved = nodeSaved != null;
		return isSaved;
	}

	/// <summary>
	/// Updates the choose menu.
	/// Called when a decoration node is selected, this
	/// function kicks off the process of displaying a 
	/// menu for showing the user which decorations they
	/// may place.
	/// </summary>
	/// <param name="decoNode">Deco node.</param>
	public void UpdateChooseMenu(DecorationNode decoNode){
		// if the menu is not showing, show it
		TweenToggleDemux tween = goChoosePanel.GetComponent<TweenToggleDemux>();
		if(!tween.IsShowing){
			tween.Show();
			tweenExit.Hide();
			//ClickManager.Instance.Lock( UIModeTypes.EditDecos, GetClickLockExceptions());
		}
		
		// update the menu based on the incoming deco node
		chooseDecorationScript.UpdateItems(decoNode);
		
		// send out a callback for deco nodes to update their highlight state
		if(OnNodeSelected != null){
			NodeSelectedArgs args = new NodeSelectedArgs();
			args.Node = decoNode.gameObject;
			OnNodeSelected(this, args);
		}
	}
	
	/// <summary>
	/// Closes the choose menu.
	/// Closes the choose decoration menu -- note that
	/// this does not exit deco mode.  The _ version of
	/// this function is because "SendMessage" does not
	/// like default parameters...
	/// </summary>
	public void CloseChooseMenu(){
		CloseChooseMenuHelper();	
		
		// send out a callback for deco nodes to update their highlight state (the menu is closing so none should be highlighted)
		if(OnNodeSelected != null){
			NodeSelectedArgs args = new NodeSelectedArgs();
			OnNodeSelected(this, args);
		}		
	}
	
	public void CloseChooseMenuHelper(bool isShowingExit = true){
		TweenToggleDemux tween = goChoosePanel.GetComponent<TweenToggleDemux>();
		if(!tween.IsShowing)
			Debug.LogError("Something trying to close an already closed choose menu for deco edit.");
		else{
			tween.Hide();
			
			if(isShowingExit)
				tweenExit.Show();
		}
		// we possibly want to Resources.UnloadUnusedAssets() here because the menu is instantiated
	}
	
	/// <summary>
	/// Gets the edit button position. Used for tutorials
	/// </summary>
	/// <returns>The edit button position.</returns>
	public Vector3 GetEditButtonPosition(){
		return goEdit.transform.position;	
	}
	
	/// <summary>
	/// Gets the special tutorial entry. Used for tutorials
	/// </summary>
	/// <returns>The tutorial entry.</returns>
	public GameObject GetTutorialEntry(){
		GameObject goEntry = chooseDecorationScript.GetTutorialEntry();
		return goEntry;
	}

	/// <summary>
	/// Gets the shop button.
	/// </summary>
	/// <returns>The shop button.</returns>
	public GameObject GetShopButton(){
		return shopButton;
	}
	
	/// <summary>
	/// Gets the choose menu script. Used for tutorials
	/// </summary>
	/// <returns>The choose script.</returns>
	public ChooseDecorationUIController GetChooseScript(){
		return chooseDecorationScript;
	}

	public void ShowNavButton(){
		tweenEdit.Show();
		
		// unload unused resources (since we may have instantiated some that are no longer needed)
		Resources.UnloadUnusedAssets();
	}
	
	public void HideNavButton(){
		tweenEdit.Hide();
	}

	protected override void _OpenUI(){
		//Hide other UI objects
		NavigationUIManager.Instance.HidePanel();
		InventoryUIManager.Instance.HidePanel();

		RoomArrowsUIManager.Instance.HidePanel();
		Invoke("ShowDecoRoomArrows", 0.5f);
		
		// show the exit panels
		tweenExit.Show();	
		
		// hide the edit button
		HideNavButton();
		
		// hide the pet so it doesn't get in the way
		PetAnimationManager.Instance.DisableAnimation();
	}

	private void ShowDecoRoomArrows(){
		RoomArrowsUIManager.Instance.ShowPanel();
	}
	
	protected override void _CloseUI(){
		// if the choose menu was open, close it
		TweenToggleDemux tween = goChoosePanel.GetComponent<TweenToggleDemux>();
		if(tween.IsShowing)
			tween.Hide();		
		
		//Show other UI object
		NavigationUIManager.Instance.ShowPanel();	
		InventoryUIManager.Instance.ShowPanel();
		RoomArrowsUIManager.Instance.ShowPanel();
		
		// hide the exit panels
		tweenExit.Hide();	
		
		// show the edit button again
		ShowNavButton();
		
		// show the pet again
		PetAnimationManager.Instance.EnableAnimation();
		
		// clear any saved node
		nodeSaved = null;
	}
	
	/// <summary>
	/// Gets the click lock exceptions. Edit decos UI actualy allows moving
	/// </summary>
	/// <returns>The click lock exceptions.</returns>
	protected override List<ClickLockExceptions> GetClickLockExceptions(){
		List<ClickLockExceptions> listExceptions = new List<ClickLockExceptions>();
		listExceptions.Add(ClickLockExceptions.Moving);
		
		return listExceptions;
	}	

	/// <summary>
	/// Opens the store leading to decorations for the
	/// current category the playing is trying to place.
	/// This is a little messy/complicated, because we are
	/// basically faking the deco UI closing and the
	/// shop UI opening.  It's not legit because all the
	/// tweening and demux make it diffcult to do legitly.
	/// </summary>
	private void OpenShop(){
		// save the node the player was trying to use
		nodeSaved = chooseDecorationScript.GetNode();

		// hide swipe arrow because not needed in shop mode
		RoomArrowsUIManager.Instance.HidePanel();
		
		// close this UI and show the edit decos button
		CloseChooseMenuHelper(false);
		tweenEdit.Show();
		
		// push the shop mode type onto the click manager stack
		ClickManager.Instance.Lock(UIModeTypes.Store);
	
		// open the shop
		StoreUIManager.OnShortcutModeEnd += ReopenChooseMenu;	
		StoreUIManager.Instance.OpenToSubCategory("Decorations", true);
		
		// open the specific sub category in the shop
		string category = nodeSaved.GetDecoType().ToString();
		StoreUIManager.Instance.CreateSubCategoryItemsTab(category, Color.white);
	}

	/// <summary>
	/// This function is called from the store UI when the
	/// store closes and the user had opened the store
	/// from the deco system.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void ReopenChooseMenu(object sender, EventArgs args){
		// hide the edit button
		tweenEdit.Hide();

		// show swipe arrows
		RoomArrowsUIManager.Instance.ShowPanel();
		
		// update the menu
		UpdateChooseMenu(nodeSaved);	
		
		// pop the mode we pushed earlier from the click manager
		ClickManager.Instance.ReleaseLock();

		StoreUIManager.OnShortcutModeEnd -= ReopenChooseMenu;
	}
	
	/// <summary>
	/// Raises the partition changing event. When the player is changing room
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void OnPartitionChanging(object sender, PartitionChangedArgs args){
		// if the user is changing rooms in deco mode, close the choose deco UI if it is open
		TweenToggleDemux tween = goChoosePanel.GetComponent<TweenToggleDemux>();
		if(tween.IsShowing)
			CloseChooseMenu();
	}
}
