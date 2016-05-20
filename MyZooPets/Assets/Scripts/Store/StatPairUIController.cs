using UnityEngine;

/// <summary>
/// Stat pair user interface controller.
/// </summary>
public class StatPairUIController : MonoBehaviour{
	// the sprite and label for this stat pair
	public UISprite spriteIcon;
	public UILabel labelAmount;

	public void Init(StatType statType, int amount){
		// set the icon of the stat (a little hacky unless we need to expand further)
		if(statType == StatType.Hunger)
			spriteIcon.spriteName = "iconHunger";
		else if(statType == StatType.Health)
			spriteIcon.spriteName = "iconHeart";
		else if(statType == StatType.Fire)
			spriteIcon.spriteName = "iconFire";
		else{}
		
		// set the label to whatever the amount is, with a + or -, and then add the amount for the text
		string strModifier = amount > 0 ? "+" : "";
		labelAmount.text = strModifier + amount;
	}
}
