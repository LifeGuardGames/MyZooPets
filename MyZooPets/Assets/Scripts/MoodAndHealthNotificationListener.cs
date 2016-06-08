using UnityEngine;
using System;
using System.Collections;

public class MoodAndHealthNotificationListener : MonoBehaviour{
	void Start(){
		RefreshNotificationListener();
	}

	void OnApplicationPause(bool isPaused){
		if(!isPaused){
			RefreshNotificationListener();
		}
	}

	private void RefreshNotificationListener(){
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
				StatsManager.OnHealthyToSick += OnHealthyToSickHandler;
				StatsManager.OnHealthyToVerySick += OnHealthyToSickHandler;
			}
			
			PetMoods moodState = DataManager.Instance.GameData.Stats.GetMoodState();
			if(moodState == PetMoods.Sad && SceneUtils.CurrentScene == SceneUtils.BEDROOM){
				bool isMoodDecayTutorialDone = DataManager.Instance.GameData.Tutorial.IsTutorialFinished(TutorialManagerBedroom.TUT_MOOD_DEGRADE);
				if(!isMoodDecayTutorialDone) {
					Invoke("ShowPopupNoEnergy", 0.25f);
				}
			}
		}
	}
	
	void OnDestroy(){
		StatsManager.OnHealthyToSick -= OnHealthyToSickHandler;
		StatsManager.OnHealthyToVerySick -= OnHealthyToSickHandler;
	}

	private void OnHealthyToSickHandler(object sender, EventArgs args){
		ShowPopupPetSick();
	}

	/// <summary>
	/// Shows two notifications telling user how to increase the pet's mood
	/// </summary>
	private void ShowPopupNoEnergy(){
		string popupTitle = Localization.Localize("POPUP_NO_ENERGY_TITLE");
        string petName = DataManager.Instance.GameData.PetInfo.PetName;
		string popupMessage = string.Format(Localization.Localize("POPUP_NO_ENERGY"), petName);
		PopupController.Callback okButtonCallback = delegate(){
			StoreUIManager.OnShortcutModeEnd += CloseShop;
			ClickManager.Instance.Lock(UIModeTypes.Store);
			StoreUIManager.Instance.OpenToSubCategory("Food", true, StoreShortcutType.NeedFoodTutorial);
		};
		
		Hashtable notificationEntry = new Hashtable();
		notificationEntry.Add(NotificationPopupData.PrefabName, "PopupNoEnergy");
		notificationEntry.Add(NotificationPopupData.Title, popupTitle);
		notificationEntry.Add(NotificationPopupData.Message, popupMessage);
		notificationEntry.Add(NotificationPopupData.SpecialButtonCallback, okButtonCallback);
		NotificationUIManager.Instance.AddToQueue(notificationEntry);
		
		DataManager.Instance.GameData.Tutorial.ListPlayed.Add(TutorialManagerBedroom.TUT_MOOD_DEGRADE);	
	}

	// Called from the shortcut mode in shop
	private void CloseShop(){
		StoreUIManager.OnShortcutModeEnd -= CloseShop;
		ClickManager.Instance.ReleaseLock();
	}

	private void ShowPopupPetSick(){
		//unregister listener after notification is shown once
		StatsManager.OnHealthyToSick -= OnHealthyToSickHandler;
		StatsManager.OnHealthyToVerySick -= OnHealthyToSickHandler;

		// Change the necessary data
		DataManager.Instance.GameData.SickNotification.IsRemindedThisPlayPeriod = true;
		DataManager.Instance.GameData.SickNotification.DecreaseReminderCount();

		PopupController.Callback button1Function = delegate(){
			ClickManager.Instance.Lock(UIModeTypes.Store);
			NavigationUIManager.Instance.HidePanel();
			StoreUIManager.OnShortcutModeEnd += CloseShop;
			StoreUIManager.Instance.OpenToSubCategory("Items", true, StoreShortcutType.SickNotification);
		};

		Hashtable notificationEntry = new Hashtable();
		notificationEntry.Add(NotificationPopupData.PrefabName, "PopupNoEnergy");
		notificationEntry.Add(NotificationPopupData.Title, Localization.Localize("POPUP_PET_SICK_TITLE"));
		notificationEntry.Add(NotificationPopupData.Message, Localization.Localize("POPUP_PET_SICK"));
		notificationEntry.Add(NotificationPopupData.SpecialButtonCallback, button1Function);
		NotificationUIManager.Instance.AddToQueue(notificationEntry);

		AudioManager.Instance.PlayClip("superWellaSick");
	}

	private void CloseShop(object sender, EventArgs args){
		// pop the mode we pushed earlier from the click manager
		ClickManager.Instance.ReleaseLock();
		
		StoreUIManager.OnShortcutModeEnd -= CloseShop;
	}


	/*void OnGUI() {
		if(GUI.Button(new Rect(100, 100, 100, 100), "SDF")) {
			ShowPopupPetSick();
		}
	}*/
}

