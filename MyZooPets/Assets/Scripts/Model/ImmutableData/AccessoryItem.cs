using UnityEngine;
using System.Collections;

/// <summary>
/// An item that is an accessory
/// </summary>
public class AccessoryItem : Item {
	// The type of decortaion this is
	private AccessoryTypes eType;

	// decoration type property
	public AccessoryTypes AccessoryType {
		get { return eType; }
	}

	private string prefabName;
	public string PrefabName {
		get { return prefabName; }
	}

	private bool inSeason = true;
	public bool InSeason {
		get { return inSeason; }
	}

	public AccessoryItem(string id, ItemType type, Hashtable hashItemData) : base(id, type, hashItemData) {
		// Get the type of this decoration
		string strType = XMLUtils.GetString(hashItemData["AccessoryType"] as IXMLNode);
		eType = (AccessoryTypes)System.Enum.Parse(typeof(AccessoryTypes), strType);

		if(hashItemData.Contains("PrefabName")) {
			prefabName = XMLUtils.GetString(hashItemData["PrefabName"] as IXMLNode);
		}

		if(hashItemData.Contains("InSeason")) {
			inSeason = XMLUtils.GetBool(hashItemData["InSeason"] as IXMLNode);
		}
	}
}
