using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Mutable data mini pet status.
/// </summary>
public class MutableDataMiniPetStatus{
	public Level CurrentLevel {get; set;}
	public int CurrentFoodXP {get; set;}

	public MutableDataMiniPetStatus(){
		CurrentLevel = Level.Level1;
		CurrentFoodXP = 0;
	}
}

/// <summary>
/// Mutable data mini pets.
/// </summary>
public class MutableDataMiniPets{
	/// <summary>
	/// The mini pet progress.
	/// key: MiniPetID, Value: MutableDataMiniPetStats
	/// if miniPetID is not in this dictionary then it's not unlocked yet
	/// </summary>
	public Dictionary<string, MutableDataMiniPetStatus> MiniPetProgress {get; set;} 

	public void UnlockMiniPet(string miniPetID){
		if(!MiniPetProgress.ContainsKey(miniPetID))
			MiniPetProgress.Add(miniPetID, new MutableDataMiniPetStatus());
	}

	public MutableDataMiniPets(){
		MiniPetProgress = new Dictionary<string, MutableDataMiniPetStatus>();
	}
}
