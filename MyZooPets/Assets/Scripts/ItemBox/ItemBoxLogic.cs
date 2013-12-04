using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// ItemBoxLogic
// The logic script that is on an item box.
//---------------------------------------------------	

public class ItemBoxLogic : MonoBehaviour {
	// the item box ID of this box
	private string strItemBoxID;
	public void SetItemBoxID( string strID ) {
		if ( string.IsNullOrEmpty( strItemBoxID ) )
			strItemBoxID = strID;
		else
			Debug.Log("Something trying to double set item box id", this);
	}
	
	// is this item box available to be opened?
	private bool bAvailable = false;
	
	//---------------------------------------------------
	// NowAvailable()
	// When an item box is made available to open, it 
	// needs to let the save game manager know so that
	// if the game ends the unopened box will be loaded
	// next time the game starts.
	//---------------------------------------------------		
	public void NowAvailable() {
		if ( bAvailable ) {
			Debug.Log("Item box being made available twice...", this);
			return;
		}
		
		// mark it so we don't do this again
		bAvailable = true;
		
		// let the save game data know
		DataManager.Instance.GameData.Inventory.UnopenedItemBoxes.Add( strItemBoxID );
	}
	
	//---------------------------------------------------
	// IsValid()
	// Is this item box valid to open?
	//---------------------------------------------------		
	public bool IsValid() {
		bool bValid =  !string.IsNullOrEmpty( strItemBoxID );	
		return bValid;
	}
	
	//---------------------------------------------------
	// OpenBox()
	// Returns the items to be granted by this item box.
	// This is a "chain reaction" function because calling
	// it will also award the items.  Only call this function
	// when the box has been opened.
	//---------------------------------------------------	
	public List<KeyValuePair<Item, int>> OpenBox() {
		List<KeyValuePair<Item, int>> items = new List<KeyValuePair<Item, int>>();
			
		// get data for this box
		Data_ItemBox dataBox = DataLoader_ItemBoxes.GetItemBox( strItemBoxID );
		
		if ( dataBox != null )
			items = dataBox.GetItems();
		else
			Debug.Log("No valid item box data for item box", this);
		
		// award the items
		AwardItems( items );
		
		return items;
	}
	
	//---------------------------------------------------
	// AwardItems()
	// Puts the incoming list of items into the player's
	// inventory.
	//---------------------------------------------------		
	private void AwardItems( List<KeyValuePair<Item, int>> items ) {
		// loop through and add the items to the user's inventory
		foreach ( KeyValuePair<Item, int> item in items )
			InventoryLogic.Instance.AddItem( item.Key.ID, item.Value);
		
		// since items are being awarded, remove this entry from the save data
		DataManager.Instance.GameData.Inventory.UnopenedItemBoxes.Remove( strItemBoxID );
	}
		
}
