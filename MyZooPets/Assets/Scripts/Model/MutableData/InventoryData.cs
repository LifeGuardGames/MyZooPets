﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// InventoryData 
// Save data for Inventory. Mutable data 
//---------------------------------------------------

public class InventoryData{
    public Dictionary<string, InventoryItem> InventoryItems {get; set;} //Key: itemID, Value: InventoryItem instance
    public Dictionary<string, InventoryItem> DecorationItems {get; set;} //Key: itemID, Value: InventoryItem instance	
	public List<string> UnopenedItemBoxes {get; set;} // unopened item box ids

    //-------------------Initialization---------------------
    public InventoryData(){
        Init(); 
    }

    public void Init(){
        InventoryItems = new Dictionary<string, InventoryItem>();
		DecorationItems = new Dictionary<string, InventoryItem>();
		UnopenedItemBoxes = new List<string>();
    }
}
