using UnityEngine;
using System.Collections;

public class AccessoryManager : Singleton<AccessoryManager> {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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

	public void SetAccessoryAtNode(string itemID){
		DataManager.Instance.GameData.Accessories.SetAccessoryAtNode(GetAccessoryNodeType(itemID), itemID);
	}

	public void RemoveAccessoryAtNode(string itemID){
		DataManager.Instance.GameData.Accessories.RemoveAccessoryAtNode(GetAccessoryNodeType(itemID));
	}
}
