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
	
	public override string TextureName{
		get { 
			string strName = base.TextureName;
			if ( string.IsNullOrEmpty( strName ) ) {
				strName = "item" + ID;	
			}
			
			return strName;
		}
	}	
	
	public override string Description{
		get { 
			// string strDesc = description;
			// if ( string.IsNullOrEmpty( strDesc ) ) {
			// 	strDesc =  "Desc" + ID;
			// }
			// return Localization.Localize( strDesc );
			return "";
		}
	}	
	
	public override string Name{
		get { 
			string strName = name;
			if ( string.IsNullOrEmpty( strName ) ) {
				strName = "Name" + ID;
			}
			
			return Localization.Localize( strName );
		}
	}	
	
    public DecorationItem(string id, ItemType type, Hashtable hashItemData) : base (id, type, hashItemData){
		
		// get the type of this decoration
    	string strType = XMLUtils.GetString(hashItemData["DecorationType"] as IXMLNode);
		eType = (DecorationTypes) System.Enum.Parse( typeof( DecorationTypes ), strType );
    }
}