using UnityEngine;
using System.Collections;

[System.Serializable]
public class Badge{
    //needs to be public to be assigned in inspector, but please don't use this
    //variables use the getters and setters
    public string name;
    public BadgeType type;
    public string description;
    public int id;

    //===================Getters & Setters================
    public string Name{
        get{return name;}
    }
    public BadgeType Type{
        get{return type;}
    }
    public string Descriptions{
        get{return description;}
    }
    public bool IsAwarded{ //returns data that were serialized from DataManager
        get{return DataManager.BadgeStatus[id].IsAwarded;}
        set{DataManager.BadgeStatus[id].IsAwarded = value;}
    }
    public BadgeTier Tier{ //get data from DataManager
        get{return DataManager.BadgeStatus[id].Tier;}
        set{DataManager.BadgeStatus[id].Tier = value;}
    }
    //========================================
}
