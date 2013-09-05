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
	
	// decoration type property
    public DecorationTypes DecorationType {
        get{return eType;}
    }

    public DecorationItem(string id, ItemType type, Hashtable hashItemData) : base (id, type, hashItemData){
		
		// get the type of this decoration
    	string strType = XMLUtils.GetString(hashItemData["DecorationType"] as IXMLNode);
		eType = (DecorationTypes) System.Enum.Parse( typeof( DecorationTypes ), strType );
    }
}