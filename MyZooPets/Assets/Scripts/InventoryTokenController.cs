using UnityEngine;
using UnityEngine.UI;

// Actual class that handles the UI for the inventory slot items
// For use for both Inventory and DecoInventory items
public class InventoryTokenController : MonoBehaviour {

	public Image itemSprite;
	public Text amountLabel;
	private string itemID;

	public void Init(InventoryItem invItem){
		itemID = invItem.ItemID;
		gameObject.name = invItem.ItemID;
		itemSprite.sprite = SpriteCacheManager.GetItemSprite(itemID);
		SetAmount(invItem.Amount);

	}

	public void SetAmount(int amount){
		amountLabel.text = amount.ToString();
	}

	//TODO Handle StatHintController
}
