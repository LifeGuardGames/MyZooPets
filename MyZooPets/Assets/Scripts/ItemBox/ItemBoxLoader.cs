using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// ItemBoxLoader
// This is a special monobehaviour that should only
// exist in the bedroom.  It will load any saved and
// unopened item boxes the player has and place them
// in the first room for them to open.
//---------------------------------------------------

public class ItemBoxLoader : MonoBehaviour {

	//---------------------------------------------------
	// Start()
	//---------------------------------------------------	
	void Start () {
		// instantiate all the saved boxes at our location
		List<ItemBoxLogic> listBoxes = new List<ItemBoxLogic>();
		GameObject goResource = Resources.Load( "ItemBox" ) as GameObject;
		
		foreach ( string strSavedBoxID in DataManager.Instance.GameData.Inventory.UnopenedItemBoxes ) {
			Data_ItemBox dataBox = DataLoaderItemBoxes.GetItemBox( strSavedBoxID );
			if ( dataBox != null ) {
				GameObject goBox = Instantiate( goResource, transform.position, Quaternion.identity ) as GameObject;
				ItemBoxLogic scriptBox = goBox.GetComponent<ItemBoxLogic>();
				
				if ( scriptBox ) {
					// set the box's id
					scriptBox.SetItemBoxID( strSavedBoxID );
					
					// add the box to the list of created boxes
					listBoxes.Add( scriptBox );
				}
				else
					Debug.LogError("No box script!", this);
			}
		}
		
		// we now need to go through and make all the boxes available...
		
		// first, wipe our data because when the boxes are made available, they will repopulate the save data
		DataManager.Instance.GameData.Inventory.UnopenedItemBoxes = new List<string>();
		
		// make each box available
		foreach ( ItemBoxLogic box in listBoxes ) 
			box.NowAvailable();
	}
	
}
