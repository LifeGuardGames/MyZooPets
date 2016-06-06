using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Button class that loads up the real inhaler game
/// </summary>
public class ButtonRealInhaler : ButtonChangeScene{
	
	/// <summary>
	/// Processes the click.
	/// </summary>
	protected override void ProcessClick(){
		CheckToOpenInhaler();
	}
	
	/// <summary>
	/// Checks if inhaler can be used at the current time.
	/// Open if yes or show notification.
	/// </summary>
	private void CheckToOpenInhaler(){
		if(PlayPeriodLogic.Instance.CanUseEverydayInhaler()){
			OpenRealInhaler();
		}
		else{
			string soundToPlay;
			TimeFrames currentTimeFrame = PlayPeriodLogic.GetTimeFrame(LgDateTime.GetTimeNow());
			string popupMessage;

			if(currentTimeFrame == TimeFrames.Morning){
				popupMessage = "POPUP_INHALER_TONIGHT";
				soundToPlay = "superWellaInhalerTonight";
			}
			else{
				popupMessage = "POPUP_INHALER_MORNING";
				soundToPlay = "superWellaInhalerMorning";
			}
			
			//Display tutorial notification
			Hashtable notificationEntry = new Hashtable();
			notificationEntry.Add(NotificationPopupData.PrefabName, "PopupInhalerRecharging");
			notificationEntry.Add(NotificationPopupData.Title, null);
			notificationEntry.Add(NotificationPopupData.Message, Localization.Localize(popupMessage));
			notificationEntry.Add(NotificationPopupData.SpecialButtonCallback, null);
			NotificationUIManager.Instance.AddToQueue(notificationEntry);

			AudioManager.Instance.PlayClip(soundToPlay);
		}
	}

	/// <summary>
	/// Opens the real inhaler.
	/// </summary>
	public void OpenRealInhaler(){
		// use parent
		base.ProcessClick();
	}	
}
