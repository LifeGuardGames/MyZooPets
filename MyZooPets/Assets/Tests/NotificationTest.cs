using UnityEngine;
using System.Collections;

public class NotificationTest : MonoBehaviour {
    private const int BUTTON_OFFSET = 55;
    private const int BUTTON_HEIGHT = 50;
    private const int BUTTON_WIDTH = 100;

	/*
    void OnGUI(){
		if(GUI.Button(new Rect(10, 10+BUTTON_OFFSET*1, BUTTON_WIDTH, BUTTON_HEIGHT),
            "image button")){

            PopupNotificationNGUI.Callback button1Function = delegate(){
                    //
                };

            Hashtable notificationEntry = new Hashtable();
            //notificationEntry.Add(NotificationPopupData.Type, NotificationPopupType.TipWithImage);
            //notificationEntry.Add(NotificationPopupData.Message, "You need at least 99999999 stars to play!");
            //notificationEntry.Add(NotificationPopupData.SpriteName, "itemInhalerMain");
            //notificationEntry.Add(NotificationPopupData.Button1Callback, button1Function);
            
			
			// Place notification entry table in static queue
			NotificationUIManager.Instance.AddToQueue(notificationEntry);
        }

        //LEVEL UP
        if(GUI.Button(new Rect(10, 10+BUTTON_OFFSET*2, BUTTON_WIDTH, BUTTON_HEIGHT),
            "level up")){
			
			// Populate notification entry table
			Hashtable notificationEntry = new Hashtable();
			//notificationEntry.Add(NotificationPopupData.Type, NotificationPopupType.LevelUp);
			//notificationEntry.Add(NotificationPopupData.Message, "BOOO HOO!");
			//notificationEntry.Add(NotificationPopupData.Button1Callback, null);
			
			NotificationUIManager.Instance.AddToQueue(notificationEntry);
        }

        if(GUI.Button(new Rect(10, 10+BUTTON_OFFSET*6, BUTTON_WIDTH, BUTTON_HEIGHT),
            "Fire Level Up")){

            Hashtable notificationEntry = new Hashtable();
            notificationEntry.Add(NotificationPopupData.Type, NotificationPopupType.FireLevelUp);
            notificationEntry.Add(NotificationPopupData.Message, "You got a new fire!");
            notificationEntry.Add(NotificationPopupData.SpriteName, "iconFireBlue");
            notificationEntry.Add(NotificationPopupData.Button1Callback, null);

            NotificationUIManager.Instance.AddToQueue(notificationEntry);
        }


    }
	*/
}
