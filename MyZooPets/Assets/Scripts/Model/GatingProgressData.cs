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
			Debug.Log("Attempting to access a non-exitant gate from GatingProgressData");
		
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
			Debug.Log("Trying to refresh a gate not in data...is this even possible!?");
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
			Debug.Log("Attempting to access a non-exitant gate from GatingProgressData");
		
		return nHP;		
	}
	
	//---------------------------------------------------
	// DamageGate()
	// Alters a gate's hp.
	//---------------------------------------------------		
	public bool DamageGate( string strID, int nDamage ) {
		// check to make sure the gate exists
		if ( !GatingProgress.ContainsKey( strID ) ) {
			Debug.Log("Something trying to access a non-existant gate " + strID);
			return true;
		}
		
		// check to make sure the gate is active
		if ( !IsGateActive( strID ) ) {
			Debug.Log("Something trying to damage an inactive gate " + strID);
			return true;
		}
		
		// otherwise, calculate and save the new hp
		int nHP = GatingProgress[strID];
		nHP = Mathf.Max( nHP - nDamage, 0 );
		GatingProgress[strID] = nHP;
		
		// then return whether or not the gate has been destroyed
		bool bDestroyed = nHP <= 0;

		//Send analytics event
		if(bDestroyed)
			Analytics.Instance.GateUnlocked(strID);

		return bDestroyed;
	}

    //=======================Initialization==================
    public GatingProgressData(){}

    //Populate with dummy data
    public void Init(){
        GatingProgress = new Dictionary<string, int>();
		
		// load data from xml
		DataGateLoader.SetupData();
		DataMonsterLoader.SetupData();
		
		// init the data by filling the dictionary with xml data
		Dictionary<string, DataGate> dictGates = DataGateLoader.GetAllData();
		foreach(KeyValuePair<string, DataGate> entry in dictGates) {
			string strKey = entry.Key;
		    DataGate dataGate = entry.Value;
			int nHP = dataGate.GetMonster().GetMonsterHealth();
			
			// maps gate key to monster's max hp (i.e. no progress)
			GatingProgress[strKey] = nHP;
		}

		
    }
}
