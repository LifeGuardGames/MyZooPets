using UnityEngine;
using System;
using System.Collections;

public class GameTutorialFlameCrystal : GameTutorial {
	GameObject fireOrbFingerHint;

	protected override void SetMaxSteps(){
		maxSteps = 1;
	}
	
	protected override void SetKey(){
		tutorialKey = TutorialManagerBedroom.TUT_FLAME_CRYSTAL;
	}
	
	protected override void _End(bool isFinished){
	}
	
	protected override void ProcessStep(int step){
		Debug.Log("TUTORIAL FLAME " + step);
		switch(step){
		case 0:
			RoomArrowsUIManager.Instance.HidePanel();
			TutorialManager.Instance.StartCoroutine(DragFireOrbHint());
			break;
		}
	}

	private IEnumerator DragFireOrbHint(){
		yield return new WaitForSeconds(0.5f);
		FireButtonManager.FireButtonActive += FireButtonActiveEventHandler;

		// Add fire orb to clickable list
		Debug.LogWarning("Check process list for fire orb");
		GameObject fireOrbItemReference = InventoryUIManager.Instance.SearchCurrentPageObject("Usable1");
		AddToProcessList(fireOrbItemReference);

		// Show the spotlight and finger
		GameObject fireButtonReference = FireButtonManager.Instance.FireButtonObject;
		ShowFingerHint(fireButtonReference, fingerState: BedroomTutFingerController.FingerState.FireCrystalDrag);
	}

	/// <summary>
	/// When the fire button is active advance tutorial.
	/// </summary>
	private void FireButtonActiveEventHandler(object sender, EventArgs args){
		FireButtonManager.FireButtonActive -= FireButtonActiveEventHandler;
		RemoveFingerHint();
		Advance();
	}
}
