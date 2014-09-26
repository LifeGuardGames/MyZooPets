using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Save data script for the accessory system.
/// </summary>
public class MutableDataAccessories : MutableData{

	public Dictionary<string, string> PlacedAccessories {get; set;} // dictionary of placed decorations; Key: node ID, Value: item ID

	
	public MutableDataAccessories() : base(){
		PlacedAccessories = new Dictionary<string, string>();
	}

	public override void VersionCheck(System.Version currentDataVersion){
		throw new System.NotImplementedException();
	}

	public override void SyncToParseServer(){
		throw new System.NotImplementedException();
	}

}
