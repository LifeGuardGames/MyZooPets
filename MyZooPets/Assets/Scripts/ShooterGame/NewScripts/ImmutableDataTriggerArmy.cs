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
		Debug.Log("where");
		Hashtable hashElements = XMLUtils.GetChildren(xmlNode);
		Debug.Log("are");
		this.mober = id;
		Debug.Log("we");
		name = XMLUtils.GetString(hashElements["Name"] as IXMLNode, null, error);
		Debug.Log("breaking");
		spriteName = XMLUtils.GetString(hashElements["SpriteName"] as IXMLNode, null, error);
		Debug.Log("?");
		ai = XMLUtils.GetString(hashElements["AI"] as IXMLNode, null, error);
		Debug.Log("!");
		displayKey = XMLUtils.GetString(hashElements["DisPlayKey"] as IXMLNode, null, error);
		Debug.Log("here?");
	}
}
