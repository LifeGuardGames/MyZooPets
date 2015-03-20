using UnityEngine;
using System.Collections;

public class GameMaster : MiniPet {

	public MinigameTypes minigameType;
	
	void Awake(){
		name = "GameMaster";
	}
	protected override void OnTap(TapGesture gesture){	
		base.OnTap(gesture);
		turnInMission();
		if (isFinishEating){
			Hashtable has = new Hashtable();
			has[0] = minigameType.ToString() + " game";
			MiniPetHUDUIManager.Instance.OpenUIMinipetType(MiniPetTypes.GameMaster,has); 
		}
	}
	public override void FinishEating(){
		base.FinishEating();
		MiniPetManager.Instance.canLevel = true;
		isFinishEating = true; 
		miniPetSpeechAI.showChallengeMsg(minigameType);
		giveOutMission();
	}
	
	private void turnInMission(){
		if(isFinishEating){
			MutableDataMission mission = WellapadMissionController.Instance.GetMission(PickMinigameMission());
			
			if(mission != null && mission.RewardStatus == RewardStatuses.Unclaimed){
				// claim the reward
				MiniPetManager.Instance.IncreaseXP(id);
				WellapadMissionController.Instance.ClaimReward(PickMinigameMission());
				WellapadMissionController.Instance.RefreshCheck();
				MiniPetManager.Instance.IncreaseXP(id);
			}
		}
	}
	private void giveOutMission(){
		WellapadMissionController.Instance.UnlockTask(PickMinigameMission());
		//WellapadMissionController.Instance.UnlockTask("NinjaS");
		WellapadMissionController.Instance.needMission = true;
		WellapadMissionController.Instance.AddMission(PickMinigameMission());
		Hashtable has = new Hashtable();
		has[0] = minigameType.ToString() + " game";
		MiniPetHUDUIManager.Instance.OpenUIMinipetType(MiniPetTypes.Rentention,has); 
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
