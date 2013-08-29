using UnityEngine;
using System.Collections;

//Items that are stored in the inventory
[DoNotSerializePublic]
public class InventoryItem{
    [SerializeThis]
    private string itemID;
    [SerializeThis]
    private string itemTextureName;
    [SerializeThis]
    private ItemType itemType;
    [SerializeThis]
    private int amount;

    public string ItemID{
        get{return itemID;}
    }

    public string ItemTextureName{
        get{return itemTextureName;}
    }

    public ItemType ItemType{
        get{return itemType;}
    }

    public int Amount{
        get{return amount;}
        set{amount = value;}
    }

    public InventoryItem(string itemID, ItemType itemType, string textureName){
        this.itemID = itemID;
        this.itemType = itemType;
        this.itemTextureName = textureName;
        amount = 1;
    }
}
