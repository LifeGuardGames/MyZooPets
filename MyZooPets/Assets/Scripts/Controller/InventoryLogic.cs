using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

/*
	Inventory class for Pet
	Contains all items the pet owns.
*/
public class InventoryLogic : Singleton<InventoryLogic> {
	public static event EventHandler<InventoryEventArgs> OnItemAddedToInventory; //Call when an item is added
	
	public class InventoryEventArgs : EventArgs{
		private bool isItemNew = false;
		private InventoryItem invItem = null;

		public bool IsItemNew{
			get{return isItemNew;}
		}
		public InventoryItem InvItem{
			get{return invItem;}
		}

		public InventoryEventArgs(bool isItemNew, InventoryItem invItem){
			this.isItemNew = isItemNew;
			this.invItem = invItem;
		}
	}

	private bool listNeedsUpdate = true;
	private List<InventoryItem> inventoryItemList;

	//====================API===========================
	public List<InventoryItem> AllInventoryItems{ //TO DO: cache data
		get{
			if(inventoryItemList == null || listNeedsUpdate){
				inventoryItemList = (from keyValuePair in DataManager.Instance.GameData.Inventory.InventoryItems
									select keyValuePair.Value).ToList();
				listNeedsUpdate = false;
			}
			return inventoryItemList;
		}
	}	

	//---------------------------------------------------
	// CheckForWallpaper()
	// Check if wallpaper is already bought 
	//---------------------------------------------------
	public bool CheckForWallpaper(string itemID){
		bool isWallpaperBought = false;
		List<string> oneTimePurchasedInv = DataManager.Instance.GameData.Inventory.OneTimePurchasedItems;
		isWallpaperBought = oneTimePurchasedInv.Contains(itemID);

		return isWallpaperBought;
	}

	//Return InventoryItem with itemID
	//Return null if inventory item has been removed
	public InventoryItem GetInvItem(string itemID){
		Dictionary<string, InventoryItem> invItems = DataManager.Instance.GameData.Inventory.InventoryItems;
		InventoryItem invItem = null;

		if(invItems.ContainsKey(itemID))
			invItem = invItems[itemID];

		return invItem;
	}

	//Add items to inventory
	public void AddItem(string itemID, int count){
		Dictionary<string, InventoryItem> invItems = GetInventoryForItem(itemID);
		
		InventoryItem invItem = null;
		bool itemNew = false;
		listNeedsUpdate = true;

		if(invItems.ContainsKey(itemID)){ //If item already in dict. increment amount

			//if in the inventory already check if it's wallpaper
			//if it's wallpaper don't increment count just return
			if(CheckForWallpaper(itemID)) return;

			invItem = invItems[itemID];
			invItem.Amount += count; 
			invItems[itemID] = invItem;
		}else{ //Add InventoryItem into dict if key doesn't exist
			itemNew = true;
			Item itemData = DataLoaderItems.GetItem(itemID);

			invItem = new InventoryItem(itemID, itemData.Type, itemData.TextureName);
			invItems[itemID] = invItem;

			//special case: keep track of bought wallpaper in another list.
			if(itemData.Type == ItemType.Decorations){
				DecorationItem decoItem = (DecorationItem) itemData;

				if(decoItem.DecorationType == DecorationTypes.Wallpaper){
					List<string> oneTimePurchasedInv = DataManager.Instance.GameData.Inventory.OneTimePurchasedItems;
					oneTimePurchasedInv.Add(itemData.ID);
				}
			}
		}

		InventoryEventArgs args = new InventoryEventArgs(itemNew, invItem);
		if(OnItemAddedToInventory != null) OnItemAddedToInventory(this, args);
	}
	
	//---------------------------------------------------
	// GetInventoryForItem()
	// Based on the item type of strItemID, this function
	// will return the proper inventory for it.
	//---------------------------------------------------	
	private Dictionary<string, InventoryItem> GetInventoryForItem( string strItemID ) {
		// what list the item is placed in depends on what kind of item it is
		ItemType eType = DataLoaderItems.GetItemType( strItemID );
		Dictionary<string, InventoryItem> inventory = new Dictionary<string, InventoryItem>();
		
		switch ( eType ) {
			case ItemType.Decorations:
				inventory = DataManager.Instance.GameData.Inventory.DecorationItems;
				break;
			default:
				inventory = DataManager.Instance.GameData.Inventory.InventoryItems;
				break;
		}		
		
		return inventory;
	}
	
	//Use item from inventory
	public void UseItem(string itemID){
		Dictionary<string, InventoryItem> invItems = GetInventoryForItem( itemID );
		InventoryItem invItem = null;
		listNeedsUpdate = true;

		if(invItems.ContainsKey(itemID)){
			invItem = invItems[itemID];
			invItem.Amount--;

			//need to use the stats effect from item
			ItemLogic.Instance.StatsEffect(itemID);
		
			//analytics
			Analytics.Instance.ItemEvent(Analytics.ITEM_STATUS_USED, invItem.ItemType, invItem.ItemID);
			Analytics.Instance.ItemEventWithPetStats(invItem.ItemID, 
				Analytics.ITEM_STATS_HEALTH, DataManager.Instance.GameData.Stats.Health);
			Analytics.Instance.ItemEventWithPetStats(invItem.ItemID, 
				Analytics.ITEM_STATS_MOOD, DataManager.Instance.GameData.Stats.Mood);

			// play the item's sound, if it has one
			string strSound = invItem.ItemData.SoundUsed;
			if ( !string.IsNullOrEmpty(strSound) )
				AudioManager.Instance.PlayClip( strSound );
		}

		if(invItem.Amount == 0)
			invItems.Remove(itemID);
	}
	//=================================================
}
