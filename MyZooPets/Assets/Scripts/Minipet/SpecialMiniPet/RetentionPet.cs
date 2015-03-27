using UnityEngine;
using System.Collections;

public class RetentionPet : MiniPet {

	void Awake(){
		name = "retention";
	}

	protected override void OnTap(TapGesture gesture){	
		base.OnTap(gesture);
		if(!MiniPetHUDUIManager.Instance.HasContent()){
			if(isFinishEating){
				Hashtable has = new Hashtable();
				has[0] = "Do Daily Missions";
				MiniPetHUDUIManager.Instance.OpenUIMinipetType(MiniPetTypes.Rentention,has); 
				turnInMission();
			}
		}
	}
	public override void FinishEating(){
		base.FinishEating();

		MiniPetManager.Instance.canLevel = true;
		isFinishEating = true; 

		miniPetSpeechAI.ShowTipMsg();

		giveOutMission();
	}

	private void turnInMission(){
		if(isFinishEating){
			MutableDataMission mission = WellapadMissionController.Instance.GetMission("Critical");
			
			if(mission != null && mission.RewardStatus == RewardStatuses.Unclaimed){
				// claim the reward
				MiniPetManager.Instance.IncreaseXP(id);
				WellapadMissionController.Instance.ClaimReward("Critical");
				WellapadMissionController.Instance.RefreshCheck();
				MiniPetManager.Instance.IncreaseXP(id);
			}
		}
	}

	private void giveOutMission(){
		WellapadMissionController.Instance.UnlockTask("Critical");
		WellapadMissionController.Instance.needMission = true;
		WellapadMissionController.Instance.AddMission("Critical");
		Hashtable has = new Hashtable();
		has[0] = "Do Daily Missions";
		MiniPetHUDUIManager.Instance.OpenUIMinipetType(MiniPetTypes.Rentention,has); 
	}
}
