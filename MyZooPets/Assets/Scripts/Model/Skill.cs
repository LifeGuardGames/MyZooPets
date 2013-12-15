using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// Skill 
// skill data that is used during game. Contains a mix
// of mutable and immutable data 
//---------------------------------------------------
public class Skill{
    private string id;
    private string name;
    private string textureName;
	private string strFlameResource;	// what flame effect is instantiated when this skill is used
    private string description;
    private int unlockLevel;
    private int damagePoint;

    public string ID{
        get{return id;}
    }
    public string Name{
        get{return Localization.Localize(name);}
    }
    public string TextureName{
        get{return textureName;}
    }
	public string FlameResource{
		get{return strFlameResource;}
	}
    public string Description{
        get{return Localization.Localize(description);}
    }
    public int UnlockLevel{
        get{return unlockLevel;} 
    }
    //Damage to smoke monster
    public int DamagePoint{
        get{return damagePoint;}
    }

    public bool IsUnlocked{
        get { 
            return DataManager.Instance.GameData.Flame.GetIsUnlocked(id);
        } 
    }

    public Skill(string id, Hashtable hashItemData){
        this.id = id;

        name = XMLUtils.GetString(hashItemData["Name"] as IXMLNode);
        textureName = XMLUtils.GetString(hashItemData["TextureName"] as IXMLNode);
		strFlameResource = XMLUtils.GetString(hashItemData["ParticleResource"] as IXMLNode);
        description = XMLUtils.GetString(hashItemData["Description"] as IXMLNode);
        unlockLevel = XMLUtils.GetInt(hashItemData["UnlockLevel"] as IXMLNode);
        damagePoint = XMLUtils.GetInt(hashItemData["DamagePoints"] as IXMLNode);
    }

    // public string GetUnlockLevelString(){
    //     return StringUtils.Replace(Localization.Localize("UNLOCK"), StringUtils.NUM,
    //             unlockLevel.ToString());
    // }

    // public string GetDamagePointString(){
    //     return StringUtils.Replace(Localization.Localize("DAMAGE"), StringUtils.NUM,
    //             damagePoint.ToString());
    // }
}