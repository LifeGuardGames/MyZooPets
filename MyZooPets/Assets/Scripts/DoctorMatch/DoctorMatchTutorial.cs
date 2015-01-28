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
		maxSteps = 15;
	}
	
	protected override void SetKey(){
		tutorialKey = TUT_KEY;
	}
	
	protected override void _End(bool isFinished){

		if(isFinished){
			// Enable all the collider and continue the game
			zone1Collider.enabled = true;
			zone1Collider.GetComponentInChildren<SpriteRenderer>().color=Color.white;
			zone2Collider.enabled = true;
			zone2Collider.GetComponentInChildren<SpriteRenderer>().color=Color.white;
			zone3Collider.enabled = true;
			zone3Collider.GetComponentInChildren<SpriteRenderer>().color=Color.white;

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
				SetUpCharacterGroup(1,0);
				zone1Collider.enabled = true;
				zone2Collider.enabled = false;
				zone2Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
				zone3Collider.enabled = false;
				zone3Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
				fingerTutorialAnimation.Play("DoctorTut1");
			break;
		case 1:
			SetUpCharacterGroup(1,1);
			zone1Collider.enabled = true;
			zone2Collider.enabled = false;
			zone2Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
			zone3Collider.enabled = false;
			zone3Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
			fingerTutorialAnimation.Play("DoctorTut1");
			break;
		case 2:
			SetUpCharacterGroup(1,2);
			zone1Collider.enabled = true;
			zone2Collider.enabled = false;
			zone2Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
			zone3Collider.enabled = false;
			zone3Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
			fingerTutorialAnimation.Play("DoctorTut1");
			break;
		case 3:
			SetUpCharacterGroup(1,3);
			zone1Collider.enabled = true;
			zone2Collider.enabled = false;
			zone2Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
			zone3Collider.enabled = false;
			zone3Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
			fingerTutorialAnimation.Play("DoctorTut1");
			break;
		case 4:
			SetUpCharacterGroup(1,4);
			zone1Collider.enabled = true;
			zone2Collider.enabled = false;
			zone2Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
			zone3Collider.enabled = false;
			zone3Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
			fingerTutorialAnimation.Play("DoctorTut1");
			break;
		case 5:
				SetUpCharacterGroup(2,0);
				zone1Collider.enabled = false;
				zone1Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
				zone2Collider.enabled = true;
				zone2Collider.GetComponentInChildren<SpriteRenderer>().color=Color.white;
				zone3Collider.enabled = false;
				zone3Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
				fingerTutorialAnimation.Play("DoctorTut2");
			break;
		case 6:
			SetUpCharacterGroup(2,1);
			zone1Collider.enabled = false;
			zone1Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
			zone2Collider.enabled = true;
			zone2Collider.GetComponentInChildren<SpriteRenderer>().color=Color.white;
			zone3Collider.enabled = false;
			zone3Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
			fingerTutorialAnimation.Play("DoctorTut2");
			break;
		case 7:
			SetUpCharacterGroup(2,2);
			zone1Collider.enabled = false;
			zone1Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
			zone2Collider.enabled = true;
			zone2Collider.GetComponentInChildren<SpriteRenderer>().color=Color.white;
			zone3Collider.enabled = false;
			zone3Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
			fingerTutorialAnimation.Play("DoctorTut2");
			break;
		case 8:
			SetUpCharacterGroup(2,3);
			zone1Collider.enabled = false;
			zone1Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
			zone2Collider.enabled = true;
			zone2Collider.GetComponentInChildren<SpriteRenderer>().color=Color.white;
			zone3Collider.enabled = false;
			zone3Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
			fingerTutorialAnimation.Play("DoctorTut2");
			break;
		case 9:
			SetUpCharacterGroup(2,4);
			zone1Collider.enabled = false;
			zone1Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
			zone2Collider.enabled = true;
			zone2Collider.GetComponentInChildren<SpriteRenderer>().color=Color.white;
			zone3Collider.enabled = false;
			zone3Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
			fingerTutorialAnimation.Play("DoctorTut2");
			break;
		case 10:
			SetUpCharacterGroup(3,0);
			zone1Collider.enabled = false;
			zone1Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
			zone2Collider.enabled = false;
			zone2Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
			zone3Collider.enabled = true;
			zone3Collider.GetComponentInChildren<SpriteRenderer>().color=Color.white;
			fingerTutorialAnimation.Play("DoctorTut3");
			break;
		case 11:
			SetUpCharacterGroup(3,1);
			zone1Collider.enabled = false;
			zone1Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
			zone2Collider.enabled = false;
			zone2Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
			zone3Collider.enabled = true;
			zone3Collider.GetComponentInChildren<SpriteRenderer>().color=Color.white;
			fingerTutorialAnimation.Play("DoctorTut3");
			break;
		case 12:
			SetUpCharacterGroup(3,2);
			zone1Collider.enabled = false;
			zone1Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
			zone2Collider.enabled = false;
			zone2Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
			zone3Collider.enabled = true;
			zone3Collider.GetComponentInChildren<SpriteRenderer>().color=Color.white;
			fingerTutorialAnimation.Play("DoctorTut3");
			break;
		case 13:
			SetUpCharacterGroup(3,3);
			zone1Collider.enabled = false;
			zone1Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
			zone2Collider.enabled = false;
			zone2Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
			zone3Collider.enabled = true;
			zone3Collider.GetComponentInChildren<SpriteRenderer>().color=Color.white;
			fingerTutorialAnimation.Play("DoctorTut3");
			break;
		case 14:
			SetUpCharacterGroup(3,4);
			zone1Collider.enabled = false;
			zone1Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
			zone2Collider.enabled = false;
			zone2Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
			zone3Collider.enabled = true;
			zone3Collider.GetComponentInChildren<SpriteRenderer>().color=Color.white;
			fingerTutorialAnimation.Play("DoctorTut3");
			break;
		default:
			Debug.LogError("Ninja tutorial has an unhandled step: " + step);
			break;
		}
	}

	private void SetUpCharacterGroup(int itemGroupNumber, int spriteNum){
		GameObject stepItem = DoctorMatchManager.Instance.assemblyLineController.SpawnItemForTutorial();
		DoctorMatchManager.Instance.SetUpAssemblyItemSpriteTutorial(stepItem, spriteNum,itemGroupNumber: itemGroupNumber);
		
		stepItem.transform.position = new Vector3(0, 1.5f, 0);
		
		DoctorMatchManager.OnCharacterScoredRight += OnCharacterScoredRightEventHandler;
	}

	private void OnCharacterScoredRightEventHandler(object sender, EventArgs args){
		DoctorMatchManager.OnCharacterScoredRight -= OnCharacterScoredRightEventHandler;
		Advance();
	}
}
