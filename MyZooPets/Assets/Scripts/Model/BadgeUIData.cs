using UnityEngine;
using System.Collections;

[System.Serializable]
public class BadgeUIData{
    //needs to be public to be assigned in inspector, but please don't use this
    //variables use the getters and setters
    public string name;
    public BadgeType type;
    public string description;
    public int id;

    //===================Getters & Setters================
    //Name of the badge
    public string Name{
        get{return name;}
    }
    //Type of the badge
    public BadgeType Type{
        get{return type;}
    }
    //ID of the badge
    public int ID{
        get{return id;}
    }
    //Short description of how to obtain the badge
    public string Descriptions{
        get{return description;}
    }
    //True: Show badge in UI, False: Black out
    public bool IsAwarded{ //returns data that were serialized fromDataManager.Instance 
        get{return DataManager.Instance.BadgeStatus[id].IsAwarded;}
        set{DataManager.Instance.BadgeStatus[id].IsAwarded = value;}
    }
    //If badge type is Level, then Tier tells UI what kind of badge to show (gold, silver, bronze)
    public BadgeTier Tier{ //get data fromDataManager.Instance 
        get{return DataManager.Instance.BadgeStatus[id].Tier;}
        set{DataManager.Instance.BadgeStatus[id].Tier = value;}
    }
    //True: New Badge show fancy animation, False: no animation
    public bool IsBadgeNew{
        get{return DataManager.Instance.BadgeStatus[id].IsBadgeNew;}
        set{DataManager.Instance.BadgeStatus[id].IsBadgeNew = value;}        
    }
    //========================================
}
