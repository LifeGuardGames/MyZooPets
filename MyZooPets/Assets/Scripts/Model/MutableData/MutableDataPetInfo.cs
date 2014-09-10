using UnityEngine;
using System;
using System.Collections.Generic;

public class MutableDataPetInfo{	
	public string PetID { get; set; }
	public string PetName { get; set; }
	public string PetSpecies { get; set; }
	public string PetColor { get; set; }
	public bool IsHatched { get; set; }
	public int FireBreaths { get; set; } // fire breathing status of the pet
	public int nFireBreaths { get; set; } // Deprecated in 1.3.1
	public bool IsQuestionaireCollected {get; set;}
	
	public void SetFireBreaths(int amount){
		FireBreaths = amount;	
	
		// for now, we are capping the max breaths at 1
		bool isInfiniteMode = IsInfiniteFire();
		if(FireBreaths > 1)
			FireBreaths = 1;
		else if(FireBreaths < 0 && !isInfiniteMode){
			Debug.LogError("Fire breaths somehow going negative.");
			FireBreaths = 0;
		}
	}

	public bool CanBreathFire(){
		int breaths = FireBreaths;
		bool isInfiniteFire = IsInfiniteFire();
		bool canBreathFire = breaths > 0 || isInfiniteFire;
		return canBreathFire;
	}

	public bool IsInfiniteFire(){
		bool isInfinite = Constants.GetConstant<bool>("InfiniteFireMode");
		return isInfinite;
	}

	public MutableDataPetInfo(){
		Init();        
	}

	private void Init(){
		PetID = "";
		PetName = "LazyWinkle";
		PetSpecies = "Basic";
		PetColor = "OrangeYellow";
		IsHatched = false;
		FireBreaths = 0;
		IsQuestionaireCollected = false;
	}

	public void VersionCheck(Version currentDataVersion){
		Version version131 = new Version("1.3.1");
		
		if(currentDataVersion < version131){
			FireBreaths = nFireBreaths;
		}
	}
}