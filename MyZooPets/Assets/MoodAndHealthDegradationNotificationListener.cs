using UnityEngine;
using System;
using System.Collections;

public class MoodAndHealthDegradationNotificationListener : MonoBehaviour{

	void Start(){
		RefreshNotifcationListener();
	}

	void OnApplicationPause(bool isPaused){
		if(!isPaused){
			RefreshNotifcationListener();
		}
	}

	private void RefreshNotifcationListener(){
		//listen to health -> sick  or start out sick
		//we will check this only once every play period for 3 consecutive play period
		bool isRemindedThisPlayPeriod = DataManager.Instance.GameData.SickNotification.IsRemindedThisPlayPeriod;
		bool isReminderActive = DataManager.Instance.GameData.SickNotification.IsReminderActive;
		
		PetHealthStates healthState = DataManager.Instance.GameData.Stats.GetHealthState();
		if(healthState == PetHealthStates.Sick || healthState == PetHealthStates.VerySick){
			if(isReminderActive && !isRemindedThisPlayPeriod)
				Invoke("ShowSuperWellaSickReminder", 0.25f);
		}
		//else register for health state change and check if mood needs a notification
		else{
			if(isReminderActive && !isRemindedThisPlayPeriod){
				StatsController.OnHealthyToSick += OnHealthyToSickHandler;
				StatsController.OnHealthyToVerySick += OnHealthyToSickHandler;
			}
			
			PetMoods moodState = DataManager.Instance.GameData.Stats.GetMoodState();
			if(moodState == PetMoods.Sad){
				bool isMoodDecayTutorialDone = DataManager.Instance.GameData.Tutorial.IsTutorialFinished(TutorialManagerBedroom.TUT_MOOD_DEGRADE);
				if(!isMoodDecayTutorialDone)
					Invoke("ShowMoodDegradeMessage", 0.25f);
			}
		}
	}
	
	void OnDestroy(){
		StatsController.OnHealthyToSick -= OnHealthyToSickHandler;
		StatsController.OnHealthyToVerySick -= OnHealthyToSickHandler;
	}

	private void OnHealthyToSickHandler(object sender, EventArgs args){
		ShowSuperWellaSickReminder();
	}

	/// <summary>
	/// Shows two notifications telling user how to increase the pet's mood
	/// </summary>
	private void ShowMoodDegradeMessage(){
		string petName = DataManager.Instance.GameData.PetInfo.PetName;
		string timeMoodDecay1 = String.Format(Localization.Localize("TMD_1"), petName);
		string timeMoodDecay2 = String.Format(Localization.Localize("TMD_2"), petName);
		
		Hashtable notificationEntry = new Hashtable();
		notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.TipWithImage);
		notificationEntry.Add(NotificationPopupFields.Message, timeMoodDecay1);
		notificationEntry.Add(NotificationPopupFields.SpriteName, "Skull");
		
		NotificationUIManager.Instance.AddToQueue(notificationEntry);
		
		notificationEntry[NotificationPopupFields.Message] = timeMoodDecay2;
		notificationEntry[NotificationPopupFields.SpriteName] = "guiPanelStatsHealth";
		
		NotificationUIManager.Instance.AddToQueue(notificationEntry);
		
		DataManager.Instance.GameData.Tutorial.ListPlayed.Add(TutorialManagerBedroom.TUT_MOOD_DEGRADE);		
	}

	private void ShowSuperWellaSickReminder(){
		//unregister listener after notification is shown once
		StatsController.OnHealthyToSick -= OnHealthyToSickHandler;
		StatsController.OnHealthyToVerySick -= OnHealthyToSickHandler;

		//change the necessary data
		DataManager.Instance.GameData.SickNotification.IsRemindedThisPlayPeriod = true;
		DataManager.Instance.GameData.SickNotification.DecreaseReminderCount();

		PopupNotificationNGUI.Callback button1Function = delegate(){
			ClickManager.Instance.Lock(UIModeTypes.Store);

			NavigationUIManager.Instance.HidePanel();
			
			StoreUIManager.OnShortcutModeEnd += CloseShop;
			StoreUIManager.Instance.OpenToSubCategory("Items", true);
		};

		Hashtable notificationEntry = new Hashtable();
		notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.SuperWellaSickReminder);
		notificationEntry.Add(NotificationPopupFields.Button1Callback, button1Function);
		NotificationUIManager.Instance.AddToQueue(notificationEntry);

		AudioManager.Instance.PlayClip("superWellaSick");
	}

	private void CloseShop(object sender, EventArgs args){
		// pop the mode we pushed earlier from the click manager
		ClickManager.Instance.ReleaseLock();
		
		StoreUIManager.OnShortcutModeEnd -= CloseShop;
	}
}

