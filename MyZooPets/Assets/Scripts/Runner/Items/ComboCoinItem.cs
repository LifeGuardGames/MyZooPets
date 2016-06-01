using UnityEngine;
using System.Collections;

public class ComboCoinItem : RunnerItem {
	public int coinValue = 0; //how many coins this item is worth
	// Use this for initialization
	public override void Start() {
		base.Start();
	}
	
	// Update is called once per frame
	public override void Update() {
		base.Update();
	}
	
	public override void OnPickup() {
		ScoreManager.Instance.IncrementCombo(ScoreManager.Instance.Combo); //Double current combo
		SpawnFloatyText("X2");
		GameObject.Destroy(gameObject);
	}
}
