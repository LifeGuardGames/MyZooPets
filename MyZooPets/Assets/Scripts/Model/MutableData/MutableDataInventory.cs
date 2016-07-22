using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// InventoryData 
// Save data for Inventory. Mutable data 
//---------------------------------------------------

public class MutableDataInventory{
    public Dictionary<string, InventoryItem> InventoryItems {get; set;} //Key: itemID, Value: InventoryItem instance
    public Dictionary<string, InventoryItem> DecorationItems {get; set;} //Key: itemID, Value: InventoryItem instance
	public Dictionary<string, InventoryItem> AccessoryItems {get; set;} //Key: itemID, Value: InventoryItem instance
    public List<string> OneTimePurchasedItems {get; set;} // Key:itemID  keep tracks of the items that can only be purchased once 
	public List<string> UnopenedItemBoxes {get; set;} // unopened item box ids
	public LgTuple <string,string> secretCode;
	public List <string> isSecretItemUnlocked;

    //-------------------Initialization---------------------
    public MutableDataInventory(){
        Init(); 
    }

    public void Init(){
        InventoryItems = new Dictionary<string, InventoryItem>();
		DecorationItems = new Dictionary<string, InventoryItem>();
		AccessoryItems = new Dictionary<string, InventoryItem>();
        OneTimePurchasedItems = new List<string>();
		UnopenedItemBoxes = new List<string>();
		secretCode = new LgTuple <string,string>("Food3","Food2");
		isSecretItemUnlocked = new List<string> ();
    }
}
