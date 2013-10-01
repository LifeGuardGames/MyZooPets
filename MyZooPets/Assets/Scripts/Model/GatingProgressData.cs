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

    //=======================Initialization==================
    public GatingProgressData(){}

    //Populate with dummy data
    public void Init(){
        dictGatingProgress = new Dictionary<string, int>();
		
		// load data from xml
		DataGateLoader.SetupData();
		DataMonsterLoader.SetupData();
		
		Dictionary<string, DataGate> dictGates = DataGateLoader.GetAllData();
		foreach(KeyValuePair<string, DataGate> entry in dictGates) {
		    // do something with entry.Value or entry.Key
		}

		
    }
}
