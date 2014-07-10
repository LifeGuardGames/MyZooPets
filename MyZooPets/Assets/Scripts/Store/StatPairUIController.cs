using UnityEngine;
using System.Collections;

/// <summary>
/// Stat pair user interface controller.
/// </summary>
public class StatPairUIController : MonoBehaviour{
	// the sprite and label for this stat pair
	public UISprite spriteIcon;
	public UILabel labelAmount;

	/// <summary>
	/// Init the specified eStat and nAmount.
	/// </summary>
	/// <param name="eStat">E stat.</param>
	/// <param name="nAmount">N amount.</param>
	public void Init(StatType eStat, int nAmount){
		// set the icon of the stat (a little hacky unless we need to expand further)
		if(eStat == StatType.Mood)
			spriteIcon.spriteName = "iconHunger";
		else if(eStat == StatType.Health)
			spriteIcon.spriteName = "iconHeart";
		else if(eStat == StatType.Fire)
			spriteIcon.spriteName = "iconFire";
		else{}
		
		// set the label to whatever the amount is, with a + or -, and then add the amount for the text
		string strModifier = nAmount > 0 ? "+" : "";
		labelAmount.text = strModifier + nAmount;
	}
}
