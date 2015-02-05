using UnityEngine;
using System;
using System.Collections;

public class ShooterGameTutorial : MinigameTutorial {
	public static string TUT_KEY = "SHOOT_TUT";
	GameObject tutBoards;				// Gameobject that positions the tutorial boards			
	GameObject tutorialInhalerUse;		// tutorial message board		
	GameObject pressHere;				// tutorial message board

	// in each case we are going to listen to events that tell us to move along
	protected override void ProcessStep(int nStep){
		switch(nStep){
		//the user simply needs to tap the screen 
		case 0:
			ShooterGameManager.Instance.proceed +=MoveAlong;
			//prompt user to shoot
			pressHere = (GameObject)Resources.Load("ShooterTutorial");
			tutBoards = GameObjectUtils.AddChildWithPositionAndScale(GameObject.Find ("Anchor-Center"),pressHere);
			break;
		// the user must defeat the first wave which is simply a wave of 5 basic enemies
		case 1:
			ShooterGameManager.Instance.proceed -=MoveAlong;
			ShooterGameEnemyController.Instance.proceed +=MoveAlong;
			GameObject DestroyPrefabsClone = tutBoards;
			GameObject.Destroy(DestroyPrefabsClone);
			ShooterGameEnemyController.Instance.BuildEnemyList(DataLoaderTriggerArmy.GetDataList());
			break;
		//the user must click the inhaler button to end the tutorial the scene transition should pause after the sun is off screen
		case 2:
			ShooterGameEnemyController.Instance.proceed -=MoveAlong;
			ShooterInhalerManager.Instance.proceed +=MoveAlong;
			GameObject UseInhaler = (GameObject)Resources.Load("ShooterInhalerTuT");
			tutorialInhalerUse = GameObjectUtils.AddChildWithPositionAndScale(GameObject.Find ("Anchor-Center"),UseInhaler);
			break;
		
		// the user must defeat the first wave which is simply a wave of 5 basic enemies
		case 3:
			ShooterGameManager.Instance.proceed -=MoveAlong;
			ShooterGameEnemyController.Instance.proceed +=MoveAlong;
			DestroyPrefabsClone = tutBoards;
			GameObject.Destroy(DestroyPrefabsClone);
			ShooterGameEnemyController.Instance.BuildEnemyList(DataLoaderTriggerArmy.GetDataList());
		break;
		//the user must click the inhaler button to end the tutorial the scene transition should pause after the sun is off screen
		case 4:
			ShooterGameEnemyController.Instance.proceed -=MoveAlong;
			ShooterInhalerManager.Instance.proceed +=MoveAlong;
			UseInhaler = (GameObject)Resources.Load("ShooterInhalerTuT");
			tutorialInhalerUse = GameObjectUtils.AddChildWithPositionAndScale(GameObject.Find ("Anchor-Center"),UseInhaler);
		break;
		}
	}


	protected override void SetKey(){
		tutorialKey = TUT_KEY;
	}

	protected override void SetMaxSteps(){
		maxSteps = 6;
	}
	// oce we are done destroy the remaining board and reset for round 1
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
