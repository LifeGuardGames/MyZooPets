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
		GameObject fireButtonReference = null;
		GameObject fireOrbItemReference = null;

		try{
			//add fire orb to the clickable list
			FireButtonManager.FireButtonActive += FireButtonActiveEventHandler;
			//fireOrbItemReference = InventoryUIManager.Instance.GetFireOrbReference();	// TODO ------------------
			fireButtonReference = FireButtonManager.Instance.FireButtonObject;

			Debug.LogWarning("NGUI REMOVE CHANGED - CORRECT CODE HERE");
			Vector3 fireOrbItemPosition = Vector3.zero; // TEMP CODE - remove me once fixed
			Vector3 fireButtonPosition = Vector3.zero; // TEMP CODE - remove me once fixed
													   //Vector3 fireOrbItemPosition = LgNGUITools.GetScreenPosition(fireOrbItemReference);
													   //Vector3 fireButtonPosition = LgNGUITools.GetScreenPosition(fireButtonReference);

			//AddToProcessList(fireOrbItemReference);	// TODO ------------------------------------

			ShowFingerHint(fireButtonReference, fingerState: BedroomTutFingerController.FingerState.FireCrystalDrag);

			// set the hint to the right spawn location
			/*
			Vector3 hintPosition = fireOrbItemPosition;
			hintPosition.z = fireOrbFingerHint.transform.localPosition.z;
			fireOrbFingerHint.transform.localPosition = hintPosition;

			fireButtonPosition = CameraManager.Instance.TransformAnchorPosition(fireButtonPosition, 
			                                                                    InterfaceAnchors.Center, 
			                                                                    InterfaceAnchors.BottomRight);
			fireButtonPosition.z = fireOrbFingerHint.transform.localPosition.z;

			LeanTween.moveLocal(fireOrbFingerHint, fireButtonPosition, 2f)
				.setLoopClamp().setRepeat(-1).setEase(LeanTweenType.easeInOutQuad);
				*/
		}
		catch(NullReferenceException e){
			Debug.LogError(e.Message);
		}
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
