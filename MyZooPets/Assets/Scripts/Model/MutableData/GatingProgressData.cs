using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// GatingProgressData
// Save data script for the gating progress system.
//---------------------------------------------------

public class GatingProgressData{
	public Dictionary<string, int> GatingProgress { get; set; } // dictionary of gate IDs to HP remaining for the monster inside
	
	//---------------------------------------------------
	// IsActivateGate()
	// Returns whether the incoming gate is activate (i.e.
	// the gate is active if the player hasn't opened it).
	//---------------------------------------------------	
	public bool IsGateActive(string strID){
		// start off optimistic
		bool bActive = false;
		
		// if the gating system is not enabled, just bail now
		if(!IsEnabled())
			return bActive;
		
		// if the gate's HP is > 0, it hasn't been opened yet
		if(GatingProgress.ContainsKey(strID))
			bActive = GatingProgress[strID] > 0;
		else
			Debug.LogError("Attempting to access a non-exitant gate from GatingProgressData: " + strID);
		
		return bActive;
	}
	
	//---------------------------------------------------
	// RefreshGate()
	// Refreshes the incoming gate's HP.
	//---------------------------------------------------	
	public void RefreshGate(DataGate data){
		string strID = data.GetGateID();
		if(GatingProgress.ContainsKey(strID)){
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
	public bool IsEnabled(){
		bool bEnabled = Constants.GetConstant<bool>("GatingEnabled");
		return bEnabled;	
	}
	
	//---------------------------------------------------
	// GetGateHP()
	// This is probably just a temp function until the
	// gating system gets finished.
	//---------------------------------------------------		
	public int GetGateHP(string strID){
		int nHP = 0;
		
		// if the gate's HP is > 0, it hasn't been opened yet
		if(GatingProgress.ContainsKey(strID))
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
		
		// load all our gating data from xml
		LoadFromXML();		
	}

	//---------------------------------------------------
	// VersionCheck()
	//---------------------------------------------------	
	public void VersionCheck(){
		// when we are doing a version check, just load the data from xml again.
		// any existing data will be left alone, and new data will be inserted into our dictionary.
		LoadFromXML();	
	}
	
	//---------------------------------------------------
	// LoadFromXML()
	// Loads gating data from XML and copies it into our
	// save data if it does not already exist.
	//---------------------------------------------------	
	private void LoadFromXML(){
		// init the data by filling the dictionary with xml data
		Dictionary<string, DataGate> dictGates = DataGateLoader.GetAllData();
		foreach(KeyValuePair<string, DataGate> entry in dictGates){
			string strKey = entry.Key;
			DataGate dataGate = entry.Value;
			int nHP = dataGate.GetMonster().GetMonsterHealth();
			
			// maps gate key to monster's max hp (i.e. no progress)
			// don't map it if it already exists; it means that the data for that key was already loaded and contains mutable save data
			if(!GatingProgress.ContainsKey(strKey))
				GatingProgress[strKey] = nHP;
		}			
	}
}
