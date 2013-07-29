using UnityEngine;
using System.Collections;

[DoNotSerializePublic]
public class BadgeData{
    [SerializeThis]
    private bool isAwarded;
    [SerializeThis]
    private BadgeTier tier;
    [SerializeThis]
    private bool isBadgeNew;

    public bool IsAwarded{
        get{return isAwarded;}
        set{isAwarded = value;}
    }
    public BadgeTier Tier{
        get{return tier;}
        set{tier = value;}
    }
    public bool IsBadgeNew{
        get{return isBadgeNew;}
        set{isBadgeNew = value;}
    }

    public BadgeData(){
        isAwarded = false;
        isBadgeNew = false;
        tier = BadgeTier.Null;
    }
}
