using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[DoNotSerializePublic]
public class BadgeRawData{
    // [SerializeThis]
    // private bool isAwarded;
    // [SerializeThis]
    // private BadgeTier tier;
    // [SerializeThis]
    // private bool isBadgeNew;

    // public bool IsAwarded{
    //     get{return isAwarded;}
    //     set{isAwarded = value;}
    // }
    // public BadgeTier Tier{
    //     get{return tier;}
    //     set{tier = value;}
    // }
    // public bool IsBadgeNew{
    //     get{return isBadgeNew;}
    //     set{isBadgeNew = value;}
    // }

    // public BadgeData(){
    //     isAwarded = false;
    //     isBadgeNew = false;
    //     tier = BadgeTier.Null;
    // }

    private struct BadgeStatus{
        public bool IsAwarded {get; set;}
        public BadgeTier Tier {get; set;}
        public bool IsBadgeNew {get ;set;}

        public BadgeStatus(bool isAwarded, bool isBadgeNew, BadgeTier tier){
           IsAwarded = isAwarded;
           IsBadgeNew = isBadgeNew;
           Tier = tier;
        }
    }

    [SerializeThis]
    private Dictionary<int, BadgeStatus> badgeStatuses; //Key: ID of the badge, value: badge status


     /*
        Get IsUnlocked field for entity with badgeID 
        if the entity with badgeID can't be found that means it hasn't been initialized
        add the entity into the dictionary

        This is assuming that GetBadgeIsAwarded will be call first.
    */
    public bool GetBadgeIsAwarded(int badgeID){
       bool retVal = false;
       BadgeStatus status;
       if(badgeStatuses.TryGetValue(badgeID, out status)) {
            retVal = status.IsAwarded;
       }else{
            status = new BadgeStatus(false, true, BadgeTier.Null);
            badgeStatuses.Add(badgeID, status);
            retVal = status.IsAwarded;
       }
       return retVal;
    }

    public bool GetBadgeIsBadgeNew(int badgeID){
       bool retVal = false;
       BadgeStatus status;
       if(badgeStatuses.TryGetValue(badgeID, out status)) {
            retVal = status.IsBadgeNew;
       }
       return retVal;
    }
    
    public BadgeTier GetBadgeTier(int badgeID){
        BadgeTier retVal = BadgeTier.Null;
        BadgeStatus status;
       if(badgeStatuses.TryGetValue(badgeID, out status)) {
            retVal = status.Tier;
       }
       return retVal;
    }

    public void SetBadgeIsAwarded(int badgeID, bool value){
        BadgeStatus status;
        if(badgeStatuses.TryGetValue(badgeID, out status)){
            status.IsAwarded = value;
        }
    }

    public void SetBadgeIsBadgeNew(int badgeID, bool value){
        BadgeStatus status;
        if(badgeStatuses.TryGetValue(badgeID, out status)){
            status.IsBadgeNew = value;
        }
    }

    public void SetBadgeTier(int badgeID, BadgeTier tier){
        BadgeStatus status;
        if(badgeStatuses.TryGetValue(badgeID, out status)){
            status.Tier = tier;
        }
    }

    //======================Initialization====================
    public BadgeRawData(){}

    public void Init(){
      badgeStatuses = new Dictionary<int, BadgeStatus>();
    }
}
