using UnityEngine;
using System;
using System.Collections;

public class ShooterGameTutorial : MinigameTutorial {
	public static string TUT_KEY = "SHOOT_TUT";
	GameObject TutorialFinger;
	GameObject TutorialInhalerUse;
	GameObject PressHere;


	protected override void ProcessStep(int nStep){
		switch(nStep){
		case 0:
			ShooterGameManager.Instance.Proceed +=MoveAlong;
			//prompt user to shoot
			PressHere = (GameObject)Resources.Load("ShooterTuTorial");
			TutorialFinger = LgNGUITools.AddChildWithPositionAndScale(GameObject.Find ("Anchor-Center"),PressHere);
			break;
		case 1:
			ShooterGameManager.Instance.Proceed -=MoveAlong;
			ShooterGameEnemyController.Instance.Proceed +=MoveAlong;
			GameObject DestroyPrefabsClone = TutorialFinger;
			GameObject.Destroy(DestroyPrefabsClone);
			ShooterGameEnemyController.Instance.BuildEnemyList(DataLoaderTriggerArmy.GetDataList());
			break;
		case 2:
			ShooterGameEnemyController.Instance.Proceed -=MoveAlong;
			ShooterInhalerManager.Instance.Proceed +=MoveAlong;
			GameObject UseInhaler = (GameObject)Resources.Load("ShooterInhalerTuT");
			TutorialInhalerUse = LgNGUITools.AddChildWithPositionAndScale(GameObject.Find ("Anchor-Center"),UseInhaler);
			break;
		}
	}

	protected override void SetKey(){
		tutorialKey = TUT_KEY;
	}

	protected override void SetMaxSteps(){
		maxSteps = 3;
	}

	protected override void _End(bool isFinished){
		GameObject.Destroy(TutorialInhalerUse);
		ShooterGameManager.Instance.InTutorial=false;
		if(!isFinished){
			ShooterGameManager.Instance.reset();
		}
	}


	private void MoveAlong(object sender, EventArgs args){
			Advance();
		}
}
