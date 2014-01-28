using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//---------------------------------------------------
// ChooseDecorationUI
// This script is attached to the UI that appears
// when a decoration node is clicked.  It is a UI
// that lets the user set and remove decorations.
//---------------------------------------------------

public class ChooseDecorationUI : MonoBehaviour {
	
	// prefab that items in this UI are instantiated from
	public GameObject prefabChooseDecoEntry;
	
	// sound that gets played when a decoration is placed
	public string strSoundPlace;
	
	// the remove button game object
	public GameObject goRemoveButton;
	
	// Panel of the grid
	public UIPanel gridPanel;
	
	// the grid this UI places its items in
	public GameObject goGrid;	
	
	// the decoration node that this UI is currently representing
	private DecorationNode decoNodeCurrent;
	public DecorationNode GetNode() {
		return decoNodeCurrent;	
	}
	
	// save the 0th item in the deco menu for using in tutorials
	private GameObject goTutorialEntry;
	public GameObject GetTutorialEntry() {
		return goTutorialEntry;	
	}
	
	//=======================Events========================
    public EventHandler<EventArgs> OnDecoPlaced;   // when a decoration is placed	
	
	void Start(){
		// Reposition all the things nicely to stretch to the end of the screen
		Vector4 oldRange = gridPanel.clipRange;
		
		// The 52 comes from some wierd scaling issue.. not sure what it is but compensate now
		gridPanel.transform.localPosition = new Vector3(0f, gridPanel.transform.localPosition.y, 0f);
		gridPanel.clipRange = new Vector4(0f, oldRange.y, (float)(CameraManager.Instance.GetNativeWidth()), oldRange.w);
		
		// Position the grid origin to the left of the screen
		Vector3 gridPosition = goGrid.transform.localPosition;
		goGrid.transform.localPosition = new Vector3(
			(-1f * (CameraManager.Instance.GetNativeWidth()/2)) - gridPanel.transform.localPosition.x + 10,
			gridPosition.y, gridPosition.z);	
	}
	
	//---------------------------------------------------
	// UpdateItems()
	// This function updates the choose decoration menu
	// with the appropriate decorations for the incoming
	// node, decoNode.
	//---------------------------------------------------	
	public void UpdateItems( DecorationNode decoNode ) {
		// set our current deco node
		decoNodeCurrent = decoNode;
		
		// Destroy all child within grid
		// NOTE: cant enumerate (skipping), destroying backwards!
		int childs = goGrid.transform.childCount;
		for (int i = childs - 1; i > 0; i--){
			Destroy(goGrid.transform.GetChild(i).gameObject);
		}
		
		// create the decoration entries in the UI
		CreateEntries( goGrid );
		
		// show or hide the remove button as appropriate
		bool bShowRemove = decoNodeCurrent.HasRemoveOption();
		NGUITools.SetActive( goRemoveButton, bShowRemove );
		
		goGrid.GetComponent<UIGrid>().Reposition();
		Invoke("Reposition",0.00000001f);
		
		ResetUIPanelClipRange();
	}
	
	//---------------------------------------------------
	// CreateEntries()
	// Creates each individual UI entry for all decorations
	// that are in this UI.
	//---------------------------------------------------		
	private void CreateEntries( GameObject goGrid ) {
		// Destory all items in the list first (these may exist from the prefab)
		foreach(Transform child in goGrid.transform)
			Destroy(child.gameObject);
		
		// get the type of decorations to create the list for
		DecorationTypes eType = decoNodeCurrent.GetDecoType();
		
		// get the ordered list of decorations to be displayed
		List<InventoryItem> listDecos = GetDecorationList( eType );
		
		// also edit the size of the grid's box collider based on how many things are in this list
		/* // this seemed to fail, because the box collider was not being recentered or something, so it was still just too small...
		UIGrid scriptGrid = goGrid.GetComponent<UIGrid>();
		float fWidth = scriptGrid.cellWidth;
		Vector3 vSize = goGrid.GetComponent<BoxCollider>().size;
		vSize.x = ( listDecos.Count + 5 ) * fWidth;	// add +1 just for a little padding
		goGrid.GetComponent<BoxCollider>().size = vSize;
		*/
		
		// loop through the list and create an entry for each decoration
		for ( int i = 0; i < listDecos.Count; i++ ) {
			DecorationItem itemDeco = (DecorationItem) listDecos[i].ItemData;
			bool bDecoOK = itemDeco.DecorationType == eType;
			
			GameObject item = NGUITools.AddChild(goGrid, prefabChooseDecoEntry);
			ChooseDecorationUIEntry scriptEntry = item.GetComponent<ChooseDecorationUIEntry>();
			scriptEntry.SetDecoID( itemDeco.ID );
			
			// sorting in a grid is a pain in the ass...trying to use _ for decos that are OK and an X for those that are not to sort it
			string strPrefix = bDecoOK ? "_" : "X_";
			
			item.name = strPrefix + (listDecos.Count - i - 1) + "-" + itemDeco.ID;	// DO NOT CHANGE...this is what sorts it
			item.transform.FindChild("ItemDescription").GetComponent<UILabel>().text = itemDeco.Description;
			item.transform.FindChild("ItemName").GetComponent<UILabel>().text = itemDeco.Name;
			item.transform.FindChild("ItemTexture").GetComponent<UISprite>().spriteName = itemDeco.TextureName;
		
			// depending on if the deco can be placed or not, set certain attributes on the entry
			UIButtonMessage button = item.transform.FindChild("PlaceButton").GetComponent<UIButtonMessage>();
			if ( bDecoOK ) {
				// set up place button callbacks
				button.target = gameObject;
				button.functionName = "OnPlaceButton";
			}
			else {
				// destroy the place button
				Destroy(button.gameObject);
				
				// color the box bg appropriately
				item.transform.FindChild("ItemBackground").GetComponent<UISprite>().color = new Color32(201,201,201,255);
			}
			
			// save the tutorial entry (a bit hacky)
			if ( itemDeco.ID == "WallPoster1" )
				goTutorialEntry = item.transform.FindChild("PlaceButton").gameObject;
		}		
	}
	
