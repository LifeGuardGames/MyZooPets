using UnityEngine;
using System.Collections;

public class StarItem : RunnerItem {
	public override void OnPickup() {
		SpawnFloatyText("Speedy");
		GameObject.Destroy(gameObject);
		PlayerController.Instance.StartStarMode();
	}
}
