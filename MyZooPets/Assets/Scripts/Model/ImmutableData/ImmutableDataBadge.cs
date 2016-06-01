using System;
using System.Collections;

public class ImmutableDataBadge {

	private string id;
	public string ID {
		get { return id; }
	}

	private string name;
	public string Name {
		get { return Localization.Localize(name); }
	}

	private string textureName;
	public string TextureName {
		get { return textureName; }
	}

	private BadgeType type;
	public BadgeType Type {
		get { return type; }
	}

	private string description;
	public string Description {
		get {
			return String.Format(Localization.Localize(description), unlockCondition);
		}
	}

	private int unlockCondition;
	public int UnlockCondition {
		get { return unlockCondition; }
	}

	public ImmutableDataBadge(string id, IXMLNode xmlNode, string error) {
		Hashtable hashElements = XMLUtils.GetChildren(xmlNode);
		this.id = id;

		name = XMLUtils.GetString(hashElements["Name"] as IXMLNode, null, error);
		textureName = XMLUtils.GetString(hashElements["TextureName"] as IXMLNode, null, error);
		unlockCondition = XMLUtils.GetInt(hashElements["UnlockCondition"] as IXMLNode, -1, error);
		description = XMLUtils.GetString(hashElements["Description"] as IXMLNode, null, error);
		string typeString = XMLUtils.GetString(hashElements["Type"] as IXMLNode, null, error);
		type = (BadgeType)Enum.Parse(typeof(BadgeType), typeString);
	}
}