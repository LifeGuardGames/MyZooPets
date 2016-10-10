using UnityEngine;

public class ExtraCoinsItem : RunnerItem{
	public int coinValue = 0;       //how many coins this item is worth


	public override void Start(){
		base.Start();
	}
    
	public override void Update(){
		base.Update();
	}

	public override void OnPickup(){
		RunnerGameManager.Instance.AddCoins(coinValue);
		MegaHazard.Instance.IncrementHealth(coinValue);
		
		SpawnFloatyText("+" + coinValue);
        
		Destroy(gameObject);
	}
}
