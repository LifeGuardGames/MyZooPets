using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// StatsItem
// This is an item that has stat modifiers.
//---------------------------------------------------

public class StatsItem : Item{
    private Dictionary<StatType, int> stats; //item's effect on stats. Optional

    public Dictionary<StatType, int> Stats{
        get{return stats;}
    }

    public StatsItem(string id, ItemType type, Hashtable hashItemData) : base (id, type, hashItemData){
        if(hashItemData.ContainsKey("Stats"))
            stats = GetStats(hashItemData["Stats"] as IXMLNode);
    }
	
	//---------------------------------------------------
	// GetDesc()
	// Food items build their description off of their
	// stats.
	//---------------------------------------------------	
	public override string Description {
		get {
			string strDesc = "";
			
			int nCount = 0;
			foreach(KeyValuePair<StatType, int> entry in Stats) {
				if ( nCount > 0 )
					strDesc += ", ";	// add comma to separate stats
				
				// add the localized stat
			    strDesc += StatsController.Instance.GetStatText( entry.Key );
				
				// use a + or - modifier
				string strModifier = entry.Value > 0 ? "+" : "";
				strDesc += " " + strModifier;
				
				// add the value
				strDesc += entry.Value;
			}
			
			return strDesc;
		}
	}	
}