using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//index of List<Badge>   = the id of the badges
public class BadgeLogic : MonoBehaviour {
    public List<Badge> badges = new List<Badge>();
    public static int MAX_BADGE_COUNT = 20;

    void Awake(){
    }

	// Use this for initialization
	void Start () {
        //assign handlers	
        HUDAnimator.OnLevelUp += RewardBadgeOnLevelUp;
	}

    void OnDestroy(){
        HUDAnimator.OnLevelUp -= RewardBadgeOnLevelUp;
    }

    //Event listener
    private void RewardBadgeOnLevelUp(object sender, EventArgs e){
        print("reward badge");
        // DataManager.CurrentLevel
        // LevelUpLogic.AwardedBadge
    }
}
