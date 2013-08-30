using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//index of List<Badge> = the id of the badges
public class BadgeLogic : Singleton<BadgeLogic> {
    public static event EventHandler<EventArgs> OnNewBadgeAdded; //Event fires when new badge has been added
    public static int MAX_BADGE_COUNT = 20;
    public List<BadgeUIData> badges = new List<BadgeUIData>();

    //Read Only. Return a list of badges. Refer to Model/Badge for more documentation
    public List<BadgeUIData> Badges{
        get{ return badges;}
    }
    //Read Only. Return a list of Level Badges.
    public List<BadgeUIData> LevelBadges{
        get{ return badges.FindAll(badge => badge.Type.Equals(BadgeType.Level));}
    }

	// Use this for initialization
	void Start () {
        //assign listeners	
        HUDAnimator.OnLevelUp += RewardBadgeOnLevelUp;
        RewardBadgeOnLevelUp();
	}

    void OnDestroy(){
        HUDAnimator.OnLevelUp -= RewardBadgeOnLevelUp;
    }

    //Event listener
    private void RewardBadgeOnLevelUp(object sender, EventArgs e){
        RewardBadgeOnLevelUp();
        if(D.Assert(OnNewBadgeAdded != null, "OnNewBadgeAdded has no listeners"))
            OnNewBadgeAdded(this, EventArgs.Empty);
    }

    private void RewardBadgeOnLevelUp(){
        int badgeIndex = (int) DataManager.Instance.Level.CurrentLevel;

        BadgeUIData badge = badges.Find(entity => entity.ID == badgeIndex);
        if(D.Assert(badge != null) && !badge.IsAwarded){
            badge.IsAwarded = true;
            badge.Tier = LevelUpLogic.Instance.AwardedBadge;
        }      
    }
}
