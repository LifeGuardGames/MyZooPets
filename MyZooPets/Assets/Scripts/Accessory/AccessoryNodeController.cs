using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Object should go on the pet and controls a bunch of accessoryNodes,
/// one for each body accessory.
/// 
/// Note: Color of the pet will be handled differently.
/// </summary>
public class AccessoryNodeController : Singleton<AccessoryNodeController> {

	// Individual accessoryNodes will add themselves to this hash table
	public Dictionary<string, AccessoryNode> accessoryNodeHash = new Dictionary<string, AccessoryNode>();

	/// <summary>
	/// Sets the accessory.
	/// Called from AccessoryEntryUIController
	/// </summary>
	/// <param name="accessoryID">Accessory ID.</param>
	public void SetAccessory(string accessoryID){

		// Get the respective AccessoryNode to populate and then populate it
		AccessoryNode node = accessoryNodeHash[AccessoryUIManager.GetAccessoryNodeType(accessoryID)];	// Get respective node to change
		node.SetAccessoryNode(accessoryID);
	}

	public void RemoveAccessory(string accessoryID){
		AccessoryNode node = accessoryNodeHash[AccessoryUIManager.GetAccessoryNodeType(accessoryID)];	// Get respective node to change
		node.SetAccessoryNode(null);
	}
}
