using UnityEngine;
using System;
using System.Collections;

public class TutorialUIManager : Singleton<TutorialUIManager> {
    //============Time Mood Decay tutorial=================
    public void StartTimeMoodDecayTutorial(){
        string petName = DataManager.Instance.GameData.PetInfo.PetName;
        string timeMoodDecay1 = String.Format(Localization.Localize("TMD_1"), petName);
        string timeMoodDecay2 = String.Format(Localization.Localize("TMD_2"), petName);

        AddStandardTutTip( NotificationPopupType.TipWithImage, timeMoodDecay1, 
            "Skull", null, true, true, "Tutorial:MoodDecay:Intro" );
		AddStandardTutTip( NotificationPopupType.TipWithImage, timeMoodDecay2, 
            "guiPanelStatsHealth", null, false, false, "Tutorial:MoodDecay:End" );
    
		// this doesn't do what it thinks it does...it will just mark it as played right away, not when the tut is finished
		DataManager.Instance.GameData.Tutorial.ListPlayed.Add( DegradationLogic.TIME_DECAY_TUT );
	}	

	public static void AddStandardTutTip( NotificationPopupType eType, string strText, 
        string strSprite, PopupNotificationNGUI.HashEntry button1cb, bool bStartsHidden, bool bHideImmediately, string strAnalytics ) {
		/////// Send Notication ////////
		// Populate notification entry table
		Hashtable notificationEntry = new Hashtable();
		notificationEntry.Add(NotificationPopupFields.Type, eType);
		notificationEntry.Add(NotificationPopupFields.Message, strText);
		notificationEntry.Add(NotificationPopupFields.SpriteName, strSprite);
		notificationEntry.Add(NotificationPopupFields.Button1Callback, button1cb);
		// notificationEntry.Add(NotificationPopupFields.StartsHidden, bStartsHidden);
		// notificationEntry.Add(NotificationPopupFields.HideImmediately, bHideImmediately);
		// Place notification entry table in static queue
		NotificationUIManager.Instance.AddToQueue(notificationEntry);
	}	
}
