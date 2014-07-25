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
		switch(step){
		case 0:

			TutorialManager.Instance.StartCoroutine(DragFireOrbHint());
			break;
		}
	}

	private IEnumerator DragFireOrbHint(){
		yield return new WaitForSeconds(0.5f);
		
		//add fire orb to the clickable list
		FireButtonUIManager.FireButtonActive += FireButtonActiveEventHandler;
		GameObject fireOrbItemReference = InventoryUIManager.Instance.GetFireOrbReference();
		GameObject fireButtonReference = FireButtonUIManager.Instance.GetFireButtonReference();
		Vector3 fireOrbItemPosition = LgNGUITools.GetScreenPosition(fireOrbItemReference);
		Vector3 fireButtonPosition = LgNGUITools.GetScreenPosition(fireButtonReference);
		
		AddToProcessList(fireOrbItemReference);
		
		fireOrbFingerHint = LgNGUITools.AddChildWithPosition(GameObject.Find("Anchor-BottomRight"),
		                                                     (GameObject)Resources.Load("FireOrbFingerHint"));
		
		// set the hint to the right spawn location
		Vector3 hintPosition = fireOrbItemPosition;
		hintPosition.z = fireOrbFingerHint.transform.localPosition.z;
		fireOrbFingerHint.transform.localPosition = hintPosition;
		
		
		fireButtonPosition = CameraManager.Instance.TransformAnchorPosition(fireButtonPosition, 
		                                                                    InterfaceAnchors.Center, 
		                                                                    InterfaceAnchors.BottomRight);
		fireButtonPosition.z = fireOrbFingerHint.transform.localPosition.z;
		
		Hashtable optional = new Hashtable();
		optional.Add("repeat", 0);
		LeanTween.moveLocal(fireOrbFingerHint, fireButtonPosition, 3f, optional);
	}

	/// <summary>
	/// When the fire button is active advance tutorial.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void FireButtonActiveEventHandler(object sender, EventArgs args){
		FireButtonUIManager.FireButtonActive -= FireButtonActiveEventHandler;
		
		// clean up tween from last step
		LeanTween.cancel(fireOrbFingerHint);
		GameObject.Destroy(fireOrbFingerHint);
		
		Advance();
	}
}
