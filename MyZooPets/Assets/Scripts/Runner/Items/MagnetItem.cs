public class MagnetItem : RunnerItem{
	public override void OnPickup(){
		SpawnFloatyText("Magnet");
		PlayerController.Instance.StartMagnet();
		Destroy(gameObject);
	}
}
