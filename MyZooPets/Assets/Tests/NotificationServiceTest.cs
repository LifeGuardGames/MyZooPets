using UnityEngine;
using System.Collections;

public class NotificationServiceTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        // if (NotificationServices.localNotificationCount > 0) {
        //     Debug.Log(NotificationServices.localNotifications[0].alertBody);
        //     NotificationServices.ClearLocalNotifications();
        // }	
	}

    // void OnGUI(){
    //     if(GUI.Button(new Rect(10, 10, 100, 100), "Local Notif")){
    //         LgNotificationServices.ScheduleLocalNotification("sup", LgDateTime.GetTimeNow().AddSeconds(10));
    //     }

    // }
}
