using UnityEngine;
using System.Collections;

public class GameMaster : MiniPet {

	private int timesBeatened = 0;
	private string minigameType;
	
	void Awake(){
		//temp
		timesBeatened = PlayerPrefs.GetInt("TimesBeatened");
		name = "GameMaster";
		
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
			}
		}
	}
	private void giveOutMission(){
		WellapadMissionController.Instance.UnlockTask(PickMinigameMission());
		WellapadMissionController.Instance.needMission = true;
		WellapadMissionController.Instance.AddMission(PickMinigameMission());
	}

	private string PickMinigameMission(){
		int rand = Random.Range (0,3);
		switch (minigameType){
		case "Ninja":
			return "Ninja";
			break;
		case "Memory":
			return "Memory";
			break;
		case "DoctorMatch":
			return "DoctorMatch";
			break;
		case "Shooter":
			return "Shooter";
			break;
		case "Runner":
			return "Runner";
			break;
		default:
			return "Ninja";
			break;
		}
	}
}
