﻿using UnityEngine;
using System;
using System.Collections.Generic;

//---------------------------------------------------
// PetInfoData 
// Save data for PetInfo. 
// Mutable data.
//---------------------------------------------------
public class PetInfoData{	
    public string PetID {get; set;}
    public string PetName {get; set;}
    public string PetSpecies {get; set;}
    public string PetColor {get; set;}
    public bool IsHatched {get; set;}
	public int nFireBreaths {get; set;} // fire breathing status of the pet

    public string TestString{get; set;}
    public List<string> TestListString{get; set;}
    public Dictionary<string, int> TestDict{get; set;}

	public void ChangeFireBreaths( int nAmount ) {
		int nBreathsNew = nFireBreaths + nAmount;
		SetFireBreaths( nBreathsNew );
	}

	public void SetFireBreaths( int nAmount ) {
		nFireBreaths = nAmount;	
	
		// for now, we are capping the max breaths at 1
		bool bInfiniteMode = IsInfiniteFire();
		if ( nFireBreaths > 1 )
			nFireBreaths = 1;
		else if ( nFireBreaths < 0 && !bInfiniteMode ) {
			Debug.LogError("Fire breaths somehow going negative.");
			nFireBreaths = 0;
		}
	}

	public bool CanBreathFire() {
		int nBreaths = GetFireBreaths();
		bool bInfiniteMode = IsInfiniteFire();
		bool bCan = nBreaths > 0 || bInfiniteMode;
		return bCan;
	}

	private int GetFireBreaths() {
		return nFireBreaths;	
	}

	public bool IsInfiniteFire() {
		bool bInfinite = Constants.GetConstant<bool>( "InfiniteFireMode" );
		return bInfinite;
	}

    public PetInfoData(){
        Init();        
    }

    private void Init(){
        PetID = "";
        PetName = "LazyWinkle";
        PetSpecies = "Basic";
        PetColor = "OrangeYellow";
        IsHatched = false;
        nFireBreaths = 0;
    }
}