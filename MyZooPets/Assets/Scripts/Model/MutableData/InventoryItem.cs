using UnityEngine;
using System.Collections;

//---------------------------------------------------
// InventoryItem
// Items that are stored in the inventory. Mutable data 
//---------------------------------------------------
public class InventoryItem{
    public string ItemID {get; set;}
    public string ItemTextureName {get; set;}
    public ItemType ItemType {get; set;}
    public int Amount {get; set;}

    //TO DO: should problem just use ItemLogic.Instance.GetItem(itemID)	
    //redundant method
	public Item ItemData{
		get{return DataItems.GetItem(ItemID);}
	}

    public InventoryItem(string itemID, ItemType itemType, string textureName){
        ItemID = itemID;
        ItemType = itemType;
        ItemTextureName = textureName;
        Amount = 1;
    }

    public InventoryItem(){}
}
