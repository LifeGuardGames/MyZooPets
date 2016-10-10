using UnityEngine;
using System.Collections;

public class HazardItem : RunnerItem{

	public float SlowdownDivisor = 2.0f;
	public LevelGroup.eLevelGroupID ApplicableGroup;

	// Use this for initialization
	public override void Start(){
		base.Start();
		hazard = true;
	}

	public override void OnPickup(){
		// Player, sloooooowwww downnnnnnnn
		if(!PlayerController.Instance.Invincible){
			PlayerController.Instance.TriggerSlowdown(SlowdownDivisor, ID);
			RunnerGameManager.Instance.ResetCombo();
		}
		GameObject.Destroy(gameObject);
	}
}
