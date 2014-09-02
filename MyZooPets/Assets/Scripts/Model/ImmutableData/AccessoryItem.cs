using UnityEngine;
using System.Collections;

/// <summary>
/// An item that is an accessory
/// </summary>
public class AccessoryItem : Item{
	// The type of decortaion this is
	private AccessoryTypes eType;

	// decoration type property
	public AccessoryTypes AccessoryType{
		get{return eType;}
	}

	public AccessoryItem(string id, ItemType type, Hashtable hashItemData) : base(id, type, hashItemData){
		// Get the type of this decoration
		string strType = XMLUtils.GetString(hashItemData["AccessoryType"] as IXMLNode);
		eType = (AccessoryTypes)System.Enum.Parse(typeof(AccessoryTypes), strType);
	}
}
