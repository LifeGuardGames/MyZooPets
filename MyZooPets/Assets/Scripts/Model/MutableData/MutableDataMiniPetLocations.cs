using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class MutableDataMiniPetLocations : MonoBehaviour {
	public class Status{
		public Vector3 Loc{get; set;}
		public bool IsFinishEating {get; set;}
		
		
		public Status(){
			Loc = new Vector3(0,0,0);
			IsFinishEating =  false;
		}
	}
	public Dictionary<string, Status> MiniPetLoc {get; set;} 

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
			retVal = DataManager.Instance.GameData.MiniPets.IsMiniPetUnlocked(miniPetID);
		
		return retVal;
	}

	public void SaveLoc(string miniPetID, Vector3 _Loc){
		if(MiniPetLoc.ContainsKey(miniPetID)){
			Status status = MiniPetLoc[miniPetID];
			
			status.Loc = _Loc;
			
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
			
			return status.Loc;
		}
		else{
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
	public MutableDataMiniPetLocations(){
		MiniPetLoc = new Dictionary<string, Status>();
	}
}
