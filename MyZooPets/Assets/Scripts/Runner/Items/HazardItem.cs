public class HazardItem : RunnerItem{
	public float SlowdownDivisor = 2.0f;
	public LevelGroup.eLevelGroupID ApplicableGroup;

	void Start(){
		hazard = true;
	}

	public override void OnPickup(){
		BloodPanelManager.Instance.PlayBlood();
		// Player, sloooooowwww downnnnnnnn
		if(!PlayerController.Instance.Invincible){
			PlayerController.Instance.TriggerSlowdown(SlowdownDivisor);
			RunnerGameManager.Instance.ResetCombo();
		}
		Destroy(gameObject);
	}
}
