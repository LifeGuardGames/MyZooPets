using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MiniPetManager : Singleton<MiniPetManager>{
//	public class StatusUpdateEventArgs : EventArgs{
//		public UpdateStatuses UpdateStatus { get; set; }
//		public string MinipetID { get; set; }
//	}

//	public enum UpdateStatuses{
//		None,
//		IncreaseFoodXP,
//		IncreaseCurrentLevel,
//		LevelUp
//	}

//	public static EventHandler<StatusUpdateEventArgs> MiniPetStatusUpdate; //send event to UI when data have been updated

	public Dictionary<string, GameObject> MiniPetTable = new Dictionary<string, GameObject>();
	private Level maxLevel = Level.Level4;
	private bool isSpawnNewLocations = false;

	void Start(){
		// Load all minipet into the scene
		isSpawnNewLocations = CanSpawnNewMinipetLocations();
		List<ImmutableDataMiniPet> miniPetData = DataLoaderMiniPet.GetDataList();
		foreach(ImmutableDataMiniPet data in miniPetData){
			if(data.Type != MiniPetTypes.None){
				CreateMiniPet(data.ID);
			}
		}
	}

	#region Spawning minipet
	/// <summary>
	/// Determines whether new minipet locations should be spawned, also flags the datetime automatically
	/// Only call once per a play period all furture calls will be false
	/// </summary>
	public bool CanSpawnNewMinipetLocations(){
		if(DataManager.Instance.GameData.MiniPetLocations.LastestPlayPeriodUpdated < PlayPeriodLogic.GetCurrentPlayPeriod()){
			DataManager.Instance.GameData.MiniPetLocations.LastestPlayPeriodUpdated = PlayPeriodLogic.GetCurrentPlayPeriod();
			return true;
		}
		else{
			return false;
		}
	}

	private void CreateMiniPet(string miniPetId){
		GameObject goMiniPet = null;

		// Unlock in data manager
		DataManager.Instance.GameData.MiniPets.UnlockMiniPet(miniPetId);
		DataManager.Instance.GameData.MiniPetLocations.UnlockMiniPet(miniPetId);
		ImmutableDataMiniPet data = DataLoaderMiniPet.GetData(miniPetId);
		GameObject prefab = Resources.Load(data.PrefabName) as GameObject;

		switch(data.Type){
		case MiniPetTypes.Retention:
			// Check if mp needs new locations
			if(isSpawnNewLocations){
				DataManager.Instance.GameData.Wellapad.ResetMissions();
			}

			// Only spawn the retention pet in the bedroom
			if(Application.loadedLevelName == SceneUtils.BEDROOM){

				// Calculate the MP location
				LgTuple<Vector3, string> retentionLocation = PartitionManager.Instance.GetBasePositionInBedroom();
				Vector3 locationPosition = retentionLocation.Item1;
				string locationId = retentionLocation.Item2;
				int partitionNumber = 0;

				DataManager.Instance.GameData.MiniPets.SetIsHatched(miniPetId, true);
				DataManager.Instance.GameData.MiniPetLocations.SaveLocationId(miniPetId, locationId);	// locationId from tuple
				DataManager.Instance.GameData.MiniPets.SaveHunger(miniPetId, true);	// Set to always full

				goMiniPet = GameObjectUtils.AddChild(PartitionManager.Instance.GetInteractableParent(partitionNumber).gameObject, prefab);
				goMiniPet.transform.localPosition = locationPosition;	// vector3 from tuple
				goMiniPet.name = prefab.name;

				MiniPetRetentionPet retentionScript = goMiniPet.GetComponent<MiniPetRetentionPet>();
				retentionScript.Init(data);
				retentionScript.FigureOutMissions();
				if(!DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey("TutorialPart1")&& !DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey("Critical")){
					retentionScript.GiveOutMission();
				}

				// Add the pet into the dictionary to keep track
				MiniPetTable.Add(miniPetId, goMiniPet);
			}
			break;

		case MiniPetTypes.GameMaster:
			// Check if mp needs new locations
			ImmutableDataGate latestGate = GatingManager.Instance.GetLatestLockedGate();
			if(latestGate == null || (latestGate.Partition - 1 >= 1)){
				if(isSpawnNewLocations){
					// Calculate the MP location
					MinigameTypes type = PartitionManager.Instance.GetRandomUnlockedMinigameType();
					LgTuple<Vector3, string> gameMasterLocation = PartitionManager.Instance.GetPositionNextToMinigame(type);
					Vector3 locationPosition = gameMasterLocation.Item1;
					string locationId = gameMasterLocation.Item2;
					int partitionNumber = DataLoaderPartitionLocations.GetPartitionNumberFromLocationId(locationId);

					// Save information for minipet
					DataManager.Instance.GameData.MiniPetLocations.SaveLocationId(miniPetId, locationId);
					DataManager.Instance.GameData.MiniPets.SaveHunger(miniPetId, false);

					// Spawn the minipet if it is in current scene
					if(PartitionManager.Instance.IsPartitionInCurrentZone(partitionNumber)){
						goMiniPet = GameObjectUtils.AddChild(PartitionManager.Instance.GetInteractableParent(partitionNumber).gameObject, prefab);
						goMiniPet.transform.localPosition = locationPosition;
						goMiniPet.name = prefab.name;

						MiniPetGameMaster gameMasterScript = goMiniPet.GetComponent<MiniPetGameMaster>();
						gameMasterScript.minigameType = type;
						gameMasterScript.Init(data);
//						if(CanSpawnNewMinipetLocations()){ //TODO this needed here??? never reached
//							gameMasterScript.isFinishEating = false;
//						}

						// Add the pet into the dictionary to keep track
						MiniPetTable.Add(miniPetId, goMiniPet);
					}
				}
			// Spawn based on its saved location
			else{
				// If the saved minipet location is in the current zone
				if(PartitionManager.Instance.IsPartitionInCurrentZone(GetPartitionNumberForMinipet(miniPetId))){
					string locationId = DataManager.Instance.GameData.MiniPetLocations.GetLocationId(miniPetId);

					// Get relevant info to populate with given saved location ID
					int partition = DataLoaderPartitionLocations.GetPartitionNumberFromLocationId(locationId);
					Vector3 pos = DataLoaderPartitionLocations.GetOffsetFromLocationId(locationId);
					MinigameTypes minigameType = DataLoaderPartitionLocations.GetMinigameTypeFromLocationId(locationId);

					goMiniPet = GameObjectUtils.AddChild(PartitionManager.Instance.GetInteractableParent(partition).gameObject, prefab);
					goMiniPet.transform.localPosition = pos;
					goMiniPet.name = prefab.name;

					MiniPetGameMaster gameMasterScript = goMiniPet.GetComponent<MiniPetGameMaster>();
					gameMasterScript.minigameType = minigameType;
					gameMasterScript.Init(data);

					// Add the pet into the dictionary to keep track
					MiniPetTable.Add(miniPetId, goMiniPet);
				}
			}
		}

			// TODO sometimes it will spawn in vector3.zero, handle here
//			else if(DataManager.Instance.GameData.MiniPetLocations.GetLocationId(miniPetID) != new Vector3(0, 0, 0)){
//				if(Application.loadedLevelName ==  SceneUtils.BEDROOM){
//					if(GetPartitionNumberForMinipet(miniPetID) == 1 || GetPartitionNumberForMinipet(miniPetID) == 2){
//						goMiniPet = Instantiate(prefab, DataManager.Instance.GameData.MiniPetLocations.GetLocationId(miniPetID), Quaternion.identity) as GameObject;
//						goMiniPet.name = prefab.name;
//						goMiniPet.GetComponent<MiniPet>().Init(data);
//						// Add the pet into the dictionary to keep track
//						MiniPetTable.Add(miniPetID, goMiniPet);
//					}
//				}
//				else{
//					goMiniPet = Instantiate(prefab, DataManager.Instance.GameData.MiniPetLocations.GetLocationId(miniPetID), Quaternion.identity) as GameObject;
//					goMiniPet.name = prefab.name;
//					goMiniPet.GetComponent<MiniPet>().Init(data);
//					// Add the pet into the dictionary to keep track
//					MiniPetTable.Add(miniPetID, goMiniPet);
//				}
//			}
			break;

		case MiniPetTypes.Merchant:
			ImmutableDataGate latestGateAux = GatingManager.Instance.GetLatestLockedGate();
			if(latestGateAux == null || (latestGateAux.Partition - 1 >= 2)){
				// Check if mp needs new locations
				if(isSpawnNewLocations){
					Debug.Log("Merchant spawning new locations");
					if(UnityEngine.Random.Range(0, 1) == 0){	// TODO Change the spawn rate here
						// Calculate the MP location
						LgTuple<Vector3, string> merchantLocation = PartitionManager.Instance.GetRandomUnusedPosition();
						Vector3 locationPosition = merchantLocation.Item1;
						string locationId = merchantLocation.Item2;
						int partitionNumber = DataLoaderPartitionLocations.GetPartitionNumberFromLocationId(locationId);

						// Save information for minipet
						DataManager.instance.GameData.MiniPetLocations.SaveLocationId(miniPetId, locationId);
						DataManager.Instance.GameData.MiniPets.SaveHunger(miniPetId, false);

						// Set new merchant item here only
						List<ImmutableDataMerchantItem> merchantItemsList = DataLoaderMerchantItem.GetDataList();
						ImmutableDataMerchantItem merchantItemData = DataLoaderMerchantItem.GetData(merchantItemsList[UnityEngine.Random.Range(0, merchantItemsList.Count)].ID);
						DataManager.Instance.GameData.MiniPets.SetItem(miniPetId, merchantItemData);
						DataManager.Instance.GameData.MiniPets.SetItemBoughtInPP(miniPetId, false);

						Debug.Log(merchantItemData.ItemId);

						// Spawn the minipet if it is in current scene
						if(PartitionManager.Instance.IsPartitionInCurrentZone(partitionNumber)){
							goMiniPet = GameObjectUtils.AddChild(PartitionManager.Instance.GetInteractableParent(partitionNumber).gameObject, prefab);
							goMiniPet.transform.localPosition = locationPosition;
							goMiniPet.name = prefab.name;

							MiniPetMerchant merchantScript = goMiniPet.GetComponent<MiniPetMerchant>();
							merchantScript.Init(data);
							merchantScript.isFinishEating = false;	// TODO why no other scripts have this??

							// Add the pet into the dictionary to keep track
							MiniPetTable.Add(miniPetId, goMiniPet);
						}
					}
				}
				// Spawn based on its saved location
				else{
					// If the saved minipet location is in the current zone
					Debug.Log(miniPetId);
					if(PartitionManager.Instance.IsPartitionInCurrentZone(GetPartitionNumberForMinipet(miniPetId))){
						string locationId = DataManager.Instance.GameData.MiniPetLocations.GetLocationId(miniPetId);
						
						// Get relevant info to populate with given saved location ID
						int partition = DataLoaderPartitionLocations.GetPartitionNumberFromLocationId(locationId);
						Vector3 pos = DataLoaderPartitionLocations.GetOffsetFromLocationId(locationId);
						MinigameTypes minigameType = DataLoaderPartitionLocations.GetMinigameTypeFromLocationId(locationId);
						
						goMiniPet = GameObjectUtils.AddChild(PartitionManager.Instance.GetInteractableParent(partition).gameObject, prefab);
						goMiniPet.transform.localPosition = pos;
						goMiniPet.name = prefab.name;
						
						MiniPetMerchant merchantScript = goMiniPet.GetComponent<MiniPetMerchant>();
						merchantScript.Init(data);
						
						// Add the pet into the dictionary to keep track
						MiniPetTable.Add(miniPetId, goMiniPet);
					}
				}

				//TODO finish this same 000 bug as gamemaster
//				else if(Application.loadedLevelName == SceneUtils.BEDROOM && DataManager.Instance.GameData.MiniPetLocations.GetLocationId(miniPetID) != Vector3.zero){
//					goMiniPet = Instantiate(prefab, DataManager.Instance.GameData.MiniPetLocations.GetLocationId(miniPetID), Quaternion.identity) as GameObject;
//					goMiniPet.name = prefab.name;
//					goMiniPet.GetComponent<MiniPet>().Init(data);
//					// Add the pet into the dictionary to keep track
//					MiniPetTable.Add(miniPetID, goMiniPet);
//				}
			}
			break;

		default:
			Debug.LogError("Bad minipet type specified: " + data.Type.ToString());
			break;
		}
	}
	#endregion

	#region XP and leveling
	/// <summary>
	/// Need to check if mp can be leveled first using CanModifyFoodXP
	/// </summary>
	/// <param name="miniPetID">Mini pet ID.</param>
	public void IncreaseXp(string miniPetID){
		Debug.Log("Increasing xp called");
		if(CanModifyXp(miniPetID)){
			int xpNeededForLevelUp = GetXpNeededForNextLevel(miniPetID);
			int currentXp = GetCurrentXp(miniPetID);
			DataManager.Instance.GameData.MiniPets.IncreaseXp(miniPetID, 1);
			
			Debug.Log(xpNeededForLevelUp + " " + currentXp);
			
			// Check current xp with that condition
			if(currentXp >= xpNeededForLevelUp){
				Debug.Log("Leveled up!!");
				IncreaseCurrentLevelAndResetCurrentXP(miniPetID);
			}
		}
	}

	/// <summary>
	/// Gets the current level.
	/// </summary>
	public Level GetCurrentLevel(string miniPetID){
		return DataManager.Instance.GameData.MiniPets.GetCurrentLevel(miniPetID);
	}
	
	/// <summary>
	/// Gets the current food XP.
	/// </summary>
	private int GetCurrentXp(string miniPetID){
		return DataManager.Instance.GameData.MiniPets.GetCurrentXp(miniPetID);
	}

	/// <summary>
	/// Determine whether mp can be leveled
	/// </summary>
	private bool CanModifyXp(string miniPetID){
		// Make sure current level is not at max level
		Level currentLevel = GetCurrentLevel(miniPetID);
		bool canModify = ((int)currentLevel < (int)maxLevel);
		return canModify;
	}

	private bool IsMaxLevel(string miniPetID){
		Level currentLevel = GetCurrentLevel(miniPetID);
		return currentLevel == maxLevel;
	}

	/// <summary>
	/// Gets xp required for next level up 
	/// </summary>
	/// <returns>The next level up condition. -1 if at max level already</returns>
	private int GetXpNeededForNextLevel(string miniPetID){
		// Get the next level of minipet
		Level currentLevel = GetCurrentLevel(miniPetID);
		int currentLevelNum = (int)currentLevel;
		Level nextLevel = (Level)(currentLevelNum + 1);
		
		// Get the xp needed for next level
		int xpNeededForNextLevel = -1;
		if((int)currentLevel < (int)maxLevel){
			ImmutableDataMiniPet data = DataLoaderMiniPet.GetData(miniPetID);
			ImmutableDataMiniPetLevelUpConditions levelUpConditionData = DataLoaderLevelUpConditions.GetData(data.LevelUpConditionID);
			xpNeededForNextLevel = levelUpConditionData.GetXpNeededForLevel(nextLevel);
		}
		return xpNeededForNextLevel;
	}

	/// <summary>
	/// Increases the current level and reset current xp.
	/// </summary>
	/// <param name="miniPetID">Mini pet ID.</param>
	private void IncreaseCurrentLevelAndResetCurrentXP(string miniPetID){
		if(!IsMaxLevel(miniPetID)){
			DataManager.Instance.GameData.MiniPets.IncreaseCurrentLevelAndResetXp(miniPetID);
		}
	}
	#endregion

	public MiniPetTypes GetMinipetType(string miniPetID){
		ImmutableDataMiniPet miniPetData = DataLoaderMiniPet.GetData(miniPetID);
		return miniPetData.Type;
	}

	public bool IsPetFinishedEating(string miniPetID){
		return DataManager.Instance.GameData.MiniPets.IsPetFinishedEating(miniPetID);
	}

	/// <summary>
	/// Gets the food preference of this minipet at the current level
	/// </summary>
	public string GetFoodPreference(string miniPetID){
		Level currentLevel = GetCurrentLevel(miniPetID);
		ImmutableDataMiniPet data = DataLoaderMiniPet.GetData(miniPetID);
		ImmutableDataFoodPreferences foodPreferenceData = DataLoaderFoodPreferences.GetData(data.FoodPreferenceID);
		return foodPreferenceData.GetFoodPreference(currentLevel);;
	}

	/// <summary>
	/// Starts the hatch sequence for the requested pet
	/// </summary>
	public void StartHatchSequence(string miniPetID){
		// Play the respective minipet hatch animation
		CutsceneUIManager.Instance.PlayCutscene(GetHatchPrefabName(miniPetID));
		DataManager.Instance.GameData.MiniPets.SetIsHatched(miniPetID, true);
		StartCoroutine(RefreshUnlockState(miniPetID));
	}

	private string GetHatchPrefabName(string miniPetID){
		ImmutableDataMiniPet data = DataLoaderMiniPet.GetData(miniPetID);
		return data.CutsceneHatchPrefabName;
	}

	private IEnumerator RefreshUnlockState(string miniPetID){
		yield return new WaitForSeconds(2f);
		// Toggle the appearance of the actual pet, its been unlocked
		GetMinipetScript(miniPetID).RefreshUnlockState();
	}

	/// <summary>
	/// Enables/disables all minipet visilibity. Used for deco mode, shows or hides all pets
	/// </summary>
	public void ToggleAllMinipetVisilibity(bool isVisible){
		foreach(KeyValuePair<string, GameObject> entry in MiniPetTable){
			GameObject minipetGO = entry.Value;
			minipetGO.GetComponent<MiniPet>().ToggleVisibility(isVisible);
		}
	}

	public int GetPartitionNumberForMinipet(string minipetId){
		string locationId = DataManager.Instance.GameData.MiniPetLocations.GetLocationId(minipetId);
		if(string.IsNullOrEmpty(locationId)){
			Debug.LogError("Null location detected");
		}
		return DataLoaderPartitionLocations.GetPartitionNumberFromLocationId(locationId);
	}

	private MiniPet GetMinipetScript(string miniPetID){
		if(MiniPetTable.ContainsKey(miniPetID)){
			return MiniPetTable[miniPetID].GetComponent<MiniPet>();
		}
		else{
			Debug.LogError("Minipet table does not have ID: " + miniPetID);
			return null;
		}
	}
}
