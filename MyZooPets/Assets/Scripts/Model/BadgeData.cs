using UnityEngine;
using System.Collections;

[DoNotSerializePublic]
public class BadgeData{
    [SerializeThis]
    private bool isAwarded;
    [SerializeThis]
    private BadgeTier tier;

    public bool IsAwarded{
        get{return isAwarded;}
        set{isAwarded = value;}
    }
    public BadgeTier Tier{
        get{return tier;}
        set{tier = value;}
    }

    public BadgeData(){
        isAwarded = false;
        tier = BadgeTier.Null;
    }
}
