using UnityEngine;
using System;
using System.Collections;

public class ShooterGameManager : MinigameManager<ShooterGameManager>{
	public EventHandler<EventArgs> OnTutorialStepDone;
	public EventHandler<EventArgs> OnTutorialTap;
	public Camera nguiCamera;
	public float shootTime;
	private float startTime;
	public int waveNum = 0;
	public bool inTutorial;
	public int powerUpScore;
	public int highestCombo = 0;
	public GameObject BouncyWalls;
	public GameObject tutFinger;

	private ShooterUIManager shooterUI;

	void Awake(){
		quitGameScene = SceneUtils.BEDROOM;
	}

	public override string GetMinigameKey(){
		return "Shooter";
	}

	protected override bool IsTutorialOn(){
		return Constants.GetConstant<bool>("IsShooterTutorialOn");
	}
	
	public override int GetReward(MinigameRewardTypes eType){
		return GetStandardReward(eType);
	}

	protected override void _Start(){
		shooterUI = ui as ShooterUIManager;
	}

	protected override void _OnDestroy(){
		Application.targetFrameRate = 30;
	}

	protected override void _NewGame(){
		if(IsTutorialOverride() && IsTutorialOn()|| 
		   !DataManager.Instance.GameData.Tutorial.IsTutorialFinished(ShooterGameTutorial.TUT_KEY)){
			if(inTutorial){
				shooterUI.Reset();
				PlayerShooterController.Instance.Reset();
				StartTutorial();
			}
		}
		else{
			inTutorial = false;
			waveNum = 0;
			ShooterSpawnManager.Instance.Reset();
			ShooterGameEnemyController.Instance.Reset();
			shooterUI.Reset();
			PlayerShooterController.Instance.Reset();
			RemoveInhalerFingerTutorial();
		}
	}

	public void MoveTut(){
		if(inTutorial){
			if(OnTutorialStepDone != null){
				OnTutorialStepDone(this, EventArgs.Empty);
			}
		}
	}

	public void TriggerGameover(){
		GameOver();
	}

	protected override void _GameOver(){
		Analytics.Instance.ShooterGameData(DataManager.Instance.GameData.HighScore.MinigameHighScore[GetMinigameKey()], ShooterInhalerManager.Instance.missed / waveNum + 1, ShooterGameEnemyController.Instance.currentWave.Wave, highestCombo);
		WellapadMissionController.Instance.TaskCompleted("SurvivalShooter", waveNum);
		

		#if UNITY_IOS
		LeaderBoardManager.Instance.EnterScore((long)GetScore(), "ShooterLeaderBoard");
		#endif
	}

	public void OnTapped(TapGesture e){
		if(inTutorial){
			if(OnTutorialTap != null){
				OnTutorialTap(this, EventArgs.Empty);
			}
		}
		if(startTime <= Time.time - shootTime){
			if(!IsTouchingNGUI(e.Position)){
				// this handles mouse look the actual overall picture is
				// spread across 3 scripts this section deals with getting the input position
				#if !UNITY_EDITOR
				Vector3 touchPos = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 1);
				if(Camera.main.ScreenToWorldPoint(touchPos).x <= PlayerShooterController.Instance.gameObject.transform.position.x){
					PlayerShooterController.Instance.Move(touchPos);
				}
				else{
					PlayerShooterController.Instance.Shoot(touchPos);
					startTime = Time.time;
				}
				#endif
					
				#if UNITY_EDITOR
				Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
				if(Camera.main.ScreenToWorldPoint(mousePos).x <= PlayerShooterController.Instance.gameObject.transform.position.x +1.0f){
					PlayerShooterController.Instance.Move(mousePos);
				}
				else{
					PlayerShooterController.Instance.Shoot(mousePos);
					startTime = Time.time;
				}
				#endif
			}
		}
	}

	public void AddScore(int amount){
		UpdateScore(amount);
		powerUpScore += amount;
		if(powerUpScore > (75 + 25 * (waveNum %10))){
			powerUpScore = 0;
			ShooterSpawnManager.Instance.SpawnPowerUp();
		}
	}

	public void StartTimeTransition(){
		shooterUI.StartTimeTransition();
	}

	public void BeginNewWave(){
		RemoveInhalerFingerTutorial();
		ShooterInhalerManager.Instance.CanUseInhalerButton = true;
		/*if(ShooterInhalerManager.Instance.hit == false){
			missed++;
			if(missed >= 2){
				PlayerShooterController.Instance.ChangeHealth(-2);
			}
		}*/
		ShooterInhalerManager.Instance.hit = false;
		waveNum++;
		this.gameObject.GetComponent<ShooterGameEnemyController>().GenerateWave(waveNum);
	}

	// Soft remove finger if it exists
	public void RemoveInhalerFingerTutorial(){
		if(shooterUI.fingerPos != null){
			Destroy(shooterUI.fingerPos.gameObject);
		}
	}

	protected override void _Update(){
	}

	//True: if finger touches NGUI 
	/// <summary>
	/// Determines whether if the touch is touching NGUI element
	/// </summary>
	/// <returns><c>true</c> if this instance is touching NGUI; otherwise, <c>false</c>.</returns>
	/// <param name="screenPos">Screen position.</param>
	private bool IsTouchingNGUI(Vector2 screenPos){
		Ray ray = nguiCamera.ScreenPointToRay(screenPos);
		RaycastHit hit;
		int layerMask = 1 << 10; 
		bool isOnNGUILayer = false;
		
		// Raycast
		if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)){
			isOnNGUILayer = true;
		}
		return isOnNGUILayer;
	}

	public Coroutine Sync(){
		return StartCoroutine(PauseRoutine()); 
	}

	public IEnumerator PauseRoutine(){
		while(ShooterGameManager.Instance.GetGameState() == MinigameStates.Paused){
			yield return new WaitForFixedUpdate();
		}
		yield return new WaitForEndOfFrame();
	}

	private void StartTutorial(){
		SetTutorial(new ShooterGameTutorial());
	}
}
