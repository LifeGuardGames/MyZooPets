using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MutableDataMiniPetLocations {
	public class Status{
		public string Loc{get; set;}
		public bool IsFinishEating {get; set;}

		public Status(){
			Loc = new Vector3(0,0,0).ToString();
			IsFinishEating =  false;
		}
	}

	public Dictionary<string, Status> MiniPetLoc {get; set;}
	public DateTime LastestPlayPeriodUpdated {get; set;}	// This is to check if you need to refresh dictionary coming back into scene

	public MutableDataMiniPetLocations(){
		Init();
	}

	private void Init(){
		Debug.Log("aksbfviakbs");
		MiniPetLoc = new Dictionary<string, Status>();
		LastestPlayPeriodUpdated = DateTime.MinValue;
	}

	public void UnlockMiniPet(string miniPetID){
		Debug.Log(MiniPetLoc.ContainsKey(miniPetID));
		if(!string.IsNullOrEmpty(miniPetID) && !MiniPetLoc.ContainsKey(miniPetID))
			MiniPetLoc.Add(miniPetID, new Status());
	}
	
	/// <summary>
	/// Whether the minipet has been unlocked
	/// </summary>
	/// <param name="miniPetID">Mini pet ID.</param>
	public bool IsMiniPetUnlocked(string miniPetID){
		bool retVal = false;
		
		if(!string.IsNullOrEmpty(miniPetID))
			retVal = MiniPetLoc.ContainsKey(miniPetID);
		
		return retVal;
	}

	public void SaveLoc(string miniPetID, Vector3 _Loc){
		if(MiniPetLoc.ContainsKey(miniPetID)){
			Status status = MiniPetLoc[miniPetID];
			
			status.Loc = _Loc.ToString();
			MiniPetLoc[miniPetID] = status;
		}
	}

	public void SaveHunger(string miniPetID, bool _isFinishEating){
		if(MiniPetLoc.ContainsKey(miniPetID)){
			Status status = MiniPetLoc[miniPetID];
			
			status.IsFinishEating = _isFinishEating;
			
			MiniPetLoc[miniPetID] = status;
		}
	}

	public Vector3 GetLoc(string miniPetID){
		if(MiniPetLoc.ContainsKey(miniPetID)){
			Status status = MiniPetLoc[miniPetID];
			return StringUtils.ParseVector3(status.Loc);
		}
		else{
			Debug.Log(miniPetID);
			return new Vector3(0,0,0);
		}
	}

	public bool GetHunger(string miniPetID){
		if(MiniPetLoc.ContainsKey(miniPetID)){
			Status status = MiniPetLoc[miniPetID];
			
			return status.IsFinishEating;
		}
		else{
			return false;
		}
	}
}
