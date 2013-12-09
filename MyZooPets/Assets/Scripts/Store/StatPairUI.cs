using UnityEngine;
using System.Collections;

//---------------------------------------------------
// StatPairUI
// This is a single stat pair (icon + text) that
// appears on items for the item's description.
//---------------------------------------------------

public class StatPairUI : MonoBehaviour {
	// the sprite and label for this stat pair
	public UISprite spriteIcon;
	public UILabel labelAmount;
	
	//---------------------------------------------------
	// Init()
	// Set the description for this item.
	//---------------------------------------------------		
	public void Init( StatType eStat, int nAmount ) {
		// set the icon of the stat (a little hacky unless we need to expand further)
		if ( eStat == StatType.Mood )
			spriteIcon.spriteName = "iconHunger";
		else
			spriteIcon.spriteName = "iconHeart";
		
		// set the label to whatever the amount is, with a + or -, and then add the amount for the text
		string strModifier = nAmount > 0 ? "+" : "";
		labelAmount.text = strModifier + nAmount;
	}
}
