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
				Hashtable hash = new Hashtable();
				hash[0] = minigameTaskId;
				MiniPetHUDUIManager.Instance.OpenUIMinipetType(MiniPetTypes.GameMaster, hash, this); 
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
		MiniPetHUDUIManager.Instance.CheckStoreButtonPulse();
	}

	private void GiveOutMission(){
		minigameTaskId = PickMinigameMissionKey();

		WellapadMissionController.Instance.UnlockTask(minigameTaskId);
		WellapadMissionController.Instance.needMission = true;
		WellapadMissionController.Instance.AddMission(minigameTaskId);

		List<MutableDataWellapadTask> listTasks = WellapadMissionController.Instance.GetTasks(minigameTaskId); 
		DataManager.Instance.GameData.MiniPets.SetTask(minipetId,listTasks[0]);
		Hashtable hash = new Hashtable();
		hash[0] = minigameTaskId;
		MiniPetHUDUIManager.Instance.OpenUIMinipetType(MiniPetTypes.GameMaster, hash, this);
	}

	private string PickMinigameMissionKey(){
		if(minigameType == MinigameTypes.TriggerNinja){
			int rand = Random.Range(0,2);
			switch(rand){
			case 0:
				return "NinjaS";
			case 1:
				return "NinjaC";
			default:
				return "NinjaC";
			}
		}
		else if(minigameType == MinigameTypes.Memory){
			return "MemoryS";
		}
		else if(minigameType == MinigameTypes.Clinic){
			return "ClinicS";
		}
		else if(minigameType == MinigameTypes.Shooter){
			int rand = Random.Range(0,2);
			switch(rand){
			case 0:
				return "ShooterS";
			case 1:
				return "ShooterH";
			default:
				return "ShooterS";
			}
		}
		else if(minigameType == MinigameTypes.Runner){
			int rand = Random.Range(0,3);
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
}
