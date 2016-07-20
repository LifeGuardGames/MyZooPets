using UnityEngine;
using System.Collections;

public class ImmutableDataRareDeco {
	private string id;
	public string ID {
		get { return id; }
	}
	private ItemType type;
	public ItemType Type {
		get { return type; }
	}

	private string name;
	public string Name {
		get { return Localization.Localize(name); }
	}

	private string textureName;
	public string TextureName {
		get { return textureName; }
	}

	// the type of decoration this is
	private DecorationTypes eType;
	public DecorationTypes DecorationType {
		get { return eType; }
	}

	private string prefabName;
	public string PrefabName {
		get { return prefabName; }
	}

	private string materialName;
	public string MaterialName {
		get { return materialName; }
	}

	private int tier;
	public int Tier {
		get { return tier; }
	}

	public ImmutableDataRareDeco(string id, ItemType type, Hashtable hashItemData) {
		this.id = id;
		this.type = type;

		name = XMLUtils.GetString(hashItemData["Name"] as IXMLNode);
		textureName = XMLUtils.GetString(hashItemData["TextureName"] as IXMLNode);

		// get the type of this decoration
		string strType = XMLUtils.GetString(hashItemData["DecorationType"] as IXMLNode);
		eType = (DecorationTypes)System.Enum.Parse(typeof(DecorationTypes), strType);

		if(hashItemData.Contains("PrefabName")) {
			prefabName = XMLUtils.GetString(hashItemData["PrefabName"] as IXMLNode);
		}
		if(hashItemData.Contains("MaterialName")) {
			materialName = XMLUtils.GetString(hashItemData["MaterialName"] as IXMLNode);
		}
		tier = XMLUtils.GetInt(hashItemData["Tier"] as IXMLNode);

	}
}
