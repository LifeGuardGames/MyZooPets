using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MiniPetManager : Singleton<MiniPetManager> {
	public class StatusUpdateEventArgs : EventArgs{
		public UpdateStatuses UpdateStatus {get; set;}
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

	private bool spawnedGM = false;

	/// <summary>
	/// Gets or sets a value indicating whether is first time cleaning.
	/// Also sends out Event when value has been updated
	/// </summary>
	/// <value><c>true</c> if this instance is first time cleaning; otherwise, <c>false</c>.</value>
	/*public bool IsFirstTimeCleaning{
		get{ return DataManager.Instance.GameData.MiniPets.IsFirstTimeCleaning;}
		set{
			DataManager.Instance.GameData.MiniPets.IsFirstTimeCleaning = value;

			if(MiniPetStatusUpdate != null){
				StatusUpdateEventArgs args = new StatusUpdateEventArgs();
				args.UpdateStatus = UpdateStatuses.FirstTimeCleaning;
				MiniPetStatusUpdate(this, args);
			}
		}
	}
*/
	/*public bool IsFirstTimeTickling{
		get{ return DataManager.Instance.GameData.MiniPets.IsFirstTimeTickling;}
		set{ 
			DataManager.Instance.GameData.MiniPets.IsFirstTimeTickling = value;

			if(MiniPetStatusUpdate != null){
				StatusUpdateEventArgs args = new StatusUpdateEventArgs();
				args.UpdateStatus = UpdateStatuses.FirstTimeTickling;
				MiniPetStatusUpdate(this, args);
			}
		}
	}*/

	void OnLevelWasLoaded(){
		List<ImmutableDataMiniPet> miniPetData = DataLoaderMiniPet.GetDataList();
		if(Application.loadedLevelName ==  "ZoneYard"||Application.loadedLevelName == "ZoneBedroom"){
			foreach(ImmutableDataMiniPet data in miniPetData){
				if(data.Type == MiniPetTypes.None){
				}
				else{
						CreateMiniPet(data.ID);	// TODO TEMPORARY TAKING THIS OUT FOR TESTING
				}
			}
		}


	}

	// Use this for initialization
	void Start(){
		
			
		DontDestroyOnLoad(this.gameObject);
//		GatingManager.OnDestroyedGate += OnDestroyedGateHandler;

		//load all minipet into the scene
//		Dictionary<string, MutableDataMiniPets.Status> miniPetProgress = 
//			DataManager.Instance.GameData.MiniPets.MiniPetProgress;
		switchSpawn = CanSpawnNewMinipetLocations();
		canLevel = switchSpawn;
		/*foreach(ImmutableDataMiniPet data in miniPetData){
			if(data.Type == MiniPetTypes.None){
			}
			else{
				if(Application.loadedLevelName == "ZoneBedroom"){
				CreateMiniPet(data.ID);	// TODO TEMPORARY TAKING THIS OUT FOR TESTING
				}
			}
		}*/
	}

	/// <summary>
	/// Determines whether new minipet locations should be spawned, also flags the datetime automatically
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
	/// Checks to refresh mini pet status. Use this function to check whether 
	/// the IsTickled or IsCleaned variable of that specific MP should be reset to
	/// false. Should reset every 2 hrs (arbitrary)
	/// </summary>
	/*public void CheckToRefreshMiniPetStatus(string miniPetID){
		bool isTickled = IsTickled(miniPetID);
		bool isCleaned = IsCleaned(miniPetID);
		DateTime now = LgDateTime.GetTimeNow();

		if(isTickled && isCleaned){
			DateTime lastActionTime = DataManager.Instance.GameData.MiniPets.GetLastActionTime(miniPetID);

			//make sure last action time is not in a future time
			if(lastActionTime <= now){
				TimeSpan timeSinceLastAction = now - lastActionTime;
				
				if(timeSinceLastAction.Hours >= 2){
					DataManager.Instance.GameData.MiniPets.SetIsTickled(miniPetID, false);
					DataManager.Instance.GameData.MiniPets.SetIsCleaned(miniPetID, false);
				}
			}
		}
	}
*/
	/// <summary>
	/// Whether mp has been tickled
	/// </summary>
	/// <param name="miniPetID">Mini pet ID.</param>
	/*public bool IsTickled(string miniPetID){
		return DataManager.Instance.GameData.MiniPets.IsTickled(miniPetID);
	}
*/
	/// <summary>
	/// Sets the tickle status of the mp.
	/// </summary>
	/// <param name="miniPetID">Mini pet ID.</param>
	/// <param name="isTickled">If set to <c>true</c> mp is tickled.</param>
	/*public void SetTickle(string miniPetID, bool isTickled){
		DataManager.Instance.GameData.MiniPets.SetIsTickled(miniPetID, isTickled);

		if(isTickled){
			DataManager.Instance.GameData.MiniPets.UpdateLastActionTime(miniPetID);
		}

		if(MiniPetStatusUpdate != null){
			StatusUpdateEventArgs args = new StatusUpdateEventArgs();
			args.UpdateStatus = UpdateStatuses.Tickle;
			MiniPetStatusUpdate(this, args);
		}
	}
*/
	/// <summary>
	/// Whether mp has been cleaned
	/// </summary>
	/// <returns><c>true</c> true if mp is cleaned; otherwise, <c>false</c>.</returns>
	/// <param name="miniPetID">Mini pet ID.</param>
/*	public bool IsCleaned(string miniPetID){
		return DataManager.Instance.GameData.MiniPets.IsCleaned(miniPetID);
	}

	/// <summary>
	/// Sets the clean status of the mp.
	/// </summary>
	/// <param name="miniPetID">Mini pet ID.</param>
	/// <param name="isCleaned">If set to <c>true</c> mp is cleaned.</param>
	public void SetCleaned(string miniPetID, bool isCleaned){
		DataManager.Instance.GameData.MiniPets.SetIsCleaned(miniPetID, isCleaned);

		if(isCleaned)
			DataManager.Instance.GameData.MiniPets.UpdateLastActionTime(miniPetID);

		if(MiniPetStatusUpdate != null){
			StatusUpdateEventArgs args = new StatusUpdateEventArgs();
			args.UpdateStatus = UpdateStatuses.Clean;
			MiniPetStatusUpdate(this, args);
		}
	}
	*/
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
		canModify = (currentLevel != maxLevel) && canLevel && DataManager.Instance.GameData.MiniPetLocations.GetHunger(miniPetID);
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

		return 1;
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
		DataManager.Instance.GameData.MiniPets.IncreaseXP(miniPetID, 1, canLevel);
		//}
		StatusUpdateEventArgs args = new StatusUpdateEventArgs();
		args.UpdateStatus = UpdateStatuses.IncreaseFoodXP;

		//check current food xp with that condition
		if(1 >= 1){
			args.UpdateStatus = UpdateStatuses.LevelUp;
				canLevel = false;
		}
		
		if(MiniPetStatusUpdate != null)
			MiniPetStatusUpdate(this, args);


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
		DataManager.Instance.GameData.MiniPets.SetisHatched(miniPetID,true);
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
//		if(data.Type == MiniPetTypes.Basic){
//			ImmutableDataGate latestGate = GatingManager.Instance.GetLatestLockedGate();
//			if(latestGate.Partition - 1  == 1){
//				if(switchSpawn){
//					LgTuple<Vector3, string> locationTuple = PartitionManager.Instance.GetRandomUnusedPosition();
//					int partitionNumber  = DataLoaderPartitionLocations.GetData(locationTuple.Item2).Partition;
//					while (!PartitionManager.Instance.IsPartitionInCurrentZone(partitionNumber)){
//						locationTuple = PartitionManager.Instance.GetRandomUnusedPosition();
//						partitionNumber  = DataLoaderPartitionLocations.GetData(locationTuple.Item2).Partition;
//					}
//					Vector3 pos = locationTuple.Item1;
//					goMiniPet = GameObjectUtils.AddChild(PartitionManager.Instance.GetInteractableParent(partitionNumber).gameObject, prefab);
//					goMiniPet.transform.localPosition = pos;
//					goMiniPet.name = prefab.name;
//					goMiniPet.GetComponent<MiniPet>().Init(data);
//					// Add the pet into the dictionary to keep track
//					MiniPetTable.Add(miniPetID, goMiniPet);
//					DataManager.instance.GameData.MiniPetLocations.SaveLoc(miniPetID, goMiniPet.transform.position);
//					}
//				else{
//					goMiniPet = Instantiate(prefab,DataManager.Instance.GameData.MiniPetLocations.GetLoc(miniPetID), Quaternion.identity) as GameObject;
//					goMiniPet.name = prefab.name;
//					goMiniPet.GetComponent<MiniPet>().Init(data);
//					// Add the pet into the dictionary to keep track
//					MiniPetTable.Add(miniPetID, goMiniPet);
//					}
//				}
//		}
//		else 
		if (data.Type == MiniPetTypes.Rentention){
			Vector3 pos = PartitionManager.Instance.GetBasePositionInBedroom().Item1;
			int partitionNumber  = 0;
			if(PartitionManager.Instance.IsPartitionInCurrentZone(partitionNumber)){
			goMiniPet = GameObjectUtils.AddChild(PartitionManager.Instance.GetInteractableParent(partitionNumber).gameObject, prefab);
			goMiniPet.transform.localPosition = pos;
			goMiniPet.name = prefab.name;
			goMiniPet.GetComponent<RetentionPet>().Init(data);
			// Add the pet into the dictionary to keep track
			MiniPetTable.Add(miniPetID, goMiniPet);
			}
		}
		else if (data.Type == MiniPetTypes.GameMaster){
			if(switchSpawn){
				ImmutableDataGate latestGate = GatingManager.Instance.GetLatestLockedGate();
				if(latestGate == null || (latestGate.Partition - 1 == 1)){
					MinigameTypes type = PartitionManager.Instance.GetRandomUnlockedMinigameType();
					LgTuple<Vector3, string> locationTuple = PartitionManager.Instance.GetUnusedPositionNextToMinigame(type);
					int partitionNumber  = DataLoaderPartitionLocations.GetData(locationTuple.Item2).Partition;
					Vector3 pos = locationTuple.Item1;
						if(PartitionManager.Instance.IsPartitionInCurrentZone(partitionNumber)){
							goMiniPet = GameObjectUtils.AddChild(PartitionManager.Instance.GetInteractableParent(partitionNumber).gameObject, prefab);
							goMiniPet.transform.localPosition = pos;
							goMiniPet.name = prefab.name;
							goMiniPet.GetComponent<GameMaster>().minigameType = type;
							goMiniPet.GetComponent<MiniPet>().Init(data);
							// Add the pet into the dictionary to keep track
							MiniPetTable.Add(miniPetID, goMiniPet);
							DataManager.instance.GameData.MiniPetLocations.SaveLoc(miniPetID, goMiniPet.transform.position);
						}
				}
			else{
				if (DataManager.Instance.GameData.MiniPetLocations.GetLoc(miniPetID) != new Vector3 (0,0,0)){
					goMiniPet = Instantiate(prefab,DataManager.Instance.GameData.MiniPetLocations.GetLoc(miniPetID), Quaternion.identity) as GameObject;
					goMiniPet.name = prefab.name;
					goMiniPet.GetComponent<MiniPet>().Init(data);
					// Add the pet into the dictionary to keep track
					MiniPetTable.Add(miniPetID, goMiniPet);
					}
				}
			}
		}
		else if (data.Type == MiniPetTypes.Merchant){
			ImmutableDataGate latestGate = GatingManager.Instance.GetLatestLockedGate();
			if(latestGate == null || (latestGate.Partition - 1 == 2)){
				if(switchSpawn){
					if(UnityEngine.Random.Range (0,8) == 0){
						LgTuple<Vector3, string> locationTuple = PartitionManager.Instance.GetRandomUnusedPosition();
						int partitionNumber  = DataLoaderPartitionLocations.GetData(locationTuple.Item2).Partition;
						if(!PartitionManager.Instance.IsPartitionInCurrentZone(partitionNumber)){
						Vector3 pos = locationTuple.Item1;
						goMiniPet = GameObjectUtils.AddChild(PartitionManager.Instance.GetInteractableParent(partitionNumber).gameObject, prefab);
						goMiniPet.transform.localPosition = pos;
						goMiniPet.name = prefab.name;
						goMiniPet.GetComponent<MiniPet>().Init(data);
						// Add the pet into the dictionary to keep track
						MiniPetTable.Add(miniPetID, goMiniPet);
						DataManager.instance.GameData.MiniPetLocations.SaveLoc(miniPetID, goMiniPet.transform.position);
						}
					}
				}
				else{
					goMiniPet = Instantiate(prefab,DataManager.Instance.GameData.MiniPetLocations.GetLoc(miniPetID), Quaternion.identity) as GameObject;
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
		int currentLevelNum = (int) currentLevel;
		currentLevelNum++;
		
		return (Level) currentLevelNum;
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
