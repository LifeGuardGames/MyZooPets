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
	private Dictionary<DecorationTypes, List<DecorationItem>> decorationSubCatList; //decoration grouped by deco type

	public List<Item> FoodList{
		get{
			if(foodList == null){
				foodList = new List<Item>();
				Dictionary<string, Item> foodDict = DataLoaderItems.GetAllItemsOfType(ItemType.Foods);
				foodList = SelectListFromDictionaryAndSort(foodDict);
			}
			return foodList;
		}
	}

	public List<Item> UsableList{
		get{
			if(usableList == null){
				usableList = new List<Item>();
				Dictionary<string, Item> usableDict = DataLoaderItems.GetAllItemsOfType(ItemType.Usables);
				usableList = SelectListFromDictionaryAndSort(usableDict);
			}
			return usableList;
		}
	}
	
	public List<Item> DecorationList{
		get{
			if(decorationList == null){
				decorationList = new List<Item>();
				Dictionary<string, Item> decorationDict = DataLoaderItems.GetAllItemsOfType(ItemType.Decorations);
				decorationList = SelectListFromDictionaryAndSort(decorationDict);

			}
			return decorationList;
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

	//Returns Item with itemID
	public Item GetItem(string itemID){
		return DataLoaderItems.GetItem(itemID);
	}

	//Returns the type of item with itemID
	public ItemType GetItemType(string itemID){
		return DataLoaderItems.GetItemType(itemID);
	}

	//Returns the texture name of item with itemID
	public string GetItemTextureName(string itemID){
		return DataLoaderItems.GetItemTextureName(itemID);
	}

	//Returns the prefab name of item with itemID
	public string GetDecoItemPrefabName(string itemID){
		return DataLoaderItems.GetDecoItemPrefabName(itemID);
	}

	public string GetDecoItemMaterialName(string itemID){
		return DataLoaderItems.GetDecoItemMaterialName(itemID);
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
			int currentHealth = StatsController.Instance.GetStat(HUDElementType.Health);
			int currentMood = StatsController.Instance.GetStat(HUDElementType.Mood);
			
			if(moodAmount > 0 && healthAmount > 0 && currentMood == 100 && currentHealth == 100)
				isUsable = false;
			else if(moodAmount > 0 && currentMood == 100)
				isUsable = false;
			else if(healthAmount > 0 && currentHealth == 100)
				isUsable = false;
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
			if(!item.ItemBoxOnly && item.UnlockAtLevel == nextLevel)
				itemsUnlock.Add(item);
		}

		foreach(Item item in DecorationList){
			if(!item.ItemBoxOnly && item.UnlockAtLevel == nextLevel)
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
			StatsController.Instance.ChangeFireBreaths(fireBreath);
		}

		StatsController.Instance.ChangeStats(deltaHealth: healthAmount, deltaMood: moodAmount, bFloaty: true);
	}

	//Get list sorted by cost in ascending order from the item dictionary
	//Select and sort need to be done in two steps because IOS doesn't support 
	//OrderBy for value types, ex int. but works fine if use on class.
	private List<Item> SelectListFromDictionaryAndSort(Dictionary<string, Item> itemDict){
		var items = from keyValuePair in itemDict 
						select keyValuePair.Value;
		List<Item> itemList = (from item in items 
						orderby item.UnlockAtLevel ascending, item.Cost.ToString() ascending
						select item).ToList();
		return itemList;
	}
}