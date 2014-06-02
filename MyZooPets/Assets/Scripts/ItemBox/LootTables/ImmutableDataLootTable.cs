using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// An individual loot table.  A loot table is a list of
/// items with quantities and weights, with the idea that
/// one of the items will be chosen from it.
/// </summary>
public class ImmutableDataLootTable{
	// id for the loot table
	private string lootTableID;

	public string GetID(){
		return lootTableID;	
	}
	
	// list of loot in this table
	List<ImmutableDataLoot> listLoot;
	
	//---------------------------------------------------
	// GetItem()
	// Randomly picks from a weighted list of items that
	// this loot table could give, and returns it (along
	// with its quantity).
	//---------------------------------------------------	
	public KeyValuePair<Item, int> GetItem(){
		// created a weighted list of our loot
		List<ImmutableDataLoot> listWeighted = new List<ImmutableDataLoot>();
		foreach(ImmutableDataLoot loot in listLoot){
			int nWeight = loot.GetWeight();
			for(int i = 0; i < nWeight; ++i)
				listWeighted.Add(loot);
		}
		
		// now pick a random piece of loot from the list
		ImmutableDataLoot lootPicked = ListUtils.GetRandomElement<ImmutableDataLoot>(listWeighted);
		
		// create a key value pair based on the item and its quantity
		Item dataItem = ItemLogic.Instance.GetItem(lootPicked.GetID());
		int quantity = lootPicked.GetQuantity();
		KeyValuePair<Item, int> item = new KeyValuePair<Item, int>(dataItem, quantity);
		
		// return said item/quantity
		return item;
	}

	public ImmutableDataLootTable(string id, Hashtable hashAttr, List<IXMLNode> listData, string errorMessage){
		// set id
		lootTableID = id;
		
		errorMessage += "(" + id + ")";
		
		// set the loot in this table
		listLoot = new List<ImmutableDataLoot>();
		foreach(IXMLNode node in listData){
			ImmutableDataLoot loot = new ImmutableDataLoot(node, errorMessage);
			listLoot.Add(loot);
		}
	}
}
