using UnityEngine;
using System.Collections;
using System;

public class LevelUpRewardUIManager : MonoBehaviour {
	// audio to play when notification is shown
	public string sound;
	
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
		notificationEntry.Add(NotificationPopupFields.Message, "Good Work!"); //TODO-s needs to be localized
		notificationEntry.Add(NotificationPopupFields.Button1Callback, null);
		notificationEntry.Add(NotificationPopupFields.Sound, sound );
		
		NotificationUIManager.Instance.AddToQueue(notificationEntry);
    }

}
