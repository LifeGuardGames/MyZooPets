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

				miniPetSpeechAI.ShowRetentionIdelMsg();

			}
			else if (!TutorialManager.Instance.IsTutorialActive()){
				isFinishEating = true;
				DataManager.Instance.GameData.MiniPets.SaveHunger(id, isFinishEating);
			}
		}
	}

	private void TurnInMission(){
		if(isFinishEating){
			string misson = "Critical";
			MutableDataMission mission;
			if(DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey("TutorialPart1")){
				mission = WellapadMissionController.Instance.GetMission("TutorialPart1");
				misson = "TutorialPart1";
			}
			else if (DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey("TutorialPart2")){
				mission = WellapadMissionController.Instance.GetMission("TutorialPart2");
				misson = "TutorialPart2";
			}
			else{
				mission = WellapadMissionController.Instance.GetMission("Critical");
				misson = "Critical";
			}
			if(mission != null && mission.RewardStatus == RewardStatuses.Unclaimed){
				// claim the reward
				MiniPetManager.Instance.IncreaseXP(id);
				WellapadMissionController.Instance.ClaimReward(misson);
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
