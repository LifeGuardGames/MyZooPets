using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MiniPetManager : Singleton<MiniPetManager>{
	public class StatusUpdateEventArgs : EventArgs{
		public UpdateStatuses UpdateStatus { get; set; }
	}

	public enum UpdateStatuses{
		None,
		FirstTimeCleaning,
		FirstTimeTickling,
		Tickle,
		Clean,
		IncreaseFoodXP,
		IncreaseCurrentLevel,
		LevelUp
	}

	public static EventHandler<StatusUpdateEventArgs> MiniPetStatusUpdate; //send event to UI when data have been updated

	public Dictionary<string, GameObject> MiniPetTable = new Dictionary<string, GameObject>();
	private Level maxLevel = Level.Level6;
	public bool canLevel = false;
	public bool switchSpawn = false;

	// Use this for initialization
	void Start(){
		// Load all minipet into the scene
//		Dictionary<string, MutableDataMiniPets.Status> miniPetProgress = 
//			DataManager.Instance.GameData.MiniPets.MiniPetProgress;
		switchSpawn = CanSpawnNewMinipetLocations();
		canLevel = switchSpawn;
		List<ImmutableDataMiniPet> miniPetData = DataLoaderMiniPet.GetDataList();
		foreach(ImmutableDataMiniPet data in miniPetData){
			if(data.Type == MiniPetTypes.None){
			}
			else{
				CreateMiniPet(data.ID);	// TODO TEMPORARY TAKING THIS OUT FOR TESTING
			}
		}
	}

	public MiniPetTypes GetMinipetType(string petID){
		ImmutableDataMiniPet minipetData = DataLoaderMiniPet.GetData(petID);
		return minipetData.Type;
	}

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

	void OnDestroy(){
//		GatingManager.OnDestroyedGate -= OnDestroyedGateHandler;
	}

	public bool IsMaxLevel(string miniPetID){
		Level currentLevel = DataManager.Instance.GameData.MiniPets.GetCurrentLevel(miniPetID);
		return currentLevel == maxLevel;
	}

	/// <summary>
	/// Determine whether mp can be fed. 
	/// </summary>
	/// <returns><c>true</c> if mp can be fed; otherwise, <c>false</c>.</returns>
	/// <param name="miniPetID">Mini pet ID.</param>
	public bool CanModifyXP(string miniPetID){
		//make sure current level is not at max level
		Level currentLevel = DataManager.Instance.GameData.MiniPets.GetCurrentLevel(miniPetID);
		//bool isTickled = DataManager.Instance.GameData.MiniPets.IsTickled(miniPetID);
		//bool isCleaned = DataManager.Instance.GameData.MiniPets.IsCleaned(miniPetID);
		bool canModify;
		//	canModify = (currentLevel != maxLevel) && isTickled && isCleaned && canLevel;
		canModify = (currentLevel != maxLevel) && canLevel && DataManager.Instance.GameData.MiniPets.GetHunger(miniPetID);
		return canModify;
	}

	/// <summary>
	/// Gets the current level.
	/// </summary>
	/// <returns>The current level.</returns>
	/// <param name="miniPetID">Mini pet ID.</param>
	public Level GetCurrentLevel(string miniPetID){
		return DataManager.Instance.GameData.MiniPets.GetCurrentLevel(miniPetID);
	}

	/// <summary>
	/// Gets the current food XP.
	/// </summary>
	/// <returns>The current food XP.</returns>
	/// <param name="miniPetID">Mini pet ID.</param>
	public int GetCurrentXP(string miniPetID){
		return DataManager.Instance.GameData.MiniPets.GetCurrentXP(miniPetID);
	}

	/// <summary>
	/// Gets the next level up condition.
	/// </summary>
	/// <returns>The next level up condition. -1 if at max level already</returns>
	/// <param name="miniPetID">Mini pet ID.</param>
	public int GetNextLevelUpCondition(string miniPetID){
		ImmutableDataMiniPet data = DataLoaderMiniPet.GetData(miniPetID);
		ImmutableDataMiniPetLevelUpConditions levelUpConditionData = 
			DataLoaderLevelUpConditions.GetData(data.LevelUpConditionID);

		Level currentLevel = DataManager.Instance.GameData.MiniPets.GetCurrentLevel(miniPetID);
		
		//get next level
		Level nextLevel = GetNextLevel(currentLevel);
		
		//get next level up condition
		int levelUpCondition = -1;
		if(currentLevel != maxLevel)
			levelUpCondition = levelUpConditionData.GetLevelUpCondition(nextLevel);

		return levelUpCondition;
	}

	/// <summary>
	/// Call this method when food has been fed to mp. Need to check if mp can be
	/// fed first using CanModifyFoodXP
	/// </summary>
	/// <param name="miniPetID">Mini pet ID.</param>
	public void IncreaseXP(string miniPetID){

		int levelUpCondition = GetNextLevelUpCondition(miniPetID);
		//get current food xp
		int currentFoodXP = DataManager.Instance.GameData.MiniPets.GetCurrentXP(miniPetID);
		//if(canLevel){
		//Increase food xp                                                                         
		DataManager.Instance.GameData.MiniPets.IncreaseXP(miniPetID, currentFoodXP, canLevel);
		//}
		StatusUpdateEventArgs args = new StatusUpdateEventArgs();
		args.UpdateStatus = UpdateStatuses.IncreaseFoodXP;

		//check current food xp with that condition
		if(currentFoodXP >= levelUpCondition){
			args.UpdateStatus = UpdateStatuses.LevelUp;
			canLevel = false;
		}
		
		if(MiniPetStatusUpdate != null){
			MiniPetStatusUpdate(this, args);
		}
	}

	/// <summary>
	/// Increases the current level and reset current food XP.
	/// </summary>
	/// <param name="miniPetID">Mini pet ID.</param>
	public void IncreaseCurrentLevelAndResetCurrentXP(string miniPetID){
		if(!IsMaxLevel(miniPetID))
			DataManager.Instance.GameData.MiniPets.IncreaseCurrentLevel(miniPetID);

		DataManager.Instance.GameData.MiniPets.ResetCurrentXP(miniPetID);
		
		StatusUpdateEventArgs args = new StatusUpdateEventArgs();
		args.UpdateStatus = UpdateStatuses.IncreaseCurrentLevel;
		
		if(MiniPetStatusUpdate != null)
			MiniPetStatusUpdate(this, args);
		
	}

	/// <summary>
	/// Gets the food preference of this minipet at the current level
	/// </summary>
	/// <returns>The food preference food ID.</returns>
	/// <param name="miniPetID">Mini pet ID.</param>
	public string GetFoodPreference(string miniPetID){
		Level currentLevel = DataManager.Instance.GameData.MiniPets.GetCurrentLevel(miniPetID);
		ImmutableDataMiniPet data = DataLoaderMiniPet.GetData(miniPetID);
		ImmutableDataFoodPreferences foodPreferenceData = DataLoaderFoodPreferences.GetData(data.FoodPreferenceID);

		string preferredFoodID = foodPreferenceData.GetFoodPreference(currentLevel);

		return preferredFoodID;
	}

	public string GetHatchPrefabName(string miniPetID){
		ImmutableDataMiniPet data = DataLoaderMiniPet.GetData(miniPetID);
		return data.CutsceneHatchPrefabName;
	}

	/// <summary>
	/// Starts the hatch sequence for the requested pet
	/// </summary>
	/// <param name="miniPetID">Mini pet ID</param>
	public void StartHatchSequence(string miniPetID){
		// Play the respective minipet hatch animation
		CutsceneUIManager.Instance.PlayCutscene(GetHatchPrefabName(miniPetID));
		DataManager.Instance.GameData.MiniPets.SetisHatched(miniPetID, true);
		StartCoroutine(RefreshUnlockState(miniPetID));
	}

	IEnumerator RefreshUnlockState(string miniPetID){
		yield return new WaitForSeconds(2f);
		// Toggle the appearance of the actualy pet, its been unlocked
		MiniPetTable[miniPetID].GetComponent<MiniPet>().RefreshUnlockState();
		yield return new WaitForSeconds(6f);
		MiniPetTable[miniPetID].GetComponent<MiniPet>().TryShowDirtyOrSadMessage();	// Show a message telling user to pay attention
	}

	/// <summary>
	/// Creates the mini pet.
	/// </summary>
	/// <param name="miniPetID">Mini pet ID.</param>
	private void CreateMiniPet(string miniPetID){
		// Unlock in data manager
		DataManager.Instance.GameData.MiniPets.UnlockMiniPet(miniPetID);
		DataManager.Instance.GameData.MiniPetLocations.UnlockMiniPet(miniPetID);
		ImmutableDataMiniPet data = DataLoaderMiniPet.GetData(miniPetID);
		GameObject goMiniPet;
		GameObject prefab = Resources.Load(data.PrefabName) as GameObject;
		if(data.Type == MiniPetTypes.Retention){
			if(Application.loadedLevelName == SceneUtils.BEDROOM){
				if(PlayPeriodLogic.Instance.IsFirstPlayPeriod()){
					DataManager.Instance.GameData.MiniPets.SaveHunger(miniPetID, true);
				}
				Vector3 pos = PartitionManager.Instance.GetBasePositionInBedroom().Item1;
				int partitionNumber = 0;
				DataManager.Instance.GameData.MiniPetLocations.SavePartition(miniPetID, partitionNumber);
				DataManager.Instance.GameData.MiniPets.SetisHatched(miniPetID, true);
				goMiniPet = GameObjectUtils.AddChild(PartitionManager.Instance.GetInteractableParent(partitionNumber).gameObject, prefab);
				goMiniPet.transform.localPosition = pos;
				goMiniPet.name = prefab.name;
				goMiniPet.GetComponent<MiniPetRetentionPet>().Init(data);
				
				// Add the pet into the dictionary to keep track
				MiniPetTable.Add(miniPetID, goMiniPet);
				if(switchSpawn){
					DataManager.Instance.GameData.MiniPets.SaveHunger(miniPetID, false);
				}
			}
		}
		else if(data.Type == MiniPetTypes.GameMaster){
			if(switchSpawn){
				ImmutableDataGate latestGate = GatingManager.Instance.GetLatestLockedGate();
				if(latestGate == null || (latestGate.Partition - 1 == 1)){
					MinigameTypes type = PartitionManager.Instance.GetRandomUnlockedMinigameType();
					LgTuple<Vector3, string> locationTuple = PartitionManager.Instance.GetUnusedPositionNextToMinigame(type);
					int partitionNumber = DataLoaderPartitionLocations.GetData(locationTuple.Item2).Partition;
					DataManager.Instance.GameData.MiniPetLocations.SavePartition(miniPetID, partitionNumber);
					Vector3 pos = locationTuple.Item1;
					if(PartitionManager.Instance.IsPartitionInCurrentZone(partitionNumber)){
						goMiniPet = GameObjectUtils.AddChild(PartitionManager.Instance.GetInteractableParent(partitionNumber).gameObject, prefab);
						goMiniPet.transform.localPosition = pos;
						goMiniPet.name = prefab.name;
						goMiniPet.GetComponent<MiniPetGameMaster>().minigameType = type;
						goMiniPet.GetComponent<MiniPet>().Init(data);
						if(CanSpawnNewMinipetLocations()){
							goMiniPet.GetComponent<MiniPetGameMaster>().isFinishEating = false;
						}
						// Add the pet into the dictionary to keep track
						MiniPetTable.Add(miniPetID, goMiniPet);
						DataManager.instance.GameData.MiniPetLocations.SaveLoc(miniPetID, goMiniPet.transform.position);
						DataManager.Instance.GameData.MiniPets.SaveHunger(miniPetID, false);
					}
					else{
						DataManager.Instance.GameData.MiniPetLocations.SaveLoc(miniPetID, pos);
					}

				}
			}
			else if(Application.loadedLevelName == SceneUtils.YARD){
				if(DataManager.Instance.GameData.MiniPetLocations.GetPartition(miniPetID) == 5 || DataManager.Instance.GameData.MiniPetLocations.GetPartition(miniPetID) == 6){
					Vector3 pos = DataManager.Instance.GameData.MiniPetLocations.GetLoc(miniPetID);
					if(DataManager.Instance.GameData.MiniPetLocations.GetPartition(miniPetID) == 5){
						goMiniPet = GameObjectUtils.AddChild(PartitionManager.Instance.GetInteractableParent(5).gameObject, prefab);
						goMiniPet.transform.localPosition = pos;
						goMiniPet.GetComponent<MiniPetGameMaster>().minigameType = MinigameTypes.Runner;
						goMiniPet.name = prefab.name;
					}
					else{
						goMiniPet = GameObjectUtils.AddChild(PartitionManager.Instance.GetInteractableParent(6).gameObject, prefab);
						goMiniPet.transform.localPosition = pos;
						goMiniPet.GetComponent<MiniPetGameMaster>().minigameType = MinigameTypes.Shooter;
						goMiniPet.name = prefab.name;
					}
					goMiniPet.GetComponent<MiniPet>().Init(data);
					if(switchSpawn){
						goMiniPet.GetComponent<MiniPetGameMaster>().isFinishEating = false;
					}
					MiniPetTable.Add(miniPetID, goMiniPet);
					
					// Add the pet into the dictionary to keep track
				}
			}
			else if(DataManager.Instance.GameData.MiniPetLocations.GetLoc(miniPetID) != new Vector3(0, 0, 0)){
				if(Application.loadedLevelName ==  SceneUtils.BEDROOM){
					if(DataManager.Instance.GameData.MiniPetLocations.GetPartition(miniPetID) == 1 || DataManager.Instance.GameData.MiniPetLocations.GetPartition(miniPetID) == 2){
						goMiniPet = Instantiate(prefab, DataManager.Instance.GameData.MiniPetLocations.GetLoc(miniPetID), Quaternion.identity) as GameObject;
						goMiniPet.name = prefab.name;
						goMiniPet.GetComponent<MiniPet>().Init(data);
						// Add the pet into the dictionary to keep track
						MiniPetTable.Add(miniPetID, goMiniPet);
					}
				}
				else {
					goMiniPet = Instantiate(prefab, DataManager.Instance.GameData.MiniPetLocations.GetLoc(miniPetID), Quaternion.identity) as GameObject;
					goMiniPet.name = prefab.name;
					goMiniPet.GetComponent<MiniPet>().Init(data);
					// Add the pet into the dictionary to keep track
					MiniPetTable.Add(miniPetID, goMiniPet);
				}
			}
		}
		else if(data.Type == MiniPetTypes.Merchant){
			ImmutableDataGate latestGate = GatingManager.Instance.GetLatestLockedGate();
			if(latestGate == null || (latestGate.Partition - 1 == 2)){
				if(switchSpawn){
					if(Application.loadedLevelName == SceneUtils.BEDROOM){
						if(UnityEngine.Random.Range(0, 1) == 0){	// TODO Change the spawn rate here
							LgTuple<Vector3, string> locationTuple = PartitionManager.Instance.GetRandomUnusedPosition();
							int partitionNumber = DataLoaderPartitionLocations.GetData(locationTuple.Item2).Partition;
							DataManager.Instance.GameData.MiniPetLocations.SavePartition(miniPetID, partitionNumber);
							while(!PartitionManager.Instance.IsPartitionInCurrentZone(partitionNumber)){
								locationTuple = PartitionManager.Instance.GetRandomUnusedPosition();
								partitionNumber = DataLoaderPartitionLocations.GetData(locationTuple.Item2).Partition;
							}
							Vector3 pos = locationTuple.Item1;
							goMiniPet = GameObjectUtils.AddChild(PartitionManager.Instance.GetInteractableParent(partitionNumber).gameObject, prefab);
							goMiniPet.transform.localPosition = pos;
							goMiniPet.name = prefab.name;
							goMiniPet.GetComponent<MiniPet>().Init(data);
							goMiniPet.GetComponent<MiniPetMerchant>().isFinishEating = false;
							DataManager.Instance.GameData.MiniPets.SaveHunger(miniPetID, false);
							// Add the pet into the dictionary to keep track
							MiniPetTable.Add(miniPetID, goMiniPet);
							DataManager.instance.GameData.MiniPetLocations.SaveLoc(miniPetID, goMiniPet.transform.position);
						}
					}
				}
				else if(Application.loadedLevelName == SceneUtils.BEDROOM ){
					goMiniPet = Instantiate(prefab, DataManager.Instance.GameData.MiniPetLocations.GetLoc(miniPetID), Quaternion.identity) as GameObject;
					goMiniPet.name = prefab.name;
					goMiniPet.GetComponent<MiniPet>().Init(data);
					// Add the pet into the dictionary to keep track
					MiniPetTable.Add(miniPetID, goMiniPet);
				}
			}
		}
	}

	/// <summary>
	/// Gets the next level.
	/// </summary>
	/// <returns>The next level.</returns>
	/// <param name="currentLevel">Current level.</param>
	private Level GetNextLevel(Level currentLevel){
		int currentLevelNum = (int)currentLevel;
		currentLevelNum++;
		
		return (Level)currentLevelNum;
	}

	/// <summary>
	/// Enables all minipet visilibity.
	/// This is used for deco mode, shows all pets
	/// </summary>
	public void EnableAllMinipetVisilibity(){
		foreach(KeyValuePair<string, GameObject> entry in MiniPetTable){
			GameObject minipetGO = entry.Value;
			minipetGO.GetComponent<MiniPet>().ToggleVisibility(true);
		}
	}

	/// <summary>
	/// Disables all minipet visibility.
	/// This is used for deco mode, hides all pets
	/// </summary>
	public void DisableAllMinipetVisibility(){
		foreach(KeyValuePair<string, GameObject> entry in MiniPetTable){
			GameObject minipetGO = entry.Value;
			minipetGO.GetComponent<MiniPet>().ToggleVisibility(false);
		}
	}
}
