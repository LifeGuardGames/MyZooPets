using UnityEngine;
using UnityEngine.UI;

// Actual class that handles the UI for the inventory slot items
// For use for both Inventory and DecoInventory items
// Controller manages and resets the InventoryTokenDragElement as required
public class InventoryTokenController : MonoBehaviour{
	public Text amountLabel;
	public InventoryTokenDragElement dragElement;		// Child gameobject for actual dragging

	private string itemID;
	private bool pointerDownAux = false;

	public void Init(InventoryItem invItem) {
		itemID = invItem.ItemID;
		gameObject.name = invItem.ItemID;
		SetAmount(invItem.Amount);

		// Initialize the child dragging element
		dragElement.Init(itemID, transform);
	}

	public void SetAmount(int amount) {
		amountLabel.text = amount.ToString();
		// TODO reset the drag element here, if it doesnt get deleted
	}
}
