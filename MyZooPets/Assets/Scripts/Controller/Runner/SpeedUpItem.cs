using UnityEngine;
using System.Collections;

public class SpeedUpItem : RunnerItem {
    public float SpeedBoostAmmount = 10.0f;
    public float ItemDuration = 2.5f;

	// Use this for initialization
	public override void Start () {
        base.Start();
	}
	
	// Update is called once per frame
    public override void Update() {
        base.Update();
	}

    public override void OnPickup() {
        PlayerRunner player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerRunner>();
        player.TriggerSpeedBoost(ItemDuration, SpeedBoostAmmount);
        player.TriggerInvincibility(ItemDuration);

        GameObject.Destroy(gameObject);
    }
}
