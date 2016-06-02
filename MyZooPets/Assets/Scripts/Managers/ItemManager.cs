using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Reference all Items, not to be confused with InventoryManager
/// </summary>
public class ItemManager : Singleton<ItemManager>{
	private List<Item> foodList;		// List with only FoodItem. sorted by cost
	private List<Item> usableList;		// List with only UsableItem. sorted by cost
	private List<Item> decorationList;	// List with only DecorationItem. sorted by cost
	private List<Item> accessoryList;	// List with only AccessoryItem. sorted by cost
	private Dictionary<DecorationTypes, List<DecorationItem>> decorationSubCatList; // Decorations grouped by deco type
	private Dictionary<string, DateTime> farmTimeDictionary;	// Mutable dict for farm deco times

	/// <summary>
	/// Gets all foods. Sorted by cost
	/// </summary>
	public List<Item> FoodList{
		get{
			if(foodList == null){
				foodList = new List<Item>();
				Dictionary<string, Item> foodDict = DataLoaderItems.GetAllItemsOfType(ItemType.Foods);
				foodList = ListFromDictionarySortByCost(foodDict);
			}
			return foodList;
		}
	}

	/// <summary>
	/// Gets all usables. Sorted by cost
	/// </summary>
	public List<Item> UsableList{
		get{
			if(usableList == null){
				usableList = new List<Item>();
				Dictionary<string, Item> usableDict = DataLoaderItems.GetAllItemsOfType(ItemType.Usables);
				usableList = ListFromDictionarySortByCost(usableDict);
			}
			return usableList;
		}
	}

	/// <summary>
	/// Gets all decorations. Sorted by cost
	/// </summary>
	public List<Item> DecorationList{
		get{
			if(decorationList == null){
				decorationList = new List<Item>();
				Dictionary<string, Item> decorationDict = DataLoaderItems.GetAllItemsOfType(ItemType.Decorations);
				decorationList = ListFromDictionarySortByCost(decorationDict);
			}
			return decorationList;
		}		
	}

	public List<Item> AccessoryList{
		get{
			if(accessoryList == null){
				accessoryList = new List<Item>();
				Dictionary<string, Item> accesorryDict = DataLoaderItems.GetAllItemsOfType(ItemType.Accessories);
				accessoryList = ListFromDictionarySortByCategoryCost(accesorryDict);
			}
			return accessoryList;
		}
	}

	public Dictionary<DecorationTypes, List<DecorationItem>> DecorationSubCatList{
		get{
			if(decorationSubCatList == null){
				//Cast the whole arrow to type DecorationItem so we can sort the list by DecorationTypes
				List<DecorationItem> decoItems = DecorationList.Cast<DecorationItem>().ToList();
				decorationSubCatList = new Dictionary<DecorationTypes, List<DecorationItem>>();	

				//Sort and DecorationTypes and store into a dictionary
				decorationSubCatList = (from decoItem in decoItems
										group decoItem by decoItem.DecorationType into groupedClass
										select groupedClass).ToDictionary(i => i.Key, i => i.ToList());
			}
			return decorationSubCatList;
		}
	}

	public Dictionary<string, DateTime> FarmTimeDictionary{
		get{
			if(farmTimeDictionary == null){
				farmTimeDictionary = DataManager.Instance.GameData.Decorations.FarmDecorationTimes;
			}
			return farmTimeDictionary;
		}
	}

	/// <summary>
	/// For the player's own good, we stop them if they try to use an item that will buff a stat that is
	/// already at max.  This function returns whether or not the user can use an item due to this.
	/// </summary>
	/// <returns><c>true</c> if this instance can use the specified itemID; otherwise, <c>false</c>.</returns>
	public bool CanUseItem(string itemID){
		// start off with true
		bool isUsable = true;
		
		// get the stats dictionary for the item
		Dictionary<StatType, int> statsDict = GetStatsDict(itemID);
		
		// if the stats dictionary is not null, we want to be sure that the stats aren't already at max
		if(statsDict != null){		
			int moodAmount = 0;
			int healthAmount = 0;

			if(statsDict.ContainsKey(StatType.Hunger)){
				moodAmount = statsDict[StatType.Hunger];
			}

			if(statsDict.ContainsKey(StatType.Health)){
				healthAmount = statsDict[StatType.Health];
			}
			
			// if the amounts are > 0 (i.e. adding health/mood) and those values are already at 100, then the user can't use
			// the item, because it would be a waste.
			int currentHealth = StatsManager.Instance.GetStat(StatType.Health);
			int currentMood = StatsManager.Instance.GetStat(StatType.Hunger);
			
			if(moodAmount > 0 && healthAmount > 0 && currentMood == 100 && currentHealth == 100){
				isUsable = false;
			}
			else if(moodAmount > 0 && currentMood == 100){
				isUsable = false;
			}
			else if(healthAmount > 0 && currentHealth == 100){
				isUsable = false;
			}

			// Custom check for emergency inhaler, dont want to apply if not sick
			if(itemID == "Usable0"){	
				if(currentHealth > 60){
					isUsable = false;
				}
			}
		}
		return isUsable;
	}

