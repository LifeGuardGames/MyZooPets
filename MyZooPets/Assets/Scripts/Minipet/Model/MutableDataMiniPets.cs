using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Mutable data mini pets.
/// </summary>
public class MutableDataMiniPets{
	public class Status{
		public Level CurrentLevel {get; set;}
		public int CurrentFoodXP {get; set;}
		public bool IsTickled {get; set;}
		public bool IsCleaned {get; set;}
	
		public Status(){
			CurrentLevel = Level.Level1;
			CurrentFoodXP = 0;
			IsTickled = false;
			IsCleaned = false;
		}
	}

	/// <summary>
	/// The mini pet progress.
	/// key: MiniPetID, Value: Status
	/// if miniPetID is not in this dictionary then it's not unlocked yet
	/// </summary>
	public Dictionary<string, Status> MiniPetProgress {get; set;} 
	public bool IsFirstTimeCleaning {get; set;} //T: play cleaning tutorial
	public bool IsFirstTimeTickling {get; set;} //T: play tickling tutorial

	/// <summary>
	/// Unlocks the mini pet.
	/// </summary>
	/// <param name="miniPetID">Mini pet ID.</param>
	public void UnlockMiniPet(string miniPetID){
		if(!MiniPetProgress.ContainsKey(miniPetID))
			MiniPetProgress.Add(miniPetID, new Status());
	}

	/// <summary>
	/// Increases the food XP by amount
	/// </summary>
	/// <param name="miniPetID">Mini pet ID.</param>
	/// <param name="amount">Amount.</param>
	public void IncreaseFoodXP(string miniPetID, int amount){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];

			status.CurrentFoodXP += amount;

			MiniPetProgress[miniPetID] = status;
		}
	}

	/// <summary>
	/// Gets the current food XP.
	/// </summary>
	/// <returns>The current food XP.</returns>
	/// <param name="miniPetID">Mini pet ID.</param>
	public int GetCurrentFoodXP(string miniPetID){
		int foodXP = -1;

		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];
			foodXP = status.CurrentFoodXP;
		}

		return foodXP;
	}

	/// <summary>
	/// Resets the current food XP.
	/// </summary>
	/// <param name="miniPetID">Mini pet I.</param>
	public void ResetCurrentFoodXP(string miniPetID){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];
			status.CurrentFoodXP = 0;

			MiniPetProgress[miniPetID] = status;
		}
	}

	/// <summary>
	/// Increases the current level by one level
	/// </summary>
	/// <param name="miniPetID">Mini pet ID.</param>
	public void IncreaseCurrentLevel(string miniPetID){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];

			int currentLevelNum = (int)status.CurrentLevel;
			currentLevelNum++;

			status.CurrentLevel = (Level) currentLevelNum;
		}
	}

	/// <summary>
	/// Gets the current level.
	/// </summary>
	/// <returns>The current level.</returns>
	/// <param name="miniPetID">Mini pet ID.</param>
	public Level GetCurrentLevel(string miniPetID){
		Level currentLevel = Level.Level1;

		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];

			currentLevel = status.CurrentLevel;
		}

		return currentLevel;
	}

	/// <summary>
	/// Determines whether this mini pet is tickeld
	/// </summary>
	/// <param name="miniPetID">Mini pet ID.</param>
	public bool IsTickled(string miniPetID){
		bool retVal = false;

		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];

			retVal = status.IsTickled;
		}

		return retVal;
	}

	/// <summary>
	/// Sets if MP is tickled
	/// </summary>
	/// <param name="miniPetID">Mini pet ID.</param>
	/// <param name="isTickled">If set to <c>true</c> mp is tickled.</param>
	public void SetIsTickled(string miniPetID, bool isTickled){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];

			status.IsTickled = isTickled;
		}
	}

	public bool IsCleaned(string miniPetID){
		bool retVal = false;

		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];

			retVal = status.IsCleaned;
		}

		return retVal;
	}

	public void SetIsCleaned(string miniPetID, bool isCleaned){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];

			status.IsCleaned = isCleaned;
		}
	}

	public MutableDataMiniPets(){
		MiniPetProgress = new Dictionary<string, Status>();
		IsFirstTimeCleaning = true;
		IsFirstTimeTickling = true;
	}
}
