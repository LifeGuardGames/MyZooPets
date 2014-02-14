using UnityEngine;
using System;
using System.Collections;

//---------------------------------------------------
// ItemBoxEntry
// The UI of an individual item inside the item box UI.
//---------------------------------------------------

public class ItemBoxEntry : MonoBehaviour {
	// elements to be set for the item
	public UISprite spriteIcon;
	public UILabel labelName;
	public UILabel labelQuantity;
	
	//---------------------------------------------------
	// Init()
	// Inits the item UI entry based on the incoming
	// item and quantity.
	//---------------------------------------------------	
	public void Init( Item item, int nQuantity ) {
		// get the values of things to be set
		string strIcon = item.TextureName;
		string strName = item.Name;
		string strQuantity = Localization.Localize( "ITEMBOX_QUANTITY" );
		strQuantity = String.Format(strQuantity, nQuantity);
		
		// set the icon sprite, name, and quantity
		spriteIcon.spriteName = strIcon;
		labelName.text = strName;
		labelQuantity.text = strQuantity;
	}
}
