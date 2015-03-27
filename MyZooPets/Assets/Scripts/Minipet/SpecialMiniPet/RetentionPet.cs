﻿using UnityEngine;
using System.Collections;

public class RetentionPet : MiniPet {
	public bool firstPP;
	void Awake(){
		DataManager.Instance.GameData.MiniPetLocations.UnlockMiniPet(id);
		name = "retention";
		if(PlayerPrefs.GetInt("FirstPP") == 1){
			firstPP = false;
		}
		else{
			isFinishEating = true;

		}
	}

	protected override void OnTap(TapGesture gesture){	
		base.OnTap(gesture);
		if(isFinishEating && !firstPP){
			Hashtable has = new Hashtable();
			has[0] = "Do Daily Missions";
			MiniPetHUDUIManager.Instance.OpenUIMinipetType(MiniPetTypes.Rentention,has); 
			turnInMission();
		}
		else if(MiniPetManager.Instance.CanSpawnNewMinipetLocations()){
			PlayerPrefs.SetInt("FirstPP",1);
		}
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
			MutableDataMission mission = WellapadMissionController.Instance.GetMission("Critical");
			
			if(mission != null && mission.RewardStatus == RewardStatuses.Unclaimed){
				// claim the reward
				MiniPetManager.Instance.IncreaseXP(id);
				WellapadMissionController.Instance.ClaimReward("Critical");
				WellapadMissionController.Instance.RefreshCheck();
				MiniPetManager.Instance.IncreaseXP(id);
			}
		}
	}

	private void giveOutMission(){
		WellapadMissionController.Instance.UnlockTask("Critical");
		WellapadMissionController.Instance.needMission = true;
		WellapadMissionController.Instance.AddMission("Critical");
		Hashtable has = new Hashtable();
		has[0] = "Do Daily Missions";
		MiniPetHUDUIManager.Instance.OpenUIMinipetType(MiniPetTypes.Rentention,has); 
	}
}
