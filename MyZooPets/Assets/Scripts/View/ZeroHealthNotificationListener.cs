using UnityEngine;
using System;
using System.Collections;

public class ZeroHealthNotificationListener : MonoBehaviour {
    public int hospitalBillCost = 300;
	public int moodPunishment = 30;

	// Use this for initialization
	void Start () {
        StatsManager.OnZeroHealth += OnZeroHealthNotification;
	}

    void OnDestroy(){
        StatsManager.OnZeroHealth -= OnZeroHealthNotification;    
    }

    private void OnZeroHealthNotification(object sender, EventArgs args){
        //Unregister the handler so we don't get multiple notifications of the same thing
        StatsManager.OnZeroHealth -= OnZeroHealthNotification;    

        PopupNotificationNGUI.Callback button1Function = delegate(){

			StatsManager.Instance.ChangeStats(coinsDelta: -1 * hospitalBillCost,
			                                     healthDelta: 100, hungerDelta: -1 * moodPunishment, isFloaty: false);

            //Register the handler again after the notification has been cleared
            StatsManager.OnZeroHealth += OnZeroHealthNotification;
        };

        string petName = DataManager.Instance.GameData.PetInfo.PetName;
        string message = String.Format(Localization.Localize("ZERO_HEALTH"), 
            petName, hospitalBillCost.ToString());

        Hashtable notificationEntry = new Hashtable();
        //notificationEntry.Add(NotificationPopupData.Type, NotificationPopupType.ZeroHealth);
        //notificationEntry.Add(NotificationPopupData.Message, message); 
        //notificationEntry.Add(NotificationPopupData.Button1Callback, button1Function); 
        
        NotificationUIManager.Instance.AddToQueue(notificationEntry);

        //Send analytics event
        Analytics.Instance.ZeroHealth();
    }
}
