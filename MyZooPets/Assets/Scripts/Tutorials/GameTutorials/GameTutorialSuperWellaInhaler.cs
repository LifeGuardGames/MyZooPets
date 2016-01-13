using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// wait for few seconds
/// spawn super wella message to tell user to take inhaler morning and night
/// </summary>
public class GameTutorialSuperWellaInhaler : GameTutorial {
	public bool isDelegateUsed = false;	// HACK Used to make sure delegate doesnt get called twice

	public GameTutorialSuperWellaInhaler() : base(){	
	}
	
	protected override void SetMaxSteps(){
		maxSteps = 1;
	}
	
	protected override void SetKey(){
		tutorialKey = TutorialManagerBedroom.TUT_SUPERWELLA_INHALER;
	}
	
	protected override void _End(bool isFinished){
	}
	
	protected override void ProcessStep(int step){
		switch(step){
		case 0:
			TutorialManager.Instance.StartCoroutine(ShowSuperWella());
			break;
		}
	}

	private IEnumerator ShowSuperWella(){
		yield return new WaitForSeconds(1f);

		PopupNotificationNGUI.Callback okButtonCallback = delegate(){
			if(!isDelegateUsed){
				isDelegateUsed = true;
				PetAudioManager.Instance.EnableSound = true;
				Advance();
			}
		};

		//Display tutorial notification
		Hashtable notificationEntry = new Hashtable();
		notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.SuperWellaInhaler);
		notificationEntry.Add(NotificationPopupFields.Message, Localization.Localize("TUT_SUPERWELLA_INHALER"));
		notificationEntry.Add(NotificationPopupFields.Button1Callback, okButtonCallback);
		
		NotificationUIManager.Instance.AddToQueue(notificationEntry);

		AudioManager.Instance.PlayClip("superWellaInhalerTutorial");
		PetAudioManager.Instance.EnableSound = false;
	}
}
