using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

//-----------------------
//Provide data for BadgeUIManager
//Checks and manages when a badge should be unlocked
//-----------------------
public class BadgeLogic : Singleton<BadgeLogic> {
    public static event EventHandler<BadgeEventArgs> OnNewBadgeUnlocked; //Event fires when new badge has been added
    public class BadgeEventArgs : EventArgs{
        private Badge unlockedBadge;

        public Badge UnlockedBadge{
            get{return unlockedBadge;}
        }

        public BadgeEventArgs(Badge badge){
            unlockedBadge = badge;
        }
    }

    private List<Badge> allBadges; //List of all badges

    public List<Badge> AllBadges{
        get{return allBadges;}
    }

    void Awake(){
        Dictionary<string, Badge> badgesDict = DataBadges.GetAllBadges();
        allBadges = SelectListFromDictionary(badgesDict);
    }

    //Return badge with badgeID
    public Badge GetBadge(string badgeID){
        return DataBadges.GetBadge(badgeID);
    }

    //Use this function to check which one of the badges with badgeType can be unlocked
    //PARAMETERS: badgeType, currentProgress, overrideProgress.
    //            overrideProgress is used to specify whether currentProgress should replace
    //            the recorded progress from DataManager or add to the recorded progress.
    public void CheckSeriesUnlockProgress(BadgeType badgeType, int currentProgress, bool overrideProgress){
        int latestProgress;
        bool unlockedAllSeriesBadges = true;

        var sortedBadgesType = from badge in allBadges
                                where badge.Type == badgeType
                                orderby badge.UnlockCondition ascending
                                select badge;

        //Decides to override or add to recorded progress from DataManager
        if(overrideProgress){
            latestProgress = currentProgress;
        }else{
            int progress = DataManager.Instance.GameData.Badge.GetSeriesUnlockProgress(badgeType);
            latestProgress = progress += currentProgress;
        }

        //Check if a new badge can be unlocked. Multiple badges of the same type 
        //can be unlock at the same time
        foreach(Badge badge in sortedBadgesType)
            CheckUnlockProgress(badge, latestProgress);

        //Check if all badges of the same type have been unlocked
        foreach(Badge badge in sortedBadgesType){
            if(!badge.IsUnlocked){
                unlockedAllSeriesBadges = false;
                break;
            }
        }

        //Only update DataManager if there are still locked badges
        if(!unlockedAllSeriesBadges)
            DataManager.Instance.GameData.Badge.UpdateSeriesUnlockProgress(badgeType, latestProgress);
    }

    //Use this function to check if badge with badgeID can be unlocked
    public void CheckSingleUnlockProgress(string badgeID, int currentProgress, bool overrideProgress){
        int latestProgress;
        Badge badge = DataBadges.GetBadge(badgeID);

        //Decides to override or add to recorded progress from DataManager
        if(overrideProgress){
            latestProgress = currentProgress;
        }else{
            int progress = DataManager.Instance.GameData.Badge.GetSingleUnlockProgress(badgeID);
            latestProgress = progress += currentProgress;
        }

        //Update DataManager only if badge with badgeID is still locked
        if(!CheckUnlockProgress(badge, latestProgress))
            DataManager.Instance.GameData.Badge.UpdateSingleUnlockProgress(badgeID, latestProgress);
    }

    //Check if badge unlock progress meets the unlock condition
    //True: new badge unlocked
    private bool CheckUnlockProgress(Badge badge, int progress){
        bool unlockNewBadge = false;

        if(progress >= badge.UnlockCondition){ //Check if progress matches unlock conditions
            bool isUnlocked = DataManager.Instance.GameData.Badge.GetIsUnlocked(badge.ID);

            if(!isUnlocked){ //Unlock new badges
                DataManager.Instance.GameData.Badge.UpdateBadgeStatus(badge.ID, true, true);
                // Debug.Log("Unlock: " + badge.Name);

                //Send analytics 
                Analytics.Instance.BadgeUnlocked(badge.ID);

                //Fire event for UI update
                if(OnNewBadgeUnlocked != null){
                    BadgeEventArgs arg = new BadgeEventArgs(badge);
                    OnNewBadgeUnlocked(this, arg);
                }

                unlockNewBadge = true;
            }
        }

        return unlockNewBadge;
    }

    //Return a list from dictionary. 
    private List<Badge> SelectListFromDictionary(Dictionary<string, Badge> badgeDict){
        List<Badge> badgeList = (from keyValuePair in badgeDict
                                    select keyValuePair.Value).ToList();
        return badgeList;
    }
}
