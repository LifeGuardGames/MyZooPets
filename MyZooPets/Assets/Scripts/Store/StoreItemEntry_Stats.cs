using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//---------------------------------------------------
// StoreItemEntry_Stats
// An individual item entry in the store UI for a 
// stats item.
//---------------------------------------------------

public class StoreItemEntry_Stats : StoreItemEntry {
	// list of stat pair objects
	public List<StatPairUI> listStats;
	
	//---------------------------------------------------
	// SetDesc()
	// Set the description for this item.
	//---------------------------------------------------		
	protected override void SetDesc( Item itemData ) {
		// do some bounds and null checking
		StatsItem item = (StatsItem) itemData;
		if ( item == null ) {
			Debug.Log("Stats item not actually a stats item...", this);
			return;
		}
		
		// loop through the gameobject stat pairs and init or destroy them based on if there is a stat for it to show
		for ( int i = 0; i < listStats.Count; ++i ) {
			StatPairUI pair = listStats[i];
			
			// make sure that there is a stat for this game object
			if ( item.Stats.Count >= i+1 ) {
				// if there is, init the stat
				StatType eStat = item.Stats.Keys.ElementAt(i);
				int nAmount = item.Stats.Values.ElementAt(i);
				pair.Init( eStat, nAmount );
			}
			else {
				// if there isn't, destroy the gameobject
				Destroy( pair.gameObject );
			}
		}
	}
}
