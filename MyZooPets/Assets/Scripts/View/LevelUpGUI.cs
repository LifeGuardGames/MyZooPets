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
		// Populate notification entry table
		Hashtable notificationEntry = new Hashtable();
		notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.LevelUp);
		notificationEntry.Add(NotificationPopupFields.Button1Callback, null);
		
		NotificationUIManager.Instance.AddToQueue(notificationEntry);
    }

}
