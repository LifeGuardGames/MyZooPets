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

        PopupNotificationNGUI.Callback button1Function = delegate(){
            //unregister before registering listener to prevent multiple registering
            //when using spam click the button
            HUDAnimator.OnLevelUp -= OnLevelUpNotification;
        	HUDAnimator.OnLevelUp += OnLevelUpNotification;	
        };

		// Populate notification entry table
		Hashtable notificationEntry = new Hashtable();
		//notificationEntry.Add(NotificationPopupData.Type, NotificationPopupType.LevelUp);
		//notificationEntry.Add(NotificationPopupData.Message, LevelLogic.Instance.GetLevelUpMessage()); 
		//notificationEntry.Add(NotificationPopupData.Button1Callback, button1Function);
		//notificationEntry.Add(NotificationPopupData.Sound, sound );
		
		NotificationUIManager.Instance.AddToQueue(notificationEntry);

		//Send Analytics event
		Analytics.Instance.LevelUnlocked(LevelLogic.Instance.CurrentLevel);
    }

}
