using UnityEngine;
using System.Collections;

//---------------------------------------------------
// InventoryItem
// Items that are stored in the inventory. 
// Mutable data but contains reference to immutable data
//---------------------------------------------------
public class InventoryItem{
    public string ItemID {get; set;}
    public string ItemTextureName {get; set;}
    public ItemType ItemType {get; set;}
    public int Amount {get; set;}

    //Shortcut to return the immutable data of item with ItemID
	public Item ItemData{
		get{return ItemLogic.Instance.GetItem(ItemID);}
	}

    public InventoryItem(string itemID, ItemType itemType, string textureName){
        ItemID = itemID;
        ItemType = itemType;
        ItemTextureName = textureName;
        Amount = 1;
    }

    public InventoryItem(){}
}
