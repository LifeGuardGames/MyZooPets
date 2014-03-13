using UnityEngine;
using System;
using System.Collections;

public class LgNotificationServices{

    public static void ScheduleLocalNotification(string message, DateTime fireDate, string title="Wellapets"){
#if UNITY_IPHONE 
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

//     public static void CancelAllLocalNotifications(){
// #if UNITY_IPHONE
//         NotificationServices.CancelAllLocalNotifications();
// #endif

// #if UNITY_ANDROID
// #endif 
//     }

    public static void RemoveIconBadgeNumber(){
#if UNITY_IPHONE 
        var notif = new LocalNotification();
        notif.hasAction = false;
        notif.applicationIconBadgeNumber = -1;

        NotificationServices.PresentLocalNotificationNow(notif);
        NotificationServices.CancelAllLocalNotifications();
#endif        
    }
}
