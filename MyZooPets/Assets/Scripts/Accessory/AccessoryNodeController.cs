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
		AccessoryNode node = accessoryNodeHash[GetAccessoryNodeType(accessoryID)];	// Get respective node to change

		node.SetAccessoryNode(accessoryID);
	}

	/// <summary>
	/// Gets the type of the accessory node.
	/// Given a accessory(ID), check the xml to see which type it is.
	/// </summary>
	/// <returns>The accessory node type.</returns>
	/// <param name="accessoryID">Accessory ID.</param>
	public string GetAccessoryNodeType(string accessoryID){
		AccessoryItem itemDeco = (AccessoryItem)ItemLogic.Instance.GetItem(accessoryID);
		return itemDeco.AccessoryType.ToString();
	}
}
