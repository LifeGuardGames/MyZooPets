using UnityEngine;
using System.Collections;

public class AccessoryManager : Singleton<AccessoryManager> {
	/// <summary>
	/// Gets the type of the accessory node.
	/// Given a accessory(ID), check the xml to see which type it is.
	/// </summary>
	/// <returns>The accessory node type.</returns>
	/// <param name="accessoryID">Accessory ID.</param>
	public string GetAccessoryNodeType(string accessoryID){
		AccessoryItem itemDeco = (AccessoryItem)DataLoaderItems.GetItem(accessoryID);
		return itemDeco.AccessoryType.ToString();
	}

	public void SetAccessoryAtNode(string itemID){
		DataManager.Instance.GameData.Accessories.SetAccessoryAtNode(GetAccessoryNodeType(itemID), itemID);
	}

	public void RemoveAccessoryAtNode(string itemID){
		DataManager.Instance.GameData.Accessories.RemoveAccessoryAtNode(GetAccessoryNodeType(itemID));
	}
}
