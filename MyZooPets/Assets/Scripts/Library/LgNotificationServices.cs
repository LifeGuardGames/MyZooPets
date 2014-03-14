using UnityEngine;
using System;
using System.Collections;

public class LgNotificationServices{

    public static void ScheduleLocalNotification(string message, DateTime fireDate, string title="Wellapets"){
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
        TimeSpan delay = fireDate - LgDateTime.GetTimeNow();

        ELANManager.SendNotification(title, message, (long) delay.TotalSeconds);
#endif
    }

    //removes icon badge number and removed the notifications delivered to user
    public static void RemoveIconBadgeNumber(){
#if UNITY_IPHONE && !UNITY_EDITOR 
        if(NotificationServices.localNotificationCount > 0){
            var notif = new LocalNotification();
            notif.hasAction = false;
            notif.applicationIconBadgeNumber = -1;

            NotificationServices.PresentLocalNotificationNow(notif);
            NotificationServices.ClearLocalNotifications();
            NotificationServices.CancelAllLocalNotifications();
        }
#endif        
    }
}
