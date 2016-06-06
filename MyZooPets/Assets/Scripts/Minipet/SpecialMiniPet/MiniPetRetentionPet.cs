using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MiniPetRetentionPet : MiniPet {

	string missionID;

	void Awake(){
		minipetType = MiniPetTypes.Retention;
	}

	public void FigureOutMissions(){
		if(!DataManager.Instance.GameData.Tutorial.IsTutorialPart1Done()){
			Debug.Log("tut");
			missionID = "DailyInhaler";
		}
		else{
			Debug.Log("crit");
			missionID = "Critical";
		}
	}

	protected override void ShowFoodPreferenceMessage(){
		if(!PlayPeriodLogic.Instance.IsFirstPlayPeriod()){
			base.ShowFoodPreferenceMessage();
		}
	}

	protected override void OpenChildUI(){
		if(!TutorialManager.Instance.IsTutorialActive()){
			miniPetSpeechAI.ShowTipMsg();
			Hashtable hash = new Hashtable();
			hash[0] = missionID;
			MiniPetHUDUIManager.Instance.OpenUIMinipetType(MiniPetTypes.Retention, hash, this); 
		}
		
		if(!PlayPeriodLogic.Instance.IsFirstPlayPeriod()){
			if(!MiniPetHUDUIManager.Instance.HasContent()){
				miniPetSpeechAI.ShowIdleMessage(MinipetType);
			}
			else if (!TutorialManager.Instance.IsTutorialActive()){
				isFinishEating = true;
				DataManager.Instance.GameData.MiniPets.SaveHunger(minipetId, isFinishEating);
			}
		}
	}

	public void OnTurnInButton() {
		TurnInMission();
	}

	private void TurnInMission(){
		if(isFinishEating){
			List <MutableDataWellapadTask>  mission;
			if(DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey("TutorialPart1")){
				mission = WellapadMissionController.Instance.GetTaskGroup("TutorialPart1");
			}
			else{
				mission = WellapadMissionController.Instance.GetTaskGroup("Critical");
			}
			if(mission != null && WellapadMissionController.Instance.CheckGroupReward() == RewardStatuses.Unclaimed){
				// Claim the reward
				MiniPetManager.Instance.IncreaseXp(minipetId);
				MiniPetRetentionUIController retentionUI = (MiniPetRetentionUIController)MiniPetHUDUIManager.Instance.SelectedMiniPetContentUIScript;
				WellapadMissionController.Instance.ClaimReward(missionID, rewardObject:retentionUI.GetRewardButton());
				WellapadMissionController.Instance.RefreshCheck();
				retentionUI.HideRewardButton();
			}
		}
	}

	public void GiveOutMission(){
		WellapadMissionController.Instance.AddTask("DailyInhaler");
		WellapadMissionController.Instance.AddTask("FightMonster");

	}
}
