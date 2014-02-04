using UnityEngine;
using System.Collections;

public class BadgeUnlockNotificationListener : MonoBehaviour {

	// Use this for initialization
	void Start () {
        BadgeLogic.OnNewBadgeUnlocked += OnBadgeReward;
	}
	
    void OnDestroy(){
        BadgeLogic.OnNewBadgeUnlocked -= OnBadgeReward;
    }

    private void OnBadgeReward(object sender, BadgeLogic.BadgeEventArgs args){
        Badge badge = args.UnlockedBadge;
        Hashtable notificationEntry = new Hashtable();

        notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.BadgeUnlocked);
		notificationEntry.Add(NotificationPopupFields.Badge, badge.Name);
        notificationEntry.Add(NotificationPopupFields.Message, badge.Description);
        notificationEntry.Add(NotificationPopupFields.SpriteName, badge.TextureName);
        notificationEntry.Add(NotificationPopupFields.Button1Callback, null);
        
        NotificationUIManager.Instance.AddToQueue(notificationEntry);
    }
}
