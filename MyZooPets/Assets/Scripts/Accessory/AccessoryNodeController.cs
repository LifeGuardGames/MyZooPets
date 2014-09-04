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
	/// <param name="accessoryKey">Accessory key.</param>
	/// <param name="atlasName">Atlas name.</param>
	/// <param name="spriteName">Sprite name.</param>
	public void SetAccessory(string accessoryKey, string atlasName, string spriteName){
		AccessoryNode node = accessoryNodeHash[accessoryKey];	// Get respective node to change
		node.ChangeAccessory(atlasName, spriteName);
	}
}
