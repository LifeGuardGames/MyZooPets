using UnityEngine;
using System;
using System.Collections.Generic;

public class MutableDataPetInfo : MutableData{	
	public string PetID { get; set; }

	/// <summary>
	/// Gets or sets the name of the pet. **NOTE For json serialization use only. Use 
	/// ChangeName() to edit pet name otherwise data will not be save to the server
	/// </summary>
	/// <value>The name of the pet.</value>
	public string PetName { get; set; }

	public string PetSpecies { get; set; }
	public string PetColor { get; set; }
	public bool IsHatched { get; set; }
	public int FireBreaths { get; set; } // fire breathing status of the pet
	public int amountOfFireBreathsDone{get; set;}


	/// <summary>
	/// Changes the name.
	/// </summary>
	/// <param name="petName">Pet name.</param>
	public void ChangeName(string petName){
		IsDirty = true;
		if(!string.IsNullOrEmpty(petName)){
			PetName = petName;
		}
	}

	public void ChangeColor(PetColor petColorEnum){
		// Sould we do sanity color checking here?
		if(true){
			IsDirty = true;
			PetColor = petColorEnum.ToString();
		}
	}
	
	public void SetFireBreaths(int amount){
		FireBreaths = amount;
	
		// for now, we are capping the max breaths at 1
		bool isInfiniteMode = Constants.GetConstant<bool>("InfiniteFireMode");
		if(FireBreaths > 1)
			FireBreaths = 1;
		else if(FireBreaths < 0 && !isInfiniteMode){
			Debug.LogError("Fire breaths somehow going negative.");
			FireBreaths = 0;
		}
	}

	public bool CanBreathFire(){
		return FireBreaths > 0 || Constants.GetConstant<bool>("InfiniteFireMode");
	}

	#region Initialization and override functions
	public MutableDataPetInfo(){
		Init();        
	}

	private void Init(){
		PetID = "";
		PetName = "Your wizdy pet";
		PetSpecies = "Basic";
		PetColor = "OrangeYellow";
		IsHatched = false;
		FireBreaths = 0;
		amountOfFireBreathsDone = 0;
	}

	public override void VersionCheck(Version currentDataVersion){
	}
	#endregion
}
