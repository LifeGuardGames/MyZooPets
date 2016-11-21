using UnityEngine;
using System.Collections.Generic;

//---------------------------------------------------
// SpawnGroup_Cluster
// Spawns a bunch of triggers all nearby each other.
//---------------------------------------------------
public class SpawnGroupCluster : SpawnGroup {
	public SpawnGroupCluster(List<string> listObjects) : base(listObjects) {
	}

	protected override bool CheckCount(List<string> listObjects) {
		bool bOK = listObjects.Count <= listLocations.Count && listObjects.Count > 0;

		return bOK;
	}

	protected override void SpawnObjects(List<string> listObjects) {
		// for clusters, we want to get a random starting location between 0 and ( count - number of objects )
		int nCount = listObjects.Count;
		int nMax = listLocations.Count - nCount;

		// just in case
		if(nMax < 0) {
			Debug.LogError("Illegal index for spawning cluster group");
		}

		// get the range of spawn locations
		int nIndex = UnityEngine.Random.Range(0, nMax);
		List<float> listSpawnLocs = listLocations.GetRange(nIndex, nCount);

		// and then spawn the objects!
		SpawnObjects(listObjects, listSpawnLocs);
	}
}
