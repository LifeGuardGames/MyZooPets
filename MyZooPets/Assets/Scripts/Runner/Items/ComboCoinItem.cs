public class ComboCoinItem : RunnerItem{
	public int coinValue = 0;       //how many coins this item is worth

	public override void OnPickup(){
		RunnerGameManager.Instance.IncrementCombo(RunnerGameManager.Instance.Combo);	//Double current combo
		SpawnFloatyText("x2");
		Destroy(gameObject);
	}
}
