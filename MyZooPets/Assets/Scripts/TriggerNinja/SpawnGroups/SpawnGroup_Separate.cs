using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// SpawnGroup_Separate
// Spawns a bunch of triggers at random, unique
// locations.
//---------------------------------------------------

public class SpawnGroup_Separate : SpawnGroup {
	//---------------------------------------------------
	// SpawnGroup_Separate()
	//---------------------------------------------------	
	public SpawnGroup_Separate( List<string> listObjects ) : base(listObjects){
	}
	
	//---------------------------------------------------
	// CheckCount()
	//---------------------------------------------------	
	protected override bool CheckCount( List<string> listObjects ) {
		bool bOK = listObjects.Count <= listLocations.Count && listObjects.Count > 0;
	
		return bOK;
	}
	
	//---------------------------------------------------
	// SpawnObjects()
	//---------------------------------------------------	
	protected override void SpawnObjects( List<string> listObjects ) {
		// get a number of random, non-repeating spawn points
		List<float> listSpawnLocs = ListUtils.GetRandomElements<float>( listLocations, listObjects.Count );
		
		// and then spawn the objects!
		SpawnObjects( listObjects, listSpawnLocs );		
	}	
}
