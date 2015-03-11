using UnityEngine;
using System.Collections;

public class GameMaster : MiniPet {

	private int timesBeatened = 0;
	public MinigameTypes minigameType;
	
	void Awake(){
		//temp
		timesBeatened = PlayerPrefs.GetInt("TimesBeatened");
		name = "GameMaster";
		Debug.Log(minigameType);
		
	}
	protected override void OnTap(TapGesture gesture){	
		base.OnTap(gesture);
		turnInMission();
	}
	public override void FinishEating(){
		base.FinishEating();
		MiniPetManager.Instance.canLevel = true;
		isFinishEating = true; 
		miniPetSpeechAI.ShowTipMsg();
		giveOutMission();
	}
	
	private void turnInMission(){
		if(isFinishEating){
			MiniPetManager.Instance.IncreaseXP(id);
			MutableDataMission mission = WellapadMissionController.Instance.GetMission(PickMinigameMission());
			
			if(mission != null && mission.RewardStatus == RewardStatuses.Unclaimed){
				// claim the reward
				WellapadMissionController.Instance.ClaimReward(PickMinigameMission());
				WellapadMissionController.Instance.RefreshCheck();
				timesBeatened++;
				PlayerPrefs.SetInt("TimesBeatened", timesBeatened);
				MiniPetManager.Instance.IncreaseXP(id);
			}
		}
	}
	private void giveOutMission(){
		//WellapadMissionController.Instance.UnlockTask(PickMinigameMission());
		WellapadMissionController.Instance.UnlockTask("NinjaS");
		WellapadMissionController.Instance.needMission = true;
		//WellapadMissionController.Instance.AddMission(PickMinigameMission());
		WellapadMissionController.Instance.AddMission("NinjaS");
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
		return null;
	}
}
