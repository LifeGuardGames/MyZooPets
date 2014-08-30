using UnityEngine;
using System.Collections;

public class NotificationTest : MonoBehaviour {
    private const int BUTTON_OFFSET = 55;
    private const int BUTTON_HEIGHT = 50;
    private const int BUTTON_WIDTH = 100;

    void OnGUI(){
        //1 BUTTON POPUP
        if(GUI.Button(new Rect(10, 10, BUTTON_WIDTH, BUTTON_HEIGHT), "one button")){
			
			/////// Send Notication ////////
			// Assign delegate functions to be passed in hashtable
			PopupNotificationNGUI.Callback button1Function = delegate(){
	                //
	            };
			
			// Populate notification entry table
			Hashtable notificationEntry = new Hashtable();
			notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.TwoButtons);
			notificationEntry.Add(NotificationPopupFields.Message, "You need at least 99999999 stars to play!");
			notificationEntry.Add(NotificationPopupFields.Button1Callback, button1Function);
            notificationEntry.Add(NotificationPopupFields.Button2Callback, button1Function);
			
			// Place notification entry table in static queue
			NotificationUIManager.Instance.AddToQueue(notificationEntry);
        }
		
		if(GUI.Button(new Rect(10, 10+BUTTON_OFFSET*1, BUTTON_WIDTH, BUTTON_HEIGHT),
            "image button")){

            PopupNotificationNGUI.Callback button1Function = delegate(){
                    //
                };

            Hashtable notificationEntry = new Hashtable();
            notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.TipWithImage);
            notificationEntry.Add(NotificationPopupFields.Message, "You need at least 99999999 stars to play!");
            notificationEntry.Add(NotificationPopupFields.SpriteName, "itemInhalerMain");
            notificationEntry.Add(NotificationPopupFields.Button1Callback, button1Function);
            
			
			// Place notification entry table in static queue
			NotificationUIManager.Instance.AddToQueue(notificationEntry);
        }

        //LEVEL UP
        if(GUI.Button(new Rect(10, 10+BUTTON_OFFSET*2, BUTTON_WIDTH, BUTTON_HEIGHT),
            "level up")){
			
			// Populate notification entry table
			Hashtable notificationEntry = new Hashtable();
			notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.LevelUp);
			notificationEntry.Add(NotificationPopupFields.Message, "BOOO HOO!");
			notificationEntry.Add(NotificationPopupFields.Button1Callback, null);
			
			NotificationUIManager.Instance.AddToQueue(notificationEntry);
        }
		
   //      //game over reward
   //      if(GUI.Button(new Rect(10, 10+BUTTON_OFFSET*3, BUTTON_WIDTH, BUTTON_HEIGHT),
   //          "GG 2 Button")){
			
			// /////// Send Notication ////////
			// // Assign delegate functions to be passed in hashtable
			// PopupNotificationNGUI.HashEntry button1Function = delegate(){
	  //               //
	  //           };
			// PopupNotificationNGUI.HashEntry button2Function = delegate() {
			// 		//
			// 	};
			
			// // Populate notification entry table
			// Hashtable notificationEntry = new Hashtable();
			// notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.GameOverRewardTwoButton);
			// notificationEntry.Add(NotificationPopupFields.DeltaStars, 10);
			// notificationEntry.Add(NotificationPopupFields.DeltaPoints, 10);
			// notificationEntry.Add(NotificationPopupFields.Button1Callback, button1Function);
			// // notificationEntry.Add(NotificationPopupFields.Button2Callback, button2Function);
			
			// // Place notification entry table in static queue
			// NotificationUIManager.Instance.AddToQueue(notificationEntry);
   //      }
		
        if(GUI.Button(new Rect(10, 10+BUTTON_OFFSET*4, BUTTON_WIDTH, BUTTON_HEIGHT),
            "GG 1 Button")){
			
			/////// Send Notication ////////
			// Assign delegate functions to be passed in hashtable
			PopupNotificationNGUI.Callback button1Function = delegate(){
	                //
	            };
			
			// Populate notification entry table
			Hashtable notificationEntry = new Hashtable();
			notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.GameOverRewardOneButton);
			notificationEntry.Add(NotificationPopupFields.DeltaStars, 10);
			notificationEntry.Add(NotificationPopupFields.DeltaPoints, 1000);
			notificationEntry.Add(NotificationPopupFields.Button1Callback, button1Function);
			
			// Place notification entry table in static queue
			NotificationUIManager.Instance.AddToQueue(notificationEntry);
        }

        if(GUI.Button(new Rect(10, 10+BUTTON_OFFSET*5, BUTTON_WIDTH, BUTTON_HEIGHT),
            "Badge Reward")){


            Hashtable notificationEntry = new Hashtable();
            notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.BadgeUnlocked);
            notificationEntry.Add(NotificationPopupFields.Message, "You got a new badge!");
            notificationEntry.Add(NotificationPopupFields.SpriteName, "badgeLevel20");
            notificationEntry.Add(NotificationPopupFields.Button1Callback, null);

            NotificationUIManager.Instance.AddToQueue(notificationEntry);
        }

        if(GUI.Button(new Rect(10, 10+BUTTON_OFFSET*6, BUTTON_WIDTH, BUTTON_HEIGHT),
            "Fire Level Up")){

            Hashtable notificationEntry = new Hashtable();
            notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.FireLevelUp);
            notificationEntry.Add(NotificationPopupFields.Message, "You got a new fire!");
            notificationEntry.Add(NotificationPopupFields.SpriteName, "iconFireBlue");
            notificationEntry.Add(NotificationPopupFields.Button1Callback, null);

            NotificationUIManager.Instance.AddToQueue(notificationEntry);
        }


    }
}
