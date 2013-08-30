using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FoodItem : Item{
    private Dictionary<StatType, int> stats; //item's effect on stats. Optional

    public Dictionary<StatType, int> Stats{
        get{return stats;}
    }

    public FoodItem(string id, ItemType type, Hashtable hashItemData) : base (id, type, hashItemData){
        if(hashItemData.ContainsKey("Stats"))
            stats = GetStats(hashItemData["Stats"] as IXMLNode);
    }
}