	//---------------------------------------------------
	// GetDecorationList()
	// Returns the ordered decoration list to be displayed
	// for eType.  Note that the list is in reverse order;
	// the elements at the end will be displayed first.
	//---------------------------------------------------	
	private List<InventoryItem> GetDecorationList( DecorationTypes eType ) {
		// get the list of decorations the user owns
		List<InventoryItem> listDecos = (from keyValuePair in DataManager.Instance.GameData.Inventory.DecorationItems
											select keyValuePair.Value).ToList();
		
		// now order the list by the type of decoration we are looking for
		listDecos = listDecos.OrderBy(i => ((DecorationItem)i.ItemData).DecorationType == eType).ToList();	
		
		return listDecos;
	}
	
	//Delay calling reposition due to async problem Destroying/Repositioning
	//TODO Maybe change later when we have moreItems 
	private void Reposition(){
		goGrid.GetComponent<UIGrid>().Reposition();
	}
	
	//------------------------------------------
	// ResetUIPanelClipRange()
	// reset the clip range for the item area so that scrolling starts from the beginning
	//------------------------------------------
	private void ResetUIPanelClipRange(){
        Vector4 clipRange = gridPanel.GetComponent<UIPanel>().clipRange;
		
		// Stop the springing action when switching
		SpringPanel spring = gridPanel.GetComponent<SpringPanel>();
		if(spring != null){
			spring.enabled = false;	
		}
		
		// Reset the localposition and clipping position
        gridPanel.transform.localPosition = new Vector3(0f, gridPanel.transform.localPosition.y, gridPanel.transform.localPosition.z);
        gridPanel.gameObject.GetComponent<UIPanel>().clipRange = new Vector4(0f, clipRange.y, clipRange.z, clipRange.w);
	}
	
	
	//---------------------------------------------------
	// OnPlaceButton()
	// Callback function for when the user clicks the
	// place button for putting a decoration into the
	// scene.
	//---------------------------------------------------
	public void OnPlaceButton( GameObject button ) {	
		// get the ID from the UI entry that was clicked
		ChooseDecorationUIEntry scriptEntry = button.transform.parent.gameObject.GetComponent<ChooseDecorationUIEntry>();
		string strID = scriptEntry.GetDecoID();

		// set the deco on the node -- it does the instantiation of the 3d game object
		decoNodeCurrent.SetDecoration( strID );
		
		// play a sound
		AudioManager.Instance.PlayClip( strSoundPlace );
		
		//notify inventory logic that this item is being used
        InventoryLogic.Instance.UseItem(strID);		
		
		// close this menu
		EditDecosUIManager.Instance.CloseChooseMenu();
		
		// play an FX
		Vector3 vPosFX = decoNodeCurrent.gameObject.transform.position;
		string strFX = Constants.GetConstant<string>( "Deco_PlaceFX" );
		ParticleUtils.CreateParticle( strFX, vPosFX );		
		
		// send a callback
		if ( OnDecoPlaced != null )
			OnDecoPlaced( this, EventArgs.Empty );		
		
		// send out a task completion event (probably just going to be used for tutorial...)
		WellapadMissionController.Instance.TaskCompleted( "Decorate" );
	}
	
	//---------------------------------------------------
	// OnRemoveButton()
	// Callback function for when the user presses the
	// remove button to remove a decoration from the
	// scene.
	//---------------------------------------------------
	public void OnRemoveButton( GameObject button ) {
		// remove the deco from the node
		decoNodeCurrent.RemoveDecoration();
		
		// give the user back their item
		// NOTE: This is now done in DecorationNode.RemoveDecoration()
		
		// close this menu
		EditDecosUIManager.Instance.CloseChooseMenu();		
	}
}
