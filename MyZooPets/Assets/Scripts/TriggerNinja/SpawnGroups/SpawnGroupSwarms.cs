using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SpawnGroupSwarms : SpawnGroup {

	public SpawnGroupSwarms(List<string> listObjects) : base(listObjects){
	}
	
	protected override bool CheckCount(List<string> listObjects){
		bool bOK = listObjects.Count <= listLocations.Count && listObjects.Count > 0;
		
		return bOK;
	}
	protected override void SpawnObjects(List<string> listObjects){
		new SpawnGroupSplit(listObjects);
	}


}
