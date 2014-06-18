using UnityEngine;
using System;
using System.Collections;

public class ImmutableDataPetLevel{
    private Level id;
    private string levelUpMessage;
    private int levelUpCondition;

    public int Level{
        get{return (int)id;}
    }    
    public string LevelUpMessage{
        get{return Localization.Localize(levelUpMessage);}
    }
    public int LevelUpCondition{
        get{return levelUpCondition;}
    }

    public ImmutableDataPetLevel(string id, IXMLNode xmlNode, string errorMessage){
		Hashtable hashElements = XMLUtils.GetChildren(xmlNode);

        this.id = (Level) Enum.Parse(typeof(Level), id);
        levelUpCondition = XMLUtils.GetInt(hashElements["LevelUpCondition"] as IXMLNode);
        levelUpMessage = XMLUtils.GetString(hashElements["LevelUpMessage"] as IXMLNode);
    }

}
