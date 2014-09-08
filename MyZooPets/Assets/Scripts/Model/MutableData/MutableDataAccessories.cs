using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Save data script for the accessory system.
/// </summary>
public class MutableDataAccessories{

	public Dictionary<string, string> PlacedAccessories {get; set;} // dictionary of placed decorations; Key: node ID, Value: item ID
	
	//=======================Initialization==================
	public MutableDataAccessories(){
		Init();    
	}
	
	//Populate with dummy data
	private void Init(){
		PlacedAccessories = new Dictionary<string, string>();
	}
}
