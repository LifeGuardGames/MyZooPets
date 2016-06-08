using UnityEngine;
using System.Collections;
using System;

public class LevelUpNotificationListener : MonoBehaviour {
	void Start () {
    	HUDAnimator.OnLevelUp += OnLevelUpNotification;
	}

	// Clean up listener;
	void OnDestroy(){
		HUDAnimator.OnLevelUp -= OnLevelUpNotification;
	}

    private void OnLevelUpNotification(object senders, EventArgs e){
    	HUDAnimator.OnLevelUp -= OnLevelUpNotification;

		PopupController.Callback button1Function = delegate(){
            //unregister before registering listener to prevent multiple registering
            //when using spam click the button
            HUDAnimator.OnLevelUp -= OnLevelUpNotification;
        	HUDAnimator.OnLevelUp += OnLevelUpNotification;	
        };

		// Populate notification entry table
		Hashtable notificationEntry = new Hashtable();
		notificationEntry.Add(NotificationPopupData.PrefabName, "PopupLevelUp");
		notificationEntry.Add(NotificationPopupData.Title, null);
		notificationEntry.Add(NotificationPopupData.Message, Localization.Localize(LevelLogic.Instance.GetLevelUpMessage())); 
		notificationEntry.Add(NotificationPopupData.SpecialButtonCallback, button1Function);
		NotificationUIManager.Instance.AddToQueue(notificationEntry);

		AudioManager.Instance.PlayClip("fanfare3");

		//Send Analytics event
		Analytics.Instance.LevelUnlocked(LevelLogic.Instance.CurrentLevel);
    }
}
