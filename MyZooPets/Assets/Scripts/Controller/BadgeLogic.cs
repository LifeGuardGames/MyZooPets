using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//index of List<Badge> = the id of the badges
public class BadgeLogic : Singleton<BadgeLogic> {
    //=============For inspector use only. don't use these variables in UI
    public List<Badge> badges = new List<Badge>();
    //===========================================
    public static int MAX_BADGE_COUNT = 20;
    
    //===================Events======================
    public static event EventHandler<EventArgs> OnNewBadgeAdded; //Event fires when new badge has been added
    //==============================================
    
    //====================API========================
    //Read Only. Return a list of badges. Refer to Model/Badge for more documentation
    public List<Badge> Badges{
        get{ return badges;}
    }
    //Read Only. Return a list of Level Badges.
    public List<Badge> LevelBadges{
        get{return badges.FindAll(badge => badge.Type.Equals(BadgeType.Level));}
    }
    //==============================================

    void Awake(){
    }

	// Use this for initialization
	void Start () {
        //assign listeners	
        HUDAnimator.OnLevelUp += RewardBadgeOnLevelUp;
	}

    void OnDestroy(){
        HUDAnimator.OnLevelUp -= RewardBadgeOnLevelUp;
    }

    //Event listener
    private void RewardBadgeOnLevelUp(object sender, EventArgs e){
        int badgeIndex = (int) DataManager.Instance.Level.CurrentLevel;
        //award badge and make them show in UI
        DataManager.Instance.BadgeStatus[badgeIndex].IsAwarded = true;
        DataManager.Instance.BadgeStatus[badgeIndex].Tier = LevelUpLogic.AwardedBadge;
        if(D.Assert(OnNewBadgeAdded != null, "OnNewBadgeAdded has no listeners"))
            OnNewBadgeAdded(this, EventArgs.Empty);
    }
}
