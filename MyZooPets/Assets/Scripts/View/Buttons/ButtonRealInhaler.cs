using UnityEngine;
using System.Collections;

/// <summary>
/// Button class that loads up the real inhaler game
/// </summary>
public class ButtonRealInhaler : ButtonChangeScene{

	protected override void ProcessClick() {
		OnInhalerButtonClicked();
	}

	/// <summary>
	/// Processes the click.
	/// </summary>
	public void OnInhalerButtonClicked(){
		Debug.Log("Clicked");
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
			TimeFrames currentTimeFrame = PlayPeriodLogic.GetTimeFrame(LgDateTime.GetTimeNow());
			string popupMessage;

			if(currentTimeFrame == TimeFrames.Morning){
				popupMessage = "POPUP_INHALER_TONIGHT";
				AudioManager.Instance.PlayClip("superWellaInhalerTonight");
			}
			else{
				popupMessage = "POPUP_INHALER_MORNING";
				AudioManager.Instance.PlayClip("superWellaInhalerMorning");
			}
			
			// Display tutorial notification
			Hashtable notificationEntry = new Hashtable();
			notificationEntry.Add(NotificationPopupData.PrefabName, "PopupInhalerRecharging");
			notificationEntry.Add(NotificationPopupData.Title, null);
			notificationEntry.Add(NotificationPopupData.Message, Localization.Localize(popupMessage));
			notificationEntry.Add(NotificationPopupData.SpecialButtonCallback, null);
			NotificationUIManager.Instance.AddToQueue(notificationEntry);
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
