using UnityEngine;
using UnityEngine.UI;
using System;

public class ShooterGameManager : NewMinigameManager<ShooterGameManager> {
	public EventHandler<EventArgs> OnTutorialStepDone;
	public EventHandler<EventArgs> OnTutorialTap;
	public float shootTime;
	private float startTime;
	public int waveNum = 0;
	public bool inTutorial;
	public int powerUpScore;
	public int highestCombo = 0;
	public GameObject BouncyWalls;
	public GameObject tutFinger;
	public bool isGameOver = false;
	public Text scoreText;
	public delegate void OnRestart();
	public static event OnRestart onRestart;
	public GameObject minipetPowerUp;

	private ShooterUIManager shooterUI;

	void Awake() {
		minigameKey = "SHOOTER";
		quitGameScene = SceneUtils.BEDROOM;
		rewardXPMultiplier = 0.1f;
		rewardShardMultiplier = 12;
		rewardMoneyMultiplier = 8;
	}

	protected override void _Start() {
		shooterUI = ShooterUIManager.Instance;
	}

	protected override void _StartTutorial() {
		shooterUI.Reset();
		isPaused = false;
		PlayerShooterController.Instance.Reset();
		ShooterGameTutorial tut = new ShooterGameTutorial();
		tut.ProcessStep(0);
	}

	protected override void _NewGame() {
		if(onRestart != null) {
			onRestart();
		}
		minipetPowerUp.SetActive(false);
		StopAllCoroutines();
		isGameOver = false;
		inTutorial = false;
		waveNum = 0;
		score = 0;
		scoreText.text = "0";
		ShooterSpawnManager.Instance.Reset();
		ShooterGameEnemyController.Instance.Reset();
		ShooterInhalerManager.Instance.Reset();
		shooterUI.Reset();
		PlayerShooterController.Instance.Reset();
		RemoveInhalerFingerTutorial();
	}

	public void MoveTut() {
		if(inTutorial) {
			if(OnTutorialStepDone != null) {
				OnTutorialStepDone(this, EventArgs.Empty);
			}
		}
	}

	public void TriggerGameover() {
		GameOver();
	}

	protected override void _ContinueGame() {
		isGameOver = false;
		PlayerShooterController.Instance.playerHealth = 5;
		PlayerShooterController.Instance.ChangeFire();
		ShooterGameEnemyController.Instance.BuildEnemyList();
	}

	protected override void _GameOver() {
		isGameOver = true;
		Analytics.Instance.ShooterGameData(DataManager.Instance.GameData.HighScore.MinigameHighScore[minigameKey], ShooterInhalerManager.Instance.missed / (waveNum + 1), ShooterGameEnemyController.Instance.currentWave.Wave, highestCombo);
		WellapadMissionController.Instance.TaskCompleted("SurvivalShooter", waveNum);

#if UNITY_IOS
		LeaderBoardManager.Instance.EnterScore((long)GetScore(), "ShooterLeaderBoard");
#endif
	}

	public void OnTapped(TapGesture e) {
		if(inTutorial) {
			if(OnTutorialTap != null) {
				OnTutorialTap(this, EventArgs.Empty);
			}
		}
		if(startTime <= Time.time - shootTime) {
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
			if(Camera.main.ScreenToWorldPoint(mousePos).x <= PlayerShooterController.Instance.gameObject.transform.position.x + 1.0f) {
				PlayerShooterController.Instance.Move(mousePos);
				Debug.Log("Move");
			}
			else {
				Debug.Log("shoot");
				PlayerShooterController.Instance.Shoot(mousePos);
				startTime = Time.time;
			}
#endif
		}
	}

	public void AddScore(int amount) {
		UpdateScore(amount);
		scoreText.text = score.ToString();
		powerUpScore += amount;
		if(powerUpScore > (75 + 25 * (waveNum / 5))) {
			powerUpScore = 0;
			ShooterSpawnManager.Instance.SpawnPowerUp();
		}
	}

	public void StartTimeTransition() {
		shooterUI.StartTimeTransition();
	}

	public void BeginNewWave() {
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
		gameObject.GetComponent<ShooterGameEnemyController>().GenerateWave(waveNum);
	}

	// Soft remove finger if it exists
	public void RemoveInhalerFingerTutorial() {
		if(shooterUI.fingerPos != null) {
			Destroy(shooterUI.fingerPos.gameObject);
		}
	}

	protected override void _PauseGame() {
		// IsPaused tracked by parent
	}

	protected override void _ResumeGame() {
		// IsPaused tracked by parent
	}

	// Award the actual xp and money, called when tween is complete (Mission, Stats, Crystal, Badge, Analytics, Leaderboard)
	protected override void _GameOverReward() {
		
	}

	protected override void _QuitGame() {
		LoadLevelManager.Instance.StartLoadTransition(quitGameScene);
	}
}
