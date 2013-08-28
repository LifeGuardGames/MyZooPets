using UnityEngine;
using System.Collections;

public class NotificationTest : MonoBehaviour {
    private const int BUTTON_OFFSET = 55;
    private const int BUTTON_HEIGHT = 50;
    private const int BUTTON_WIDTH = 100;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

    void OnGUI(){
        //1 button popup notification
        if(GUI.Button(new Rect(10, 10, BUTTON_WIDTH, BUTTON_HEIGHT), "one button")){
			
			/////// Send Notication ////////
			// Assign delegate functions to be passed in hashtable
			PopupNotificationNGUI.HashEntry button1Function = delegate(){
	                //
	            };
			
			// Populate notification entry table
			Hashtable notificationEntry = new Hashtable();
			notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.OneButton);
			notificationEntry.Add(NotificationPopupFields.Message, "You need at least 99999999 stars to play!");
			notificationEntry.Add(NotificationPopupFields.Button1Label, "Back");
			notificationEntry.Add(NotificationPopupFields.Button1Callback, button1Function);
			
			// Place notification entry table in static queue
			NotificationUIManager.Instance.AddToQueue(notificationEntry);
			
//            NotificationUIManager.Instance.EnqueuePopupNotificationOneButton("testing", null);
        }

        //level up notification
        if(GUI.Button(new Rect(10, 10+BUTTON_OFFSET*1, BUTTON_WIDTH, BUTTON_HEIGHT),
            "level up")){
			
			// Populate notification entry table
			Hashtable notificationEntry = new Hashtable();
			notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.LevelUp);
			notificationEntry.Add(NotificationPopupFields.Badge, BadgeTier.Bronze);
			notificationEntry.Add(NotificationPopupFields.Button1Callback, null);
			
//            NotificationUIManager.Instance.EnqueueLevelUpMessage(BadgeTier.Bronze, null);
        }

        //popup texture "great"
        if(GUI.Button(new Rect(10, 10+BUTTON_OFFSET*2, BUTTON_WIDTH, BUTTON_HEIGHT),
            "great")){
            NotificationUIManager.Instance.PopupTexture("great");	// Textures are not queued
        }

        //game over reward
        if(GUI.Button(new Rect(10, 10+BUTTON_OFFSET*3, BUTTON_WIDTH, BUTTON_HEIGHT),
            "GG Reward")){
			
			/////// Send Notication ////////
			// Assign delegate functions to be passed in hashtable
			PopupNotificationNGUI.HashEntry button1Function = delegate(){
	                //
	            };
			PopupNotificationNGUI.HashEntry button2Function = delegate() {
					//
				};
			
			// Populate notification entry table
			Hashtable notificationEntry = new Hashtable();
			notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.GameOverRewardTwoButton);
			notificationEntry.Add(NotificationPopupFields.DeltaStars, 10);
			notificationEntry.Add(NotificationPopupFields.DeltaPoints, 10);
			notificationEntry.Add(NotificationPopupFields.Button1Callback, button1Function);
			notificationEntry.Add(NotificationPopupFields.Button2Callback, button2Function);
			
			// Place notification entry table in static queue
			NotificationUIManager.Instance.AddToQueue(notificationEntry);
			
//            NotificationUIManager.Instance.EnqueueGameOverRewardMessage(1000, 0, null, null);
        }

        if(GUI.Button(new Rect(10, 10+BUTTON_OFFSET*4, BUTTON_WIDTH, BUTTON_HEIGHT),
            "two button")){
			
			// Assign delegate functions to be passed in hashtable
			PopupNotificationNGUI.HashEntry button1Function = delegate(){
	            	//
	            };
			PopupNotificationNGUI.HashEntry button2Function = delegate() {
					//
				};
			
			// Populate notification entry table
			Hashtable notificationEntry = new Hashtable();
			notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.TwoButtons);
			notificationEntry.Add(NotificationPopupFields.Message, "testing");
			notificationEntry.Add(NotificationPopupFields.Button1Callback, button1Function);
			notificationEntry.Add(NotificationPopupFields.Button2Callback, button2Function);
			
			// Place notification entry table in static queue
			NotificationUIManager.Instance.AddToQueue(notificationEntry);
			
//            NotificationUIManager.Instance.EnqueuePopupNotificationTwoButtons("testing", null, null);
        }

        if(GUI.Button(new Rect(10, 10+BUTTON_OFFSET*5, BUTTON_WIDTH, BUTTON_HEIGHT),
            "1 button GG")){
			
			/////// Send Notication ////////
			// Assign delegate functions to be passed in hashtable
			PopupNotificationNGUI.HashEntry button1Function = delegate(){
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

//            NotificationUIManager.Instance.ShowGameOverRewardMessage(1000, 10000, null);
        }

        if(GUI.Button(new Rect(10, 10+BUTTON_OFFSET*6, BUTTON_WIDTH, BUTTON_HEIGHT),
            "tutorial test")){
			
			/////// Send Notication ////////
			// Assign delegate functions to be passed in hashtable
			PopupNotificationNGUI.HashEntry button1Function = delegate(){
	               //;
	            };
			// Populate notification entry table
			Hashtable notificationEntry = new Hashtable();
			notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.TipWithImage);
			notificationEntry.Add(NotificationPopupFields.Message, "TESTTTT");
			notificationEntry.Add(NotificationPopupFields.SpriteName, "advairPurple");
			notificationEntry.Add(NotificationPopupFields.Button1Callback, button1Function);
			notificationEntry.Add(NotificationPopupFields.StartsHidden, true);
			notificationEntry.Add(NotificationPopupFields.HideImmediately, false);
			// Place notification entry table in static queue
			NotificationUIManager.Instance.AddToQueue(notificationEntry);
			
//            NotificationUIManager.Instance.ShowTutorialMessage(TutorialImageType.CalendarGreenStamp, null, "");
        }
    }
}
