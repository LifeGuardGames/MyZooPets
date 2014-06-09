using UnityEngine;
using System.Collections;

/// <summary>
/// An individual piece of loot in a loot table.
/// </summary>

public class ImmutableDataLoot{

	private string lootID; 	// id of the item
	private int quantity; // quantity of the item
	private int weight; // weight of the item

	public string GetID(){
		return lootID;	
	}

	public int GetQuantity(){
		return quantity;	
	}

	public int GetWeight(){
		return weight;	
	}
	
	public ImmutableDataLoot(IXMLNode node, string errorMessage){
		Hashtable hashAttr = XMLUtils.GetAttributes(node);
		
		// get the item id of the loot
		lootID = HashUtils.GetHashValue<string>(hashAttr, "ID", "Food0", errorMessage);
		
		// get the quantity this loot would give
		quantity = int.Parse(HashUtils.GetHashValue<string>(hashAttr, "Quantity", "1", errorMessage));
		
		// give the weight that this piece of loot may be selected
		weight = int.Parse(HashUtils.GetHashValue<string>(hashAttr, "Weight", "1", errorMessage));
	}
}
