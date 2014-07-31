using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Skill. Constains a mix of mutable and immutable data
/// </summary>
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

	public Skill(string id, IXMLNode xmlNode, string error){
		Hashtable hashElements = XMLUtils.GetChildren(xmlNode);

		this.id = id;

		name = XMLUtils.GetString(hashElements["Name"] as IXMLNode, null, error);
		textureName = XMLUtils.GetString(hashElements["TextureName"] as IXMLNode, null, error);
		strFlameResource = XMLUtils.GetString(hashElements["ParticleResource"] as IXMLNode, null, error);
		description = XMLUtils.GetString(hashElements["Description"] as IXMLNode, null, error);
		unlockLevel = XMLUtils.GetInt(hashElements["UnlockLevel"] as IXMLNode, 0, error);
		damagePoint = XMLUtils.GetInt(hashElements["DamagePoints"] as IXMLNode, 0, error);
	}
}