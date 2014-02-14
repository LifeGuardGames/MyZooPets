using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// BadgeMutableData 
// Save the data for badges. Mutable data
//---------------------------------------------------

public class BadgeMutableData{
    public class Status{
        public bool IsUnlocked {get; set;}
        public bool IsNew {get; set;}

        public Status(){}

        public Status(bool isUnlocked, bool isNew){
            IsUnlocked = isUnlocked; 
            IsNew = isNew; 
        }
    }

    public Dictionary<string, Status> BadgeStatus {get; set;} //Key: Badge ID, Value: instance of status
    public Dictionary<string, int> SingleUnlockProgress {get; set;} //Key: Badge ID, Value: data to check with Badge's unlock condition
    public Dictionary<BadgeType, int> SeriesBadgeUnlockProgress {get; set;} //Key: Badge type, Value: data to check with badges' unlock condition
                                                                //series Badge are accumulative. For example, the level badges are awarded
                                                                //at level 1, 5, 10, 15, 20. Even though all the level badges have their own
                                                                //badge id they are considered as the same type

    //Change badge status. locked or unlocked
    public void UpdateBadgeStatus(string badgeID, bool isUnlocked, bool isNew){
        if(BadgeStatus.ContainsKey(badgeID)){
            Status status = BadgeStatus[badgeID];
            status.IsUnlocked = isUnlocked;
            status.IsNew = isNew;
            BadgeStatus[badgeID] = status;
        }else{
            Status status = new Status(isUnlocked, isNew);
            BadgeStatus.Add(badgeID, status);
        }
    }

    //Update the latest progress for a single badge
    public void UpdateSingleUnlockProgress(string badgeID, int progress){
        if(SingleUnlockProgress.ContainsKey(badgeID)){
            SingleUnlockProgress[badgeID] = progress;
        }else{
            SingleUnlockProgress.Add(badgeID, progress);
        }
    }

    //Update the latest progress for a badge type
    public void UpdateSeriesUnlockProgress(BadgeType type, int progress){
        if(SeriesBadgeUnlockProgress.ContainsKey(type)){
            SeriesBadgeUnlockProgress[type] = progress;
        }else{
            SeriesBadgeUnlockProgress.Add(type, progress);
        }
    }

    public bool GetIsUnlocked(string badgeID){
        bool retVal = false;
        if(BadgeStatus.ContainsKey(badgeID)){
            retVal = BadgeStatus[badgeID].IsUnlocked;
        }
        return retVal;
    }

    public bool GetIsNew(string badgeID){
        bool retVal = false;
        if(BadgeStatus.ContainsKey(badgeID)){
            retVal = BadgeStatus[badgeID].IsNew;
        }
        return retVal;
    }

    public int GetSingleUnlockProgress(string badgeID){
        int retVal = 0;
        if(SingleUnlockProgress.ContainsKey(badgeID)){
            retVal = SingleUnlockProgress[badgeID];
        }
        return retVal;
    }

    public int GetSeriesUnlockProgress(BadgeType type){
        int retVal = 0;
        if(SeriesBadgeUnlockProgress.ContainsKey(type)){
            retVal = SeriesBadgeUnlockProgress[type];
        }
        return retVal;
    }

    //============================Initialization===============================
    public BadgeMutableData(){
        Init();
    }

    private void Init(){
        BadgeStatus = new Dictionary<string, Status>();
        SingleUnlockProgress = new Dictionary<string, int>();
        SeriesBadgeUnlockProgress = new Dictionary<BadgeType, int>();
    }
}
