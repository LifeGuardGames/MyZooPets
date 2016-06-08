using UnityEngine;
using System;
using System.Collections;

public class ShooterGameTutorial {
	public static string TUT_KEY = "SHOOT_TUT";
	GameObject tutBoards;				// Gameobject that positions the tutorial boards	
	GameObject tutorialInhalerUse;		// tutorial message board		
	GameObject pressHere;				// tutorial message board
	GameObject tutorialFinger;			// tutorial finger
	GameObject tutEnemy;				// tutorial enemies
	GameObject fingerPos;
	private int currentStep = 0;


	// in each case we are going to listen to events that tell us to move along
	public void ProcessStep(int step){
		switch(step){
			//the user simply needs to tap the screen
		case 0:
			PlayerShooterController.Instance.OnTutorialMove += MoveAlong;
			//prompt user to shoot
			ShooterGameManager.Instance.tutFinger.SetActive(true);
			break;
		case 1:
			PlayerShooterController.Instance.OnTutorialMove -= MoveAlong;
			//prompt user to shoot
			ShooterGameManager.Instance.tutFinger.SetActive(false);
			pressHere = (GameObject)Resources.Load("ShooterTut");
			tutBoards = GameObjectUtils.AddChildWithPositionAndScale(GameObject.Find("Canvas"), pressHere);
			ShooterGameManager.Instance.OnTutorialTap += MoveAlong;
			break;
		case 2:
			ShooterGameManager.Instance.OnTutorialTap -= MoveAlong;
			ShooterGameManager.Instance.OnTutorialStepDone += MoveAlong;
			GameObject DestroyPrefabsClone = tutBoards;
			GameObject.Destroy(DestroyPrefabsClone);
			tutEnemy = (GameObject)Resources.Load("ShooterTutEnemy");
			GameObjectUtils.AddChildWithPositionAndScale(GameObject.Find("MidPoint"), tutEnemy);
			break;
		case 3:
			//prompt user to shoot
			tutEnemy = (GameObject)Resources.Load("ShooterTutEnemy");
			GameObjectUtils.AddChildWithPositionAndScale(GameObject.Find("Upper"), tutEnemy);
			break;
		case 4:
			//prompt user to shoot
			tutEnemy = (GameObject)Resources.Load("ShooterTutEnemy");
			GameObjectUtils.AddChildWithPositionAndScale(GameObject.Find("Lower"), tutEnemy);
			break;
		// the user must defeat the first wave which is simply a wave of 5 basic enemies
		case 5:
			ShooterGameManager.Instance.OnTutorialStepDone -= MoveAlong;
			ShooterGameEnemyController.Instance.OnTutorialStepDone += MoveAlong;
			ShooterGameEnemyController.Instance.BuildEnemyList();
			break;
		//the user must click the inhaler button to end the tutorial the scene transition should pause after the sun is off screen
		case 6:
			ShooterGameEnemyController.Instance.OnTutorialStepDone -= MoveAlong;
			ShooterInhalerManager.Instance.proceed += MoveAlong;
			GameObject useInhalerTut = (GameObject)Resources.Load("ShooterInhalerTutorial");
			tutorialInhalerUse = GameObjectUtils.AddChildWithPositionAndScale(GameObject.Find("Anchor-Center"), useInhalerTut);
			tutorialFinger = (GameObject)Resources.Load("ShooterPressTut");
			fingerPos = GameObjectUtils.AddChildWithPositionAndScale(GameObject.Find("Anchor-BottomRight"), tutorialFinger);
			break;
		// the user must defeat the first wave which is simply a wave of 5 basic enemies
		case 7:
			GameObject.Destroy(fingerPos);
			GameObject.Destroy(tutorialInhalerUse);
			ShooterGameManager.Instance.OnTutorialStepDone -= MoveAlong;
			ShooterGameEnemyController.Instance.OnTutorialStepDone += MoveAlong;
			DestroyPrefabsClone = tutBoards;
			GameObject.Destroy(DestroyPrefabsClone);
			ShooterGameEnemyController.Instance.BuildEnemyList();
			break;
		//the user must click the inhaler button to end the tutorial the scene transition should pause after the sun is off screen
		case 8:
			ShooterGameEnemyController.Instance.OnTutorialStepDone -= MoveAlong;
			ShooterInhalerManager.Instance.proceed += MoveAlong;
			useInhalerTut = (GameObject)Resources.Load("ShooterInhalerTutorial");
			tutorialInhalerUse = GameObjectUtils.AddChildWithPositionAndScale(GameObject.Find("Anchor-Center"), useInhalerTut);
			tutorialFinger = (GameObject)Resources.Load("ShooterPressTut");
			fingerPos = GameObjectUtils.AddChildWithPositionAndScale(GameObject.Find("Anchor-BottomRight"), tutorialFinger);
			break;
			case 9:
				Debug.Log("step 9");
				GameObject.Destroy(tutorialInhalerUse);
				GameObject.Destroy(fingerPos);
				ShooterGameManager.Instance.FinishedTutorial();
				ShooterGameManager.Instance.inTutorial = false;
                ShooterGameManager.Instance.NewGame();
			break;
		}
	}


	private void MoveAlong(object sender, EventArgs args){
		if(currentStep < 9) {
			currentStep++;
			ProcessStep(currentStep);
		}
	}
}
