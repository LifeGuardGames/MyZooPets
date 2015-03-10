using UnityEngine;
using System;
using System.Collections;

public class ShooterGameManager : MinigameManager<ShooterGameManager>{

	// the number of times the user has missed the optimal use window
	//public int NumMissed=0;
	public EventHandler<EventArgs> proceed;
	public EventHandler<EventArgs> QuitEvent;
	public Camera nguiCamera;
	public float shootTime;
	private float startTime;
	public int waveNum = 0;
	public bool inTutorial;
	private int missed; 

	void Awake(){
		quitGameScene = SceneUtils.BEDROOM;
	}

	public override void QuitGame(){
		if(QuitEvent != null)
			QuitEvent(this, EventArgs.Empty);
		ShooterUIManager.Instance.Quit();
		ShooterSpawnManager.Instance.Quit();
		base.QuitGame();
	}

	protected override void _Start(){
	}

	protected override void _OnDestroy(){
		Application.targetFrameRate = 30;
	}

	protected override string GetMinigameKey(){
		return "ShooterGame";
	}

	protected override void _NewGame(){

		if(IsTutorialOverride() && IsTutorialOn()|| 
		   !DataManager.Instance.GameData.Tutorial.IsTutorialFinished(ShooterGameTutorial.TUT_KEY)){
			if(inTutorial){
				ShooterUIManager.Instance.Reset();
				PlayerShooterController.Instance.Reset();
				StartTutorial();
			}
		}
		else{
			PlayerShooterController.Instance.changeInHealth += HealthUpdate;
			waveNum = 0;
			missed = 0;
			ShooterSpawnManager.Instance.Reset();
			ShooterGameEnemyController.Instance.Reset();
			ShooterUIManager.Instance.Reset();
			PlayerShooterController.Instance.Reset();
			if(ShooterUIManager.Instance.fingerPos != null){
				Destroy(ShooterUIManager.Instance.fingerPos.gameObject);
			}
		}
	}
	protected override bool IsTutorialOn(){
		return Constants.GetConstant<bool>("IsShooterTutorialOn");
	}

	public void Reset(){
		waveNum = 0;
		missed = 0;
		ShooterGameEnemyController.Instance.Reset();
		ShooterUIManager.Instance.Reset();
		PlayerShooterController.Instance.Reset();
		if(ShooterUIManager.Instance.fingerPos != null){
			Destroy(ShooterUIManager.Instance.fingerPos.gameObject);
		}
	}

	public override int GetReward(MinigameRewardTypes eType){
		return GetStandardReward(eType);
	}

	protected override void _GameOver(){
		//BadgeLogic.Instance.CheckSeriesUnlockProgress(BadgeType.PatientNumber, numOfCorrectDiagnose, true);
	}

	public void ClickIt(TapGesture e){
		if(inTutorial){
			if(proceed != null)
				proceed(this, EventArgs.Empty);
		}
		if(startTime <= Time.time - shootTime){
			if(!IsTouchingNGUI(e.Position)){
	// this handles mouse look the actual overall picture is spread across 3 scripts this section deals with getting the input position
#if !UNITY_EDITOR
		
				Vector3 TouchPos = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 1);
				PlayerShooterController.Instance.shoot(TouchPos);
#endif
#if UNITY_EDITOR
				Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
				PlayerShooterController.Instance.Shoot(mousePos);
#endif
			}
			startTime = Time.time;
		}
	}

	public void AddScore(int amount){
		UpdateScore(amount);
	}

	public void ChangeWaves(){
		if(ShooterUIManager.Instance.fingerPos != null){
			Destroy(ShooterUIManager.Instance.fingerPos.gameObject);
		}
		ShooterInhalerManager.Instance.CanUseInhalerButton = true;
		if(ShooterInhalerManager.Instance.hit == false){
			missed++;
			if(missed >= 2){
				PlayerShooterController.Instance.RemoveHealth(-2);
			}
		}
		ShooterInhalerManager.Instance.hit = false;
		waveNum++;
		this.gameObject.GetComponent<ShooterGameEnemyController>().GenerateWave(waveNum);
	}

	// Update is called once per frame
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

	public void HealthUpdate(object sender, EventArgs args){
		UpdateLives(-1);
		UpdateLives(4);
		PlayerShooterController.Instance.changeInHealth -= HealthUpdate;	
	}
}