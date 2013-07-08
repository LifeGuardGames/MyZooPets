using UnityEngine;
using System.Collections;
using System;

public class LevelUpGUI : MonoBehaviour {
    public NotificationUIManager notificationUIManager;

	// Use this for initialization
	void Start () {
       HUDAnimator.OnLevelUp += OnLevelUpNotification;
	}

    private void OnLevelUpNotification(object senders, EventArgs e){
        notificationUIManager.LevelUpMessage(LevelUpLogic.AwardedTrophy, null);
    }
}
