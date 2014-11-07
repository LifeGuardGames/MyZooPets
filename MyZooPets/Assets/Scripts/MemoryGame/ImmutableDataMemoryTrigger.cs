using UnityEngine;
using System.Collections;

/// <summary>
/// Immutable data memory trigger. Data loaded from XML
/// </summary>
public class ImmutableDataMemoryTrigger {

	private string id;
	public string Id{
		get{ return id; }
	}

	private string name;
	public string Name{
		get{ return name; }
	}

	private string spriteName;
	public string SpriteName{
		get{ return spriteName; }
	}

	public ImmutableDataMemoryTrigger(string id, IXMLNode xmlNode, string error){
		Hashtable hashElements = XMLUtils.GetChildren(xmlNode);
		this.id = id;
		name = XMLUtils.GetString(hashElements["Name"] as IXMLNode, null, error);
		spriteName = XMLUtils.GetString(hashElements["SpriteName"] as IXMLNode, null, error);
	}
}
