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
		public int CurrentXP {get; set;}
	//	public bool IsTickled {get; set;}
	//	public bool IsCleaned {get; set;}
	//	public DateTime LastActionTime {get; set;}
		public bool CanGiveMission{get; set;}
		public bool isHatched{get; set;}

	
		public Status(){
			CurrentLevel = Level.Level1;
			CurrentXP = 0;
			CanGiveMission = false;
			isHatched = false;
			//IsTickled = false;
			//IsCleaned = false;
			//LastActionTime = LgDateTime.GetTimeNow();
		}
	}

	/// <summary>
	/// The mini pet progress.
	/// key: MiniPetID, Value: Status
	/// if miniPetID is not in this dictionary then it's not unlocked yet
	/// </summary>
	public Dictionary<string, Status> MiniPetProgress {get; set;} 
	//public bool IsFirstTimeCleaning {get; set;} //T: play cleaning tutorial
	//public bool IsFirstTimeTickling {get; set;} //T: play tickling tutorial
	public List<string> SecretMerchantSellList{get; set;}
	/// <summary>
	/// Unlocks the mini pet.
	/// </summary>
	/// <param name="miniPetID">Mini pet ID.</param>
	public void UnlockMiniPet(string miniPetID){
		if(!string.IsNullOrEmpty(miniPetID) && !MiniPetProgress.ContainsKey(miniPetID)){
			MiniPetProgress.Add(miniPetID, new Status());

		}
	}

	/// <summary>
	/// Whether the minipet has been unlocked
	/// </summary>
	/// <param name="miniPetID">Mini pet ID.</param>
	public bool IsMiniPetUnlocked(string miniPetID){
		bool retVal = false;

		if(!string.IsNullOrEmpty(miniPetID))
			retVal = MiniPetProgress.ContainsKey(miniPetID);

		return retVal;
	}

	/// <summary>
	/// Increases the food XP by amount
	/// </summary>
	/// <param name="miniPetID">Mini pet ID.</param>
	/// <param name="amount">Amount.</param>
	public void IncreaseXP(string miniPetID, int amount, bool canLevel = false){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];

			status.CurrentXP += amount;

			MiniPetProgress[miniPetID] = status;
		}
	}

	/// <summary>
	/// Gets the current food XP.
	/// </summary>
	/// <returns>The current food XP.</returns>
	/// <param name="miniPetID">Mini pet ID.</param>
	public int GetCurrentXP(string miniPetID){
		int XP = -1;

		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];
			XP = status.CurrentXP;
		}

		return XP;
	}

	/// <summary>
	/// Resets the current food XP.
	/// </summary>
	/// <param name="miniPetID">Mini pet I.</param>
	public void ResetCurrentXP(string miniPetID){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];
			status.CurrentXP = 0;

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

			Analytics.Instance.MiniPetLevelUp(miniPetID, currentLevelNum);
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
	/// Gets the last action time.
	/// </summary>
	/// <returns>The last action time.</returns>
	/// <param name="miniPetID">Mini pet I.</param>
	/*public DateTime GetLastActionTime(string miniPetID){
		DateTime retVal = LgDateTime.GetTimeNow();

		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];

			retVal = status.LastActionTime;
		}

		return retVal;
	}*/

	/// <summary>
	/// Sets the last action time. last time the tickle or clean status 
	/// </summary>
	/// <param name="miniPetID">Mini pet I.</param>
	/*public void UpdateLastActionTime(string miniPetID){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];

			status.LastActionTime = LgDateTime.GetTimeNow();
		}
	}*/

	/// <summary>
	/// Determines whether this mini pet is tickeld
	/// </summary>
	/// <param name="miniPetID">Mini pet ID.</param>
	/*public bool IsTickled(string miniPetID){
		bool retVal = false;

		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];

			retVal = status.IsTickled;
		}

		return retVal;
	}
*/
	/// <summary>
	/// Sets if MP is tickled
	/// </summary>
	/// <param name="miniPetID">Mini pet ID.</param>
	/// <param name="isTickled">If set to <c>true</c> mp is tickled.</param>
	/*public void SetIsTickled(string miniPetID, bool isTickled){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];

			status.IsTickled = isTickled;
		}
	}
*/
/*	public bool IsCleaned(string miniPetID){
		bool retVal = false;

		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];

			retVal = status.IsCleaned;
		}

		return retVal;
	}
*/
	public bool CanGiveMission(string miniPetID){
		bool retVal = false;
		
		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];
			
			retVal = status.CanGiveMission;
		}
		
		return retVal;
	}
/*
	public void SetIsCleaned(string miniPetID, bool isCleaned){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];

			status.IsCleaned = isCleaned;
		}
	}
*/
	public void SetCanGiveMission(string miniPetID, bool canGiveMission){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];
			
			status.CanGiveMission = canGiveMission;
		}
	}

	public void SetisHatched(string miniPetID, bool hatched){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];
			
			status.isHatched = hatched;
		}
	}

	public bool GetHatched(string miniPetID){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];
			
			return status.isHatched;
		}
		else{
			return true;
		}
	}

	public MutableDataMiniPets(){
		MiniPetProgress = new Dictionary<string, Status>();
	}
}
