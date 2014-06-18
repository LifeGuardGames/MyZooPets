using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// Badge 
// Badge data that is used during game. Contains a mix
// of mutable and immutable data 
//---------------------------------------------------

public class Badge{
    private string id;
    private string name;
    private string textureName;
    private BadgeType type;
    private string description;
    private int unlockCondition;

    public string ID{
        get{return id;}
    }
    public string Name{
        get{return Localization.Localize(name);}
    }
    public string TextureName{
        get{return textureName;}
    }
    public BadgeType Type{
        get{return type;}
    }
    public string Description{
        get{
            return String.Format(Localization.Localize(description), unlockCondition);
        }
    }
    //How to unlock this badge
    public int UnlockCondition{
        get{return unlockCondition;}
    } 
    //Is badge unlocked?
    public bool IsUnlocked{
        get{return DataManager.Instance.GameData.Badge.GetIsUnlocked(id);}
    }
    //Is badge new?
    public bool IsNew{
        get{return DataManager.Instance.GameData.Badge.GetIsNew(id);}
    }

	public Badge(string id, IXMLNode xmlNode, string error){
		Hashtable hashElements = XMLUtils.GetChildren(xmlNode);
        this.id = id;

        name = XMLUtils.GetString(hashElements["Name"] as IXMLNode, null, error);
        textureName = XMLUtils.GetString(hashElements["TextureName"] as IXMLNode, null, error);
        unlockCondition = XMLUtils.GetInt(hashElements["UnlockCondition"] as IXMLNode, -1, error);
        description = XMLUtils.GetString(hashElements["Description"] as IXMLNode, null, error);
        string typeString = XMLUtils.GetString(hashElements["Type"] as IXMLNode, null, error);
        this.type = (BadgeType)Enum.Parse(typeof(BadgeType), typeString);
    }

}