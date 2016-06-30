using UnityEngine;
using System.Collections.Generic;

//---------------------------------------------------
// SpawnGroup_Split
// Spawns a bunch of triggers all from one location
// that then spread out.
//---------------------------------------------------
public class SpawnGroupSplit : SpawnGroup {
	public SpawnGroupSplit(List<string> listObjects) : base(listObjects) {
	}

	protected override bool CheckCount(List<string> listObjects) {
		bool bOK = listObjects.Count <= listLocations.Count && listObjects.Count > 0;

		return bOK;
	}

	protected override void SpawnObjects(List<string> listObjects) {
		// for splits, there needs to be 2 or more objects
		int nCount = listObjects.Count;
		int nMax = listLocations.Count;

		// just in case
		if(nCount < 2) {
			Debug.LogError("Illegal index for spawning split group");
		}

		// for splits we want something kind of close to the center of the screen, so trim the first and last 2
		int nIndex = UnityEngine.Random.Range(2, nMax - 2);
		float fSpawnLoc = listLocations[nIndex];

		// constants to draw from
		int nForceRangeX = 200;
		int nForceX = 0;

		// loop through all the objects to be spawned and assign them a random x and y force
		for(int i = 0; i < nCount; ++i) {
			if(NinjaManager.Instance.bonusRound)
				NinjaManager.Instance.bonusRoundEnemies++;
			// y force
			int nForceY = GetRandomForce();

			// x force -- okay, so here's the deal...if i is EVEN we want to get a random force.
			// if it's ODD, we just want to reverse the force.
			if(i % 2 == 0)
				nForceX = UnityEngine.Random.Range(-nForceRangeX, nForceRangeX);
			else
				nForceX *= -1;

			Vector3 vForce = new Vector3(nForceX, nForceY, 0);

			// spawn the object
			SpawnObject(listObjects[i], fSpawnLoc, vForce);
		}
	}
}
