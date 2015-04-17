using UnityEngine;
using System.Collections;

public class MiniPetRetentionPet : MiniPet {

	void Awake(){
		name = "retention";
	}

	protected override void OnTap(TapGesture gesture){	
		base.OnTap(gesture);
		if(!TutorialManager.Instance.IsTutorialActive()){
		miniPetSpeechAI.ShowTipMsg();
		}
		Hashtable has = new Hashtable();
		has[0] = "Do Daily Missions";
		MiniPetHUDUIManager.Instance.OpenUIMinipetType(MiniPetTypes.Retention,has); 
		if(!PlayPeriodLogic.Instance.IsFirstPlayPeriod()){
			if(!MiniPetHUDUIManager.Instance.HasContent()){
				//if(isFinishEating){
					miniPetSpeechAI.ShowRetentionIdelMsg();
					//Hashtable has = new Hashtable();
					//has[0] = "Do Daily Missions";
					//MiniPetHUDUIManager.Instance.OpenUIMinipetType(MiniPetTypes.Retention,has); 
				//}
			}
			else if (!TutorialManager.Instance.IsTutorialActive()){
				isFinishEating = true;
				DataManager.Instance.GameData.MiniPets.SaveHunger(id, isFinishEating);
			}
		}
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

	public void GiveOutMission(){
		isFinishEating = true; 
		MiniPetManager.Instance.canLevel = true;
		WellapadMissionController.Instance.UnlockTask("Critical");
		WellapadMissionController.Instance.needMission = true;
		WellapadMissionController.Instance.AddMission("Critical");

	}
}
