using UnityEngine;
using System.Collections;

[DoNotSerializePublic]
public class InventoryData{
    [SerializeThis]
    private int[] inventory; //array for all items, index as Item Id

    //===============Getters & Setters=================
    public int[] InventoryArray{
        get{ return inventory;}
        set{ inventory = value;}
    }

    //================Initialization============
    public InventoryData(){}

    public void Init(){
        inventory = new int[ItemLogic.MAX_ITEM_COUNT];
    }
}
