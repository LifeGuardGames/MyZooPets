public class ExtraCoinsItem : RunnerItem{
	public int coinValue = 0;       //how many coins this item is worth

	public override void OnPickup(){
		RunnerGameManager.Instance.AddCoins(coinValue);
		MegaHazard.Instance.IncrementHealth(coinValue);
		SpawnFloatyText("+" + coinValue);
		Destroy(gameObject);
	}
}
