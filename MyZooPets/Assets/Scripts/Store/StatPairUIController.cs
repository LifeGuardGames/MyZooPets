using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Stat pair user interface controller.
/// </summary>
public class StatPairUIController : MonoBehaviour{
	// the sprite and label for this stat pair
	public Image spriteIcon;
	public Text labelAmount;

	public void Init(StatType statType, int amount){
		// set the icon of the stat (a little hacky unless we need to expand further)
		if(statType == StatType.Hunger) {
			spriteIcon.sprite = SpriteCacheManager.GetSprite("iconHungerBlank");
		}
		else if(statType == StatType.Health) {
			spriteIcon.sprite = SpriteCacheManager.GetSprite("iconHealthBlank");
		}
		else if(statType == StatType.Fire) {
			spriteIcon.sprite = SpriteCacheManager.GetSprite("iconFire");
		}
		else { }
		
		// set the label to whatever the amount is, with a + or -, and then add the amount for the text
		string strModifier = amount > 0 ? "+" : "";
		labelAmount.text = strModifier + amount;
	}
}
