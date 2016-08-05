using UnityEngine;
using System;
using System.Collections;

public class MiniPetGameMaster : MiniPet {

	public MinigameTypes minigameType;
	public string minigameTaskId;
	public GameObject notifier;

	void Awake() {
		minipetType = MiniPetTypes.GameMaster;
	}

	protected override void OpenChildUI() {
		Debug.Log(MiniPetHUDUIManager.Instance.HasContent());
		if(!MiniPetHUDUIManager.Instance.HasContent()) {
			if(isFinishEating) {
				minigameTaskId = DataManager.Instance.GameData.MiniPets.GetTask(minipetId).TaskID;

				miniPetSpeechAI.ShowIdleMessage(MinipetType);

				OpenGameMasterContent();
			}
		}
		else {
			OpenGameMasterContent();
		}
	}

	public override void FinishEating() {
		if(!isFinishEating) {
			base.FinishEating();
			isFinishEating = true;
			PetSpeechManager.Instance.BeQuiet();
			miniPetSpeechAI.ShowChallengeMsg(minigameType);
			minigameTaskId = PickMinigameMissionKey(minigameType);

			WellapadMissionController.Instance.AddTask(minigameTaskId);

			MutableDataWellapadTask task = WellapadMissionController.Instance.GetTask(minigameTaskId);
			DataManager.Instance.GameData.MiniPets.SetTask(minipetId, task);

			OpenGameMasterContent();
		}
		MiniPetHUDUIManager.Instance.CheckStoreButtonPulse();
	}

	private void OpenGameMasterContent() {
		Hashtable hash = new Hashtable();
		hash[0] = minigameTaskId;
		hash[1] = minigameType.ToString();
		MiniPetHUDUIManager.Instance.OpenUIMinipetType(MiniPetTypes.GameMaster, hash, this);
	}

	private string PickMinigameMissionKey(MinigameTypes type) {
		if(type == MinigameTypes.TriggerNinja) {
			int rand = UnityEngine.Random.Range(0, 2);
			switch(rand) {
				case 0:
					return "ScoreNinja";
				case 1:
					return "ComboNinja";
				default:
					return "ComboNinja";
			}
		}
		else if(type == MinigameTypes.Memory) {
			return "ScoreMemory";
		}
		else if(type == MinigameTypes.Clinic) {
			return "ScoreClinic";
		}
		else if(type == MinigameTypes.Shooter) {
			int rand = UnityEngine.Random.Range(0, 2);
			switch(rand) {
				case 0:
					return "ScoreShooter";
				case 1:
					return "SurvivalShooter";
				default:
					return "ScoreShooter";
			}
		}
		else if(type == MinigameTypes.Runner) {
			int rand = UnityEngine.Random.Range(0, 3);
			switch(rand) {
				case 0:
					return "ScoreRunner";
				case 1:
					return "CoinsRunner";
				case 2:
					return "DistanceRunner";
				default:
					return "CoinsRunner";
			}
		}
		else {
			Debug.LogError("Invalid minigame type detected");
			return "ScoreNinja";
		}
	}
	public void OnTurnInButton() {
		TurnInMission();
	}

	private void TurnInMission() {
		if(isFinishEating) {
			MutableDataWellapadTask task = WellapadMissionController.Instance.GetTask(minigameTaskId);
			if(task != null && task.isReward == RewardStatuses.Unclaimed) {
				// Claim the reward
				MiniPetManager.Instance.IncreaseXp(minipetId);
				MiniPetGameMasterUIController gameMasterUI = (MiniPetGameMasterUIController)MiniPetHUDUIManager.Instance.SelectedMiniPetContentUIScript;
				WellapadMissionController.Instance.ClaimReward(minigameTaskId, rewardObject: gameMasterUI.GetRewardButton());
				WellapadMissionController.Instance.RefreshCheck();
				gameMasterUI.HideRewardButton();
			}
		}
	}
	void OnBecameVisible() {
		MutableDataWellapadTask task = WellapadMissionController.Instance.GetTask(minigameTaskId);
		if(task != null && task.isReward == RewardStatuses.Unclaimed) {
			notifier.SetActive(true);
		}
	}

	public void OnItemDropped(InventoryItem itemData) {
		throw new NotImplementedException();
	}
}
