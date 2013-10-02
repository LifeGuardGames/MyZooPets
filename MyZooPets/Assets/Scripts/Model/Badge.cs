using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

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
            return StringUtils.Replace(Localization.Localize(description), StringUtils.NUM, 
                unlockCondition.ToString());
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

    public Badge(string id, Hashtable hashItemData){
        this.id = id;

        name = XMLUtils.GetString(hashItemData["Name"] as IXMLNode);
        textureName = XMLUtils.GetString(hashItemData["TextureName"] as IXMLNode);
        unlockCondition = XMLUtils.GetInt(hashItemData["UnlockCondition"] as IXMLNode);
        description = XMLUtils.GetString(hashItemData["Description"] as IXMLNode);
        string typeString = XMLUtils.GetString(hashItemData["Type"] as IXMLNode);
        this.type = (BadgeType)Enum.Parse(typeof(BadgeType), typeString);
    }

}