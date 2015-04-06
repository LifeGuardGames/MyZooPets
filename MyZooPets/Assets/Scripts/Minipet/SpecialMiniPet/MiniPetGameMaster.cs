using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniPetGameMaster : MiniPet {

	public MinigameTypes minigameType;

	public string miniGameTaskId;

	void Awake(){
		name = "GameMaster";
	}

	protected override void OnTap(TapGesture gesture){	
		base.OnTap(gesture);
		if(!MiniPetHUDUIManager.Instance.HasContent()){
			if(isFinishEating){
				miniGameTaskId = DataManager.Instance.GameData.MiniPets.GetTask(id).MissionID;

				miniPetSpeechAI.ShowGMIdleMsg();
				Hashtable has = new Hashtable();
				has[0] = miniGameTaskId;
				MiniPetHUDUIManager.Instance.OpenUIMinipetType(MiniPetTypes.GameMaster,has); 
			}
		}
	}

	public override void FinishEating(){
		base.FinishEating();
		MiniPetManager.Instance.canLevel = true;
		isFinishEating = true; 
		miniPetSpeechAI.showChallengeMsg(minigameType);
		GiveOutMission();
	}
	
	private void TurnInMission(){
		if(isFinishEating){
			MutableDataMission mission = WellapadMissionController.Instance.GetMission(miniGameTaskId);
			if(mission != null && mission.RewardStatus == RewardStatuses.Unclaimed){
				// claim the reward
				MiniPetManager.Instance.IncreaseXP(id);
				WellapadMissionController.Instance.ClaimReward(miniGameTaskId);
				WellapadMissionController.Instance.RefreshCheck();
			}
		}
	}

	private void GiveOutMission(){
		miniGameTaskId = PickMinigameMission();
		//miniGameTaskId = "NinjaS";
		WellapadMissionController.Instance.UnlockTask(miniGameTaskId);
		//WellapadMissionController.Instance.UnlockTask("NinjaS");
		WellapadMissionController.Instance.needMission = true;
		WellapadMissionController.Instance.AddMission(miniGameTaskId);
		List<MutableDataWellapadTask> listTasks = WellapadMissionController.Instance.GetTasks(miniGameTaskId); 
		DataManager.Instance.GameData.MiniPets.SetTask(id,listTasks[0]);
		Hashtable has = new Hashtable();
		has[0] = miniGameTaskId;
		MiniPetHUDUIManager.Instance.OpenUIMinipetType(MiniPetTypes.GameMaster,has); 
		//WellapadMissionController.Instance.AddMission("NinjaS");
	}

	private string PickMinigameMission(){
		if(minigameType == MinigameTypes.TriggerNinja){
			int rand = Random.Range (0,2);
			switch(rand){
			case 0:
				return "NinjaS";
				break;
			case 1:
				return "NinjaC";
				break;
			default:
				return "NinjaC";
				break;
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
				break;
			case 1:
				return "ShooterH";
				break;
			default:
				return "ShooterS";
				break;
			}
		}
		if(minigameType == MinigameTypes.Runner){
			int rand = Random.Range (0,2);
			switch(rand){
			case 0:
				return "RunnerS";
				break;
			case 1:
				return "RunnerC";
				break;
			case 2:
				return "RunnerD";
				break;
			default:
				return "RunnerC";
				break;
			}
		}		
		else{
			return "NinjaS";
		}
	}
}
