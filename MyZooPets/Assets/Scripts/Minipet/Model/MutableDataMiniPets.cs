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
		public int timesVisited {get; set;}
		public bool CanGiveMission {get; set;}
		public bool IsHatched {get; set;}
		public bool IsFinishEating {get; set;}
		public List<string> SecretMerchantSellList {get; set;}
		public ImmutableDataMerchantItem currItem;
		public MutableDataWellapadTask task {get; set;}
	
		public Status(){
			CurrentLevel = Level.Level1;
			CurrentXP = 0;
			CanGiveMission = false;
			IsHatched = false;
			IsFinishEating =  false;
			SecretMerchantSellList = new List<string>();
			currItem = null;
			timesVisited = 0;
		}
	}

	/// <summary>
	/// The mini pet progress.
	/// key: MiniPetID, Value: Status
	/// if miniPetID is not in this dictionary then it's not unlocked yet
	/// </summary>
	public Dictionary<string, Status> MiniPetProgress {get; set;}

	public MutableDataMiniPets(){
		MiniPetProgress = new Dictionary<string, Status>();
	}

	/// <summary>
	/// Unlocks the mini pet.
	/// </summary>
	public void UnlockMiniPet(string miniPetID){
		if(!string.IsNullOrEmpty(miniPetID) && !MiniPetProgress.ContainsKey(miniPetID)){
			MiniPetProgress.Add(miniPetID, new Status());
		}
	}

	/// <summary>
	/// Whether the minipet has been unlocked
	/// </summary>
	public bool IsMiniPetUnlocked(string miniPetID){
		bool retVal = false;
		if(!string.IsNullOrEmpty(miniPetID)){
			retVal = MiniPetProgress.ContainsKey(miniPetID);
		}
		return retVal;
	}

	/// <summary>
	/// Gets the current food XP.
	/// </summary>
	/// <returns>The current food XP.</returns>
	/// <param name="miniPetID">Mini pet ID.</param>
	public int GetCurrentXp(string miniPetID){
		int xp = -1;
		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];
			xp = status.CurrentXP;
		}
		else{
			Debug.LogError("Bad miniPetID " + miniPetID);
		}
		return xp;
	}

	/// <summary>
	/// Increases the XP by amount
	/// </summary>
	/// <param name="miniPetID">Mini pet ID.</param>
	/// <param name="amount">Amount.</param>
	public void IncreaseXp(string miniPetID, int amount){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			MiniPetProgress[miniPetID].CurrentXP += amount;
		}
		else{
			Debug.LogError("Bad miniPetID " + miniPetID);
		}
	}

	/// <summary>
	/// Increases the current level by one level and resets current xp
	/// </summary>
	/// <param name="miniPetID">Mini pet ID.</param>
	public void IncreaseCurrentLevelAndResetXp(string miniPetID){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			MiniPetProgress[miniPetID].CurrentLevel++;	// Increment level
			MiniPetProgress[miniPetID].CurrentXP = 0;	// Reset xp
			Analytics.Instance.MiniPetLevelUp(miniPetID, (int)MiniPetProgress[miniPetID].CurrentLevel);
		}
		else{
			Debug.LogError("Bad miniPetID " + miniPetID);
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
		else{
			Debug.LogError("Bad miniPetID " + miniPetID);
		}
		return currentLevel;
	}

	public bool CanGiveMission(string miniPetID){
		bool retVal = false;
		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];
			retVal = status.CanGiveMission;
		}
		else{
			Debug.LogError("Bad miniPetID " + miniPetID);
		}
		return retVal;
	}

	public void SetCanGiveMission(string miniPetID, bool canGiveMission){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];
			status.CanGiveMission = canGiveMission;
		}
		else{
			Debug.LogError("Bad miniPetID " + miniPetID);
		}
	}

	public void SetIsHatched(string miniPetID, bool hatched){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];
			status.IsHatched = hatched;
		}
		else{
			Debug.LogError("Bad miniPetID " + miniPetID);
		}
	}

	public bool GetHatched(string miniPetID){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			return MiniPetProgress[miniPetID].IsHatched;
		}
		else{
			Debug.LogError("Bad miniPetID " + miniPetID);
			return true;
		}
	}

	public void SaveMerchantList(List<string> merch, string miniPetID){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			MiniPetProgress[miniPetID].SecretMerchantSellList = merch;
		}
		else{
			Debug.LogError("Bad miniPetID " + miniPetID);
		}
	}

	public List<string> GetMerchantList(string miniPetID){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];
			if(status.SecretMerchantSellList.Count != 0){
				return status.SecretMerchantSellList;
			}
			else{
				return null;
			}
		}
		else{
			Debug.LogError("Bad miniPetID " + miniPetID);
			return null;
		}
	}

	public void SaveHunger(string miniPetID, bool isFinishedEating){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			MiniPetProgress[miniPetID].IsFinishEating = isFinishedEating;
		}
		else{
			Debug.LogError("Bad miniPetID " + miniPetID);
		}
	}

	public bool IsPetFinishedEating(string miniPetID){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			return MiniPetProgress[miniPetID].IsFinishEating;
		}
		else{
			Debug.LogError("Bad miniPetID " + miniPetID);
			return false;
		}
	}

	public MutableDataWellapadTask GetTask(string miniPetID){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			return MiniPetProgress[miniPetID].task;
		}
		else{
			return null;
		}
	}

	public void SetItem(string miniPetID, ImmutableDataMerchantItem item){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];
			
			status.currItem = item;
			MiniPetProgress[miniPetID] = status;
		}
	}

	public ImmutableDataMerchantItem GetItem(string miniPetID){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];
			
			return status.currItem;
		}
		else{
			return null;
		}
	}
	
	public void SetTask(string miniPetID, MutableDataWellapadTask Task){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];
			
			status.task = Task;
			MiniPetProgress[miniPetID] = status;
		}
	}

	public int GetVisits(string miniPetID){
		int _timesVisited = 0;
		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];
			
			_timesVisited = status.timesVisited;
		}

		return _timesVisited;
	}

	public void SetVisits(string miniPetID){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			Status status = MiniPetProgress[miniPetID];
			
			status.timesVisited++;
			MiniPetProgress[miniPetID] = status;
		}
	}

}
