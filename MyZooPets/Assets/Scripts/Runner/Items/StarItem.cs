using UnityEngine;
using System.Collections;

public class StarItem : RunnerItem {
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public override void OnPickup() {
		SpawnFloatyText("Speedy");
		GameObject.Destroy(gameObject);
		PlayerController.Instance.StartStarMode();
	}
}
