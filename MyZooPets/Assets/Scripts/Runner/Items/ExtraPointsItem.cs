using UnityEngine;
using System.Collections;

public class ExtraPointsItem : RunnerItem{
	
	public override void OnPickup(){
		SpawnFloatyText();
		GameObject.Destroy(gameObject);
	}
}
