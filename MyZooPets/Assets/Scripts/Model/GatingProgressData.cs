using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// GatingProgressData
// Save data script for the gating progress system.
//---------------------------------------------------

[DoNotSerializePublic]
public class GatingProgressData{
    
	[SerializeThis]
    private Dictionary<string, int> dictGatingProgress; // dictionary of gate IDs to HP remaining for the monster inside
	
    //===============Getters & Setters=================
    public Dictionary<string, int> GatingProgress {
        get{return dictGatingProgress;}
        set{dictGatingProgress = value;}
    }
	
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

    //=======================Initialization==================
    public GatingProgressData(){}

    //Populate with dummy data
    public void Init(){
        dictGatingProgress = new Dictionary<string, int>();
		
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
			dictGatingProgress[strKey] = nHP;
		}

		
    }
}
