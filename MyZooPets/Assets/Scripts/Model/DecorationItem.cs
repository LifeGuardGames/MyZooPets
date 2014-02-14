using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// DecorationItem
// An item that is a decoration.
//---------------------------------------------------

public class DecorationItem : Item {
    // the type of decoration this is
	private DecorationTypes eType;	
	private string prefabName;
	private string materialName;
	
	// decoration type property
    public DecorationTypes DecorationType {
        get{return eType;}
    }

    public string PrefabName{
    	get{return prefabName;}
    }

    public string MaterialName{
    	get{return materialName;}
    }
	
    public DecorationItem(string id, ItemType type, Hashtable hashItemData) : base (id, type, hashItemData){
		
		// get the type of this decoration
    	string strType = XMLUtils.GetString(hashItemData["DecorationType"] as IXMLNode);
		eType = (DecorationTypes) System.Enum.Parse( typeof( DecorationTypes ), strType );

		if(hashItemData.Contains("PrefabName"))
			prefabName = XMLUtils.GetString(hashItemData["PrefabName"] as IXMLNode);

		if(hashItemData.Contains("MaterialName"))
			materialName = XMLUtils.GetString(hashItemData["MaterialName"] as IXMLNode);
    }
}