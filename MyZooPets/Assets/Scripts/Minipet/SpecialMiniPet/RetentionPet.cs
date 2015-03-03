using UnityEngine;
using System.Collections;

public class RetentionPet : MiniPet {

	private int timesVisited = 0;

	void Awake(){
		//temp
		timesVisited = PlayerPrefs.GetInt("TimesVisited");
		name = "retention";

	}
	protected override void OnTap(TapGesture gesture){	
		base.OnTap(gesture);
		turnInMission();
	}
	public override void FinishEating(){
		base.FinishEating();
		MiniPetManager.Instance.canLevel = true;
		isFinishEating = true; 
		miniPetSpeechAI.ShowTipMsg();
		timesVisited++;
		PlayerPrefs.SetInt("TimesVisited", timesVisited);
		giveOutMission();
	}

	private void turnInMission(){
		if(isFinishEating){
		MiniPetManager.Instance.IncreaseXP(id);
		MutableDataMission mission = WellapadMissionController.Instance.GetMission("Critical");
		
		if(mission != null && mission.RewardStatus == RewardStatuses.Unclaimed){
			// claim the reward
			WellapadMissionController.Instance.ClaimReward("Critical");
			WellapadMissionController.Instance.RefreshCheck();
		}
	}
}
	private void giveOutMission(){
		WellapadMissionController.Instance.UnlockTask("Critical");
		WellapadMissionController.Instance.needMission = true;
		WellapadMissionController.Instance.AddMission("Critical");
	}


}
