using UnityEngine;
using System.Collections;

public class MiniPetRetentionPet : MiniPet {

	string missionID;
	void Awake(){
		name = "retention";
	}

	public void FigureOutMissions(){
		if(DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey("TutorialPart1")){
			missionID = "TutorialPart1";
		}
		else if (DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey("TutorialPart2")){
			missionID = "TutorialPart2";
		}
		else{
			missionID = "Critical";
		}
	}

	protected override void OnTap(TapGesture gesture){	
		base.OnTap(gesture);
		Debug.Log(TutorialManager.Instance.IsTutorialActive());
		if(!TutorialManager.Instance.IsTutorialActive()){
		miniPetSpeechAI.ShowTipMsg();
		Hashtable has = new Hashtable();
		has[0] = missionID;
		MiniPetHUDUIManager.Instance.OpenUIMinipetType(MiniPetTypes.Retention,has); 
		}
		
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

			MutableDataMission mission;
			if(DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey("TutorialPart1")){
				mission = WellapadMissionController.Instance.GetMission("TutorialPart1");
			}
			else if (DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey("TutorialPart2")){
				mission = WellapadMissionController.Instance.GetMission("TutorialPart2");
			}
			else{
				mission = WellapadMissionController.Instance.GetMission("Critical");
			}
			if(mission != null && mission.RewardStatus == RewardStatuses.Unclaimed){
				// claim the reward
				MiniPetManager.Instance.IncreaseXP(id);
				WellapadMissionController.Instance.ClaimReward(missionID);
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
