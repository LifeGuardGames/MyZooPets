using UnityEngine;
using System.Collections;
using System;

public class LevelUpGUI : MonoBehaviour {
	// audio to play when notification is shown
	public AudioClip sound;
	
	void Start () {
    	HUDAnimator.OnLevelUp += OnLevelUpNotification;
	}

	// Clean up listener;
	void OnDestroy(){
		HUDAnimator.OnLevelUp -= OnLevelUpNotification;
	}

    private void OnLevelUpNotification(object senders, EventArgs e){
		// Populate notification entry table
		Hashtable notificationEntry = new Hashtable();
		notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.LevelUp);
		notificationEntry.Add(NotificationPopupFields.Badge, LevelUpLogic.Instance.AwardedBadge);
		notificationEntry.Add(NotificationPopupFields.Button1Callback, null);
		notificationEntry.Add(NotificationPopupFields.Sound, sound );
		
		NotificationUIManager.Instance.AddToQueue(notificationEntry);
//        NotificationUIManager.Instance.EnqueueLevelUpMessage(LevelUpLogic.Instance.AwardedBadge, null);
    }

}
