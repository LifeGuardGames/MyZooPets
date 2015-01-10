using UnityEngine;
using System.Collections;

public class ImmutableDataTriggerArmy : MonoBehaviour {

	private string mober;
	public string Mober{
		get {return mober;}
	}
	private string name;
	public string Name{
		get{ return name;}
	}
	private string spriteName;
	public string SpriteName{
		get{ return spriteName;}
	}
	private string ai;
	public string AI{
		get{ return ai;}
	}
	private string displayKey;
	public string DisplayKey{
		get{ return displayKey;}
	}
	public ImmutableDataTriggerArmy(string id, IXMLNode xmlNode, string error){
		Hashtable hashElements = XMLUtils.GetChildren(xmlNode);
		this.mober = id;
		name = XMLUtils.GetString(hashElements["Name"] as IXMLNode, null, error);
		spriteName = XMLUtils.GetString(hashElements["SpriteName"] as IXMLNode, null, error);
		ai = XMLUtils.GetString(hashElements["AI"] as IXMLNode, null, error);
		displayKey = XMLUtils.GetString(hashElements["DisplayKey"] as IXMLNode, null, error);
	}
}
