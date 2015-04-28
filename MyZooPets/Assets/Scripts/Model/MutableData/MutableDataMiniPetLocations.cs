using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MutableDataMiniPetLocations {
	public class Status{
		public string PartitionLocationID {get; set;}

		public Status(){
			PartitionLocationID = "";
		}
	}

	public Dictionary<string, Status> MiniPetLoc {get; set;}
	public DateTime LastestPlayPeriodUpdated {get; set;}	// This is to check if you need to refresh dictionary coming back into scene

	public MutableDataMiniPetLocations(){
		Init();
	}

	private void Init(){
		MiniPetLoc = new Dictionary<string, Status>();
		LastestPlayPeriodUpdated = DateTime.MinValue;
	}

	public void UnlockMiniPet(string miniPetID){
		if(!string.IsNullOrEmpty(miniPetID) && !MiniPetLoc.ContainsKey(miniPetID)){
			MiniPetLoc.Add(miniPetID, new Status());
		}
	}
	
	/// <summary>
	/// Whether the minipet has been unlocked
	/// </summary>
	/// <param name="miniPetID">Mini pet ID.</param>
	public bool IsMiniPetUnlocked(string miniPetID){
		bool retVal = false;
		
		if(!string.IsNullOrEmpty(miniPetID)){
			retVal = MiniPetLoc.ContainsKey(miniPetID);
		}
		return retVal;
	}

	public void SaveLocationId(string miniPetID, string locationID){
		if(MiniPetLoc.ContainsKey(miniPetID)){
			MiniPetLoc[miniPetID].PartitionLocationID = locationID;
		}
		else{
			Debug.LogError("Bad miniPetID " + miniPetID);
		}
	}


	public string GetLocationId(string miniPetID){
		if(MiniPetLoc.ContainsKey(miniPetID)){
			return MiniPetLoc[miniPetID].PartitionLocationID;
		}
		else{
			Debug.LogError("Bad miniPetID " + miniPetID);
			return null;
		}
	}
}
