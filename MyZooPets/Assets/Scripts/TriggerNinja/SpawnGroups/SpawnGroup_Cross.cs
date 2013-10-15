using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// SpawnGroup_Cross
// Spawns two trigger that cross paths.
// NOTE: This class is very similar to the Meet
// class, but I decided not to derrive them from the
// same parent quite yet.
//---------------------------------------------------

public class SpawnGroup_Cross : SpawnGroup {
	//---------------------------------------------------
	// SpawnGroup_Cross()
	//---------------------------------------------------	
	public SpawnGroup_Cross( List<string> listObjects ) : base(listObjects){
	}
	
	//---------------------------------------------------
	// CheckCount()
	//---------------------------------------------------	
	protected override bool CheckCount( List<string> listObjects ) {
		bool bOK = listObjects.Count <= listLocations.Count && listObjects.Count == 2;
	
		return bOK;
	}
	
	//---------------------------------------------------
	// SpawnObjects()
	//---------------------------------------------------	
	protected override void SpawnObjects( List<string> listObjects ) {
		// for cross, we want to get two random locations separated by at least X distance.
		// hack central
		int nBuffer = UnityEngine.Random.Range( 3, listLocations.Count - 1 );
		int nRandom = UnityEngine.Random.Range( 0, listLocations.Count - nBuffer );
		List<float> listSpawnLocs = new List<float>();
		listSpawnLocs.Add( listLocations[nRandom] );
		
		// because i'm kind of paranoid at this point
		if ( nRandom + nBuffer >= listLocations.Count ) {
			Debug.Log("Something going wrong in spawning a Cross group.");
			return;
		}
		
		listSpawnLocs.Add( listLocations[nRandom + nBuffer] );
		
		//Debug.Log("What are these: " + listLocations[nRandom] + " and " + listLocations[nRandom+nBuffer]);
		
		// and then spawn the objects!
		SpawnObjects( listObjects, listSpawnLocs );	
	}	
	
	//---------------------------------------------------
	// SpawnObjects()
	// Spawns a list of objects at a given location.
	//---------------------------------------------------	
	protected override void SpawnObjects( List<string> listObjects, List<float> listSpawnLocs ) {
		// just in case
		if ( listObjects.Count != listSpawnLocs.Count && listObjects.Count != 2 ) {
			Debug.Log("Something wrong with Cross group spawn count");
			return;
		}
		
		// hack central, inc -- we want to specify forces so that the two objects cross paths at their highest point
		int nIndexLeft = listSpawnLocs[0] < listSpawnLocs[1] ? 0 : 1;
		int nIndexRight = listSpawnLocs[0] < listSpawnLocs[1] ? 1 : 0;
		
		// get the forces we want to apply
		int nForceY = GetRandomForce();
		int nForceX = Constants.GetConstant<int>("CrossForce");
		Vector3 vForceLeft = new Vector3( nForceX, nForceY, 0 );
		Vector3 vForceRight = new Vector3( -nForceX, nForceY, 0 );
		
		// spawn the objects from here
		SpawnObject( listObjects[nIndexLeft], listSpawnLocs[nIndexLeft], vForceLeft );
		SpawnObject( listObjects[nIndexRight], listSpawnLocs[nIndexRight], vForceRight );		
	}	
}
