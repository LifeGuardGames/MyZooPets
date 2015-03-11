using UnityEngine;
using System.Collections;

public class ImmutableDataTriggerArmy{

	private string id;
	public string Id{
		get {return id;}
	}

	private string name;
	public string Name{
		get{ return name;}
	}

	private string prefabName;
	public string PrefabName{
		get{ return prefabName;}
	}

	private string type;
	public string Type{
		get{ return type;}
	}

	public ImmutableDataTriggerArmy(string id, IXMLNode xmlNode, string error){
		Hashtable hashElements = XMLUtils.GetChildren(xmlNode);
		this.id = id;
		name = XMLUtils.GetString(hashElements["Name"] as IXMLNode, null, error);
		prefabName = XMLUtils.GetString(hashElements["PrefabName"] as IXMLNode, null, error);
		type = XMLUtils.GetString(hashElements["Type"] as IXMLNode, null, error);
	}
}
