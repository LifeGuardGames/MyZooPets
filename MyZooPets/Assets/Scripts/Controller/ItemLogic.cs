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

	public List<Item> FoodList{
		get{
			if(foodList == null){
				foodList = new List<Item>();
				Dictionary<string, Item> foodDict = DataItems.GetAllItemsOfType(ItemType.Foods);
				foodList = SelectListFromDictionaryAndSort(foodDict);
			}
			return foodList;
		}
	}

	public List<Item> UsableList{
		get{
			if(usableList == null){
				usableList = new List<Item>();
				Dictionary<string, Item> usableDict = DataItems.GetAllItemsOfType(ItemType.Usables);
				usableList = SelectListFromDictionaryAndSort(usableDict);
			}
			return usableList;
		}
	}
	
	public List<Item> DecorationList {
		get{
			if(decorationList == null){
				decorationList = new List<Item>();
				Dictionary<string, Item> decorationDict = DataItems.GetAllItemsOfType(ItemType.Decorations);
				decorationList = SelectListFromDictionaryAndSort(decorationDict);
			}
			return decorationList;
		}		
	}

	void Awake(){
		DataItems.SetupData();
	}

	//Returns Item with itemID
	public Item GetItem(string itemID){
		return DataItems.GetItem(itemID);
	}

	//Returns the type of item with itemID
	public ItemType GetItemType(string itemID){
		return DataItems.GetItemType(itemID);
	}

	//Returns the texture name of item with itemID
	public string GetItemTextureName(string itemID){
		return DataItems.GetItemTextureName(itemID);
	}

	//Apply the stats effect that the Item with itemID has to the appropriate stats
	public void StatsEffect(string itemID){
		Item item = GetItem(itemID);
		Dictionary<StatType, int> statDict = null;
		switch(item.Type){
			case ItemType.Foods:
				FoodItem foodItem = (FoodItem) item;
				statDict = foodItem.Stats;

				if(statDict != null)
					StatsEffect(statDict);
			break;
			case ItemType.Usables:
				UsableItem usableItem = (UsableItem) item;
				statDict = usableItem.Stats;

				if(statDict != null)
					StatsEffect(statDict);
			break;
		}
	}

	//StatsEffect helper method
	private void StatsEffect(Dictionary<StatType, int> statDict){
		int moodAmount = 0;
		int healthAmount = 0;

		if(statDict.ContainsKey(StatType.Mood))	{
			moodAmount = statDict[StatType.Mood];
		}
		if(statDict.ContainsKey(StatType.Health)){
			moodAmount = statDict[StatType.Health];
		}

		StatsController.Instance.ChangeStats(0, Vector3.zero, 0, Vector3.zero,
			healthAmount, Vector3.zero, moodAmount, Vector3.zero);	
	}

	//Get list sorted by cost in ascending order from the item dictionary
	//Select and sort need to be done in two steps because IOS doesn't support 
	//OrderBy for value types, ex string. but works fine if use on class.
	private List<Item> SelectListFromDictionaryAndSort(Dictionary<string, Item> itemDict){
		var items = from keyValuePair in itemDict 
						select keyValuePair.Value;
		List<Item> itemList = (from item in items 
						orderby item.Cost
						select item).ToList();
		return itemList;
	}
}