using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Stats item. This is an item that has stat modifiers
/// </summary>
public class StatsItem : Item{
    private Dictionary<StatType, int> stats; //item's effect on stats. Optional

    public Dictionary<StatType, int> Stats{
        get{return stats;}
    }

    public StatsItem(string id, ItemType type, Hashtable hashItemData) : base (id, type, hashItemData){
        if(hashItemData.ContainsKey("Stats"))
            stats = LoadStats(hashItemData["Stats"] as IXMLNode);
    }
}