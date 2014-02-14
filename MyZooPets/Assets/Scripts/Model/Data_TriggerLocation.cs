using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// Data_TriggerLocation
// An individual trigger location in data.  Used by
// the degradation system to place triggers in the 
// scene.
// This is immutable data.
//---------------------------------------------------

public class Data_TriggerLocation {
	// id for the trigger location
	private string strID;
	public string GetID() {
		return strID;	
	}
	
	// position the trigger occupies as a string
	private string strPosition;
	public string GetPosition() {
		return strPosition;	
	}
	
	// partition this trigger belongs to
	private int nPartition;
	public int GetPartition() {
		return nPartition;	
	}
	
	// location id of this trigger; bedroom, yard, etc
	private string strScene;
	public string GetScene() {
		return strScene;	
	}

	public Data_TriggerLocation( string id, Hashtable hashAttr, string strError ) {
		// set id
		strID = id;
		
		strError += "(" + id + ")";
		
		// get position of the location
		strPosition = HashUtils.GetHashValue<string>( hashAttr, "Position", "0,0,0", strError );
		
		// get the partition this location exists in
		nPartition = int.Parse( HashUtils.GetHashValue<string>( hashAttr, "Partition", "0", strError ) );
		
		// get the scene this location is for
		strScene = HashUtils.GetHashValue<string>( hashAttr, "Scene", "Bedroom", strError );
	}
}
