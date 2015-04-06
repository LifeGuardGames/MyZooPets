using UnityEngine;
using System.Collections;

public class MiniPetRetentionPet : MiniPet {

	void Awake(){
		name = "retention";
	}

	protected override void OnTap(TapGesture gesture){	
		base.OnTap(gesture);
		if(!PlayPeriodLogic.Instance.IsFirstPlayPeriod()){
			if(!MiniPetHUDUIManager.Instance.HasContent()){
				if(isFinishEating){
					miniPetSpeechAI.ShowRetentionIdelMsg();
					Hashtable has = new Hashtable();
					has[0] = "Do Daily Missions";
					MiniPetHUDUIManager.Instance.OpenUIMinipetType(MiniPetTypes.Rentention,has); 
				}
			}
			else if (!TutorialManager.Instance.IsTutorialActive()){
				isFinishEating = true;
				DataManager.Instance.GameData.MiniPets.SaveHunger(id, isFinishEating);
			}
		}
	}

	public override void FinishEating(){
		base.FinishEating();

		MiniPetManager.Instance.canLevel = true;
		isFinishEating = true; 

		miniPetSpeechAI.ShowTipMsg();

		GiveOutMission();
	}

	private void TurnInMission(){
		if(isFinishEating){
			MutableDataMission mission = WellapadMissionController.Instance.GetMission("Critical");
			
			if(mission != null && mission.RewardStatus == RewardStatuses.Unclaimed){
				// claim the reward
				MiniPetManager.Instance.IncreaseXP(id);
				WellapadMissionController.Instance.ClaimReward("Critical");
				WellapadMissionController.Instance.RefreshCheck();
			}
		}
	}

	private void GiveOutMission(){
		WellapadMissionController.Instance.UnlockTask("Critical");
		WellapadMissionController.Instance.needMission = true;
		WellapadMissionController.Instance.AddMission("Critical");
		Hashtable has = new Hashtable();
		has[0] = "Do Daily Missions";
		MiniPetHUDUIManager.Instance.OpenUIMinipetType(MiniPetTypes.Rentention,has); 
	}
}
