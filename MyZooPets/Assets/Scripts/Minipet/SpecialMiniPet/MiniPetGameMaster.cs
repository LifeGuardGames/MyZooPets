using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniPetGameMaster : MiniPet{

	public MinigameTypes minigameType;
	public string minigameTaskId;

	void Awake(){
		minipetType = MiniPetTypes.GameMaster;
	}

	protected override void OpenChildUI(){
		if(!MiniPetHUDUIManager.Instance.HasContent()){
			if(isFinishEating){
				minigameTaskId = DataManager.Instance.GameData.MiniPets.GetTask(minipetId).MissionID;
				
				miniPetSpeechAI.ShowIdleMessage(MinipetType);
				Hashtable has = new Hashtable();
				has[0] = minigameTaskId;
				MiniPetHUDUIManager.Instance.OpenUIMinipetType(MiniPetTypes.GameMaster,has); 
			}
		}
	}

	public override void FinishEating(){
		if(!isFinishEating){
		base.FinishEating();
		isPetCanGainXP = true;
		isFinishEating = true; 
		miniPetSpeechAI.ShowChallengeMsg(minigameType);
		GiveOutMission();
		}
	}
	
	private void TurnInMission(){
		if(isFinishEating){
			MutableDataMission mission = WellapadMissionController.Instance.GetMission(minigameTaskId);
			if(mission != null && mission.RewardStatus == RewardStatuses.Unclaimed){
				// claim the reward
				MiniPetManager.Instance.IncreaseXp(minipetId);
				WellapadMissionController.Instance.ClaimReward(minigameTaskId);
				WellapadMissionController.Instance.RefreshCheck();
			}
		}
	}

	private void GiveOutMission(){
		minigameTaskId = PickMinigameMission();
		//miniGameTaskId = "NinjaS";
		WellapadMissionController.Instance.UnlockTask(minigameTaskId);
		//WellapadMissionController.Instance.UnlockTask("NinjaS");
		WellapadMissionController.Instance.needMission = true;
		WellapadMissionController.Instance.AddMission(minigameTaskId);
		List<MutableDataWellapadTask> listTasks = WellapadMissionController.Instance.GetTasks(minigameTaskId); 
		DataManager.Instance.GameData.MiniPets.SetTask(minipetId,listTasks[0]);
		Hashtable has = new Hashtable();
		has[0] = minigameTaskId;
		MiniPetHUDUIManager.Instance.OpenUIMinipetType(MiniPetTypes.GameMaster,has); 
		//WellapadMissionController.Instance.AddMission("NinjaS");
	}

	private string PickMinigameMission(){
		if(minigameType == MinigameTypes.TriggerNinja){
			int rand = Random.Range (0,2);
			switch(rand){
			case 0:
				return "NinjaS";
			case 1:
				return "NinjaC";
			default:
				return "NinjaC";
			}
		}

		if(minigameType == MinigameTypes.Memory){
			return "MemoryS";
		}
		if(minigameType == MinigameTypes.Clinic){
			return "ClinicS";
		}
		if(minigameType == MinigameTypes.Shooter){
			int rand = Random.Range (0,2);
			switch(rand){
			case 0:
				return "ShooterS";
			case 1:
				return "ShooterH";
			default:
				return "ShooterS";
			}
		}
		if(minigameType == MinigameTypes.Runner){
			int rand = Random.Range (0,2);
			switch(rand){
			case 0:
				return "RunnerS";
			case 1:
				return "RunnerC";
			case 2:
				return "RunnerD";
			default:
				return "RunnerC";
			}
		}		
		else{
			return "NinjaS";
		}
	}
}
