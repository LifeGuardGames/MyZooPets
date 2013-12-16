using UnityEngine;
using System.Collections;

public class PetLevel{
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

    public PetLevel(Level id, Hashtable hashItemData){
        this.id = id;
        levelUpCondition = XMLUtils.GetInt(hashItemData["LevelUpCondition"] as IXMLNode);
        levelUpMessage = XMLUtils.GetString(hashItemData["LevelUpMessage"] as IXMLNode);
    }

}
