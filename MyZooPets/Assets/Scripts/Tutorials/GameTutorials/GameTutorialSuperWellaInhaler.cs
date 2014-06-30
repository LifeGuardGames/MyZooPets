using UnityEngine;
using System;
using System.Collections;

public class GameTutorialSuperWellaInhaler : GameTutorial {
	// wait for few seconds
	// fire orb spew out of inhaler
	// then focus on fire orb. tell the user to pick it up
	// spawn super wella message to tell user to take inhaler morning and night

	public GameTutorialSuperWellaInhaler() : base(){	
	}
	
	protected override void SetMaxSteps(){
		maxSteps = 2;
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
			TutorialManager.Instance.StartCoroutine(DropFireOrb());
			break;
		case 1:
			TutorialManager.Instance.StartCoroutine(ShowSuperWella());
			break;
		}
	}

	private IEnumerator DropFireOrb(){
		yield return new WaitForSeconds(0.5f);

		BedroomInhalerUIManager.Instance.CheckToDropFireOrb();
		GameObject fireOrbObject = BedroomInhalerUIManager.Instance.FireOrbReference;
	
		if(fireOrbObject != null){
			fireOrbObject.GetComponent<DroppedObjectItem>().TurnAutoCollectOff();
			fireOrbObject.GetComponent<LgButton>().OnProcessed += PickedUpFireOrb;
			AddToProcessList(fireOrbObject);
		}
		//need to get reference of the dropped fire orb and add it to the process list
	}

	private void PickedUpFireOrb(object sender, EventArgs args){
		LgButton button = (LgButton)sender;
		button.OnProcessed -= PickedUpFireOrb;

		Advance();
	}

	private IEnumerator ShowSuperWella(){
		yield return new WaitForSeconds(1f);

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
