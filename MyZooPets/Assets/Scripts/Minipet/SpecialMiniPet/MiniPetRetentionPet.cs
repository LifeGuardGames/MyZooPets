using UnityEngine;
using System.Collections;

public class MiniPetRetentionPet : MiniPet {

	string missionID;

	void Awake(){
		minipetType = MiniPetTypes.Retention;
	}

	public void FigureOutMissions(){
		if(DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey("TutorialPart1")){
			missionID = "TutorialPart1";
		}
		else{
			missionID = "Critical";
		}
	}

	protected override void OpenChildUI(){
		Debug.Log("Opening retention UI");
		if(!TutorialManager.Instance.IsTutorialActive()){
			Debug.Log("1");
			miniPetSpeechAI.ShowTipMsg();
			Hashtable hash = new Hashtable();
			hash[0] = missionID;
			MiniPetHUDUIManager.Instance.OpenUIMinipetType(MiniPetTypes.Retention, hash); 
		}
		
		if(!PlayPeriodLogic.Instance.IsFirstPlayPeriod()){
			Debug.Log("2");
			if(!MiniPetHUDUIManager.Instance.HasContent()){
				miniPetSpeechAI.ShowIdleMessage(MinipetType);
			}
			else if (!TutorialManager.Instance.IsTutorialActive()){
				isFinishEating = true;
				DataManager.Instance.GameData.MiniPets.SaveHunger(minipetId, isFinishEating);
			}
		}
	}

	private void TurnInMission(){
		if(isFinishEating){

			MutableDataMission mission;
			if(DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey("TutorialPart1")){
				mission = WellapadMissionController.Instance.GetMission("TutorialPart1");
			}
			else{
				mission = WellapadMissionController.Instance.GetMission("Critical");
			}
			if(mission != null && mission.RewardStatus == RewardStatuses.Unclaimed){
				// claim the reward
				MiniPetManager.Instance.IncreaseXP(minipetId);
				WellapadMissionController.Instance.ClaimReward(missionID);
				WellapadMissionController.Instance.RefreshCheck();
			}
		}
	}

	public void GiveOutMission(){
		isPetCanGainXP = true;

		WellapadMissionController.Instance.UnlockTask("Critical");
		WellapadMissionController.Instance.needMission = true;
		WellapadMissionController.Instance.AddMission("Critical");

	}
}
