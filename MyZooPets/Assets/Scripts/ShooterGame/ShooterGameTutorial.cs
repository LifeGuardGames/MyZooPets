using UnityEngine;
using System;
using System.Collections;

public class ShooterGameTutorial : MinigameTutorial {
	public static string TUT_KEY = "SHOOT_TUT";
	GameObject tutorialFinger;
	GameObject tutorialInhalerUse;
	GameObject pressHere;


	protected override void ProcessStep(int nStep){
		switch(nStep){
		case 0:
			ShooterGameManager.Instance.proceed +=MoveAlong;
			//prompt user to shoot
			pressHere = (GameObject)Resources.Load("ShooterTuTorial");
			tutorialFinger = GameObjectUtils.AddChildWithPositionAndScale(GameObject.Find ("Anchor-Center"),pressHere);
			break;
		case 1:
			ShooterGameManager.Instance.proceed -=MoveAlong;
			ShooterGameEnemyController.Instance.proceed +=MoveAlong;
			GameObject DestroyPrefabsClone = tutorialFinger;
			GameObject.Destroy(DestroyPrefabsClone);
			ShooterGameEnemyController.Instance.BuildEnemyList(DataLoaderTriggerArmy.GetDataList());
			break;
		case 2:
			ShooterGameEnemyController.Instance.proceed -=MoveAlong;
			ShooterInhalerManager.Instance.proceed +=MoveAlong;
			GameObject UseInhaler = (GameObject)Resources.Load("ShooterInhalerTuT");
			tutorialInhalerUse = GameObjectUtils.AddChildWithPositionAndScale(GameObject.Find ("Anchor-Center"),UseInhaler);
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
		GameObject.Destroy(tutorialInhalerUse);
		ShooterGameManager.Instance.inTutorial=false;
		if(!isFinished){
			ShooterGameManager.Instance.reset();
		}
	}


	private void MoveAlong(object sender, EventArgs args){
			Advance();
		}
}
