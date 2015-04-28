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

				OpenGameMasterContent();
			}
		}
	}

	public override void FinishEating(){
		if(!isFinishEating){
			base.FinishEating();
		if(minigameType == MinigameTypes.None){
				minigameType = DataManager.Instance.GameData.MiniPets.GetMinigames(minipetId);
		}
			isPetCanGainXP = true;
			isFinishEating = true; 
			miniPetSpeechAI.ShowChallengeMsg(minigameType);
			minigameTaskId = PickMinigameMissionKey(minigameType);
			
			WellapadMissionController.Instance.UnlockTask(minigameTaskId);
			WellapadMissionController.Instance.needMission = true;
			WellapadMissionController.Instance.AddMission(minigameTaskId);
			
			List<MutableDataWellapadTask> listTasks = WellapadMissionController.Instance.GetTasks(minigameTaskId); 
			DataManager.Instance.GameData.MiniPets.SetTask(minipetId,listTasks[0]);

			OpenGameMasterContent();
		}
		MiniPetHUDUIManager.Instance.CheckStoreButtonPulse();
	}

	private void OpenGameMasterContent(){
		Hashtable hash = new Hashtable();
		hash[0] = minigameTaskId;
		MiniPetHUDUIManager.Instance.OpenUIMinipetType(MiniPetTypes.GameMaster, hash, this);
	}

	private string PickMinigameMissionKey(MinigameTypes type){
		if(type == MinigameTypes.TriggerNinja){
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
		else if(type == MinigameTypes.Memory){
			return "MemoryS";
		}
		else if(type == MinigameTypes.Clinic){
			return "ClinicS";
		}
		else if(type == MinigameTypes.Shooter){
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
		else if(type == MinigameTypes.Runner){
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
			Debug.LogError("Invalid minigame type detected");
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
