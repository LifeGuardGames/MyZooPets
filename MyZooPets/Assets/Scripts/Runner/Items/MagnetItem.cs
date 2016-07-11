using UnityEngine;
using System.Collections;

public class MagnetItem : RunnerItem{
	public override void OnPickup(){
		SpawnFloatyText("Magnet");
		GameObject.Destroy(gameObject);
		PlayerController.Instance.StartMagnet();
	}
}
