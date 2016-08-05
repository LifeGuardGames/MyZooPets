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
		public bool CanGiveMission {get; set;}
		public bool IsHatched {get; set;}
		public bool IsFinishEating {get; set;}
		public int? FoodChoice;

		public int CurrItem {get; set;}
		public bool IsMerchanItemBoughtInPP {get; set;}

		public MutableDataWellapadTask Task {get; set;}
	
		public Status(){
			CurrentLevel = Level.Level1;
			CurrentXP = 0;
			CanGiveMission = false;
			IsHatched = false;
			IsFinishEating =  false;
			CurrItem =  -1;
			IsMerchanItemBoughtInPP = false;
			FoodChoice = null;
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

	public void SaveHunger(string miniPetID, bool isFinishedEating){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			MiniPetProgress[miniPetID].IsFinishEating = isFinishedEating;
		}
		else{
			Debug.LogError("Can not find minipet in progress dictionary " + miniPetID);
		}
	}

	public bool IsPetFinishedEating(string miniPetID){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			return MiniPetProgress[miniPetID].IsFinishEating;
		}
		else{
			Debug.LogError("Can not find minipet in progress dictionary " + miniPetID);
			return false;
		}
	}

	public MutableDataWellapadTask GetTask(string miniPetID){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			return MiniPetProgress[miniPetID].Task;
		}
		else{
			Debug.LogError("Can not find minipet in progress dictionary " + miniPetID);
			return null;
		}
	}

	public void SetItem(string miniPetID, int item){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			MiniPetProgress[miniPetID].CurrItem = item;
		}
		else{
			Debug.LogError("Can not find minipet in progress dictionary " + miniPetID);
		}
	}

	public int GetItem(string miniPetID){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			return MiniPetProgress[miniPetID].CurrItem;
		}
		else{
			Debug.LogError("Can not find minipet in progress dictionary " + miniPetID);
			return -1;
		}
	}

	public void SetItemBoughtInPP(string miniPetID, bool isBought){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			MiniPetProgress[miniPetID].IsMerchanItemBoughtInPP = isBought;
		}
		else{
			Debug.LogError("Can not find minipet in progress dictionary " + miniPetID);
		}
	}

	public bool IsItemBoughtInPP(string miniPetID){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			return MiniPetProgress[miniPetID].IsMerchanItemBoughtInPP;
		}
		else{
			Debug.LogError("Can not find minipet in progress dictionary " + miniPetID);
			return false;
		}
	}
	
	public void SetTask(string miniPetID, MutableDataWellapadTask Task){
		if(MiniPetProgress.ContainsKey(miniPetID)){
			MiniPetProgress[miniPetID].Task = Task;
		}
		else{
			Debug.LogError("Can not find minipet in progress dictionary " + miniPetID);
		}
	}

	public int? GetMiniPetFoodChoice(string minipetID) {
		Status stats = MiniPetProgress[minipetID];
		return stats.FoodChoice;
	}

	public void SetMiniPetFoodChoice(string minipetID, int? foodChoice) {
		Status stats = MiniPetProgress[minipetID];
		stats.FoodChoice = foodChoice;
	}
}
