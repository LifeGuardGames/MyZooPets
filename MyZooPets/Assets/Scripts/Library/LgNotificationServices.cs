using UnityEngine;
using System;
using System.Collections;
using Area730.Notifications;

public class LgNotificationServices{

    public static void ScheduleLocalNotification(){
#if UNITY_IPHONE && !UNITY_EDITOR 
        var notif = new LocalNotification();
        notif.fireDate = fireDate;
        notif.alertAction = title;
        notif.alertBody = message; 
        notif.soundName = LocalNotification.defaultSoundName;
        notif.applicationIconBadgeNumber = 1;

        NotificationServices.ScheduleLocalNotification(notif);
#endif

#if UNITY_ANDROID
		AndroidNotifications.cancelNotification(1);
		int id = 1;
		string titleA = DataManager.Instance.GameData.PetInfo.PetName + " misses you";
		string body = "Why not stop by and visit?";
		NotificationBuilder build = new NotificationBuilder (id,titleA,body);
		TimeSpan interval = new TimeSpan(168,0,0);
		build.setInterval(interval);
		build.setAutoCancel(false);
		build.setDelay(interval);
		AndroidNotifications.scheduleNotification(build.build());
#endif
    }

    //removes icon badge number and removed the notifications delivered to user
    public static void RemoveIconBadgeNumber(){
#if UNITY_IPHONE && !UNITY_EDITOR 
        var notif = new LocalNotification();
        notif.hasAction = false;
        notif.applicationIconBadgeNumber = -1;

        NotificationServices.PresentLocalNotificationNow(notif);
        NotificationServices.ClearLocalNotifications();
        NotificationServices.CancelAllLocalNotifications();
#endif        
    }
}
