using UnityEngine;
using System;
using System.Collections;
using Area730.Notifications;

public class LgNotificationServices{

    public static void ScheduleLocalNotification(){
		string title = DataManager.Instance.GameData.PetInfo.PetName + " misses you!";
		string body = "Why not stop by and visit?";

#if UNITY_IPHONE
		NotificationServices.ClearLocalNotifications();
		NotificationServices.CancelAllLocalNotifications();			// Clear all previous notifications
		DateTime fireDate = LgDateTime.GetTimeNow().AddDays(7);		// Schedule for 7 days from now

		LocalNotification notif = new LocalNotification();
        notif.fireDate = fireDate;
        notif.alertAction = title;
		notif.alertBody = body;
		notif.soundName = LocalNotification.defaultSoundName;
		notif.repeatInterval = CalendarUnit.Week;
        notif.applicationIconBadgeNumber = 1;

        NotificationServices.ScheduleLocalNotification(notif);
#endif

#if UNITY_ANDROID
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

    //removes icon badge number and removed the notifications delivered to user		// TODO delete
    public static void RemoveIconBadgeNumber(){
#if UNITY_IPHONE && !UNITY_EDITOR 
//        var notif = new LocalNotification();
//        notif.hasAction = false;
//        notif.applicationIconBadgeNumber = -1;
//
//        NotificationServices.PresentLocalNotificationNow(notif);
//        NotificationServices.ClearLocalNotifications();
//        NotificationServices.CancelAllLocalNotifications();
#endif        
    }
}
