using UnityEngine;
using System.Collections;

//this logic should be in any class that an asthma flare up could happen
//when the flare up happens this logic will notify the NotificationUIManager
//and pauses the game
public class DiagnoseTimerLogic : MonoBehaviour {
    private float timer = 0;
    private float timeInterval = 40f; //time interval for triggers to affect health
    private bool allowTimer = true;

	// Use this for initialization
	void Start () {
        timer = timeInterval;
	}

	// Update is called once per frame
	void Update () {
        if(allowTimer){
            timer -= Time.deltaTime;
            if (timer <= 0){
                timer = timeInterval;
                SendNotification();
                allowTimer = false;
            }
        }
	}
	
    //Increases the chance of this happening if health is low
    private void SendNotification(){
		
		// Assign delegate functions to be passed in hashtable
		PopupNotificationNGUI.HashEntry button1Function = delegate(){
                Application.LoadLevel("DiagnosePet");
            };
		PopupNotificationNGUI.HashEntry button2Function = delegate() {
				//ignore. no punishment. unpause the game
                //fewer rewards
			};
		
		// Populate notification entry table
		Hashtable notificationEntry = new Hashtable();
		notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.TwoButtons);
		notificationEntry.Add(NotificationPopupFields.Message, Localization.Localize("NOTIFICATION_DIAGNOSE"));
		notificationEntry.Add(NotificationPopupFields.Button1Label, Localization.Localize("OKAY"));
		notificationEntry.Add(NotificationPopupFields.Button1Callback, button1Function);
		notificationEntry.Add(NotificationPopupFields.Button2Label, Localization.Localize("IGNORE"));
		notificationEntry.Add(NotificationPopupFields.Button2Callback, button2Function);
		
		// Place notification entry table in static queue
		NotificationUIManager.Instance.AddToQueue(notificationEntry);
    }
}
