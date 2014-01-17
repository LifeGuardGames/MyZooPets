using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// GatingProgressData
// Save data script for the gating progress system.
//---------------------------------------------------

public class GatingProgressData{
    public Dictionary<string, int> GatingProgress {get; set;} // dictionary of gate IDs to HP remaining for the monster inside
	
	//---------------------------------------------------
	// IsActivateGate()
	// Returns whether the incoming gate is activate (i.e.
	// the gate is active if the player hasn't opened it).
	//---------------------------------------------------	
	public bool IsGateActive( string strID ) {
		// start off optimistic
		bool bActive = false;
		
		// if the gating system is not enabled, just bail now
		if ( !IsEnabled() )
			return bActive;
		
		// if the gate's HP is > 0, it hasn't been opened yet
		if ( GatingProgress.ContainsKey( strID ) )
			bActive = GatingProgress[strID] > 0;
		else
			Debug.LogError("Attempting to access a non-exitant gate from GatingProgressData: " + strID);
		
		return bActive;
	}
	
	//---------------------------------------------------
	// RefreshGate()
	// Refreshes the incoming gate's HP.
	//---------------------------------------------------	
	public void RefreshGate( DataGate data ) {
		string strID = data.GetGateID();
		if ( GatingProgress.ContainsKey( strID ) ) {
			int nHP = data.GetMonster().GetMonsterHealth();
			GatingProgress[strID] = nHP;
		}
		else
			Debug.LogError("Trying to refresh a gate not in data...is this even possible!?");
	}	
	
	//---------------------------------------------------
	// IsEnabled()
	// Used for testing purposes.
	//---------------------------------------------------	
	public bool IsEnabled() {
		bool bEnabled = Constants.GetConstant<bool>( "GatingEnabled" );
		return bEnabled;	
	}
	
	//---------------------------------------------------
	// GetGateHP()
	// This is probably just a temp function until the
	// gating system gets finished.
	//---------------------------------------------------		
	public int GetGateHP( string strID ) {
		int nHP = 0;
		
		// if the gate's HP is > 0, it hasn't been opened yet
		if ( GatingProgress.ContainsKey( strID ) )
			nHP = GatingProgress[strID];
		else
			Debug.LogError("Attempting to access a non-exitant gate from GatingProgressData");
		
		return nHP;		
	}

    //=======================Initialization==================
    public GatingProgressData(){
    	Init();
    }

    //Populate with dummy data
    private void Init(){
        GatingProgress = new Dictionary<string, int>();		
		GatingManager.InitSaveData();
    }
}
