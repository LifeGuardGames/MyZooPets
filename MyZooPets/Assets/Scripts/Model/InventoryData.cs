using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[DoNotSerializePublic]
public class InventoryData{
    [SerializeThis]
    private Dictionary<string, InventoryItem> inventoryItems; //Key: itemID, Value: InventoryItem instance
	
    [SerializeThis]
    private Dictionary<string, InventoryItem> decorationItems; //Key: itemID, Value: InventoryItem instance	
	
    public Dictionary<string, InventoryItem> InventoryItems{
        get{return inventoryItems;}
        set{inventoryItems = value;}
    }
	
    public Dictionary<string, InventoryItem> DecorationItems{
        get{return decorationItems;}
        set{decorationItems = value;}
    }	

    public InventoryData(){}

    public void Init(){
        inventoryItems = new Dictionary<string, InventoryItem>();
		decorationItems = new Dictionary<string, InventoryItem>();
    }
}
