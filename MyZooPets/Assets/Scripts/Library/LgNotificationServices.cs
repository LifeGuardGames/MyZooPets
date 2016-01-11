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

	public void ScheduleLocalNotification(){
#if UNITY_IOS && !UNITY_EDITOR
		// Reset the badge icon
		LocalNotification resetNotif = new LocalNotification();
		resetNotif.applicationIconBadgeNumber = -1;
		resetNotif.hasAction = false;
		NotificationServices.PresentLocalNotificationNow(resetNotif);

		// Clear and cancel all notifications
		NotificationServices.ClearLocalNotifications();				// Clear all received notifications
		NotificationServices.CancelAllLocalNotifications();			// Cancel all previous notifications

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
