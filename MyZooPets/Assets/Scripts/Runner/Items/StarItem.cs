using UnityEngine;
using System.Collections;

public class StarItem : RunnerItem{
	public override void OnPickup(){
		GameObject.Destroy(gameObject);
		PlayerController.Instance.StartStarMode();
	}
}
