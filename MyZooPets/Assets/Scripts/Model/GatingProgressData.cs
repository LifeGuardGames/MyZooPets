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
		
		// if the gate's HP is > 0, it hasn't been opened yet
		if ( GatingProgress.ContainsKey( strID ) )
			bActive = GatingProgress[strID] > 0;
		else
			Debug.Log("Attempting to access a non-exitant gate from GatingProgressData");
		
		return bActive;
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
