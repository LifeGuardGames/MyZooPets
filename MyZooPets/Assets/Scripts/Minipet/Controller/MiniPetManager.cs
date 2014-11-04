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

	/// <summary>
	/// Gets or sets a value indicating whether is first time cleaning.
	/// Also sends out Event when value has been updated
	/// </summary>
	/// <value><c>true</c> if this instance is first time cleaning; otherwise, <c>false</c>.</value>
	public bool IsFirstTimeCleaning{
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

	public bool IsFirstTimeTickling{
		get{ return DataManager.Instance.GameData.MiniPets.IsFirstTimeTickling;}
		set{ 
			DataManager.Instance.GameData.MiniPets.IsFirstTimeTickling = value;

			if(MiniPetStatusUpdate != null){
				StatusUpdateEventArgs args = new StatusUpdateEventArgs();
				args.UpdateStatus = UpdateStatuses.FirstTimeTickling;
				MiniPetStatusUpdate(this, args);
			}
		}
	}

	public bool IsFirstTimeReceivingGems{
		get{ return DataManager.Instance.GameData.MiniPets.IsFirsTimeReceivingGems;}
		set{ DataManager.Instance.GameData.MiniPets.IsFirsTimeReceivingGems = value;}
	}

	// Use this for initialization
	void Start(){
		GatingManager.OnDestroyedGate += OnDestroyedGateHandler;

		//load all minipet into the scene
//		Dictionary<string, MutableDataMiniPets.Status> miniPetProgress = 
//			DataManager.Instance.GameData.MiniPets.MiniPetProgress;

		List<ImmutableDataMiniPet> miniPetData = DataLoaderMiniPet.GetDataList();
		foreach(ImmutableDataMiniPet data in miniPetData){
			CreateMiniPet(data.ID);
		}
	}

	void OnDestroy(){
		GatingManager.OnDestroyedGate -= OnDestroyedGateHandler;
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
	public void CheckToRefreshMiniPetStatus(string miniPetID){
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

	/// <summary>
	/// Whether mp has been tickled
	/// </summary>
	/// <param name="miniPetID">Mini pet ID.</param>
	public bool IsTickled(string miniPetID){
		return DataManager.Instance.GameData.MiniPets.IsTickled(miniPetID);
	}

	/// <summary>
	/// Sets the tickle status of the mp.
	/// </summary>
	/// <param name="miniPetID">Mini pet ID.</param>
	/// <param name="isTickled">If set to <c>true</c> mp is tickled.</param>
	public void SetTickle(string miniPetID, bool isTickled){
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

	/// <summary>
	/// Whether mp has been cleaned
	/// </summary>
	/// <returns><c>true</c> true if mp is cleaned; otherwise, <c>false</c>.</returns>
	/// <param name="miniPetID">Mini pet ID.</param>
	public bool IsCleaned(string miniPetID){
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

	/// <summary>
	/// Determine whether mp can be fed. 
	/// </summary>
	/// <returns><c>true</c> if mp can be fed; otherwise, <c>false</c>.</returns>
	/// <param name="miniPetID">Mini pet ID.</param>
	public bool CanModifyFoodXP(string miniPetID){
		//make sure current level is not at max level
		bool canModify = true;
		Level currentLevel = DataManager.Instance.GameData.MiniPets.GetCurrentLevel(miniPetID);
		bool isTickled = DataManager.Instance.GameData.MiniPets.IsTickled(miniPetID);
		bool isCleaned = DataManager.Instance.GameData.MiniPets.IsCleaned(miniPetID);

		canModify = (currentLevel != maxLevel) && isTickled && isCleaned;

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
	public int GetCurrentFoodXP(string miniPetID){
		return DataManager.Instance.GameData.MiniPets.GetCurrentFoodXP(miniPetID);
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
	public void IncreaseFoodXP(string miniPetID){
		//Increase food xp                                                                         
		DataManager.Instance.GameData.MiniPets.IncreaseFoodXP(miniPetID, 1);

		int levelUpCondition = GetNextLevelUpCondition(miniPetID);

		//get current food xp
		int currentFoodXP = DataManager.Instance.GameData.MiniPets.GetCurrentFoodXP(miniPetID);

		StatusUpdateEventArgs args = new StatusUpdateEventArgs();
		args.UpdateStatus = UpdateStatuses.IncreaseFoodXP;

		//check current food xp with that condition
		if(currentFoodXP >= levelUpCondition){
			args.UpdateStatus = UpdateStatuses.LevelUp;
		}

		if(MiniPetStatusUpdate != null)
			MiniPetStatusUpdate(this, args);
	}

	/// <summary>
	/// Increases the current level and reset current food XP.
	/// </summary>
	/// <param name="miniPetID">Mini pet ID.</param>
	public void IncreaseCurrentLevelAndResetCurrentFoodXP(string miniPetID){
		if(!IsMaxLevel(miniPetID))
			DataManager.Instance.GameData.MiniPets.IncreaseCurrentLevel(miniPetID);

		DataManager.Instance.GameData.MiniPets.ResetCurrentFoodXP(miniPetID);
		
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
	/// Raises the destroyed gate handler event. Check to spawn mini pet after
	/// a gate has been destroyed
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void OnDestroyedGateHandler(object sender, DestroyedGateEventArgs args){
		string miniPetID = args.MiniPetID;

		if(!string.IsNullOrEmpty(miniPetID)){
			//unlock in data manager
			DataManager.Instance.GameData.MiniPets.UnlockMiniPet(miniPetID);

			// Play the respective minipet hatch animation
			StartCoroutine(PlayHatchCutscene(GetHatchPrefabName(miniPetID)));
			StartCoroutine(RefreshUnlockState(miniPetID));
		}
	}

	IEnumerator PlayHatchCutscene(string cutscenePrefabName){
		yield return new WaitForSeconds(2f);
		CutsceneUIManager.Instance.PlayCutscene(cutscenePrefabName);
	}

	IEnumerator RefreshUnlockState(string miniPetID){
		yield return new WaitForSeconds(3f);
		MiniPetTable[miniPetID].GetComponent<MiniPet>().RefreshUnlockState();
		yield return new WaitForSeconds(8f);
		MiniPetTable[miniPetID].GetComponent<MiniPet>().TryShowDirtyOrSadMessage();	// Show a message telling user to pay attention
	}

	/// <summary>
	/// Creates the mini pet.
	/// </summary>
	/// <param name="miniPetID">Mini pet ID.</param>
	private void CreateMiniPet(string miniPetID){
		ImmutableDataMiniPet data = DataLoaderMiniPet.GetData(miniPetID);
		GameObject prefab = Resources.Load(data.PrefabName) as GameObject;
		GameObject goMiniPet = Instantiate(prefab, data.SpawnLocation, Quaternion.identity) as GameObject;
		goMiniPet.name = prefab.name;
		goMiniPet.GetComponent<MiniPet>().Init(data);

		// Add the pet into the dictionary to keep track
		MiniPetTable.Add(miniPetID, goMiniPet);
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
