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
				Dictionary<string, Item> foodDict = DataLoader_Items.GetAllItemsOfType(ItemType.Foods);
				foodList = SelectListFromDictionaryAndSort(foodDict);
			}
			return foodList;
		}
	}

	public List<Item> UsableList{
		get{
			if(usableList == null){
				usableList = new List<Item>();
				Dictionary<string, Item> usableDict = DataLoader_Items.GetAllItemsOfType(ItemType.Usables);
				usableList = SelectListFromDictionaryAndSort(usableDict);
			}
			return usableList;
		}
	}
	
	public List<Item> DecorationList {
		get{
			if(decorationList == null){
				decorationList = new List<Item>();
				Dictionary<string, Item> decorationDict = DataLoader_Items.GetAllItemsOfType(ItemType.Decorations);
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

	void Awake(){
	}

	//Returns Item with itemID
	public Item GetItem(string itemID){
		return DataLoader_Items.GetItem(itemID);
	}

	//Returns the type of item with itemID
	public ItemType GetItemType(string itemID){
		return DataLoader_Items.GetItemType(itemID);
	}

	//Returns the texture name of item with itemID
	public string GetItemTextureName(string itemID){
		return DataLoader_Items.GetItemTextureName(itemID);
	}

	//Returns the prefab name of item with itemID
	public string GetDecoItemPrefabName(string itemID){
		return DataLoader_Items.GetDecoItemPrefabName(itemID);
	}

	public string GetDecoItemMaterialName(string itemID){
		return DataLoader_Items.GetDecoItemMaterialName(itemID);
	}
	
	//---------------------------------------------------
	// CanUseItem()
	// For the player's own good, we stop them if they
	// try to use an item that will buff a stat that is
	// already at max.  This function returns whether or
	// not the user can use an item due to this.
	//---------------------------------------------------		
	public bool CanUseItem( string strItemID ) {
		// start off with true
		bool bCanUse = true;
		
		// get the stats dictionary for the item
		Dictionary<StatType, int> statsDict = GetStatsDict( strItemID );
		
		// if the stats dictionary is not null, we want to be sure that the stats aren't already at max
		if ( statsDict != null ) {		
			int moodAmount = 0;
			int healthAmount = 0;
	
			if(statsDict.ContainsKey(StatType.Mood))
				moodAmount = statsDict[StatType.Mood];
		
			if(statsDict.ContainsKey(StatType.Health))
				healthAmount = statsDict[StatType.Health];
			
			// if the amounts are > 0 (i.e. adding health/mood) and those values are already at 100, then the user can't use
			// the item, because it would be a waste.
			int nCurHealth = DataManager.Instance.GameData.Stats.GetStat( HUDElementType.Health );
			int nCurMood = DataManager.Instance.GameData.Stats.GetStat( HUDElementType.Mood );
			
			if ( moodAmount > 0 && healthAmount > 0 && nCurMood == 100 && nCurHealth == 100 )
				bCanUse = false;
			else if ( moodAmount > 0 && nCurMood == 100 )
				bCanUse = false;
			else if ( healthAmount > 0 && nCurHealth == 100 )
				bCanUse = false;
		}
		
		return bCanUse;
	}
	
	//---------------------------------------------------
	// GetStasDict()
	// Returns a dictionary of stats info on the incoming
	// item.  May return null.
	//---------------------------------------------------		
	private Dictionary<StatType, int> GetStatsDict( string strItemID ) {
		Item item = GetItem(strItemID);
		Dictionary<StatType, int> dictStats = null;
		switch(item.Type){
			case ItemType.Foods:
				FoodItem foodItem = (FoodItem) item;
				dictStats = foodItem.Stats;
			break;
			case ItemType.Usables:
				UsableItem usableItem = (UsableItem) item;
				dictStats = usableItem.Stats;
			break;
		}		
		
		return dictStats;
	}

	//Apply the stats effect that the Item with itemID has to the appropriate stats
	public void StatsEffect(string itemID){
		Dictionary<StatType, int> statDict = GetStatsDict( itemID );
		
		if(statDict != null)
			StatsEffect(statDict);
	}

	//StatsEffect helper method
	private void StatsEffect(Dictionary<StatType, int> statDict){
		int moodAmount = 0;
		int healthAmount = 0;

		if(statDict.ContainsKey(StatType.Mood))	{
			moodAmount = statDict[StatType.Mood];
		}
		if(statDict.ContainsKey(StatType.Health)){
			healthAmount = statDict[StatType.Health];
		}

		StatsController.Instance.ChangeStats(0, Vector3.zero, 0, Vector3.zero,
			healthAmount, Vector3.zero, moodAmount, Vector3.zero, true, bFloaty:true);	
	}

	//Get list sorted by cost in ascending order from the item dictionary
	//Select and sort need to be done in two steps because IOS doesn't support 
	//OrderBy for value types, ex string. but works fine if use on class.
	private List<Item> SelectListFromDictionaryAndSort(Dictionary<string, Item> itemDict){
		var items = from keyValuePair in itemDict 
						select keyValuePair.Value;
		List<Item> itemList = (from item in items 
						orderby item.GetLockedLevel()
						select item).ToList();
		return itemList;
	}
}