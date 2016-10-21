using UnityEngine;
using UnityEngine.UI;

// Actual class that handles the UI for the inventory slot items
// For use for both Inventory and DecoInventory items
// Controller manages and resets the InventoryTokenDragElement as required
public class InventoryTokenController : MonoBehaviour{
	public Text amountLabel;
	public InventoryTokenDragElement dragElement;		// Child gameobject for actual dragging

	private InventoryItem itemData;

	public void Init(InventoryItem invItem) {
		itemData = invItem;
		gameObject.name = invItem.ItemID;
		SetAmount(invItem.Amount);

		// Initialize the child dragging element
		dragElement.Init(itemData, transform);
	}

	public void SetAmount(int amount) {
		amountLabel.text = amount.ToString();
		// TODO reset the drag element here, if it doesnt get deleted
	}
}
