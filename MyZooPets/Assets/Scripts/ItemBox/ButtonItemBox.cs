using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// ButtonItemBox
// Opens the item box UI.
//---------------------------------------------------

public class ButtonItemBox : LgButton {
	
	//---------------------------------------------------
	// ProcessClick()
	//---------------------------------------------------		
	protected override void ProcessClick() {
		if ( ItemBoxUI.Instance == null ) {
			// if the box UI doesn't exist, we are creating it
			
			// instantiate the actual item box UI
			GameObject resourceBox = Resources.Load( "ItemBoxUI" ) as GameObject;
			GameObject goBox = LgNGUITools.AddChildWithPosition( GameObject.Find("Anchor-Center"), resourceBox );
			ItemBoxUI scriptBox = goBox.GetComponent<ItemBoxUI>();
			
			StartCoroutine( scriptBox.InitBox( "Box_0" ) );
			
			// listen for when the box closes
			ItemBoxUI.Instance.OnManagerOpen += OnBoxOpen;
			
			DataLoader_LootTables.SetupData();
		}
		else {
			// otherwise, we are closing the box
			ItemBoxUI.Instance.CloseUI();
		}
	}
	
	//---------------------------------------------------
	// OnBoxOpen()
	// Callback for when the box UI is closed or opened.
	//---------------------------------------------------		
    private void OnBoxOpen(object sender, UIManagerEventArgs e){
		// if the box UI is closing, destroy the box itself (this object)
       //if(e.Opening == false)
       //    Destroy( gameObject );
    }	
}
