using UnityEngine;
using System.Collections;

public class FoodItem : Item{
    private Hashtable stats; //item's effect on stats. Optional

    public Hashtable Stats{
        get{return stats;}
        set{stats = value;}
    }

    public FoodItem(string id, ItemType type, Hashtable hashItemData) : base (id, type, hashItemData){
        if(hashItemData.ContainsKey("Stats"))
            stats = GetStats(hashItemData["Stats"] as IXMLNode);
    }
}