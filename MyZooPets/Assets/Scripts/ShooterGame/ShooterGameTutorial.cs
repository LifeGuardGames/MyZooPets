using UnityEngine;
using System;

public class ShooterGameTutorial {
	public static string TUT_KEY = "SHOOT_TUT";
	private GameObject shootUITut;               // Gameobject that positions the tutorial boards	
	private GameObject tutorialInhalerUse;      // tutorial message board		
	private GameObject tutEnemy = null;                // tutorial enemies
	private GameObject fingerUI;
	private int currentStep = 0;

	// in each case we are going to listen to events that tell us to move along
	public void ProcessStep(int step) {
		Debug.Log("Step");
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
				UnityEngine.Object.Destroy(shootUITut);
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
				ShooterGameEnemyController.Instance.OnTutorialStepDone -= MoveAlong;
				ShooterInhalerManager.Instance.proceed += MoveAlong;
				GameObject useInhalerTut = (GameObject)Resources.Load("ShooterInhalerTutorial");
				tutorialInhalerUse = GameObjectUtils.AddChildGUI(GameObject.Find("Canvas"), useInhalerTut);
				GameObject tutorialFingerRef = (GameObject)Resources.Load("ShooterPressTut");
				fingerUI = GameObjectUtils.AddChildGUI(GameObject.Find("Canvas"), tutorialFingerRef);
				RectTransform rect = fingerUI.GetComponent<RectTransform>();
				rect.anchorMax = new Vector2(1f, 0);
				rect.anchorMin = new Vector2(1f, 0);
				rect.anchoredPosition = new Vector2(-100f, 100f);
				break;
			// the user must defeat the first wave which is simply a wave of 5 basic enemies
			case 8:
				UnityEngine.Object.Destroy(fingerUI);
				UnityEngine.Object.Destroy(tutorialInhalerUse);
				ShooterGameManager.Instance.OnTutorialStepDone -= MoveAlong;
				ShooterGameEnemyController.Instance.OnTutorialStepDone += MoveAlong;
				UnityEngine.Object.Destroy(shootUITut);
				ShooterGameEnemyController.Instance.BuildEnemyList();
				break;
			//the user must click the inhaler button to end the tutorial the scene transition should pause after the sun is off screen
			case 9:
				ShooterGameEnemyController.Instance.OnTutorialStepDone -= MoveAlong;
				ShooterInhalerManager.Instance.proceed += MoveAlong;
				useInhalerTut = (GameObject)Resources.Load("ShooterInhalerTutorial");
				tutorialInhalerUse = GameObjectUtils.AddChildGUI(GameObject.Find("Canvas"), useInhalerTut);
				GameObject tutorialFingerRef2 = (GameObject)Resources.Load("ShooterPressTut");
				fingerUI = GameObjectUtils.AddChildGUI(GameObject.Find("Canvas"), tutorialFingerRef2);
				RectTransform rect2 = fingerUI.GetComponent<RectTransform>();
				rect2.anchorMax = new Vector2(1f, 0);
				rect2.anchorMin = new Vector2(1f, 0);
				rect2.anchoredPosition = new Vector2(-100f, 100f);
				break;
			case 10:
				UnityEngine.Object.Destroy(tutorialInhalerUse);
				UnityEngine.Object.Destroy(fingerUI);
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
		if(currentStep < 10) {
			currentStep++;
			ProcessStep(currentStep);
		}
	}
}
