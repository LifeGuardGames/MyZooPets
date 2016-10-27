﻿using UnityEngine;
using System;
using System.Collections;
#if UNITY_IOS
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;
#elif UNITY_ANDROID
using Area730.Notifications;
#endif

public class LgNotificationServices : Singleton<LgNotificationServices> {
	
	void Start(){
		#if UNITY_IOS
		// Register for iOS local notifications
		Debug.Log("Registering iOS notifications");
		NotificationServices.RegisterForNotifications(NotificationType.Alert | NotificationType.Badge | NotificationType.Sound);
#endif

		// Schedule notification on start
		ScheduleLocalNotification();
	}

	/// <summary>
	/// Local recurring notification. Recurring weekly once you havent touched the app in more than 7 days
	/// Also populates rate app notification
	/// </summary>
	public void ScheduleLocalNotification(){
		#if UNITY_IOS
		// Reset the badge icon
		UnityEngine.iOS.LocalNotification resetNotif = new UnityEngine.iOS.LocalNotification();
		resetNotif.applicationIconBadgeNumber = -1;
		resetNotif.hasAction = false;
		NotificationServices.PresentLocalNotificationNow(resetNotif);

		// Clear and cancel
		NotificationServices.ClearLocalNotifications();				// Clear all received notifications
		foreach(UnityEngine.iOS.LocalNotification localNotif in NotificationServices.scheduledLocalNotifications){		// Remove reminder notifications
			if(localNotif.repeatInterval == UnityEngine.iOS.CalendarUnit.Week){
				NotificationServices.CancelLocalNotification(localNotif);
				Debug.Log("CANCELLING RECURRING NOTIFICATIONS");
			}
		}

		// Prepare to fire new notification
		string iOSAction = "visit " + DataManager.Instance.GameData.PetInfo.PetName; 	// Action (ie. slide to _)
		string iOSBody = DataManager.Instance.GameData.PetInfo.PetName + " misses you!";

		DateTime fireDate = LgDateTime.GetTimeNow().AddDays(7);		// Schedule for 7 days from now

		UnityEngine.iOS.LocalNotification notif = new UnityEngine.iOS.LocalNotification();
        notif.fireDate = fireDate;
		notif.alertAction = iOSAction;
		notif.alertBody = iOSBody;
		notif.soundName = UnityEngine.iOS.LocalNotification.defaultSoundName;
		notif.repeatInterval = UnityEngine.iOS.CalendarUnit.Week;
		notif.applicationIconBadgeNumber = -1;
        NotificationServices.ScheduleLocalNotification(notif);


		// Also check if we need to push the rate app notification
		// Conditions - passed day 7 retention, only seen once
		TimeSpan difference = LgDateTime.GetTimeNow().Subtract(DataManager.Instance.GameData.PlayPeriod.FirstPlayPeriod);
		if(!DataManager.Instance.GameData.PlayPeriod.IsDisplayedAppNotification			// Displayed for first time
			&& DataManager.Instance.GameData.PlayPeriod.IsFirstPlayPeriodAux			// Started first play session in
			&& difference > new TimeSpan(7, 0, 0 ,0)){									// Past 7 days

			UnityEngine.iOS.LocalNotification rateNotif = new UnityEngine.iOS.LocalNotification();

			// Shoot for next 8:47am
			DateTime now = LgDateTime.GetTimeNow();
			DateTime today847am = now.Date.AddHours(8).AddMinutes(47);
			DateTime next847am = now <= today847am ? today847am : today847am.AddDays(1);
			
			rateNotif.fireDate = next847am;
			rateNotif.alertAction = "open game";
			rateNotif.alertBody = "Is 'Wizdy Pets' helping your kids with asthma? Leave us a review in the AppStore!";
			rateNotif.soundName = UnityEngine.iOS.LocalNotification.defaultSoundName;
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
