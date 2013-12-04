using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// Data_ItemBox
// An individual item box.  An item box is a list of
// a list of loot tables and weights.  One of the
// lists is picked, and then all the items within
// the loot tables are picked.
// This is immutable data.
//---------------------------------------------------

// within the data of an item box, are multiple item box variations, so that one box could reward different things
public struct ItemBoxVariation {
	// weight that this particular variation will be selected
	public int nWeight;
	
	// list of loot tables in this variation
	public List<string> listLootTables;
	
	public ItemBoxVariation( int nWeight, List<string> listLootTables ) {
		this.nWeight = nWeight;
		this.listLootTables = listLootTables;
	}
	
	//---------------------------------------------------
	// GetItems()
	// Returns list of items for this variation's loot
	// tables.
	//---------------------------------------------------	
	public List<KeyValuePair<Item, int>> GetItems() {
		List<KeyValuePair<Item, int>> items = new List<KeyValuePair<Item, int>>();
		
		// loop through each loot table and get the item and quantity from that loot table
		foreach ( string strLootTableKey in listLootTables ) {
			// get the loot table from the id
			Data_LootTable dataTable = DataLoader_LootTables.GetLootTable( strLootTableKey );
			
			// null check
			if ( dataTable != null ) {
				// get the item from the loot table
				KeyValuePair<Item, int> item = dataTable.GetItem();
				
				// add it to our list
				items.Add( item );
			}
		}
		
		// this dictionary will have a list of all items + quantities that this loot table gave
		return items;
	}	
}

public class Data_ItemBox {
	// id for the item box
	private string strID;
	public string GetID() {
		return strID;	
	}
	
	// list of all potential variations on this item box
	List<ItemBoxVariation> listVariations;	

	public Data_ItemBox( string id, Hashtable hashAttr, List<IXMLNode> listData, string strError ) {
		// set id
		strID = id;
		
		strError += "(" + id + ")";
		
		// set the variations for this item box
		listVariations = new List<ItemBoxVariation>();
		foreach( IXMLNode node in listData ) {
			Hashtable hashVariationAttr = XMLUtils.GetAttributes( node );
			
			// get the weight
			int nWeight = int.Parse( HashUtils.GetHashValue<string>( hashVariationAttr, "Weight", "1" ) );
			
			// get the list of loot tables
			List<string> listLootTables = new List<string>();
			List<IXMLNode> listLootTableNodes = XMLUtils.GetChildrenList( node );
			foreach (IXMLNode nodeLootTable in listLootTableNodes ) {
				string strLootTable = XMLUtils.GetString( nodeLootTable, "", strError );
				listLootTables.Add( strLootTable );
			}
			
			ItemBoxVariation variation = new ItemBoxVariation( nWeight, listLootTables );
			listVariations.Add( variation );
		}
	}
	
	//---------------------------------------------------
	// GetItems()
	// This function will return a list of items and their
	// quantities as determined by one of the variations
	// in this item box.
	//---------------------------------------------------	
	public List<KeyValuePair<Item, int>> GetItems() {
		// first, create a weighted list of our variations
		List<ItemBoxVariation> listWeighted = new List<ItemBoxVariation>();
		foreach ( ItemBoxVariation variation in listVariations ) {
			int nWeight = variation.nWeight;
			for ( int i = 0; i < nWeight; i++ )
				listWeighted.Add( variation );
		}
		
		// now, pick a random variation
		ItemBoxVariation variationPicked = ListUtils.GetRandomElement<ItemBoxVariation>( listWeighted );
		
		// now create our dictionary based off this data
		List<KeyValuePair<Item, int>> items = variationPicked.GetItems();
		
		return items;
	}	
}
