using UnityEngine;
using System;
using System.Collections;

public class GameTutorialSuperWellaInhaler : GameTutorial {
	// wait for few seconds
	// spawn super wella message to tell user to take inhaler morning and night

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
			PetAudioManager.Instance.EnableSound = true;
			Advance();
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
