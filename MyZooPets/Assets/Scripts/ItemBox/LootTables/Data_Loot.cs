using UnityEngine;
using System.Collections;

//---------------------------------------------------
// Data_Loot
// An individual piece of loot in a loot table.
// Immutable.
//---------------------------------------------------

public class Data_Loot {
	// id of the item
	private string strID;
	public string GetID() {
		return strID;	
	}
	
	// quantity of the item
	private int nQuantity;
	public int GetQuantity() {
		return nQuantity;	
	}
	
	// weight of the item
	private int nWeight;
	public int GetWeight() {
		return nWeight;	
	}
	
	public Data_Loot( IXMLNode node, string strError ) {
		Hashtable hashAttr = XMLUtils.GetAttributes(node);
		
		// get the item id of the loot
		strID = HashUtils.GetHashValue<string>( hashAttr, "ID", "Food0", strError );
		
		// get the quantity this loot would give
		nQuantity = int.Parse( HashUtils.GetHashValue<string>( hashAttr, "Quantity", "1", strError ) );
		
		// give the weight that this piece of loot may be selected
		nWeight = int.Parse( HashUtils.GetHashValue<string>( hashAttr, "Weight", "1", strError ) );
	}
}
