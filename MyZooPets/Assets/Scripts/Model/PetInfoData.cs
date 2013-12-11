using UnityEngine;
using System;

//---------------------------------------------------
// PetInfoData 
// Save data for PetInfo. Mutable data 
//---------------------------------------------------
public class PetInfoData{	
    public string PetID {get; set;}
    public string PetName {get; set;}
    public string PetColor {get; set;}
    public bool IsHatched {get; set;}
	
	// fire breathing status of the pet
	private int nFireBreaths;
	public void ChangeFireBreaths( int nAmount ) {
		nFireBreaths += nAmount;
		
		// for now, we are capping the max breaths at 1 -- repealed by Joe as we move to the task list system
		bool bInfiniteMode = Constants.GetConstant<bool>( "InfiniteFireMode" );
		if ( nFireBreaths > 1 ){
			// with the task list system, the player can now amass more than one fire breath
			//nFireBreaths = 1;
		}
		else if ( nFireBreaths < 0 && !bInfiniteMode ) {
			Debug.Log("Fire breaths somehow going negative.");
			nFireBreaths = 0;
		}
	}
	public bool CanBreathFire() {
		int nBreaths = GetFireBreaths();
		bool bInfiniteMode = Constants.GetConstant<bool>( "InfiniteFireMode" );
		bool bCan = nBreaths > 0 || bInfiniteMode;
		return bCan;
	}
	private int GetFireBreaths() {
		return nFireBreaths;	
	}

    public PetInfoData(){}

    public void Init(){
        PetID = "";
        PetName = "LazyWinkle";
        PetColor = "OrangeYellow";
        IsHatched = false;
		
		nFireBreaths = 0;
    }
}