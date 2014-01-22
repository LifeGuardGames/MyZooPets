using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// Data_LootTable
// An individual loot table.  A loot table is a list of
// items with quantities and weights, with the idea that
// one of the items will be chosen from it.
// This is immutable data.
//---------------------------------------------------

public class Data_LootTable {
	// id for the loot table
	private string strID;
	public string GetID() {
		return strID;	
	}
	
	// list of loot in this table
	List<Data_Loot> listLoot;
	
	//---------------------------------------------------
	// GetItem()
	// Randomly picks from a weighted list of items that
	// this loot table could give, and returns it (along
	// with its quantity).
	//---------------------------------------------------	
	public KeyValuePair<Item, int> GetItem() {
		// created a weighted list of our loot
		List<Data_Loot> listWeighted = new List<Data_Loot>();
		foreach ( Data_Loot loot in listLoot ) {
			int nWeight = loot.GetWeight();
			for ( int i = 0; i < nWeight; ++i )
				listWeighted.Add( loot );
		}
		
		// now pick a random piece of loot from the list
		Data_Loot lootPicked = ListUtils.GetRandomElement<Data_Loot>( listWeighted );
		
		// create a key value pair based on the item and its quantity
		Item dataItem = ItemLogic.Instance.GetItem( lootPicked.GetID() );
		int nQuantity = lootPicked.GetQuantity();
		KeyValuePair<Item, int> item = new KeyValuePair<Item, int>( dataItem, nQuantity );
		
		// return said item/quantity
		return item;
	}

	public Data_LootTable( string id, Hashtable hashAttr, List<IXMLNode> listData, string strError ) {
		// set id
		strID = id;
		
		strError += "(" + id + ")";
		
		// set the loot in this table
		listLoot = new List<Data_Loot>();
		foreach( IXMLNode node in listData ) {
			Data_Loot loot = new Data_Loot( node, strError );
			listLoot.Add( loot );
		}
	}
}
