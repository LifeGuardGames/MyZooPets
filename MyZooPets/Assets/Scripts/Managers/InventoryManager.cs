using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Contains all items the pet owns, not to be confused with ItemManager
/// This class handles the different types of inventory ItemTypes - Consumables(Food + Usables), Decorations, and Accessories
/// </summary>
public class InventoryManager : Singleton<InventoryManager> {
	public List<InventoryItem> AllConsumableInventoryItems {
		get {
			List<InventoryItem> consumables = (from keyValuePair in DataManager.Instance.GameData.Inventory.InventoryItems
											   select keyValuePair.Value).ToList();
			return consumables;
		}
	}

	public List<InventoryItem> AllDecoInventoryItems {
		get {
			List<InventoryItem> decorations = (from keyValuePair in DataManager.Instance.GameData.Inventory.DecorationItems
											   select keyValuePair.Value).ToList();
			return decorations;
		}
	}

	//	public List<InventoryItem> GetDecorationInventoryItemsOrderyByType(DecorationTypes type){
	//		List<InventoryItem> decorations = AllDecoInventoryItems;
	//		decorations = decorations.OrderBy(i => ((DecorationItem)i.ItemData).DecorationType == type).ToList();
	//		return decorations;
	//	}

	public bool IsWallpaperBought(string itemID) {
		List<string> oneTimePurchasedInv = DataManager.Instance.GameData.Inventory.OneTimePurchasedItems;
		return oneTimePurchasedInv.Contains(itemID);
	}

	public bool IsAccessoryBought(string itemID) {
		List<string> oneTimePurchasedInv = DataManager.Instance.GameData.Inventory.OneTimePurchasedItems;
		return oneTimePurchasedInv.Contains(itemID);
	}

	/// <summary>
	/// Gets the inv item. Return null if inventory item has been removed
	/// </summary>
	public InventoryItem GetItemInInventory(string itemID) {
		Dictionary<string, InventoryItem> invItems = DataManager.Instance.GameData.Inventory.InventoryItems;
		if(invItems.ContainsKey(itemID)) {
			return invItems[itemID];
		}
		else {
			return null;
		}
	}

	/// <summary>
	/// Gets the deco inv item. Return null if deco inventory item has been removed
	/// </summary>
	public InventoryItem GetDecoInInventory(string itemID) {
		Dictionary<string, InventoryItem> decoInvItems = DataManager.Instance.GameData.Inventory.DecorationItems;
		if(decoInvItems.ContainsKey(itemID)) {
			return decoInvItems[itemID];
		}
		else {
			return null;
		}
	}

	//void OnGUI() {
	//	if(GUI.Button(new Rect(100, 100, 100, 100), "FIRE")){
	//		Debug.Log("Adding");
	//		AddItemToInventory("Usable1", 1);
	//		InventoryUIManager.Instance.RefreshPage();
	//	}
	//}
	
	public void AddItemToInventory(string itemID, int count = 1) {
		Dictionary<string, InventoryItem> specificTypeInventory = GetInventoryTypeForItem(itemID);
		Item itemData = DataLoaderItems.GetItem(itemID);

		InventoryItem invItem = null;
		
		// If item already in dictionary increment amount
		if(specificTypeInventory.ContainsKey(itemID)) {
			// Only one allowed for wallpaper/accessory
			if(IsWallpaperBought(itemID) || IsAccessoryBought(itemID)) {
				return;
			}

			specificTypeInventory[itemID].Amount += count;
		}
		else { //Add InventoryItem into dict if key doesn't exist
			invItem = new InventoryItem(itemID, itemData.Type, itemData.TextureName);
			specificTypeInventory[itemID] = invItem;

			//special case: keep track of bought wallpaper in another list.
			if(itemData.Type == ItemType.Decorations) {
				DecorationItem decoItem = (DecorationItem)itemData;
				if(decoItem.DecorationType == DecorationTypes.Wallpaper) {
					List<string> oneTimePurchasedInv = DataManager.Instance.GameData.Inventory.OneTimePurchasedItems;
					oneTimePurchasedInv.Add(itemData.ID);
				}
			}
			//special case: keep track of all bought accessories in another list.
			else if(itemData.Type == ItemType.Accessories) {
				List<string> oneTimePurchasedInv = DataManager.Instance.GameData.Inventory.OneTimePurchasedItems;
				oneTimePurchasedInv.Add(itemData.ID);
			}
			// Account for item amount
			else {
				if(count > 1) {
					specificTypeInventory[itemID].Amount += (count - 1);	// We already have one, so minus one
				}
			}
		}

		// NOTE: No need to add items to UI here, the complete buy tween will refresh by itself
	}

