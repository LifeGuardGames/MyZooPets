using UnityEngine;
using System.Collections;
using System;
using Area730.Notifications;

public class NotificationController : MonoBehaviour {

	// Use this for initialization
	void Start () {
#if UNITY_ANDROID
		AndroidNotifications.cancelNotification(1);
		int id = 1;
		string title = DataManager.Instance.GameData.PetInfo.PetName + " misses you";
		string body = "Why not stop by and visit?";
		NotificationBuilder build = new NotificationBuilder (id,title,body);
		TimeSpan interval = new TimeSpan(168,0,0);
		build.setInterval(interval);
		build.setAutoCancel(false);
		build.setDelay(interval);
		AndroidNotifications.scheduleNotification(build.build());
#endif
	}
}
