using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Mutable data gating progress.
/// </summary>
public class MutableDataGatingProgress{
	public Dictionary<string, int> GatingProgress { get; set; } // key: gateID, value: HP remaining for the monster inside
	public DateTime LastRecurringGateSpawnedPlayPeriod {get; set;}
	
	/// <summary>
	/// Determines whether incoming gate is active.
	/// </summary>
	/// <returns><c>true</c> if gate with the specified gateID is active; otherwise, <c>false</c>.</returns>
	/// <param name="strID">String I.</param>
	public bool IsGateActive(string gateID){
		// start off optimistic
		bool isActive = false;
		
		// if the gating system is not enabled, just bail now
		if(!IsEnabled())
			return isActive;
		
		// if the gate's HP is > 0, it hasn't been opened yet
		if(GatingProgress.ContainsKey(gateID))
			isActive = GatingProgress[gateID] > 0;
		else
			Debug.LogError("Attempting to access a non-exitant gate from GatingProgressData: " + gateID);
		
		return isActive;
	}

	/// <summary>
	/// Refreshes the gate.
	/// </summary>
	/// <param name="data">Data.</param>
	public void RefreshGate(ImmutableDataGate data){
		string gateID = data.GateID;
		if(GatingProgress.ContainsKey(gateID)){
			int hp = data.GetMonster().MonsterHealth;
			GatingProgress[gateID] = hp;
		}
		else
			Debug.LogError("Trying to refresh a gate not in data...is this even possible!?");
	}	
	
	/// <summary>
	/// Determines whether gating is enabled. Used for testing
	/// </summary>
	/// <returns><c>true</c> if gating is enabled; otherwise, <c>false</c>.</returns>
	public bool IsEnabled(){
		bool isEnabled = Constants.GetConstant<bool>("GatingEnabled");
		return isEnabled;	
	}

	/// <summary>
	/// Gets the gate HP.
	/// </summary>
	/// <returns>The gate HP.</returns>
	/// <param name="gateID">Gate ID.</param>
	public int GetGateHP(string gateID){
		int hp = 0;
		
		// if the gate's HP is > 0, it hasn't been opened yet
		if(GatingProgress.ContainsKey(gateID))
			hp = GatingProgress[gateID];
		else
			Debug.LogError("Attempting to access a non-exitant gate from GatingProgressData");
		
		return hp;		
	}

	//=======================Initialization==================
	public MutableDataGatingProgress(){
		Init();
	}

	//Populate with dummy data
	private void Init(){
		GatingProgress = new Dictionary<string, int>();
		LastRecurringGateSpawnedPlayPeriod = PlayPeriodLogic.GetCurrentPlayPeriod();
		GatingProgress.Add ("Gate_Bedroom_1", 0);
		// load all our gating data from xml
		LoadFromXML();		
	}

	/*
	public void VersionCheck(Version currentDataVersion){
		Version version131 = new Version("1.3.1");

		if(currentDataVersion < version131){
			ConvertGateHP();	
		}
		// when we are doing a version check, just load the data from xml again.
		// any existing data will be left alone, and new data will be inserted into our dictionary.
		LoadFromXML();	
	}

	/// <summary>
	/// Converts the gate HP.
	/// New game design change. Monster health is now symbolize by the number of heads, 
	/// so need to convert from the old health value into the number of heads the monster
	/// have left
	/// </summary>
	private void ConvertGateHP(){
		string[] convertingGateIDs = new string[]{"Gate_Bedroom_1", "Gate_Bedroom_2", "Gate_Bedroom_3", "Gate_Yard_R"};
		int[] oldFullHealths = new int[]{10, 40, 80, 5};

		for(int index=0; index < convertingGateIDs.Length; index++){
			string gateID = convertingGateIDs[index];

			if(GatingProgress.ContainsKey(gateID)){
				int oldFullHealth = oldFullHealths[index];
				ImmutableDataGate gate = DataLoaderGate.GetData(gateID);
				ImmutableDataMonster monster = gate.GetMonster();
				int newFullHealth = monster.MonsterHealth;

				int oldCurrentHealth = GatingProgress[gateID];

				//if data is already in the right scale don't convert it
				//don't need to convert if the gate is opened already
				if(oldCurrentHealth != newFullHealth && oldCurrentHealth != 0){
					//use this to convert the old health data to new health data
					int conversionFactor = oldFullHealth / newFullHealth;
					int newCurrentHealth = oldCurrentHealth / conversionFactor;

					//if newCurrentHealth is 0 we set it to 1 so it doesn't look like
					//the monster all of the suden disappeared
					if(newCurrentHealth == 0)
						newCurrentHealth = 1;

					Debug.Log(newCurrentHealth);
					
					GatingProgress[gateID] = newCurrentHealth;
				}
			}
		}
	}
	*/

	/// <summary>
	/// Loads gating data from XML and copies it into our
	/// save data if it does not already exist.
	/// </summary>
	private void LoadFromXML(){
		// init the data by filling the dictionary with xml data
		List<ImmutableDataGate> gates = DataLoaderGate.GetAllData();
		foreach(ImmutableDataGate gate in gates){
			string gateID = gate.GateID;

			int hp = gate.GetMonster().MonsterHealth;
			
			// maps gate key to monster's max hp (i.e. no progress)
			// don't map it if it already exists; it means that the data for that key was already loaded and contains mutable save data
			if(!GatingProgress.ContainsKey(gateID))
				GatingProgress[gateID] = hp;
		}			
	}
}
