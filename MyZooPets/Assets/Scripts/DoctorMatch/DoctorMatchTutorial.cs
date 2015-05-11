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
	// handles multiple sprites needed for completeion 
	private int numOfCompleteions = 0;

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
				SetUpCharacterGroup(1);
				zone1Collider.enabled = true;
				zone2Collider.enabled = false;
				// greys out non active collider
				zone2Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
				zone3Collider.enabled = false;
				// greys out non active collider
				zone3Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
				fingerTutorialAnimation.Play("DoctorTut1");
			break;
		
		case 1:
				SetUpCharacterGroup(2);
				zone1Collider.enabled = false;
				zone1Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
				zone2Collider.enabled = true;
				zone2Collider.GetComponentInChildren<SpriteRenderer>().color=Color.white;
				zone3Collider.enabled = false;
				zone3Collider.GetComponentInChildren<SpriteRenderer>().color=Color.grey;
				fingerTutorialAnimation.Play("DoctorTut2");
			break;
		
		case 2:
			SetUpCharacterGroup(3);
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
	// spawns the two sprites associated with the situation
	private void SetUpCharacterGroup(int itemGroupNumber){
		GameObject[] stepItem = new GameObject[2];
		for (int i = 0; i <2; i++){
		stepItem[i] = DoctorMatchManager.Instance.assemblyLineController.SpawnItemForTutorial();
		DoctorMatchManager.Instance.SetUpAssemblyItemSpriteTutorial(stepItem[i], i,itemGroupNumber: itemGroupNumber);
		}
		stepItem[0].transform.position = new Vector3(0, 1.5f, 0);
		stepItem[1].transform.position = new Vector3(2.8f, 1.5f, 0);

		
		DoctorMatchManager.OnCharacterScoredRight += OnCharacterScoredRightEventHandler;
	}

	private void OnCharacterScoredRightEventHandler(object sender, EventArgs args){
		numOfCompleteions++;
		// handles more than one completion
		if(numOfCompleteions == 2){
			numOfCompleteions = 0;
		DoctorMatchManager.OnCharacterScoredRight -= OnCharacterScoredRightEventHandler;
		Advance();
		}
	}
}
