using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//Save the data for badges. Unlock progress and isUnlocked?

[DoNotSerializePublic]
public class BadgeMutableData{
    private struct Status{
        public bool isUnlocked;
        public bool isNew;

        public Status(bool isUnlocked, bool isNew){
            this.isUnlocked = isUnlocked;
            this.isNew = isNew;
        }
    }

    [SerializeThis]
    private Dictionary<string, Status> badgeStatus; //Key: Badge ID, Value: instance of status
    [SerializeThis]
    private Dictionary<string, int> singleBadgeUnlockProgress; //Key: Badge ID, Value: data to check with Badge's unlock condition
    [SerializeThis]
    private Dictionary<BadgeType, int> seriesBadgeUnlockProgress; //Key: Badge type, Value: data to check with badges' unlock condition
                                                                //series Badge are accumulative. For example, the level badges are awarded
                                                                //at level 1, 5, 10, 15, 20. Even though all the level badges have their own
                                                                //badge id they are considered as the same type

    public void UpdateBadgeStatus(string badgeID, bool isUnlocked, bool isNew){
        if(badgeStatus.ContainsKey(badgeID)){
            Status status = badgeStatus[badgeID];
            status.isUnlocked = isUnlocked;
            status.isNew = isNew;
            badgeStatus[badgeID] = status;
        }else{
            Status status = new Status(isUnlocked, isNew);
            badgeStatus.Add(badgeID, status);
        }
    }

    public void UpdateSingleUnlockProgress(string badgeID, int progress){
        if(singleBadgeUnlockProgress.ContainsKey(badgeID)){
            singleBadgeUnlockProgress[badgeID] = progress;
        }else{
            singleBadgeUnlockProgress.Add(badgeID, progress);
        }
    }

    public void UpdateSeriesUnlockProgress(BadgeType type, int progress){
        if(seriesBadgeUnlockProgress.ContainsKey(type)){
            seriesBadgeUnlockProgress[type] = progress;
        }else{
            seriesBadgeUnlockProgress.Add(type, progress);
        }
    }

    public bool GetIsUnlocked(string badgeID){
        bool retVal = false;
        if(badgeStatus.ContainsKey(badgeID)){
            retVal = badgeStatus[badgeID].isUnlocked;
        }
        return retVal;
    }

    public bool GetIsNew(string badgeID){
        bool retVal = false;
        if(badgeStatus.ContainsKey(badgeID)){
            retVal = badgeStatus[badgeID].isNew;
        }
        return retVal;
    }

    public int GetSingleUnlockProgress(string badgeID){
        int retVal = 0;
        if(singleBadgeUnlockProgress.ContainsKey(badgeID)){
            retVal = singleBadgeUnlockProgress[badgeID];
        }
        return retVal;
    }

    public int GetSeriesUnlockProgress(BadgeType type){
        int retVal = 0;
        if(seriesBadgeUnlockProgress.ContainsKey(type)){
            retVal = seriesBadgeUnlockProgress[type];
        }
        return retVal;
    }

    //============================Initialization===============================
    public BadgeMutableData(){}

    public void Init(){
        badgeStatus = new Dictionary<string, Status>();
        singleBadgeUnlockProgress = new Dictionary<string, int>();
        seriesBadgeUnlockProgress = new Dictionary<BadgeType, int>();
    }
}
