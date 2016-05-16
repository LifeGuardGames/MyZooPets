using UnityEngine;
using System.Collections;

public class MagnetItem : RunnerItem {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public override void OnPickup() {
		SpawnFloatyText("Magnet");
        GameObject.Destroy(gameObject);
		PlayerController.Instance.StartMagnet();
	}
}
