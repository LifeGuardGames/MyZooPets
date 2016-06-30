using System.Collections.Generic;

//---------------------------------------------------
// SpawnGroup_Separate
// Spawns a bunch of triggers at random, unique
// locations.
//---------------------------------------------------
public class SpawnGroupSeparate : SpawnGroup {
	public SpawnGroupSeparate(List<string> listObjects) : base(listObjects) {
	}

	protected override bool CheckCount(List<string> listObjects) {
		bool bOK = listObjects.Count <= listLocations.Count && listObjects.Count > 0;
		return bOK;
	}

	protected override void SpawnObjects(List<string> listObjects) {
		// get a number of random, non-repeating spawn points
		List<float> listSpawnLocs = ListUtils.GetRandomElements<float>(listLocations, listObjects.Count);

		// and then spawn the objects!
		SpawnObjects(listObjects, listSpawnLocs);
	}
}
