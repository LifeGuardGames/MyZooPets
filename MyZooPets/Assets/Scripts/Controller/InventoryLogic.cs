using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

/*
	Inventory class for Pet
	Contains all items the pet owns.
*/
public class InventoryLogic : Singleton<InventoryLogic>{
	public static event EventHandler<InventoryEventArgs> OnItemAddedToInventory; //Call when an item is added
	public static event EventHandler<InventoryEventArgs> OnItemUsed; //Call when an item has been used
	
	public class InventoryEventArgs : EventArgs{
		public bool IsItemNew{ get; set; }

		public InventoryItem InvItem{ get; set; }
	}

	private bool listNeedsUpdate = true;
	private List<InventoryItem> inventoryItemList; //list of all consumable items


	/// <summary>
	/// Gets all inventory items.
	/// </summary>
	/// <value>All inventory items.</value>
	public List<InventoryItem> AllInventoryItems{ 
		get{
			if(inventoryItemList == null || listNeedsUpdate){
				inventoryItemList = (from keyValuePair in DataManager.Instance.GameData.Inventory.InventoryItems
									select keyValuePair.Value).ToList();
				listNeedsUpdate = false;
			}
			return inventoryItemList;
		}
	}

	/// <summary>
	/// Gets all decoration inventory items.
	/// </summary>
	/// <value>All decoration inventory items.</value>
	public List<InventoryItem> GetDecorationInventoryItemsOrderyByType(DecorationTypes type){
		// get the list of decorations the user owns
		List<InventoryItem> decorations = (from keyValuePair in DataManager.Instance.GameData.Inventory.DecorationItems
		                                 select keyValuePair.Value).ToList();
		
		// now order the list by the type of decoration we are looking for
		decorations = decorations.OrderBy(i => ((DecorationItem)i.ItemData).DecorationType == type).ToList();	

		return decorations;
	}
	
	/// <summary>
	/// Checks if wallpaper is already bought
	/// </summary>
	/// <returns><c>true</c>, if for wallpaper was checked, <c>false</c> otherwise.</returns>
	/// <param name="itemID">Item I.</param>
	public bool CheckForWallpaper(string itemID){
		bool isWallpaperBought = false;
		List<string> oneTimePurchasedInv = DataManager.Instance.GameData.Inventory.OneTimePurchasedItems;
		isWallpaperBought = oneTimePurchasedInv.Contains(itemID);

		return isWallpaperBought;
	}
	
	/// <summary>
	/// Gets the inv item. Return null if inventory item has been removed
	/// </summary>
	/// <returns>The inv item.</returns>
	/// <param name="itemID">Item ID.</param>
	public InventoryItem GetInvItem(string itemID){
		Dictionary<string, InventoryItem> invItems = DataManager.Instance.GameData.Inventory.InventoryItems;
		InventoryItem invItem = null;

		if(invItems.ContainsKey(itemID))
			invItem = invItems[itemID];

		return invItem;
	}
	
	/// <summary>
	/// Adds the item.
	/// </summary>
	/// <param name="itemID">Item ID.</param>
	/// <param name="count">Count.</param>
	public void AddItem(string itemID, int count){
		Dictionary<string, InventoryItem> invItems = GetInventoryForItem(itemID);
		
		InventoryItem invItem = null;
		bool itemNew = false;
		listNeedsUpdate = true;

		if(invItems.ContainsKey(itemID)){ //If item already in dict. increment amount

			//if in the inventory already check if it's wallpaper
			//if it's wallpaper don't increment count just return
			if(CheckForWallpaper(itemID))
				return;

			invItem = invItems[itemID];
			invItem.Amount += count; 
			invItems[itemID] = invItem;
		}
		else{ //Add InventoryItem into dict if key doesn't exist
			itemNew = true;
			Item itemData = DataLoaderItems.GetItem(itemID);

			invItem = new InventoryItem(itemID, itemData.Type, itemData.TextureName);
			invItems[itemID] = invItem;

			//special case: keep track of bought wallpaper in another list.
			if(itemData.Type == ItemType.Decorations){
				DecorationItem decoItem = (DecorationItem)itemData;

				if(decoItem.DecorationType == DecorationTypes.Wallpaper){
					List<string> oneTimePurchasedInv = DataManager.Instance.GameData.Inventory.OneTimePurchasedItems;
					oneTimePurchasedInv.Add(itemData.ID);
				}
			}
		}

		if(OnItemAddedToInventory != null){
			InventoryEventArgs args = new InventoryEventArgs();
			args.IsItemNew = itemNew;
			args.InvItem = invItem;

			OnItemAddedToInventory(this, args);
		}
	}
	
	/// <summary>
	/// Uses the pet item.
	/// </summary>
	/// <param name="itemID">Item ID.</param>
	public void UsePetItem(string itemID){
		Dictionary<string, InventoryItem> invItems = GetInventoryForItem(itemID);
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
			string itemSound = invItem.ItemData.SoundUsed;
			if(!string.IsNullOrEmpty(itemSound))
				AudioManager.Instance.PlayClip(itemSound);
			
			//remove inv item if there is none left
			if(invItem.Amount == 0)
				invItems.Remove(itemID);
			
			// fire item used event
			if(OnItemUsed != null){
				InventoryEventArgs args = new InventoryEventArgs();
				
				args.InvItem = invItem;
				OnItemUsed(this, args);
			}
		}
	}

	public void UseMiniPetItem(string itemID){
		Dictionary<string, InventoryItem> invItems = GetInventoryForItem(itemID);
		InventoryItem invItem = null;
		listNeedsUpdate = true;

		if(invItems.ContainsKey(itemID)){
			invItem = invItems[itemID];
			invItem.Amount--;

			//analytics
			Analytics.Instance.ItemEvent(Analytics.ITEM_STATUS_USED, invItem.ItemType, invItem.ItemID);
			
			//remove inv item if there is none left
			if(invItem.Amount == 0)
				invItems.Remove(itemID);
			
			// fire item used event
			if(OnItemUsed != null){
				InventoryEventArgs args = new InventoryEventArgs();
				
				args.InvItem = invItem;
				OnItemUsed(this, args);
			}
		}

		Debug.Log("MiniPet receives item");
	}

	/// <summary>
	/// Gets the inventory for item. Based on the item type of itemID, this function
	/// will return the proper inventory for it
	/// </summary>
	/// <returns>The inventory for item.</returns>
	/// <param name="itemID">Item I.</param>
	private Dictionary<string, InventoryItem> GetInventoryForItem(string itemID){
		// what list the item is placed in depends on what kind of item it is
		ItemType eType = DataLoaderItems.GetItemType(itemID);
		Dictionary<string, InventoryItem> inventory = new Dictionary<string, InventoryItem>();
		
		switch(eType){
		case ItemType.Decorations:
			inventory = DataManager.Instance.GameData.Inventory.DecorationItems;
			break;
		default:
			inventory = DataManager.Instance.GameData.Inventory.InventoryItems;
			break;
		}		
		
		return inventory;
	}
}
