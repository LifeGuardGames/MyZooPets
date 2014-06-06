using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Immutable data trigger location. Used by
// the degradation system to place triggers in the 
// scene.
/// </summary>
public class ImmutableDataTriggerLocation{

	private string id; // id for the trigger location

	private string position; // position the trigger occupies as a string

	private int parition; // partition this trigger belongs to

	private string strScene; // location id of this trigger; bedroom, yard, etc

	public string GetID(){
		return id;	
	}

	public string GetPosition(){
		return position;	
	}
		
	public int GetPartition(){
		return parition;	
	}
		
	public string GetScene(){
		return strScene;	
	}

	public ImmutableDataTriggerLocation(string id, Hashtable hashAttr, string strError){
		// set id
		this.id = id;
		
		strError += "(" + id + ")";
		
		// get position of the location
		position = HashUtils.GetHashValue<string>(hashAttr, "Position", "0,0,0", strError);
		
		// get the partition this location exists in
		parition = int.Parse(HashUtils.GetHashValue<string>(hashAttr, "Partition", "0", strError));
		
		// get the scene this location is for
		strScene = HashUtils.GetHashValue<string>(hashAttr, "Scene", "Bedroom", strError);
	}
}
