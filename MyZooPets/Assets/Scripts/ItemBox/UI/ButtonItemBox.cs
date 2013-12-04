using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// ButtonItemBox
// Opens the item box UI.
//---------------------------------------------------

public class ButtonItemBox : LgButton {	
	// logic script for this item box
	public ItemBoxLogic scriptLogic;
	
	//---------------------------------------------------
	// ProcessClick()
	//---------------------------------------------------		
	protected override void ProcessClick() {
		if ( ItemBoxUI.Instance == null ) {
			// if the box UI doesn't exist, we are creating it
			
			// check to make sure we have a valid box
			if ( !scriptLogic.IsValid() ) {
				Debug.Log("Trying to open an invalid box", this);
				return;
			}
		
			// get items to be granted from this box
			List<KeyValuePair<Item, int>> items = scriptLogic.OpenBox();	

			// create the UI to show the player what they got
			CreateItemBoxUI( items );
			
			// listen for when the box closes
			ItemBoxUI.Instance.OnManagerOpen += OnBoxOpen;
		}
		else {
			// otherwise, we are closing the box
			ItemBoxUI.Instance.CloseUI();
		}
	}

	//---------------------------------------------------
	// CreateItemBoxUI()
	// Sets up and creates the UI for displaying what is
	// inside this item box.
	//---------------------------------------------------		
	private void CreateItemBoxUI( List<KeyValuePair<Item, int>> items ) {
		// instantiate the actual item box UI
		GameObject resourceBox = Resources.Load( "ItemBoxUI" ) as GameObject;
		GameObject goBox = LgNGUITools.AddChildWithPosition( GameObject.Find("Anchor-Center"), resourceBox );
		ItemBoxUI scriptBox = goBox.GetComponent<ItemBoxUI>();		
		
		StartCoroutine( scriptBox.InitBox( items ) );		
	}
	
	//---------------------------------------------------
	// OnBoxOpen()
	// Callback for when the box UI is closed or opened.
	//---------------------------------------------------		
    private void OnBoxOpen(object sender, UIManagerEventArgs e){
		// if the box UI is closing, destroy the box itself (this object)
     	if(e.Opening == false)
        	Destroy( gameObject );
    }	
}
