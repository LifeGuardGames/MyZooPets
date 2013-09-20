using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Item {
	private string id; //id of item
	private string name; //name of item
	private string textureName; //name of texture in the atlas
	private ItemType type;
	private int cost; //cost of item
	private string description;
	private int unlockAtLevel = 0; //the level when item is unlocked
	
	// sound item makes when it is used
	private string strSoundUsed;

	public string ID{
		get{return id;}
	}
	public string Name{ 
		get{return Localization.Localize( name );} 
	}
	public string TextureName{
		get{return textureName;}
	}
	public ItemType Type{
		get{return type;}
	}
	public int Cost{
		get{return cost;}
	}
	public string Description{
		get{return Localization.Localize( description );}
	}
	public int UnlockAtLevel{
		get{return unlockAtLevel;}
	}
	public string SoundUsed{
		get{return strSoundUsed;}	
	}

	public Item(string id, ItemType type, Hashtable hashItemData){
		this.id = id;
		this.type = type;

		name = XMLUtils.GetString(hashItemData["Name"] as IXMLNode);
        textureName = XMLUtils.GetString(hashItemData["TextureName"] as IXMLNode);
        cost = XMLUtils.GetInt(hashItemData["Cost"] as IXMLNode);
        description = XMLUtils.GetString(hashItemData["Desc"] as IXMLNode);
		
		// optional for now
		if ( hashItemData.Contains("UnlockAtLevel") )
       		unlockAtLevel = XMLUtils.GetInt(hashItemData["UnlockAtLevel"] as IXMLNode);
		
		if ( hashItemData.Contains("Sound") )
			strSoundUsed = XMLUtils.GetString(hashItemData["Sound"] as IXMLNode);
	}

	//Returns all attributes of all the children of a IXMLNode in a hastable
    protected Dictionary<StatType, int> GetStats(IXMLNode node){
        Dictionary<StatType, int> statsDict = new Dictionary<StatType, int>();
        List<IXMLNode> childrenList = XMLUtils.GetChildrenList(node);

        foreach(IXMLNode xmlNode in childrenList){
            Hashtable attrHash = XMLUtils.GetAttributes(xmlNode);
            StatType statType = (StatType)Enum.Parse(typeof(StatType), (string)attrHash["ID"]);
            int amount = Convert.ToInt32(attrHash["Amount"]);
            statsDict.Add(statType, amount);
        }
        return statsDict;
    }
	
	//---------------------------------------------------
	// GetDesc()
	// Returns this item's description.
	//---------------------------------------------------	
	public virtual string GetDesc() {
		return Description;
	}
}
