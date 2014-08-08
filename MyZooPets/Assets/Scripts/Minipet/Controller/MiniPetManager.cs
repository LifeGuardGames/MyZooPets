using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MiniPetManager : Singleton<MiniPetManager> {
	public class StatusUpdateEventArgs : EventArgs{
		public UpdateStatuses UpdateStatus {get; set;}
	}

	public enum UpdateStatuses{
		FirstTimeCleaning,
		Tickle,
		Clean,
		IncreaseFoodXP,
		IncreaseCurrentLevel,
		LevelUp
	}

	public static EventHandler<StatusUpdateEventArgs> MiniPetStatusUpdate;

	private Level maxLevel = Level.Level6;
	// Use this for initialization
	void Start(){
		GatingManager.OnDestroyedGate += OnDestroyedGateHandler;
		//iterate through the MiniPetProgress
		//if minipet not in MiniPetProgress then it's not unlock yet
		//if in MiniPetProgress spawn the appropriate mini pet
		Dictionary<string, MutableDataMiniPets.Status> miniPetProgress = 
			DataManager.Instance.GameData.MiniPets.MiniPetProgress;

		foreach(KeyValuePair<string, MutableDataMiniPets.Status> progress in miniPetProgress){
			string miniPetID = progress.Key;
			MutableDataMiniPets.Status miniPetStatus = progress.Value;

			CreateMiniPet(miniPetID);
		}
	}

	void OnDestroy(){
		GatingManager.OnDestroyedGate -= OnDestroyedGateHandler;
	}

	public bool IsFirstTimeCleaning(string miniPetID){
		return DataManager.Instance.GameData.MiniPets.IsFirstTimeCleaning(miniPetID);
	}

	public void SetFirstTimeCleaning(string miniPetID){
		DataManager.Instance.GameData.MiniPets.SetFirstTimeCleaning(miniPetID, false);

		if(MiniPetStatusUpdate != null){
			StatusUpdateEventArgs args = new StatusUpdateEventArgs();
			args.UpdateStatus = UpdateStatuses.FirstTimeCleaning;
			MiniPetStatusUpdate(this, args);
		}
	}

	public bool IsMaxLevel(string miniPetID){
		Level currentLevel = DataManager.Instance.GameData.MiniPets.GetCurrentLevel(miniPetID);
		return currentLevel == maxLevel;
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
	/// Raises the destroyed gate handler event. Check to spawn mini pet after
	/// a gate has been destroyed
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void OnDestroyedGateHandler(object sender, DestroyedGateEventArgs args){
		string gateID = args.DestroyedGateID;
		string miniPetID = args.MiniPetID;

		//when a gate is destroyed load the proper minipet and spawned it
		CreateMiniPet(miniPetID);

		//unlock in data manager
		DataManager.Instance.GameData.MiniPets.UnlockMiniPet(miniPetID);
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
}
