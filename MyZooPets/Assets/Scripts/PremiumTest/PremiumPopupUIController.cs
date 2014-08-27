using UnityEngine;
using System.Collections;

//TODO: hacky class need to be removed after focus group
public class PremiumPopupUIController : MonoBehaviour {
	

	public void OnBuyPremium(GameObject button){

//		NotificationUIManager.Instance.CleanupNotification();

		PopupNotificationNGUI.Callback okButtonCallback = delegate(){
			StatsController.Instance.ChangeStats(deltaGems: 5);
		};
		
		Hashtable notificationEntry = new Hashtable();
		notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.PremiumTest);
		notificationEntry.Add(NotificationPopupFields.Button2Callback, okButtonCallback);
		
		NotificationUIManager.Instance.AddToQueue(notificationEntry);
		NotificationUIManager.Instance.TryNextNotification();

	}
}
