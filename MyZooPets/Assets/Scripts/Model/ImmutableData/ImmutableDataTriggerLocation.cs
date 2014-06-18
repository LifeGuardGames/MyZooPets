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

	private string scene; // location id of this trigger; bedroom, yard, etc

	public string ID{
		get{
			return id;	
		}
	}

	public string Position{
		get{
			return position;	
		}
	}
		
	public int Partition{
		get{
			return parition;	
		}
	}
		
	public string Scene{
		get{
			return scene;	
		}
	}

	public ImmutableDataTriggerLocation(string id, IXMLNode xmlNode, string errorMsg){
		Hashtable hashAttr = XMLUtils.GetAttributes(xmlNode);

		this.id = id;
		
		errorMsg += "(" + id + ")";
		
		// get position of the location
		position = HashUtils.GetHashValue<string>(hashAttr, "Position", "0,0,0", errorMsg);
		
		// get the partition this location exists in
		parition = int.Parse(HashUtils.GetHashValue<string>(hashAttr, "Partition", "0", errorMsg));
		
		// get the scene this location is for
		scene = HashUtils.GetHashValue<string>(hashAttr, "Scene", "Bedroom", errorMsg);
	}
}
