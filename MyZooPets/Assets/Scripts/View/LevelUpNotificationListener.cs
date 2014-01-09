using UnityEngine;
using System.Collections;
using System;

public class LevelUpNotificationListener : MonoBehaviour {
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
    	HUDAnimator.OnLevelUp -= OnLevelUpNotification;	

        PopupNotificationNGUI.HashEntry button1Function = delegate(){
        	HUDAnimator.OnLevelUp += OnLevelUpNotification;	
        };

		// Populate notification entry table
		Hashtable notificationEntry = new Hashtable();
		notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.LevelUp);
		notificationEntry.Add(NotificationPopupFields.Message, LevelLogic.Instance.GetLevelUpMessage()); 
		notificationEntry.Add(NotificationPopupFields.Button1Callback, button1Function);
		notificationEntry.Add(NotificationPopupFields.Sound, sound );
		
		NotificationUIManager.Instance.AddToQueue(notificationEntry);

		//Send Analytics event
		Analytics.Instance.LevelUnlocked(LevelLogic.Instance.CurrentLevel);
    }

}
