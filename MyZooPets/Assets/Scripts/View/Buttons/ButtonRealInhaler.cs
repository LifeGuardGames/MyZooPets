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
//			PlayNotProcessSound();
			string soundToPlay;
			TimeFrames currentTimeFrame = PlayPeriodLogic.GetTimeFrame(LgDateTime.GetTimeNow());
			string popupMessage = "TUT_SUPERWELLA_INHALER";

			if(currentTimeFrame == TimeFrames.Morning){
				popupMessage = "NOTIFICATION_INHALER_TONIGHT";
				soundToPlay = "superWellaInhalerTonight";
			}
			else{
				popupMessage = "NOTIFICATION_INHALER_MORNING";
				soundToPlay = "superWellaInhalerMorning";
			}
				
			PopupNotificationNGUI.Callback okButtonCallback = delegate(){	
			};
			
			//Display tutorial notification
			Hashtable notificationEntry = new Hashtable();
			notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.SuperWellaInhaler);
			notificationEntry.Add(NotificationPopupFields.Message, Localization.Localize(popupMessage));
			notificationEntry.Add(NotificationPopupFields.Button1Callback, okButtonCallback);
			
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
