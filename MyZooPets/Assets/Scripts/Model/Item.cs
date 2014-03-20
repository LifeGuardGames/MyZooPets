using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// Items
// Immutable data
//---------------------------------------------------
public class Item {
	protected string id; //id of item
	protected string name; //name of item
	protected string textureName; //name of texture in the atlas
	protected ItemType type;
	protected int cost; //cost of item
	protected string description;
	protected int unlockAtLevel = 0; //the level when item is unlocked
	
	// sound item makes when it is used
	private string strSoundUsed;

	public string ID{
		get{return id;}
	}
	public virtual string Name{ 
		get{return Localization.Localize( name );} 
	}
	public virtual string TextureName{
		get{return textureName;}
	}
	public ItemType Type{
		get{return type;}
	}
	public int Cost{
		get{return cost;}
	}
	public virtual string Description{
		// get{return Localization.Localize( description );}
        get{return "";}
	}
	public int UnlockAtLevel{
		get{return unlockAtLevel;}
	}
	public string SoundUsed{
		get{return strSoundUsed;}	
	}
	
	public bool IsLocked(){
        bool isLocked = true;
		int lockLevel = UnlockAtLevel;
		int currentPetLevel = (int) (LevelLogic.Instance.CurrentLevel);
		isLocked = lockLevel > 0 && lockLevel > currentPetLevel;

        //Don't allow item unlock in lite version
        if(VersionManager.IsLite() && LevelLogic.Instance.IsAtMaxLevel())
            isLocked = true;

		return isLocked;
	}
	// public int GetLockedLevel() {
	// 	return unlockAtLevel;	
	// }

	public Item(string id, ItemType type, Hashtable hashItemData){
		this.id = id;
		this.type = type;

		name = XMLUtils.GetString(hashItemData["Name"] as IXMLNode);
        textureName = XMLUtils.GetString(hashItemData["TextureName"] as IXMLNode);
        cost = XMLUtils.GetInt(hashItemData["Cost"] as IXMLNode);
        // description = XMLUtils.GetString(hashItemData["Desc"] as IXMLNode);
        description = "";
		
		// optional for now
		if ( hashItemData.Contains("UnlockAtLevel") )
       		unlockAtLevel = XMLUtils.GetInt(hashItemData["UnlockAtLevel"] as IXMLNode);
		
		if ( hashItemData.Contains("Sound") )
			strSoundUsed = XMLUtils.GetString(hashItemData["Sound"] as IXMLNode);
	}

	//Returns all attributes of all the children of a IXMLNode in a hastable
    protected Dictionary<StatType, int> LoadStats(IXMLNode node){
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
}
