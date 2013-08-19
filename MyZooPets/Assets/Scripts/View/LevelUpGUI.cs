using UnityEngine;
using System.Collections;
using System;

public class LevelUpGUI : MonoBehaviour {

	void Start () {
    	HUDAnimator.OnLevelUp += OnLevelUpNotification;
	}

	// Clean up listener;
	void OnDestroy(){
		HUDAnimator.OnLevelUp -= OnLevelUpNotification;
	}

    private void OnLevelUpNotification(object senders, EventArgs e){
        NotificationUIManager.Instance.EnqueueLevelUpMessage(LevelUpLogic.AwardedBadge, null);
    }

}
