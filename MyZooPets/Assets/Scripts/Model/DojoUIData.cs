using UnityEngine;
using System.Collections;

[System.Serializable]
public class DojoUIData{
    //Needs to be public to be serialize by inspector. 
    //DO NOT!! use these variables. Use the Getter & Setter instead
    public DojoSkillName skillName; //name of the dojo skill
    public int skillID; //numberic id of the skill
    public string desc; //description of the dojo skill.

    //TO DO use atlas name or sprite collection name
    public string iconName; 
    public string petAnimationName; //name of the animation in sprite collection
    public string gestureAnimationName; //name of the gesture animation in sprite collection
    public int costStars; //how much it costs to buy the dojo skill

    // public bool isUnLocked; //has the skill been unlocked
    //====================Getters======================
    public DojoSkillName SkillName{ 
        get{return skillName;}
    }
    public int SkillID{
        get{return skillID;}
    }
    public string Desc{ 
        get{return desc;}
    }
    public string IconName{
        get{return iconName;}
    }
    public string PetAnimationName{ 
        get{return petAnimationName;}
    }
    public string GestureAnimationName{ 
        get{return gestureAnimationName;}
    }
    public int CostStars{ 
        get{return costStars;}
    }

    //The following data are dynamic so they need to be stored in datamanager and
    //serialize. The following getters/setters are wrappers that expose the raw data
    //to the UI layer
    public bool IsUnlocked{ 
        get{return DataManager.Instance.Dojo.GetSkillIsUnlocked(skillID);}
        set{DataManager.Instance.Dojo.SetSkillIsUnlocked(skillID, value);}
    }
    public bool IsPurchased{
        get{return DataManager.Instance.Dojo.GetSkillIsPurchased(skillID);}
        set{DataManager.Instance.Dojo.SetSkillIsPurchased(skillID, value);}
    }
    //==============================================

    public DojoUIData(){}
}
