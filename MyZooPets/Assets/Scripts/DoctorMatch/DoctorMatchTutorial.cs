using UnityEngine;
using System;
using System.Collections;

public class DoctorMatchTutorial : MinigameTutorial {
	public static string TUT_KEY = "DOCTORMATCH_TUT";
	private Collider2D zone1Collider;
	private Collider2D zone2Collider;
	private Collider2D zone3Collider;
	private GameObject fingerTutorialPrefab;
	private GameObject fingerTutorialObject;
	private Animation fingerTutorialAnimation;

	protected override void SetMaxSteps(){
		maxSteps = 3;
	}
	
	protected override void SetKey(){
		tutorialKey = TUT_KEY;
	}
	
	protected override void _End(bool isFinished){

		if(isFinished){
			// Enable all the collider and continue the game
			zone1Collider.enabled = true;
			zone2Collider.enabled = true;
			zone3Collider.enabled = true;

			if(fingerTutorialObject)
				GameObject.Destroy(fingerTutorialObject);
		}
		//tutorial aborted so need to do some clean up
		else{
			DoctorMatchManager.OnCharacterScoredRight -= OnCharacterScoredRightEventHandler;

			if(fingerTutorialObject)
				GameObject.Destroy(fingerTutorialObject);
		}
	}
	
	protected override void ProcessStep(int step){
		// Cache this on initial call
		if(zone1Collider == null || zone2Collider == null || zone3Collider == null){
			zone1Collider = GameObject.Find("Zone1").collider2D;
			zone2Collider = GameObject.Find("Zone2").collider2D;
			zone3Collider = GameObject.Find("Zone3").collider2D;
		}

		// Cache the finger tutorial animation
		if(fingerTutorialPrefab == null){
			fingerTutorialPrefab = Resources.Load("DoctorMatchTut") as GameObject;
			fingerTutorialObject = GameObject.Instantiate(fingerTutorialPrefab) as GameObject;
			GameObject animObject = fingerTutorialObject.transform.FindChild("AnimationParent").gameObject;
			fingerTutorialAnimation = animObject.GetComponent<Animation>();
			fingerTutorialAnimation.Play();
		}

		switch(step){
		case 0:
				SetUpCharacterGroup(1);
				zone1Collider.enabled = true;
				zone2Collider.enabled = false;
				zone3Collider.enabled = false;
				fingerTutorialAnimation.Play("DoctorTut1");
			break;
		case 1:
				SetUpCharacterGroup(2);
				zone1Collider.enabled = false;
				zone2Collider.enabled = true;
				zone3Collider.enabled = false;
				fingerTutorialAnimation.Play("DoctorTut2");
			break;
		case 2:
				SetUpCharacterGroup(3);
				zone1Collider.enabled = false;
				zone2Collider.enabled = false;
				zone3Collider.enabled = true;
				fingerTutorialAnimation.Play("DoctorTut3");
			break;
		default:
			Debug.LogError("Ninja tutorial has an unhandled step: " + step);
			break;
		}
	}

	private void SetUpCharacterGroup(int itemGroupNumber){
		GameObject stepItem = DoctorMatchManager.Instance.assemblyLineController.SpawnItemForTutorial();
		DoctorMatchManager.Instance.SetUpAssemblyItemSprite(stepItem, itemGroupNumber: itemGroupNumber);
		
		stepItem.transform.position = new Vector3(0, 1.5f, 0);
		
		DoctorMatchManager.OnCharacterScoredRight += OnCharacterScoredRightEventHandler;
	}

	private void OnCharacterScoredRightEventHandler(object sender, EventArgs args){
		DoctorMatchManager.OnCharacterScoredRight -= OnCharacterScoredRightEventHandler;
		Advance();
	}
}
