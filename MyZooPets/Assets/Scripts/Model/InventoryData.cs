using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// InventoryData 
// Save data for Inventory. Mutable data 
//---------------------------------------------------

public class InventoryData{
    public Dictionary<string, InventoryItem> InventoryItems {get; set;} //Key: itemID, Value: InventoryItem instance
    public Dictionary<string, InventoryItem> DecorationItems {get; set;} //Key: itemID, Value: InventoryItem instance	
	
	// unopened item box ids
	public List<string> UnopenedItemBoxes {get; set;}

    //-------------------Initialization---------------------

    public InventoryData(){}

    public void Init(){
        InventoryItems = new Dictionary<string, InventoryItem>();
		DecorationItems = new Dictionary<string, InventoryItem>();
		UnopenedItemBoxes = new List<string>();
		
		// testing
		//UnopenedItemBoxes.Add( "Box_0" );
		//UnopenedItemBoxes.Add( "Box_1" );
		//UnopenedItemBoxes.Add( "Box_1" );
    }
}
