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
		Debug.Log(MiniPetHUDUIManager.Instance.HasContent());
		if(!MiniPetHUDUIManager.Instance.HasContent()){
			if(isFinishEating){
				minigameTaskId = DataManager.Instance.GameData.MiniPets.GetTask(minipetId).TaskID;
				
				miniPetSpeechAI.ShowIdleMessage(MinipetType);

				OpenGameMasterContent();
			}
		}
		else{
			OpenGameMasterContent();
		}
	}

	public override void FinishEating(){
		if(!isFinishEating){
			base.FinishEating();
			isFinishEating = true; 
			PetSpeechManager.Instance.BeQuiet();
			miniPetSpeechAI.ShowChallengeMsg(minigameType);
			minigameTaskId = PickMinigameMissionKey(minigameType);
			
			WellapadMissionController.Instance.AddTask(minigameTaskId);
			
			MutableDataWellapadTask task = WellapadMissionController.Instance.GetTask(minigameTaskId); 
			DataManager.Instance.GameData.MiniPets.SetTask(minipetId,task);

			OpenGameMasterContent();
		}
		MiniPetHUDUIManager.Instance.CheckStoreButtonPulse();
	}

	private void OpenGameMasterContent(){
		Hashtable hash = new Hashtable();
		hash[0] = minigameTaskId;
		hash[1] = minigameType.ToString();
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
			MutableDataWellapadTask task= WellapadMissionController.Instance.GetTask(minigameTaskId);
			if(task != null && task.isReward == RewardStatuses.Unclaimed){
				// Claim the reward
				MiniPetManager.Instance.IncreaseXp(minipetId);
				MiniPetGameMasterUIController gameMasterUI = (MiniPetGameMasterUIController)MiniPetHUDUIManager.Instance.SelectedMiniPetContentUIScript;
				WellapadMissionController.Instance.ClaimReward(minigameTaskId, rewardObject:gameMasterUI.GetRewardButton());
				WellapadMissionController.Instance.RefreshCheck();
				gameMasterUI.HideRewardButton();
			}
		}
	}


}
