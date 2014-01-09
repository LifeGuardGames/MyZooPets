using UnityEngine;
using System;
using System.Collections;

public class ZeroHealthNotificationListener : MonoBehaviour {
    public int hospitalBillCost = 300;

	// Use this for initialization
	void Start () {
        StatsController.OnZeroHealth += OnZeroHealthNotification;
	}

    void OnDestroy(){
        StatsController.OnZeroHealth -= OnZeroHealthNotification;    
    }

    private void OnZeroHealthNotification(object sender, EventArgs args){
        //Unregister the handler so we don't get multiple notifications of the same thing
        StatsController.OnZeroHealth -= OnZeroHealthNotification;    

        PopupNotificationNGUI.HashEntry button1Function = delegate(){
            StatsController.Instance.ChangeStats(0, Vector3.zero, -1 * hospitalBillCost, 
                Vector3.zero, 100, Vector3.zero, -30, Vector3.zero, true, bFloaty: false);

            //Register the handler again after the notification has been cleared
            StatsController.OnZeroHealth += OnZeroHealthNotification;
        };
        string message = StringUtils.Replace(Localization.Localize("ZERO_HEALTH"), 
            StringUtils.NUM, hospitalBillCost.ToString());

        Hashtable notificationEntry = new Hashtable();
        notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.OneButton);
        notificationEntry.Add(NotificationPopupFields.Message, message); 
        notificationEntry.Add(NotificationPopupFields.Button1Callback, button1Function); 
        
        NotificationUIManager.Instance.AddToQueue(notificationEntry);

        //Send analytics event
        Analytics.Instance.ZeroHealth();
    }
}
