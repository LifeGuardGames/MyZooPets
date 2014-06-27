using UnityEngine;
using System.Collections;

public class GameTutorialSuperWellaInhaler : GameTutorial {
	public GameTutorialSuperWellaInhaler() : base(){	
	}
	
	protected override void SetMaxSteps(){
		maxSteps = 1;
	}
	
	protected override void SetKey(){
		tutorialKey = TutorialManagerBedroom.TUT_SUPERWELLA_INHALER;
	}
	
	protected override void _End(bool isFinished){
		NotificationUIManager.Instance.CleanupNotification();
	}
	
	protected override void ProcessStep(int step){
		switch(step){
		case 0:
			// the start of the focus inhaler tutorial

			TutorialManager.Instance.StartCoroutine(ShowSuperWella());
			
			break;
			
		
		}
	}

	private IEnumerator ShowSuperWella(){
		yield return new WaitForSeconds(0.5f);

		PopupNotificationNGUI.HashEntry okButtonCallback = delegate(){	
			Advance();
		};
		
		Hashtable notificationEntry = new Hashtable();
		notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.InhalerTutorial);
		notificationEntry.Add(NotificationPopupFields.Button1Callback, okButtonCallback);
		
		NotificationUIManager.Instance.AddToQueue(notificationEntry);

		AudioManager.Instance.PlayClip("tutorialSuperWellaInhaler");
	}
	
}
