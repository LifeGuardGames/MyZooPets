using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using fastJSON;

/// <summary>
/// Save data script for the accessory system.
/// </summary>
public class MutableDataAccessories : MutableData{

	/// <summary>
	/// Gets or sets the placed accessories.
	/// NOTE: Don't use this property. Use the methods provided by this class
	/// otherwise data will not be synced to the backend properly
	/// </summary>
	/// <value>The placed accessories.</value>
	public Dictionary<string, string> PlacedAccessories {get; set;} // dictionary of placed decorations; Key: node ID, Value: item ID

	/// <summary>
	/// Gets the placed accessory.
	/// </summary>
	/// <returns>The placed accessory itemID.</returns>
	/// <param name="nodeID">Node ID.</param>
	public string GetPlacedAccessory(string nodeID){
		string retVal = "";

		if(PlacedAccessories.ContainsKey(nodeID))
			retVal = PlacedAccessories[nodeID];

		return retVal;
	}

	/// <summary>
	/// Sets the accessory at node.
	/// </summary>
	/// <param name="nodeID">Node ID.</param>
	/// <param name="itemID">Item ID.</param>
	public void SetAccessoryAtNode(string nodeID, string itemID){
		if(!PlacedAccessories.ContainsKey(nodeID)){
			PlacedAccessories.Add(nodeID, itemID);
			IsDirty = true;
		}
	}

	/// <summary>
	/// Removes the accessory at node.
	/// </summary>
	/// <param name="nodeID">Node ID.</param>
	public void RemoveAccessoryAtNode(string nodeID){
		if(PlacedAccessories.ContainsKey(nodeID)){
			PlacedAccessories.Remove(nodeID);
			IsDirty = true;
		}
	}

	#region Initialization and override functions
	public MutableDataAccessories() : base(){
		PlacedAccessories = new Dictionary<string, string>();
	}

	public override void VersionCheck(System.Version currentDataVersion){

	}
	#endregion
}
