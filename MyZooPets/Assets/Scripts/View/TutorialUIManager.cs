using UnityEngine;
using System;
using System.Collections;

public class TutorialUIManager : Singleton<TutorialUIManager> {

    public ClickManager clickManager;

    private const string DEGRAD_TIP1 = "Good job! You just removed an asthma trigger.";
    private const string DEGRAD_TIP2 = "Make sure you clean them up when you see them, or your pet will get sick!";
   
    //============Time Mood Decay tutorial=================
    public void StartTimeMoodDecayTutorial(){
        AddStandardTutTip( NotificationPopupType.TipWithImage, Localization.Localize( "TMD_1" ), "Skull", null, true, true, "Tutorial:MoodDecay:Intro" );
		AddStandardTutTip( NotificationPopupType.TipWithImage, Localization.Localize( "TMD_2" ), "guiPanelStatsHealth", null, false, false, "Tutorial:MoodDecay:End" );
    
		// this doesn't do what it thinks it does...it will just mark it as played right away, not when the tut is finished
		DataManager.Instance.GameData.Tutorial.ListPlayed.Add( DegradationLogic.TIME_DECAY_TUT );
	}	

	public static void AddStandardTutTip( NotificationPopupType eType, string strText, string strSprite, PopupNotificationNGUI.HashEntry button1cb, bool bStartsHidden, bool bHideImmediately, string strAnalytics ) {
		/////// Send Notication ////////
		// Populate notification entry table
		Hashtable notificationEntry = new Hashtable();
		notificationEntry.Add(NotificationPopupFields.Type, eType);
		notificationEntry.Add(NotificationPopupFields.Message, strText);
		notificationEntry.Add(NotificationPopupFields.SpriteName, strSprite);
		notificationEntry.Add(NotificationPopupFields.Button1Callback, button1cb);
		notificationEntry.Add(NotificationPopupFields.StartsHidden, bStartsHidden);
		notificationEntry.Add(NotificationPopupFields.HideImmediately, bHideImmediately);
		// Place notification entry table in static queue
		NotificationUIManager.Instance.AddToQueue(notificationEntry);
	}	
}
