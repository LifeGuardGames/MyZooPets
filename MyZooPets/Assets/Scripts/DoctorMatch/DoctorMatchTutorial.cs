using UnityEngine;
using System;
using System.Collections;

public class DoctorMatchTutorial : MinigameTutorial {
	public static string TUT_KEY = "DOCTORMATCH_TUT";

	
	protected override void SetMaxSteps(){
		maxSteps = 3;
	}
	
	protected override void SetKey(){
		tutorialKey = TUT_KEY;
	}
	
	protected override void _End(bool isFinished){
	}
	
	protected override void ProcessStep(int step){
		Hashtable option = new Hashtable();
		
		switch(step){
		case 0:
				SetUpCharacterGroup(1);
			break;
		case 1:
				SetUpCharacterGroup(2);
			break;
		case 2:
				SetUpCharacterGroup(3);
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
