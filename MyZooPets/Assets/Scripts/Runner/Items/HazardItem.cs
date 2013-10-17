using UnityEngine;
using System.Collections;

public class HazardItem : RunnerItem {

    public float SlowdownDivisor = 2.0f;
    public LevelGroup.eLevelGroupID ApplicableGroup;

	// Use this for initialization
	public override void Start () {
        base.Start();
	}
	
	// Update is called once per frame
    public override void Update() {
        base.Update();
	}

	public override void OnPickup()
	{
		Debug.Log("Hazard Hit!");

        // Player, sloooooowwww downnnnnnnn
        PlayerRunner player = PlayerRunner.Instance;
        player.TriggerSlowdown(SlowdownDivisor);

		GameObject.Destroy(gameObject);
	}
}
