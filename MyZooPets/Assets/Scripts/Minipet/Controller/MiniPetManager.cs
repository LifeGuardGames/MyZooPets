using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MiniPetManager : Singleton<MiniPetManager>{
	public Dictionary<string, GameObject> MiniPetTable = new Dictionary<string, GameObject>();
	private Level maxLevel = Level.Level4;
	private bool isSpawnNewLocations = false;
	private bool merchantSpawnChance;

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
//		Debug.Log("---Checking can spawn minipets " + DataManager.Instance.GameData.MiniPetLocations.LastestPlayPeriodUpdated + " " + PlayPeriodLogic.GetCurrentPlayPeriod());
		if(DataManager.Instance.GameData.MiniPetLocations.LastestPlayPeriodUpdated < PlayPeriodLogic.GetCurrentPlayPeriod()){
			DataManager.Instance.GameData.MiniPetLocations.LastestPlayPeriodUpdated = PlayPeriodLogic.GetCurrentPlayPeriod();
//			Debug.Log("----SPAWN NEW LOCATIONS?: YES");
			if(UnityEngine.Random.Range(0, 0) == 0){
				merchantSpawnChance = true;
				DataManager.Instance.GameData.MiniPetLocations.SetMerchantSpawning(merchantSpawnChance);
			}
			else{
				merchantSpawnChance = false;
				DataManager.Instance.GameData.MiniPetLocations.SetMerchantSpawning(merchantSpawnChance);
			}
			return true;
		}
		else{
//			Debug.Log("----SPAWN NEW LOCATIONS?: NO");
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
			if(SceneManager.GetActiveScene().name == SceneUtils.BEDROOM){

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
				if(!DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey("DailyInhaler")){
					retentionScript.GiveOutMission();
				}

				// Add the pet into the dictionary to keep track
				MiniPetTable.Add(miniPetId, goMiniPet);
			}
			break;

		case MiniPetTypes.GameMaster:
			// Check if mp needs new locations
			ImmutableDataGate latestGate = GatingManager.Instance.GetLatestLockedGate();

//			if(latestGate == null)
//				Debug.Log("-----Spawning GameMaster: " + (latestGate == null) + " gate null");
//			else
//				Debug.Log("-----Spawning GameMaster: " + (latestGate.AbsolutePartition - 1 >= 1) + " || latest gate " + latestGate.AbsolutePartition);

			if(latestGate == null || (latestGate.AbsolutePartition - 1 >= 1)){
				// NOTE: Besides spawning new locations, there may not be any data for a minipet when coming back to same PP, do or check
				if(isSpawnNewLocations || GetPartitionNumberForMinipet(miniPetId) == -1){
					// Calculate the MP location
					MinigameTypes type = PartitionManager.Instance.GetRandomUnlockedMinigameType();
					LgTuple<Vector3, string> gameMasterLocation = PartitionManager.Instance.GetPositionNextToMinigame(type);
					Vector3 locationPosition = gameMasterLocation.Item1;
					string locationId = gameMasterLocation.Item2;
					int partitionNumber = DataLoaderPartitionLocations.GetAbsolutePartitionNumberFromLocationId(locationId);

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
						gameMasterScript.isFinishEating = false;

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
						int partition = DataLoaderPartitionLocations.GetAbsolutePartitionNumberFromLocationId(locationId);
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
			break;

		case MiniPetTypes.Merchant:
			ImmutableDataGate latestGate2 = GatingManager.Instance.GetLatestLockedGate();

//			if(latestGate2 == null)
//				Debug.Log("-----Spawning merchant: " + (latestGate2 == null) + " gate null");
//			else
//				Debug.Log("-----Spawning merchant: " + (latestGate2.AbsolutePartition - 1 >= 2) + " || latest gate " + latestGate2.AbsolutePartition);
			if(DataManager.Instance.GameData.MiniPetLocations.IsMerchantSpawning()){
				if(latestGate2 == null || (latestGate2.AbsolutePartition - 1 >= 2)){
					// Check if mp needs new locations
					// NOTE: Besides spawning new locations, there may not be any data for a minipet when coming back to same PP, do or check
					if(isSpawnNewLocations || GetPartitionNumberForMinipet(miniPetId) == -1){
			
						// Calculate the MP location
						LgTuple<Vector3, string> merchantLocation = PartitionManager.Instance.GetRandomUnusedPosition();
						Vector3 locationPosition = merchantLocation.Item1;
						string locationId = merchantLocation.Item2;
						int partitionNumber = DataLoaderPartitionLocations.GetAbsolutePartitionNumberFromLocationId(locationId);

						// Save information for minipet
						DataManager.instance.GameData.MiniPetLocations.SaveLocationId(miniPetId, locationId);
						DataManager.Instance.GameData.MiniPets.SaveHunger(miniPetId, false);

						// Set new merchant item here only
						List<ImmutableDataMerchantItem> merchantItemsList = DataLoaderMerchantItem.GetDataList();
						int rand = UnityEngine.Random.Range(0, merchantItemsList.Count);
						DataManager.Instance.GameData.MiniPets.SetItem(miniPetId, rand);
						DataManager.Instance.GameData.MiniPets.SetItemBoughtInPP(miniPetId, false);

						// Spawn the minipet if it is in current scene
						if(PartitionManager.Instance.IsPartitionInCurrentZone(partitionNumber)){
							goMiniPet = GameObjectUtils.AddChild(PartitionManager.Instance.GetInteractableParent(partitionNumber).gameObject, prefab);
							goMiniPet.transform.localPosition = locationPosition;
							goMiniPet.name = prefab.name;

							MiniPetMerchant merchantScript = goMiniPet.GetComponent<MiniPetMerchant>();
							merchantScript.Init(data);
							merchantScript.isFinishEating = false;

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
							int partition = DataLoaderPartitionLocations.GetAbsolutePartitionNumberFromLocationId(locationId);
							Vector3 pos = DataLoaderPartitionLocations.GetOffsetFromLocationId(locationId);
							//MinigameTypes minigameType = DataLoaderPartitionLocations.GetMinigameTypeFromLocationId(locationId);
							
							goMiniPet = GameObjectUtils.AddChild(PartitionManager.Instance.GetInteractableParent(partition).gameObject, prefab);
							goMiniPet.transform.localPosition = pos;
							goMiniPet.name = prefab.name;
							
							MiniPetMerchant merchantScript = goMiniPet.GetComponent<MiniPetMerchant>();
							merchantScript.Init(data);
							
							// Add the pet into the dictionary to keep track
							MiniPetTable.Add(miniPetId, goMiniPet);
						}
					}
				}
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
		if(CanModifyXp(miniPetID)){
			int xpNeededForLevelUp = GetXpNeededForNextLevel(miniPetID);
			int currentXp = GetCurrentXp(miniPetID);
			DataManager.Instance.GameData.MiniPets.IncreaseXp(miniPetID, 1);
			
//			Debug.Log(xpNeededForLevelUp + " " + currentXp);
			
			// Check current xp with that condition
			if(currentXp >= xpNeededForLevelUp){
				Debug.Log("Leveled up!!");
				IncreaseCurrentLevelAndResetCurrentXP(miniPetID);
				GetMinipetScript(miniPetID).GainedLevel();		// Show animations/effects
				MiniPetHUDUIManager.Instance.GainedLevel();
			}
			else{	// Play gain experience animation
				GetMinipetScript(miniPetID).GainedExperience();	// Show animations/effects
				MiniPetHUDUIManager.Instance.GainedExperience();
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
		string foodID = "";
		int rand;
		if(DataManager.Instance.GameData.MiniPets.GetMiniPetFoodChoice(miniPetID) == null) {
			switch(currentLevel) {
				case Level.Level1:
					rand = UnityEngine.Random.Range(0, 4);
					foodID = "Food" + rand.ToString();
					break;
				case Level.Level2:
					rand = UnityEngine.Random.Range(4, 9);
					foodID = "Food" + rand;
					break;
				case Level.Level3:
					rand = UnityEngine.Random.Range(9, 14);
					foodID = "Food" + rand;
					break;
				default:
					rand = UnityEngine.Random.Range(0, 4);
					foodID = "Food" + rand;
					break;
			}
			DataManager.Instance.GameData.MiniPets.SetMiniPetFoodChoice(miniPetID, rand);
        }
		else {
			foodID = "Food" + DataManager.Instance.GameData.MiniPets.GetMiniPetFoodChoice(miniPetID);
        }
		return foodID;
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
			Debug.LogWarning("Null location detected");
			return -1;
		}
		return DataLoaderPartitionLocations.GetAbsolutePartitionNumberFromLocationId(locationId);
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
