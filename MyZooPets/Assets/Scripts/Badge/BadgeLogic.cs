using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Badge logic.Checks and manages when a badge should be unlocked
/// </summary>
public class BadgeLogic : Singleton<BadgeLogic>{
    public static event EventHandler<BadgeEventArgs> OnNewBadgeUnlocked; //Event fires when new badge has been added
    public class BadgeEventArgs : EventArgs{
        private Badge unlockedBadge;

        public Badge UnlockedBadge{
            get{ return unlockedBadge; }
        }

        public BadgeEventArgs(Badge badge){
            unlockedBadge = badge;
        }
    }

    private List<Badge> allBadges; //List of all badges

    public List<Badge> AllBadges{
        get{ return allBadges; }
    }

    void Awake(){
        allBadges = DataLoaderBadges.GetDataList();
    }

    /// <summary>
    /// Gets the badge.
    /// </summary>
    /// <returns>The badge.</returns>
    /// <param name="badgeID">Badge ID.</param>
    public Badge GetBadge(string badgeID){
        return DataLoaderBadges.GetData(badgeID);
    }

	/// <summary>
	/// Determines whether the badge is unlocked or not
	/// </summary>
	/// <returns><c>true</c> if badge unlocked; otherwise, <c>false</c>.</returns>
	/// <param name="badgeID">Badge ID</param>
	public bool IsBadgeUnlocked(string badgeID){
		return DataManager.Instance.GameData.Badge.GetIsUnlocked(badgeID);
	}

	public int GetUnlockedBadgesCount(){
		int count = 0;
		foreach(Badge badge in allBadges){
			if(IsBadgeUnlocked(badge.ID)){
				count++;
			}
		}
		return count;
	}

	/// <summary>
	/// Gets the badge unlock at next level.
	/// </summary>
	/// <returns>The badge unlock at next level.</returns>
    public Badge GetBadgeUnlockAtNextLevel(){
        int nextLevel = LevelLogic.Instance.NextLevel;
        Badge selectedBadge = null;
        
        foreach(Badge badge in allBadges){
            if(badge.Type == BadgeType.Level && badge.UnlockCondition == nextLevel){
                selectedBadge = badge;
                break;
            }
        }
        
        // selectedBadge = (from badge in allBadges
        //                 where badge.Type == BadgeType.Level &&
        //                     badge.UnlockCondition == nextLevel 
        //                 select badge).First();

        return selectedBadge;
    }
	
	/// <summary>
	/// Checks the series unlock progress.
	/// </summary>
	/// <param name="badgeType">Badge type.</param>
	/// <param name="currentProgress">Current progress.</param>
	/// <param name="overrideProgress">If set to <c>true</c> override progress from DataManager. else add to the record progress</param>
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

        foreach(Badge badge in sortedBadgesType){
			CheckUnlockProgress(badge, latestProgress);
		}

        //Check if all badges of the same type have been unlocked
        foreach(Badge badge in sortedBadgesType){
            if(!badge.IsUnlocked){
                unlockedAllSeriesBadges = false;
                break;
            }
        }

        //Only update DataManager if there are still locked badges
        if(!unlockedAllSeriesBadges){
            DataManager.Instance.GameData.Badge.UpdateSeriesUnlockProgress(badgeType, latestProgress);
		}
    }
	
	/// <summary>
	/// Checks the single unlock progress.
	/// </summary>
	/// <param name="badgeID">Badge I.</param>
	/// <param name="currentProgress">Current progress.</param>
	/// <param name="overrideProgress">If set to <c>true</c> override progress.</param>
    public void CheckSingleUnlockProgress(string badgeID, int currentProgress, bool overrideProgress){
        int latestProgress;
        Badge badge = DataLoaderBadges.GetData(badgeID);

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
	
	/// <summary>
	/// Checks the unlock progress meets the unlock condition
	/// </summary>
	/// <returns><c>true</c>, if new badge unlocked.</returns>
	/// <param name="badge">Badge.</param>
	/// <param name="progress">Progress.</param>
    private bool CheckUnlockProgress(Badge badge, int progress){
        bool unlockNewBadge = false;

        if(progress >= badge.UnlockCondition){ //Check if progress matches unlock conditions
            bool isUnlocked = IsBadgeUnlocked(badge.ID);

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
}
