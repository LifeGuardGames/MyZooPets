using UnityEngine;
using System;
using System.Collections;

public class TutorialUIManager : Singleton<TutorialUIManager> {

    public ClickManager clickManager;

    private const string DEGRAD_TIP1 = "Good job! You just removed an asthma trigger.";
    private const string DEGRAD_TIP2 = "Make sure you clean them up when you see them, or your pet will get sick!";
    
    void Start(){
        //use a if else if here to make sure that any tutorials not visited get called
        TutorialLogic.OnTutorialUpdated += UpdateTutorial;
        UpdateTutorial();
    }

    void OnDestroy(){
        TutorialLogic.OnTutorialUpdated -= UpdateTutorial;
    }

    //Event listener that activates new tutorial when an old tutorial is completed
    private void UpdateTutorial(object sender, EventArgs args){
        UpdateTutorial();
    }

    private void UpdateTutorial(){
		/*
        if(TutorialLogic.Instance.FirstTimeRealInhaler){
            // realInhaler.GetComponent<TutorialHighlighting>().ShowArrow();
        }else if(TutorialLogic.Instance.FirstTimeCalendar){
            // calendar.GetComponent<TutorialHighlighting>().ShowArrow();
        }else if(TutorialLogic.Instance.FirstTimeDegradTrigger){
            //start trigger tutorial after others are done
            DegradationUIManager.Instance.ActivateParticleEffects();
        }
        */
    }

   
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
		
        GA.API.Design.NewEvent( strAnalytics );		
	}	

    //============Trigger tutorial=================
    public void StartDegradTriggerTutorial(){
        /*if(TutorialLogic.Instance.FirstTimeDegradTrigger){
            AddDegradTipIntro();
			AddDegradTipConclude();
		}*/
		//TutorialLogic.Instance.FirstTimeDegradTrigger = false;
		//DegradationUIManager.Instance.ActivateParticleEffects();
    }

    private void AddDegradTipIntro(){
		/////// Send Notication ////////
		// Populate notification entry table
		Hashtable notificationEntry = new Hashtable();
		notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.TipWithImage);
		notificationEntry.Add(NotificationPopupFields.Message, DEGRAD_TIP1);
		notificationEntry.Add(NotificationPopupFields.SpriteName, "guiPanelStatsHealth");
		notificationEntry.Add(NotificationPopupFields.Button1Callback, null);
		notificationEntry.Add(NotificationPopupFields.StartsHidden, true);
		notificationEntry.Add(NotificationPopupFields.HideImmediately, true);
		// Place notification entry table in static queue
		NotificationUIManager.Instance.AddToQueue(notificationEntry);
		
        GA.API.Design.NewEvent("Tutorial:Trigger:Intro");
    }

    private void AddDegradTipConclude(){
        // disappear immediately when done, because the level up message should pop up right away
		
		/////// Send Notication ////////
		// Populate notification entry table
		Hashtable notificationEntry = new Hashtable();
		notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.TipWithImage);
		notificationEntry.Add(NotificationPopupFields.Message, DEGRAD_TIP2);
		notificationEntry.Add(NotificationPopupFields.SpriteName, "Skull");
		notificationEntry.Add(NotificationPopupFields.Button1Callback, null);
		notificationEntry.Add(NotificationPopupFields.StartsHidden, false);
		notificationEntry.Add(NotificationPopupFields.HideImmediately, false);
		// Place notification entry table in static queue
		NotificationUIManager.Instance.AddToQueue(notificationEntry);
		
        TutorialLogic.Instance.FirstTimeDegradTrigger = false;
        DegradationUIManager.Instance.ActivateParticleEffects();
        GA.API.Design.NewEvent("Tutorial:Trigger:End");
    }
}
