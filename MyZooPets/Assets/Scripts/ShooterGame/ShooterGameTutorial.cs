using UnityEngine;
using System;

public class ShooterGameTutorial {
	public static string TUT_KEY = "SHOOT_TUT";
	private GameObject tutEnemy = null;                // tutorial enemies
	private int currentStep = 0;

	// in each case we are going to listen to events that tell us to move along
	public void ProcessStep(int step) {
		switch(step) {
			case 0:		// Prompt user to hold device with both hands
				ShooterGameManager.Instance.OnTutorialTap += MoveAlong;
				ShooterGameManager.Instance.tutUIAnimator.Play("ShooterTutorialUIHoldDevice");
				ShooterGameManager.Instance.tutUITextLocalize.key = "SHOOTER_TUT_DEVICE";
				ShooterGameManager.Instance.tutUITextLocalize.Localize();
                break;
			case 1:     //prompt user to move around, the user simply needs to tap the screen
				ShooterGameManager.Instance.OnTutorialTap -= MoveAlong;
				PlayerShooterController.Instance.OnTutorialMove += MoveAlong;
				ShooterGameManager.Instance.tutUIAnimator.Play("ShooterTutorialUIMoveClick");
				ShooterGameManager.Instance.tutUITextLocalize.key = "SHOOTER_TUT_MOVE";
				ShooterGameManager.Instance.tutUITextLocalize.Localize();
				break;
			case 2:     //prompt user to shoot
				PlayerShooterController.Instance.OnTutorialMove -= MoveAlong;
				ShooterGameManager.Instance.OnTutorialTap += MoveAlong;
				ShooterGameManager.Instance.tutUIAnimator.Play("ShooterTutorialUIShootClick");
				ShooterGameManager.Instance.tutUITextLocalize.key = "SHOOTER_TUT_SHOOT";
				ShooterGameManager.Instance.tutUITextLocalize.Localize();
				break;
			case 3:
				ShooterGameManager.Instance.OnTutorialTap -= MoveAlong;
				ShooterGameManager.Instance.OnTutorialStepDone += MoveAlong;
				ShooterGameManager.Instance.tutUIAnimator.Play("ShooterTutorialUINone");
				GameObjectUtils.AddChild(GameObject.Find("MidPoint"), LoadTutorialEnemyRef());
				break;
			case 4:
				GameObjectUtils.AddChild(GameObject.Find("Upper"), LoadTutorialEnemyRef());
				break;
			case 5:
				GameObjectUtils.AddChild(GameObject.Find("Lower"), LoadTutorialEnemyRef());
				break;
			// the user must defeat the first wave which is simply a wave of 5 basic enemies
			case 6:
				ShooterGameManager.Instance.OnTutorialStepDone -= MoveAlong;
				ShooterGameEnemyController.Instance.OnTutorialStepDone += MoveAlong;
				ShooterGameEnemyController.Instance.BuildEnemyList();
				break;
			//the user must click the inhaler button to end the tutorial the scene transition should pause after the sun is off screen
			case 7:
				ShooterGameManager.Instance.ShooterTutInhalerStep = true;
                ShooterGameEnemyController.Instance.OnTutorialStepDone -= MoveAlong;
				ShooterInhalerManager.Instance.proceed += MoveAlong;
				break;
			// the user must defeat the first wave which is simply a wave of 5 basic enemies
			case 8:
				ShooterGameManager.Instance.ShooterTutInhalerStep = false;
				ShooterGameManager.Instance.OnTutorialStepDone -= MoveAlong;
				ShooterGameEnemyController.Instance.OnTutorialStepDone += MoveAlong;
				ShooterGameEnemyController.Instance.BuildEnemyList();
				break;
			//the user must click the inhaler button to end the tutorial the scene transition should pause after the sun is off screen
			case 9:
				ShooterGameManager.Instance.ShooterTutInhalerStep = true;
				ShooterGameEnemyController.Instance.OnTutorialStepDone -= MoveAlong;
				ShooterInhalerManager.Instance.proceed += MoveAlong;
				break;
			case 10:
				ShooterGameManager.Instance.ShooterTutInhalerStep = false;
				ShooterGameManager.Instance.inTutorial = false;
				ShooterGameManager.Instance.NewGame();
				break;
		}
	}

	private GameObject LoadTutorialEnemyRef() {
		if(tutEnemy == null) {
			tutEnemy = (GameObject)Resources.Load("ShooterTutEnemy");
		}
		return tutEnemy;
	}

	private void MoveAlong(object sender, EventArgs args) {
		Debug.Log("MOVING ALONG");
		if(currentStep < 10) {
			currentStep++;
			ProcessStep(currentStep);
		}
	}
}
