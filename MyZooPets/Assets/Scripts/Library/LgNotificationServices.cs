using UnityEngine;
using System;
using System.Collections;
using Area730.Notifications;

public class LgNotificationServices : Singleton<LgNotificationServices> {
	
	void Start(){
#if UNITY_IOS //&& !UNITY_EDITOR
		// Register for iOS local notifications
		Debug.Log("Registering iOS notifications");
		NotificationServices.RegisterForLocalNotificationTypes(LocalNotificationType.Alert | LocalNotificationType.Badge | LocalNotificationType.Sound);
#endif

		// Schedule notification on start
		ScheduleLocalNotification();
	}

	/// <summary>
	/// Local recurring notification. Recurring weekly once you havent touched the app in more than 7 days
	/// Also populates rate app notification
	/// </summary>
	public void ScheduleLocalNotification(){
#if UNITY_IOS //&& !UNITY_EDITOR
		// Reset the badge icon
		LocalNotification resetNotif = new LocalNotification();
		resetNotif.applicationIconBadgeNumber = -1;
		resetNotif.hasAction = false;
		NotificationServices.PresentLocalNotificationNow(resetNotif);

		// Clear and cancel
		NotificationServices.ClearLocalNotifications();				// Clear all received notifications
		foreach(LocalNotification localNotif in NotificationServices.scheduledLocalNotifications){		// Remove reminder notifications
			Debug.Log("----SCHEDULED NOTIF");
			if(localNotif.repeatInterval == CalendarUnit.Week){
				NotificationServices.CancelLocalNotification(localNotif);
				Debug.Log("CANCELLING RECURRING NOTIFICATIONS");
			}
		} // TODO needs testing

		// Prepare to fire new notification
		string iOSAction = "visit " + DataManager.Instance.GameData.PetInfo.PetName; 	// Action (ie. slide to _)
		string iOSBody = DataManager.Instance.GameData.PetInfo.PetName + " misses you!";

		DateTime fireDate = LgDateTime.GetTimeNow().AddDays(7);		// Schedule for 7 days from now

		LocalNotification notif = new LocalNotification();
        notif.fireDate = fireDate;
		notif.alertAction = iOSAction;
		notif.alertBody = iOSBody;
		notif.soundName = LocalNotification.defaultSoundName;
		notif.repeatInterval = CalendarUnit.Week;
		notif.applicationIconBadgeNumber = -1;
        NotificationServices.ScheduleLocalNotification(notif);


		// Also check if we need to push the rate app notification
		// Conditions - passed day 7 retention, only seen once
		TimeSpan difference = LgDateTime.GetTimeNow().Subtract(DataManager.Instance.GameData.PlayPeriod.FirstPlayPeriod);
		if(!DataManager.Instance.GameData.PlayPeriod.IsDisplayedAppNotification		// Displayed for first time
		   && DataManager.Instance.GameData.PlayPeriod.IsFirstPlayPeriodAux			// Started first play session in
		   && difference.CompareTo(new TimeSpan(7, 0, 0 ,0)) > 1){						// Past 7 days

			LocalNotification rateNotif = new LocalNotification();
			rateNotif.fireDate = LgDateTime.GetTimeNow().AddSeconds(10);
			rateNotif.alertAction = "open game";
			rateNotif.alertBody = "Is 'Wizdy Pets' helping your kids with asthma? Leave us a review in the AppStore!";
			rateNotif.soundName = LocalNotification.defaultSoundName;
			rateNotif.applicationIconBadgeNumber = -1;

			NotificationServices.ScheduleLocalNotification(rateNotif);
			DataManager.Instance.GameData.PlayPeriod.IsDisplayedAppNotification = true;
		}
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
		string title = DataManager.Instance.GameData.PetInfo.PetName + " misses you!";
		string body = "Why not stop by and visit?";
		
		AndroidNotifications.cancelNotification(1);
		int id = 1;
		NotificationBuilder build = new NotificationBuilder(id, title, body);
		TimeSpan interval = new TimeSpan(168, 0, 0);
		build.setInterval(interval);
		build.setAutoCancel(false);
		build.setDelay(interval);
		AndroidNotifications.scheduleNotification(build.build());
#endif
    }
}
