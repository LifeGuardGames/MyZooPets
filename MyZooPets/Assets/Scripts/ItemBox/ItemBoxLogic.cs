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
	// OpenBox()
	// Opens this item box and sends all the items inside
	// it flying out.
	//---------------------------------------------------	
	public void OpenBox() {
		List<KeyValuePair<Item, int>> items = new List<KeyValuePair<Item, int>>();
		
		// do a null check
		if ( string.IsNullOrEmpty( strItemBoxID ) ) {
			Debug.Log("Attempting to open an item box with an unset id...defaulting to Box_0", gameObject);
			strItemBoxID = "Box_0";
		}
			
		// get data for this box
		Data_ItemBox dataBox = DataLoader_ItemBoxes.GetItemBox( strItemBoxID );
		
		if ( dataBox != null )
			items = dataBox.GetItems();
		else
			Debug.Log("No valid item box data for item box", this);
		
		// create all the items to be obtained from this box
		BurstItems( items );
	}
	
	//---------------------------------------------------
	// BurstItems()
	// Instantiates all the items to be gained from this
	// box and then "bursts" them out onto the bedroom
	// floor.
	//---------------------------------------------------		
	private void BurstItems( List<KeyValuePair<Item, int>> items ) {
		foreach ( KeyValuePair<Item, int> item in items ) {
			int nValue = item.Value;
			for ( int i = 0; i < nValue; ++i ) {
				// spawn the item to be coming out of this box
				GameObject goPrefab = Resources.Load( "DroppedItem" ) as GameObject;
				GameObject goDroppedItem = Instantiate( goPrefab, new Vector3(0, 0, 0), Quaternion.identity ) as GameObject;
				goDroppedItem.GetComponent<DroppedObject_Item>().Init( item.Key );
				
				// set the position of the newly spawned item to be wherever this item box is
				Vector3 vPosition = gameObject.transform.position;
				goDroppedItem.transform.position = vPosition;
				
				// make the item "burst" out
				goDroppedItem.GetComponent<DroppedObject>().Burst();
			}
		}
	}	
}
