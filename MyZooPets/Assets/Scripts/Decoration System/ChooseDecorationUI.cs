﻿using UnityEngine;
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
	
	// prefab of the area that items are populated to -- this exists because if we don't instantiate it, the list "remembers" where it was last scrolled to
	public GameObject prefabChooseArea;
	private GameObject goChooseArea;
	
	// the grid this UI places its items in
	private GameObject goGrid;	
	
	// the decoration node that this UI is currently representing
	private DecorationNode decoNodeCurrent;
	
	//---------------------------------------------------
	// UpdateItems()
	// This function updates the choose decoration menu
	// with the appropriate decorations for the incoming
	// node, decoNode.
	//---------------------------------------------------	
	public void UpdateItems( DecorationNode decoNode ) {
		// set our current deco node
		decoNodeCurrent = decoNode;
		
		// instantiate the item area
		if ( goChooseArea )
			Destroy( goChooseArea );	// destroy the section of the UI with all the entries if it existed already
		goChooseArea = LgNGUITools.AddChildWithPosition( gameObject, prefabChooseArea );
		goGrid = goChooseArea.transform.Find("Grid").gameObject;
		
		// create the decoration entries in the UI
		CreateEntries( goGrid );	
		
		// the last item in the list (so it shows up first) is the removal option (if there is a decoration at this node)
		if ( decoNodeCurrent.HasRemoveOption() )
			AddRemoveEntry( goGrid );
		
		goGrid.GetComponent<UIGrid>().Reposition();
		Invoke("Reposition",0.00000001f);		
	}
	
	//---------------------------------------------------
	// AddRemoveEntry()
	// Adds an entry for a removal option to goGrid.
	//---------------------------------------------------		
	private void AddRemoveEntry( GameObject goGrid ) {
		GameObject itemRemove = NGUITools.AddChild(goGrid, prefabChooseDecoEntry);
		itemRemove.name = "_item_remove";	// DO NOT CHANGE its used for sorting
		itemRemove.transform.FindChild("ItemDescription").GetComponent<UILabel>().text = Localization.Localize( "DECO_REMOVE_DESC" );
		itemRemove.transform.FindChild("ItemCost").GetComponent<UILabel>().text = "100";
		itemRemove.transform.FindChild("ItemName").GetComponent<UILabel>().text = Localization.Localize( "DECO_REMOVE" );
		itemRemove.transform.FindChild("ItemTexture").GetComponent<UISprite>().spriteName = "apple";
		itemRemove.transform.FindChild("PlaceButton").transform.FindChild("Label").GetComponent<UILabel>().text = Localization.Localize("DECO_REMOVE");
		itemRemove.transform.FindChild("PlaceButton").GetComponent<UIButtonMessage>().target = gameObject;
		itemRemove.transform.FindChild("PlaceButton").GetComponent<UIButtonMessage>().functionName = "OnRemoveButton";			
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
		
		// loop through the list and create an entry for each decoration
		for ( int i = 0; i < listDecos.Count; i++ ) {
			DecorationItem itemDeco = (DecorationItem) listDecos[i].ItemData;
			bool bDecoOK = itemDeco.DecorationType == eType;
			
			GameObject item = NGUITools.AddChild(goGrid, prefabChooseDecoEntry);
			ChooseDecorationUIEntry scriptEntry = item.GetComponent<ChooseDecorationUIEntry>();
			scriptEntry.SetDecoID( itemDeco.ID );
			item.name = (listDecos.Count - i - 1) + "-" + itemDeco.ID;	// DO NOT CHANGE...this is what sorts it
			item.transform.FindChild("ItemDescription").GetComponent<UILabel>().text = itemDeco.Description;
			item.transform.FindChild("ItemCost").GetComponent<UILabel>().text = itemDeco.Cost.ToString();
			item.transform.FindChild("ItemName").GetComponent<UILabel>().text = itemDeco.Name;
			item.transform.FindChild("ItemTexture").GetComponent<UISprite>().spriteName = itemDeco.TextureName;
			item.transform.FindChild("PlaceButton").transform.FindChild("Label").GetComponent<UILabel>().text = Localization.Localize("DECO_PLACE");
			
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
		List<InventoryItem> listDecos = (from keyValuePair in DataManager.Instance.Inventory.DecorationItems
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
		
		//notify inventory logic that this item is being used
        InventoryLogic.Instance.UseItem(strID);		
		
		// close this menu
		EditDecosUIManager.Instance.CloseChooseMenu();
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
