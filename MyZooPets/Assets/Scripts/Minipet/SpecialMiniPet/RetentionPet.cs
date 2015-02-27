using UnityEngine;
using System.Collections;

public class RetentionPet : MiniPet {

	private int timesVisted = 0;

	void Awake(){
		//temp
		timesVisted = PlayerPrefs.GetInt("TimesVisted");
		name = "retention";
		isFinishEating = false;
	}

	public override void FinishEating(){
		MiniPetManager.Instance.canLevel = true;
		isFinishEating = true; 
		miniPetSpeechAI.ShowTipMsg();
		timesVisted++;
		giveOutMission();
	}
/*	protected override void OnTap(TapGesture gesture){

		base.OnTap(gesture);
		turnInMission();
	}*/

	private void turnInMission(){
		if (isFinishEating){
			MutableDataMission mission = WellapadMissionController.Instance.GetMission("Ninja");
		
			if(mission != null && mission.RewardStatus == RewardStatuses.Unclaimed){
				MiniPetManager.Instance.IncreaseXP(id);
				// claim the reward
				WellapadMissionController.Instance.ClaimReward("Ninja");

				}
			WellapadMissionController.Instance.RefreshCheck();
		}
	}
	private void giveOutMission(){
		WellapadMissionController.Instance.UnlockTask("Ninja");
		MiniPetManager.Instance.needMission = true;
		WellapadMissionController.Instance.AddMission("Ninja");
	}


}
