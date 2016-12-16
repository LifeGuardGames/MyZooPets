using System.Collections;

//---------------------------------------------------
// DecorationItem
// An item that is a decoration.
//---------------------------------------------------
public class DecorationItem : Item{

    // the type of decoration this is
	private DecorationTypes eType;
	public DecorationTypes DecorationType{
		get{ return eType; }
	}

	private string prefabName;
	public string PrefabName{
		get{ return prefabName; }
	}

	private string materialName;
	public string MaterialName{
		get{ return materialName; }
	}

	private bool inSeason = true;
	public bool InSeason {
		get { return inSeason; }
	}

	public DecorationItem(string id, ItemType type, Hashtable hashItemData) : base (id, type, hashItemData){
		
		// get the type of this decoration
    	string strType = XMLUtils.GetString(hashItemData["DecorationType"] as IXMLNode);
		eType = (DecorationTypes) System.Enum.Parse( typeof( DecorationTypes ), strType );

		if(hashItemData.Contains("PrefabName")){
			prefabName = XMLUtils.GetString(hashItemData["PrefabName"] as IXMLNode);
		}
		if(hashItemData.Contains("MaterialName")){
			materialName = XMLUtils.GetString(hashItemData["MaterialName"] as IXMLNode);
		}
		if(hashItemData.Contains("InSeason")) {
			inSeason = XMLUtils.GetBool(hashItemData["InSeason"] as IXMLNode);
		}
	}
}