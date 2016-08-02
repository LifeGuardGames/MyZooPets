using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Store item entry stats. An individual item entry in the sore UI for a stats
/// item
/// </summary>
public class StoreItemStatsController : StoreItemController{
	// list of stat pair objects
	public List<StatPairUIController> listStats;

	/// <summary>
	/// Sets the desc.
	/// </summary>
	/// <param name="itemData">Item data.</param>
	protected override void SetDescription(Item itemData){
		// do some bounds and null checking
		StatsItem item = (StatsItem)itemData;
		if(item == null){
			Debug.LogError("Stats item not actually a stats item...", this);
			return;
		}
		
		// loop through the gameobject stat pairs and init or destroy them based on if there is a stat for it to show
		for(int i = 0; i < listStats.Count; ++i){
			StatPairUIController pair = listStats[i];
			
			// make sure that there is a stat for this game object
			if(item.Stats.Count >= i + 1){
				// if there is, init the stat
				StatType eStat = item.Stats.Keys.ElementAt(i);
				int nAmount = item.Stats.Values.ElementAt(i);
				pair.Init(eStat, nAmount);
			}
			else{
				// if there isn't, destroy the gameobject
				Destroy(pair.gameObject);
			}
		}
	}
}
