using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Mutable data gating progress.
/// </summary>
public class MutableDataGatingProgress {
	public Dictionary<string, int> GatingProgress { get; set; } // key: gateID, value: HP remaining for the monster inside
	public DateTime LastRecurringGateSpawnedPlayPeriod { get; set; }

	/// <summary>
	/// Determines whether incoming gate is active.
	/// </summary>
	/// <returns><c>true</c> if gate with the specified gateID is active; otherwise, <c>false</c>.</returns>
	/// <param name="strID">String I.</param>
	public bool IsGateActive(string gateID) {
		// start off optimistic
		bool isActive = false;

		// if the gating system is not enabled, just bail now
		if(!IsEnabled()) {
			return isActive;
		}

		// if the gate's HP is > 0, it hasn't been opened yet
		if(GatingProgress.ContainsKey(gateID)) {
			isActive = GatingProgress[gateID] > 0;
		}
		else {
			Debug.LogError("Attempting to access a non-exitant gate from GatingProgressData: " + gateID);
		}
		return isActive;
	}

	/// <summary>
	/// Refreshes the gate.
	/// </summary>
	public void RefreshGate(ImmutableDataGate data) {
		string gateID = data.GateID;
		if(GatingProgress.ContainsKey(gateID)) {
			int hp = data.GetMonster().MonsterHealth;
			GatingProgress[gateID] = hp;
		}
		else {
			Debug.LogError("Trying to refresh a gate not in data...is this even possible!?");
		}
	}

	/// <summary>
	/// Determines whether gating is enabled. Used for testing
	/// </summary>
	public bool IsEnabled() {
		return DataManager.Instance.isDebug ? Constants.GetConstant<bool>("GatingEnabled") : true;
	}

	/// <summary>
	/// Gets the gate HP.
	/// </summary>
	/// <returns>The gate HP.</returns>
	/// <param name="gateID">Gate ID.</param>
	public int GetGateHP(string gateID) {
		int hp = 0;

		// if the gate's HP is > 0, it hasn't been opened yet
		if(GatingProgress.ContainsKey(gateID)) {
			hp = GatingProgress[gateID];
		}
		else {
			Debug.LogError("Attempting to access a non-exitant gate from GatingProgressData");
		}

		return hp;
	}

	//=======================Initialization==================
	public MutableDataGatingProgress() {
		Init();
	}

	//Populate with dummy data
	private void Init() {
		GatingProgress = new Dictionary<string, int>();
		LastRecurringGateSpawnedPlayPeriod = PlayPeriodLogic.GetCurrentPlayPeriod();

		// load all our gating data from xml
		LoadFromXML();
	}

	/// <summary>
	/// Loads gating data from XML and copies it into our
	/// save data if it does not already exist.
	/// </summary>
	private void LoadFromXML() {
		// init the data by filling the dictionary with xml data
		List<ImmutableDataGate> gates = DataLoaderGate.GetAllData();
		foreach(ImmutableDataGate gate in gates) {
			string gateID = gate.GateID;

			int hp = gate.GetMonster().MonsterHealth;

			// maps gate key to monster's max hp (i.e. no progress)
			// don't map it if it already exists; it means that the data for that key was already loaded and contains mutable save data
			if(!GatingProgress.ContainsKey(gateID)) {
				GatingProgress[gateID] = hp;
			}
		}
	}
}