	// Item used on pet
	public void UsePetItem(string itemID) {
		Dictionary<string, InventoryItem> invItems = GetInventoryTypeForItem(itemID);
		InventoryItem invItem = null;

		if(invItems.ContainsKey(itemID)) {
			invItem = invItems[itemID];
			invItem.Amount--;

			//need to use the stats effect from item
			ItemManager.Instance.StatsEffect(itemID);

			//analytics
			//Analytics.Instance.ItemEvent(Analytics.ITEM_STATUS_USED, invItem.ItemType, invItem.ItemID);
			if(invItem.ItemType == ItemType.Foods) {
				Analytics.Instance.ConsumableEventWithPetStats(invItem.ItemID,
					Analytics.ITEM_STATS_HEALTH, DataManager.Instance.GameData.Stats.Health);
				Analytics.Instance.ConsumableEventWithPetStats(invItem.ItemID,
					Analytics.ITEM_STATS_MOOD, DataManager.Instance.GameData.Stats.Mood);
			}

			//remove inv item if there is none left
			if(invItem.Amount == 0) {
				invItems.Remove(itemID);
			}

			// Update UI to reflect changes based on their type
			if(invItem.ItemType == ItemType.Foods || invItem.ItemType == ItemType.Usables) {
				InventoryUIManager.Instance.RefreshPage();
			}
			if(invItem.ItemType == ItemType.Decorations && DecoInventoryUIManager.Instance != null) {
				DecoInventoryUIManager.Instance.RefreshPage();
			}
		}
	}

	public void UseDecoItem(string itemID) {
		Dictionary<string, InventoryItem> invItems = GetInventoryTypeForItem(itemID);
		InventoryItem invItem = null;

		if(invItems.ContainsKey(itemID)) {
			invItem = invItems[itemID];
			invItem.Amount--;
			//remove inv item if there is none left
			if(invItem.Amount == 0) {
				invItems.Remove(itemID);
			}

			if(invItem.ItemType == ItemType.Decorations && DecoInventoryUIManager.Instance != null) {
				DecoInventoryUIManager.Instance.RefreshPage();
			}
		}
	}

	// Item used on minipet
	public void UseMiniPetItem(string itemID) {
		Dictionary<string, InventoryItem> invItems = GetInventoryTypeForItem(itemID);
		InventoryItem invItem = null;

		if(invItems.ContainsKey(itemID)) {
			invItem = invItems[itemID];
			invItem.Amount--;

			// remove inv item if there is none left
			if(invItem.Amount == 0) {
				invItems.Remove(itemID);
			}

			// Update UI to reflect changes based on their type
			if(invItem.ItemType == ItemType.Foods || invItem.ItemType == ItemType.Usables) {
				InventoryUIManager.Instance.RefreshPage();
			}
		}
	}

	/// <summary>
	/// Gets the inventory for item. Based on the item type of itemID, this function
	/// will return the proper inventory for it
	/// </summary>
	private Dictionary<string, InventoryItem> GetInventoryTypeForItem(string itemID) {
		switch(DataLoaderItems.GetItemType(itemID)) {
			case ItemType.Decorations:
				return DataManager.Instance.GameData.Inventory.DecorationItems;
			case ItemType.Accessories:
				return DataManager.Instance.GameData.Inventory.AccessoryItems;
			default:    // Foods and Usables
				return DataManager.Instance.GameData.Inventory.InventoryItems;
		}
	}
}
