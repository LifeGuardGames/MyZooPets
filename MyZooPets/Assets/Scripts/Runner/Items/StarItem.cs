public class StarItem : RunnerItem{
	public override void OnPickup(){
		SpawnFloatyText("Super!");
		PlayerController.Instance.StartStarMode();
		Destroy(gameObject);
	}
}
