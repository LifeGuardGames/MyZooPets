using UnityEngine;
using System;
using System.Collections;

public class TutorialUIManager : Singleton<TutorialUIManager> {

    public GameObject calendar;
    // public GameObject challenges;
    // public GameObject diary;
    // public GameObject slotMachine;
    public GameObject realInhaler;
	public ButtonRealInhaler buttonRealInhaler;
    public GameObject teddyInhaler;

    public ClickManager clickManager;

    private const string CALENDAR_TIP_INTRO = "This calendar shows if you have been using the inhaler on your pet on time.";
    private const string CALENDAR_TIP_GREEN_STAMP = "Good Job! Green stamp means you used the inhaler on your pet on time. You can click on the green stamps to earn stars!";
    private const string CALENDAR_TIP_RED_STAMP = "Ah Oh. Red stamp means you forgot to use the inhaler on your pet on time. This is not good for the pets heath.";
    private const string CALENDAR_TIP_BONUS = "Extra stars can be collected ever 12 hours. Remember to check often!";
    private const string DEGRAD_TIP1 = "Good job! You just removed an asthma trigger.";
    private const string DEGRAD_TIP2 = "Make sure you clean them up when you see them, or your pet will get sick!";
	private const string INHALER_TIP = "Use this inhaler every morning and afternoon to keep your pet healthy!";
    
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
        if(TutorialLogic.Instance.FirstTimeRealInhaler){
            realInhaler.GetComponent<TutorialHighlighting>().ShowArrow();
        }else if(TutorialLogic.Instance.FirstTimeCalendar){
            calendar.GetComponent<TutorialHighlighting>().ShowArrow();
        }else if(TutorialLogic.Instance.FirstTimeDegradTrigger){
            //start trigger tutorial after others are done
            DegradationUIManager.Instance.ActivateParticleEffects();
        }
    }

    //========================Calendar Tutorial======================
    private void StartCalendarTutorial(){
        EnqueueCalendarTipIntro();
		EnqueueCalendarTipGreenStamp();
		EnqueueCalendarTipRedStamp();
		EnqueueCalendarTipBonus();
    }

	private void EnqueueCalendarTipIntro(){
		/////// Send Notication ////////
		// Assign delegate functions to be passed in hashtable
		PopupNotificationNGUI.HashEntry button1Function = delegate(){
               CalendarTutorialHelper.Instance.SetUpGreenStampTip();
            };
		// Populate notification entry table
		Hashtable notificationEntry = new Hashtable();
		notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.TutorialLeft);
		notificationEntry.Add(NotificationPopupFields.TutorialImageType, TutorialImageType.CalendarIntro);
		notificationEntry.Add(NotificationPopupFields.Button1Callback, button1Function);
		notificationEntry.Add(NotificationPopupFields.Button1Label, Localization.Localize("NEXT"));

		// Place notification entry table in static queue
		NotificationUIManager.Instance.AddToQueue(notificationEntry);
		
		GA.API.Design.NewEvent("Tutorial:Calendar:Intro");	// TODO-j Right semantic??
	}

	public void EnqueueCalendarTipGreenStamp(){
		/////// Send Notication ////////
		// Assign delegate functions to be passed in hashtable
		PopupNotificationNGUI.HashEntry button1Function = delegate(){
               CalendarTutorialHelper.Instance.SetUpRedExTip();
            };
		// Populate notification entry table
		Hashtable notificationEntry = new Hashtable();
		notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.TutorialLeft);
		notificationEntry.Add(NotificationPopupFields.TutorialImageType, TutorialImageType.CalendarGreenStamp);
		notificationEntry.Add(NotificationPopupFields.Button1Callback, button1Function);
		notificationEntry.Add(NotificationPopupFields.Button1Label, Localization.Localize("NEXT"));

		// Place notification entry table in static queue
		NotificationUIManager.Instance.AddToQueue(notificationEntry);
		
        GA.API.Design.NewEvent("Tutorial:Calendar:1");
    }

    public void EnqueueCalendarTipRedStamp(){
		/////// Send Notication ////////
		// Assign delegate functions to be passed in hashtable
		PopupNotificationNGUI.HashEntry button1Function = delegate(){
               CalendarTutorialHelper.Instance.SetUpBonusTip();
        };
		// Populate notification entry table
		Hashtable notificationEntry = new Hashtable();
		notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.TutorialLeft);
		notificationEntry.Add(NotificationPopupFields.TutorialImageType, TutorialImageType.CalendarRedStamp);
		notificationEntry.Add(NotificationPopupFields.Button1Callback, button1Function);
		notificationEntry.Add(NotificationPopupFields.Button1Label, Localization.Localize("NEXT"));

		// Place notification entry table in static queue
		NotificationUIManager.Instance.AddToQueue(notificationEntry);

        GA.API.Design.NewEvent("Tutorial:Calendar:2");
    }

    public void EnqueueCalendarTipBonus(){
		/////// Send Notication ////////
		// Assign delegate functions to be passed in hashtable
		PopupNotificationNGUI.HashEntry button1Function = delegate(){
               ShowCalendarTipConclude();
        };
		// Populate notification entry table
		Hashtable notificationEntry = new Hashtable();
		notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.TutorialLeft);
		notificationEntry.Add(NotificationPopupFields.TutorialImageType, TutorialImageType.CalendarBonus);
		notificationEntry.Add(NotificationPopupFields.Button1Callback, button1Function);
		notificationEntry.Add(NotificationPopupFields.Button1Label, Localization.Localize("DONE"));

		// Place notification entry table in static queue
		NotificationUIManager.Instance.AddToQueue(notificationEntry);
		
        GA.API.Design.NewEvent("Tutorial:Calendar:3");
    }

    /*
        1)Conclude the calendar tutorial. 
        2)Turn this tutorial off in DataManager. 
        3)Unregister the tap item listener. 
        4)Show Arrow for Real Inhaler.
        5)Clean calendar UI
    */
    public void ShowCalendarTipConclude(){
        TutorialLogic.Instance.FirstTimeCalendar = false;
        calendar.GetComponent<TutorialHighlighting>().HideArrow();
        CalendarTutorialHelper.Instance.CleanUpTutorial();
        GA.API.Design.NewEvent("Tutorial:Calendar:End");
    }

    //============Trigger tutorial=================
    public void StartDegradTriggerTutorial(){
        if(TutorialLogic.Instance.FirstTimeDegradTrigger == true){
            AddDegradTipIntro();
			AddDegradTipConclude();
		}
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

    //==============Inhaler tutorial=================
    public void StartRealInhalerTutorial(){
		
		/////// Send Notication ////////
		// Assign delegate functions to be passed in hashtable
		PopupNotificationNGUI.HashEntry button1Function = delegate(){
               buttonRealInhaler.OpenRealInhaler();
            };
		// Populate notification entry table
		Hashtable notificationEntry = new Hashtable();
		notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.TipWithImage);
		notificationEntry.Add(NotificationPopupFields.Message, INHALER_TIP);
		notificationEntry.Add(NotificationPopupFields.SpriteName, "advairPurple");
		notificationEntry.Add(NotificationPopupFields.Button1Callback, button1Function);
		notificationEntry.Add(NotificationPopupFields.StartsHidden, true);
		notificationEntry.Add(NotificationPopupFields.HideImmediately, false);
		// Place notification entry table in static queue
		NotificationUIManager.Instance.AddToQueue(notificationEntry);
		
        TutorialHighlighting highlight = realInhaler.GetComponent<TutorialHighlighting>();
        highlight.HideArrow();
        TutorialLogic.Instance.FirstTimeRealInhaler = false;
        GA.API.Design.NewEvent("Tutorial:Inhaler:Intro");
        GA.API.Design.NewEvent("Tutorial:Inhaler:End");
    }

    
    // private void openChallenges(){
    //     DataManager.Instance.Tutorial.FirstTimeChallenges = false;
    //     TutorialHighlighting highlight = challenges.GetComponent<TutorialHighlighting>();
    //     highlight.HideArrow();
    // }

    // void openDiary(){
    //     DataManager.Instance.Tutorial.FirstTimeDiary = false;
    //     TutorialHighlighting highlight = diary.GetComponent<TutorialHighlighting>();
    //     highlight.HideArrow();
    // }

    // void openSlotMachine(){
    //     DataManager.Instance.Tutorial.FirstTimeSlotMachine = false;
    //     TutorialHighlighting highlight = slotMachine.GetComponent<TutorialHighlighting>();
    //     highlight.HideArrow();
    // }

    

    // void openShelf(){
    //     // added for the demo
    //     if (DataManager.Instance.Tutorial.FirstTimeHelpTrophy){
    //         DataManager.Instance.Tutorial.FirstTimeShelf = false;

    //         TutorialHighlighting highlight = shelf.GetComponent<TutorialHighlighting>();
    //         highlight.HideArrow();

    //         helpTrophy.GetComponent<TutorialHighlighting>().ShowArrow();
    //     }
    // }
    // void openHelpTrophy(){

    //     // make sure we are in trophy mode
    //     // todo: have a better way of checking if we are in trophy mode
    //     if (!ClickManager.CanRespondToTap()){ // meaning we have clicked something
    //         TutorialHighlighting highlight = helpTrophy.GetComponent<TutorialHighlighting>();
    //         highlight.HideArrow();
    //         DataManager.Instance.Tutorial.FirstTimeHelpTrophy = false;
    //     }
    // }

   
}
