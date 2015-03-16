using UnityEngine;
using System.Collections;

public class ImmutableDataMerchantItem  {

	private string type;
	private string id;
	private string itemId;

	public string Type {
		get{return type;}
	}

	public string ID{
		get{return id;}
	}

	public string ItemId{
		get{return itemId;}
	}

	public ImmutableDataMerchantItem(string id, IXMLNode xmlNode, string error){
		Hashtable hashElements = XMLUtils.GetChildren(xmlNode);
		
		this.id = id;
		type = XMLUtils.GetString(hashElements["type"] as IXMLNode, null, error);
		itemId = XMLUtils.GetString(hashElements["ItemID"] as IXMLNode, null,error);
	}
}
