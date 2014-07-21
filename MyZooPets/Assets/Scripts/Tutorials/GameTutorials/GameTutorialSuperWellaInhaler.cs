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
	
		//add fire orb to interactable list after some setting change
		if(fireOrbObject != null){
			fireOrbObject.GetComponent<DroppedObjectItem>().TurnAutoCollectOff();
			fireOrbObject.GetComponent<LgButton>().OnProcessed += PickedUpFireOrb;
			AddToProcessList(fireOrbObject);
		}
	}

	/// <summary>
	/// Pickeds up fire orb. 
	/// Advance the tutorial after fire orb is picked up
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void PickedUpFireOrb(object sender, EventArgs args){
		LgButton button = (LgButton)sender;
		button.OnProcessed -= PickedUpFireOrb;

		Advance();
	}

	private IEnumerator ShowSuperWella(){
		yield return new WaitForSeconds(1.5f);

		PopupNotificationNGUI.HashEntry okButtonCallback = delegate(){	
			Advance();
		};

		//Display tutorial notification
		Hashtable notificationEntry = new Hashtable();
		notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.SuperWellaInhaler);
		notificationEntry.Add(NotificationPopupFields.Message, Localization.Localize("TUT_SUPERWELLA_INHALER"));
		notificationEntry.Add(NotificationPopupFields.Button1Callback, okButtonCallback);
		
		NotificationUIManager.Instance.AddToQueue(notificationEntry);

		AudioManager.Instance.PlayClip("tutorialSuperWellaInhaler");
	}
}
