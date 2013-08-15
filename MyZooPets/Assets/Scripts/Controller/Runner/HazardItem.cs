using UnityEngine;
using System.Collections;

public class HazardItem : RunnerItem {

    public float SlowdownDivisor = 2.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void OnPickup()
	{
		Debug.Log("Hazard Hit!");

        // Player, sloooooowwww downnnnnnnn
        PlayerRunner player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerRunner>();
        player.TriggerSlowdown(SlowdownDivisor);

        MegaHazard megaHazard = GameObject.FindGameObjectWithTag("MegaHazard").GetComponent<MegaHazard>();
        megaHazard.TriggerPlayerSlowdown();

		GameObject.Destroy(gameObject);
	}
}
