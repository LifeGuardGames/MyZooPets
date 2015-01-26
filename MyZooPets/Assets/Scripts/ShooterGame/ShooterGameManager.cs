using UnityEngine;
using System;
using System.Collections;

public class ShooterGameManager : MinigameManager<ShooterGameManager>{

	// the number of times the user has missed the optimal use window
	//public int NumMissed=0;
	public EventHandler<EventArgs> Proceed;
	public Camera nguiCamera;
	public float ShootTime;
	private float StartTime;
	public int WaveNum = 0;
	public bool InTutorial= false;

	void Awake(){
		quitGameScene = SceneUtils.BEDROOM;
	}

	protected override void _Start(){
	}

	protected override void _OnDestroy(){
		Application.targetFrameRate = 30;
	}

	protected override string GetMinigameKey(){
		return "ShooterGame";
	}

	protected override bool IsTutorialOn(){
		return false;	//TODO Change
	}
	
	protected override bool HasCutscene(){
		return false;	//TODO Change
	}

	protected override void _NewGame(){

		if(IsTutorialOverride())
		{
			InTutorial = true;
			StartTutorial();
		}

		else{
			PlayerShooterController.Instance.changeInHealth+=healthUpdate;
			WaveNum = 0;
			ShooterSpawnManager.Instance.reset();
			ShooterGameEnemyController.Instance.reset();
			ShooterUIManager.Instance.reset();
			PlayerShooterController.Instance.reset();
		}
	}

	public void reset(){
		WaveNum = 0;
		ShooterGameEnemyController.Instance.reset();
		ShooterUIManager.Instance.reset();
		PlayerShooterController.Instance.reset();
	}

	public override int GetReward(MinigameRewardTypes eType){
		return GetStandardReward(eType);
	}

	protected override void _GameOver(){
		//BadgeLogic.Instance.CheckSeriesUnlockProgress(BadgeType.PatientNumber, numOfCorrectDiagnose, true);
	}

	void OnTap(TapGesture e){
		if(InTutorial){
			if(Proceed != null)
				Proceed(this, EventArgs.Empty);
		}
		if(StartTime <= Time.time-ShootTime){
		if(!IsTouchingNGUI(e.Position)){
		
#if !UNITY_EDITOR
		
			Vector3 TouchPos = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 1);
			PlayerShooterController.Instance.shoot(TouchPos);
#endif
#if UNITY_EDITOR
		Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
			PlayerShooterController.Instance.shoot(mousePos);
#endif
			}
			StartTime = Time.time;
		}
	}

	public void AddScore(int amount)
	{
		UpdateScore(amount);
	}

	public void ChangeWaves(){
		ShooterInhalerManager.Instance.CanUseInhalerButton = true;
			WaveNum++;
		this.gameObject.GetComponent<ShooterGameEnemyController>().GenerateWave(WaveNum);
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
	public Coroutine sync(){
		return StartCoroutine(PauseRoutine()); 
	}
	public IEnumerator PauseRoutine(){
		while (ShooterGameManager.Instance.GetGameState() == MinigameStates.Paused){
			yield return new WaitForFixedUpdate();
		}
		yield return new WaitForEndOfFrame();
	}
	private void StartTutorial(){
		SetTutorial(new ShooterGameTutorial());
	}
	public void healthUpdate(object sender, EventArgs args){
		UpdateLives(-1);
		UpdateLives(4);
		PlayerShooterController.Instance.changeInHealth-=healthUpdate;	
	}
}