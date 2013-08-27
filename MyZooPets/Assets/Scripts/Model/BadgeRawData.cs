using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[DoNotSerializePublic]
public class BadgeRawData{
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
        if(badgeStatuses.ContainsKey(badgeID)){
            BadgeStatus status = badgeStatuses[badgeID];
            status.IsAwarded = value;
            badgeStatuses[badgeID] = status;
        }
    }

    public void SetBadgeIsBadgeNew(int badgeID, bool value){
        if(badgeStatuses.ContainsKey(badgeID)){
            BadgeStatus status = badgeStatuses[badgeID];
            status.IsBadgeNew = value;
            badgeStatuses[badgeID] = status;
        }
    }

    public void SetBadgeTier(int badgeID, BadgeTier value){
        if(badgeStatuses.ContainsKey(badgeID)){
            BadgeStatus status = badgeStatuses[badgeID];
            status.Tier = value;
            badgeStatuses[badgeID] = status;
        }      
    }

    //======================Initialization====================
    public BadgeRawData(){}

    public void Init(){
      badgeStatuses = new Dictionary<int, BadgeStatus>();
    }
}
