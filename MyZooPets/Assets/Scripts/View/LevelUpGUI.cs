using UnityEngine;
using System.Collections;
using System;

public class LevelUpGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
       HUDAnimator.OnLevelUp += OnLevelUpNotification;
	}

    private void OnLevelUpNotification(object senders, EventArgs e){
        NotificationUIManager.Instance.LevelUpMessage(LevelUpLogic.AwardedBadge, null);
    }
}
