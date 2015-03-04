﻿using UnityEngine;
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
		WellapadMissionController.Instance.UnlockTask("Ninja");
		WellapadMissionController.Instance.needMission = true;
		//WellapadMissionController.Instance.AddMission(PickMinigameMission());
		WellapadMissionController.Instance.AddMission("Ninja");
	}

	private string PickMinigameMission(){
		int rand = Random.Range (0,3);
		switch (minigameType){
		case MinigameTypes.TriggerNinja:
			return "Ninja";
			break;
		case MinigameTypes.Memory:
			return "Memory";
			break;
		case MinigameTypes.Clinic:
			return "Clinic";
			break;
		case MinigameTypes.Shooter:
			return "Shooter";
			break;
		case MinigameTypes.Runner:
			return "Runner";
			break;
		default:
			return "Ninja";
			break;
		}
	}
}
