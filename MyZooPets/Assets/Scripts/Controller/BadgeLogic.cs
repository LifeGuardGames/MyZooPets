﻿using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

//-----------------------
//Provide data for BadgeUIManager
//Checks and manages when a badge should be unlocked
//-----------------------
public class BadgeLogic : Singleton<BadgeLogic> {
    public static event EventHandler<EventArgs> OnNewBadgeAdded; //Event fires when new badge has been added

    private List<Badge> allBadges;

    public List<Badge> AllBadges{
        get{
            if(allBadges == null){
                allBadges = new List<Badge>();
                Dictionary<string, Badge> badgesDict = DataBadges.GetAllBadges();
                allBadges = SelectListFromDictionary(badgesDict);
            }
            return allBadges;
        }
    }

    void Awake(){
        DataBadges.SetupData(); 
    }

    //Use this function to check which one of the badges with badgeType can be unlocked
    //PARAMETERS: badgeType, currentProgress, overrideProgress.
    //            overrideProgress is used to specify whether currentProgress should replace
    //            the recorded progress from DataManager or add to the recorded progress.
    public void CheckSeriesUnlockProgress(BadgeType badgeType, int currentProgress, bool overrideProgress){
        int latestProgress;
        bool unlockedAllSeriesBadges = true;

        var sortedBadgesType = from badge in AllBadges
                                where badge.Type == badgeType
                                orderby badge.UnlockCondition ascending
                                select badge;

        //Decides to override or add to recorded progress from DataManager
        if(overrideProgress){
            latestProgress = currentProgress;
        }else{
            int progress = DataManager.Instance.Badge.GetSeriesUnlockProgress(badgeType);
            latestProgress = progress += currentProgress;
        }

        //Check if a new badge can be unlocked
        foreach(Badge badge in sortedBadgesType)
            if(CheckUnlockProgress(badge, latestProgress)) break;

        //Check if all badges of the same type have been unlocked
        foreach(Badge badge in sortedBadgesType){
            if(!badge.IsUnlocked){
                unlockedAllSeriesBadges = false;
                break;
            }
        }

        //Only update DataManager if there are still locked badges
        if(!unlockedAllSeriesBadges)
            DataManager.Instance.Badge.UpdateSeriesUnlockProgress(badgeType, latestProgress);
    }

    //Use this function to check if badge with badgeID can be unlocked
    public void CheckSingleUnlockProgress(string badgeID, int currentProgress, bool overrideProgress){
        int latestProgress;
        Badge badge = DataBadges.GetBadge(badgeID);

        //Decides to override or add to recorded progress from DataManager
        if(overrideProgress){
            latestProgress = currentProgress;
        }else{
            int progress = DataManager.Instance.Badge.GetSingleUnlockProgress(badgeID);
            latestProgress = progress += currentProgress;
        }

        //Update DataManager only if badge with badgeID is still locked
        if(!CheckUnlockProgress(badge, latestProgress))
            DataManager.Instance.Badge.UpdateSingleUnlockProgress(badgeID, latestProgress);
    }

    //Check if badge unlock progress meets the unlock condition
    //True: new badge unlocked
    private bool CheckUnlockProgress(Badge badge, int progress){
        bool unlockNewBadge = false;

        if(progress >= badge.UnlockCondition){ //Check if progress matches unlock conditions
            bool isUnlocked = DataManager.Instance.Badge.GetIsUnlocked(badge.ID);

            if(!isUnlocked){ //Unlock new badges
                DataManager.Instance.Badge.UpdateBadgeStatus(badge.ID, true, true);

                if(OnNewBadgeAdded != null)
                    OnNewBadgeAdded(this, EventArgs.Empty);

                print(badge.Name);
                unlockNewBadge = true;
            }
        }

        return unlockNewBadge;
    }

    //Return a list from dictionary
    private List<Badge> SelectListFromDictionary(Dictionary<string, Badge> badgeDict){
        List<Badge> badgeList = (from keyValuePair in badgeDict
                                    select keyValuePair.Value).ToList();
        return badgeList;
    }
}