	/// <summary>
	/// Apply the stats effect that the Item with itemID has to the appropriate stats
	/// </summary>
	/// <param name="itemID">Item ID.</param>
	public void StatsEffect(string itemID){
		Dictionary<StatType, int> statDict = GetStatsDict(itemID);
		
		if(statDict != null){
			StatsEffect(statDict);
		}
	}

	public List<Item> GetItemsUnlockAtNextLevel(){
		int nextLevel = LevelLogic.Instance.NextLevel;
		List<Item> itemsUnlock = new List<Item>();
		List<Item> retList;

		foreach(Item item in FoodList){
			if(!item.IsSecretItem && item.UnlockAtLevel == nextLevel){
				itemsUnlock.Add(item);
			}
		}

		foreach(Item item in DecorationList){
			if(!item.IsSecretItem && item.UnlockAtLevel == nextLevel){
				itemsUnlock.Add(item);
			}
		}

		//check how many items are selected
		//select only 3 by random
		if(itemsUnlock.Count > 3){
			retList = ListUtils.GetRandomElements<Item>(itemsUnlock, 3);
		}
		else{
			retList = itemsUnlock;
		}
		return retList; 
	}

	/// <summary>
	/// Returns a dictionary of stats info on the incoming item.  May return null.
	/// </summary>
	/// <returns>The stats dict.</returns>
	private Dictionary<StatType, int> GetStatsDict(string itemID){
		Item item = DataLoaderItems.GetItem(itemID);
		Dictionary<StatType, int> dictStats = null;
		switch(item.Type){
		case ItemType.Foods:
			FoodItem foodItem = (FoodItem)item;
			dictStats = foodItem.Stats;
			break;
		case ItemType.Usables:
			UsableItem usableItem = (UsableItem)item;
			dictStats = usableItem.Stats;
			break;
		}
		return dictStats;
	}

	//StatsEffect helper method
	private void StatsEffect(Dictionary<StatType, int> statDict){
		int moodAmount = 0;
		int healthAmount = 0;

		if(statDict.ContainsKey(StatType.Hunger)){
			moodAmount = statDict[StatType.Hunger];
		}
		if(statDict.ContainsKey(StatType.Health)){
			healthAmount = statDict[StatType.Health];
		}
		if(statDict.ContainsKey(StatType.Fire)){
			//add one more fire blow here
			int fireBreath = statDict[StatType.Fire];
			StatsManager.Instance.ChangeFireBreaths(fireBreath);
		}
		StatsManager.Instance.ChangeStats(healthDelta: healthAmount, hungerDelta: moodAmount, isFloaty: true);
	}

	/// <summary>
	/// Get list sorted by cost in ascending order from the item dictionary
	/// Select and sort need to be done in two steps because IOS doesn't support 
	/// OrderBy for value types, ex int. but works fine if use on class.
	/// </summary>
	/// <returns>The list from dictionary and sort.</returns>
	/// <param name="itemDict">Item dict.</param>
	private List<Item> ListFromDictionarySortByCost(Dictionary<string, Item> itemDict){
		var items = from keyValuePair in itemDict 
						select keyValuePair.Value;
		List<Item> itemList = (from item in items 
						orderby item.UnlockAtLevel ascending, item.Cost.ToString() ascending
						select item).ToList();
		return itemList;
	}

	/// <summary>
	/// Lists from dictionary sort by category cost.
	/// NOTE: Requires 'SortCategory' component in xml (ie accessory)
	/// </summary>
	/// <returns>The from dictionary sort by category cost.</returns>
	/// <param name="itemDict">Item dict.</param>
	private List<Item> ListFromDictionarySortByCategoryCost(Dictionary<string, Item> itemDict){
		var items = from keyValuePair in itemDict 
						select keyValuePair.Value;
		List<Item> itemList = (from item in items 
		                       orderby item.SortCategory ascending ,item.UnlockAtLevel.ToString() ascending, item.Cost.ToString() ascending
		                       select item).ToList();
		return itemList;
	}

	#region Farm Decoration use
	public DateTime GetFarmLastRedeemTime(string farmItemId){
		return FarmTimeDictionary[farmItemId];
	}

	public void UpdateFarmLastRedeemTime(string farmItemId, DateTime lastRedeemTime){
		if(FarmTimeDictionary.ContainsKey(farmItemId)){
			FarmTimeDictionary[farmItemId] = lastRedeemTime;
		}
		else{
			FarmTimeDictionary.Add(farmItemId, lastRedeemTime);
		}
	}

	public void RemoveFarmItem(string farmItemId){
		if(FarmTimeDictionary.ContainsKey(farmItemId)){
			FarmTimeDictionary.Remove(farmItemId);
		}
		else{
			Debug.LogWarning("Removing non-existant farmItemId from saved list - " + farmItemId);
		}
	}
	#endregion
}