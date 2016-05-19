using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

//Item Logic Class
//Reference all Items.
public class ItemLogic : Singleton<ItemLogic>{
	private List<Item> foodList; //list with only FoodItem. sorted by cost
	private List<Item> usableList; //list with only UsableItem. sorted by cost
	private List<Item> decorationList; //list with only DecorationItem. sorted by cost
	private List<Item> accessoryList; // List with only AccessoryItem. sorted by cost
	private Dictionary<DecorationTypes, List<DecorationItem>> decorationSubCatList; //decoration grouped by deco type
	private Dictionary<string, DateTime> farmTimeDictionary;	// Mutable dict for farm deco times

	/// <summary>
	/// Gets the food list. Sorted by cost
	/// </summary>
	/// <value>The food list.</value>
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
	/// Gets the usable list. Sorted by cost
	/// </summary>
	/// <value>The usable list.</value>
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
	/// Gets the decoration list. Sorted by cost
	/// </summary>
	/// <value>The decoration list.</value>
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
	/// Gets the item.
	/// </summary>
	/// <returns>The item.</returns>
	/// <param name="itemID">Item ID.</param>
	public Item GetItem(string itemID){
		return DataLoaderItems.GetItem(itemID);
	}
	
	/// <summary>
	/// Gets the type of the item.
	/// </summary>
	/// <returns>The item type.</returns>
	/// <param name="itemID">Item ID.</param>
	public ItemType GetItemType(string itemID){
		return DataLoaderItems.GetItemType(itemID);
	}
	
	/// <summary>
	/// Gets the name of the item texture.
	/// </summary>
	/// <returns>The item texture name.</returns>
	/// <param name="itemID">Item ID.</param>
	public string GetItemTextureName(string itemID){
		return DataLoaderItems.GetItemTextureName(itemID);
	}
	
	/// <summary>
	/// Gets the name of the deco item prefab.
	/// </summary>
	/// <returns>The deco item prefab name.</returns>
	/// <param name="itemID">Item ID.</param>
	public string GetDecoItemPrefabName(string itemID){
		return DataLoaderItems.GetDecoItemPrefabName(itemID);
	}

	/// <summary>
	/// Gets the name of the deco item material.
	/// </summary>
	/// <returns>The deco item material name.</returns>
	/// <param name="itemID">Item ID.</param>
	public string GetDecoItemMaterialName(string itemID){
		return DataLoaderItems.GetDecoItemMaterialName(itemID);
	}

	/// <summary>
	/// Gets the name of the accessory item prefab.
	/// </summary>
	/// <returns>The accessory item prefab name.</returns>
	/// <param name="itemID">Item ID.</param>
	public string GetAccessoryItemPrefabName(string itemID){
		return DataLoaderItems.GetAccessoryItemPrefabName(itemID);
	}

	/// <summary>
	/// For the player's own good, we stop them if they
	/// try to use an item that will buff a stat that is
	/// already at max.  This function returns whether or
	/// not the user can use an item due to this.
	/// </summary>
	/// <returns><c>true</c> if this instance can use item the specified itemID; otherwise, <c>false</c>.</returns>
	/// <param name="itemID">Item ID.</param>
	public bool CanUseItem(string itemID){
		// start off with true
		bool isUsable = true;
		
		// get the stats dictionary for the item
		Dictionary<StatType, int> statsDict = GetStatsDict(itemID);
		
		// if the stats dictionary is not null, we want to be sure that the stats aren't already at max
		if(statsDict != null){		
			int moodAmount = 0;
			int healthAmount = 0;
	
			if(statsDict.ContainsKey(StatType.Mood))
				moodAmount = statsDict[StatType.Mood];
		
			if(statsDict.ContainsKey(StatType.Health))
				healthAmount = statsDict[StatType.Health];
			
			// if the amounts are > 0 (i.e. adding health/mood) and those values are already at 100, then the user can't use
			// the item, because it would be a waste.
			int currentHealth = StatsManager.Instance.GetStat(HUDElementType.Health);
			int currentMood = StatsManager.Instance.GetStat(HUDElementType.Hunger);
			
			if(moodAmount > 0 && healthAmount > 0 && currentMood == 100 && currentHealth == 100)
				isUsable = false;
			else if(moodAmount > 0 && currentMood == 100)
				isUsable = false;
			else if(healthAmount > 0 && currentHealth == 100)
				isUsable = false;

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
		
		if(statDict != null)
			StatsEffect(statDict);
	}

	public List<Item> GetItemsUnlockAtNextLevel(){
		int nextLevel = LevelLogic.Instance.NextLevel;
		List<Item> itemsUnlock = new List<Item>();
		List<Item> retList;

		//ItemBoxOnly == false
		//UnlockAtLevel == nextLevel
		foreach(Item item in FoodList){
			if(!item.IsSecretItem && item.UnlockAtLevel == nextLevel)
				itemsUnlock.Add(item);
		}

		foreach(Item item in DecorationList){
			if(!item.IsSecretItem && item.UnlockAtLevel == nextLevel)
				itemsUnlock.Add(item);
		}

		//check how many items are selected
		//select only 3 by random
		if(itemsUnlock.Count > 3)
			retList = ListUtils.GetRandomElements<Item>(itemsUnlock, 3);
		else
			retList = itemsUnlock;

		return retList; 
	}

	/// <summary>
	/// Returns a dictionary of stats info on the incoming
	/// item.  May return null.
	/// </summary>
	/// <returns>The stats dict.</returns>
	/// <param name="strItemID">String item ID.</param>
	private Dictionary<StatType, int> GetStatsDict(string strItemID){
		Item item = GetItem(strItemID);
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

		if(statDict.ContainsKey(StatType.Mood)){
			moodAmount = statDict[StatType.Mood];
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