using System;
using UnityEngine;
using System.Collections;

public class DoctorMatchManager : NewMinigameManager<DoctorMatchManager> {
	public enum DoctorMatchButtonTypes {
		None,
		Green,
		Yellow,
		Red
	}

	public AssemblyLineController assemblyLineController;
	public DoctorMatchLifeBarController lifeBarController;
	public Animation cameraShake;
	public DoctorMatchZone zoneGreen;
	public DoctorMatchZone zoneYellow;
	public DoctorMatchZone zoneRed;
	public GameObject pointerPrefab;

	private int numOfCorrectDiagnose;
	private int combo = 0;
	private float timeToCombo = 3f;
	//Time between each correct diagnoses for it to be consistent
	private float currentComboTime = 0;
	private bool paused = true;
	private bool clearing = false;

	private DoctorMatchTutorial doctorMatchTutorial;
	private bool tutorial = false;
	private int tutorialZone = 0;
	private FingerController finger = null;

	public bool Paused {
		get {
			return paused || clearing;
		}
	}

	public int NumOfCorrectDiagnose {
		get{ return numOfCorrectDiagnose; }
	}

	void Update() {
		if (paused)
			return;
		#if UNITY_EDITOR

		if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			OnZoneClicked(DoctorMatchButtonTypes.Green);
		} else if (Input.GetKeyDown(KeyCode.UpArrow)) {
			OnZoneClicked(DoctorMatchButtonTypes.Yellow);
		} else if (Input.GetKeyDown(KeyCode.RightArrow)) {
			OnZoneClicked(DoctorMatchButtonTypes.Red);
		} else if (Input.GetKeyDown(KeyCode.Q)) {
			ComboBonus();
		}
		#endif
		if (currentComboTime > 0) {
			currentComboTime -= Time.deltaTime;
		} else if (combo != 0) {
			combo = 0;
		}
	}

	void Awake() {
		// Parent settings
		minigameKey = "DOCTOR";
		quitGameScene = SceneUtils.BEDROOM;
		ResetScore();
	}

	protected override void _Start() {
		zoneGreen.ToggleButtonInteractable(false);
		zoneYellow.ToggleButtonInteractable(false);
		zoneRed.ToggleButtonInteractable(false);
	}

	public IEnumerator StartTutorial() {
		ResetScore();
		tutorial = true;
		yield return assemblyLineController.Initialize(true); //If a tutorial is called after a game is played, we need to wait a frame for objects to be cleared
		lifeBarController.ResetBar(); //Thus this needs to be a coroutine

		zoneGreen.ToggleButtonInteractable(true);
		zoneYellow.ToggleButtonInteractable(true);
		zoneRed.ToggleButtonInteractable(true);

		doctorMatchTutorial = new DoctorMatchTutorial();

	}

	protected override void _NewGame() { //Reset everything then start again
		ResetScore();

		paused = false;
		tutorial = false;
		StartCoroutine(assemblyLineController.Initialize(false));
		lifeBarController.ResetBar();
		lifeBarController.StartDraining();

		zoneGreen.ToggleButtonInteractable(true);
		zoneYellow.ToggleButtonInteractable(true);
		zoneRed.ToggleButtonInteractable(true);

	}

	protected override void _PauseGame(bool isShow) {
		paused = !isShow;
		if (isShow && !tutorial) {
			lifeBarController.StartDraining();
		} else {
			lifeBarController.StopDraining();
		}
		// not implemented yet!
	}

	protected override void _GameOver() {
		lifeBarController.StopDraining();
		zoneGreen.ToggleButtonInteractable(false);
		zoneYellow.ToggleButtonInteractable(false);
		zoneRed.ToggleButtonInteractable(false);

		/*
		Analytics.Instance.DoctorHighScore(DataManager.Instance.GameData.HighScore.MinigameHighScore[GetMinigameKey()]);
		Analytics.Instance.DoctorTimesPlayedTick();
		#if UNITY_IOS
		LeaderBoardManager.Instance.EnterScore((long)GetScore(), "DoctorLeaderBoard");
		#endif
		*/

	}

	// award the actual xp and money, called when tween is complete
	protected override void _GameOverReward() {
		StatsManager.Instance.ChangeStats(
			xpDelta: rewardXPAux,
			xpPos: GenericMinigameUI.Instance.GetXPPanelPosition(),
			coinsDelta: rewardMoneyAux,
			coinsPos: GenericMinigameUI.Instance.GetCoinPanelPosition(),
			animDelay: 0.5f);
		FireCrystalManager.Instance.RewardShards(rewardShardAux);
		BadgeLogic.Instance.CheckSeriesUnlockProgress(BadgeType.DoctorMatch, NumOfCorrectDiagnose, true);
	}

	protected override void _QuitGame() {
	}

	//protected override bool IsTutorialOn(){
	//	return Constants.GetConstant<bool>("IsDoctorMatchTutorialOn");
	//}


	// When your timer runs out

	public void FinishClear() {
		clearing = false;
	}

	public void OnTimerBarEmpty() {
		GameOver();
	}

	public void SpawnFinger(int zone) {
		tutorialZone = zone;
		if (finger == null) {
			Debug.Log("INSTANT");
			finger = Instantiate(pointerPrefab).GetComponent<FingerController>();
		}
		finger.transform.position = GetTutorialPosition();
	}

	public void BarFinger() {
		finger.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 50));
		finger.transform.position = lifeBarController.transform.position + new Vector3(120, -30);
		finger.Shake(new Vector3(20, 0), true);
	}
	// Input coming from button scripts
	public void OnZoneClicked(DoctorMatchButtonTypes buttonType) {
		if (tutorial) { //Tutorial uses seperate logic
			HandleTutorial(buttonType);
		} else if (combo == 1000000) { //Out of commission for now
			ComboBonus();
			Debug.Log("COMBO");
		} else {
			HandleNormal(buttonType);
			Debug.Log("Normal");
		}
	}
	private void HandleNormal(DoctorMatchButtonTypes buttonType) {
		AssemblyLineItem poppedItem = assemblyLineController.PopFirstItem();
		if (poppedItem.ItemType == buttonType) {
			CharacterScoredRight();
		} else {
			CharacterScoredWrong();
		}


		// Have item do what it does when activated
		poppedItem.Activate();
		assemblyLineController.ShiftAndAddNewItem();
	}
	private void HandleTutorial(DoctorMatchButtonTypes buttonType) {
		AssemblyLineItem poppedItem = assemblyLineController.PeekFirstItem();
		finger.StopShake(GetTutorialPosition());
		if (poppedItem.ItemType == buttonType) {
			poppedItem.Activate();
			assemblyLineController.PopFirstItem();
			assemblyLineController.MoveUpLine();
			AudioManager.Instance.PlayClip("clinicCorrect");
			if (assemblyLineController.lineComplete) {
				doctorMatchTutorial.Advance();

			}
		} else {
			AudioManager.Instance.PlayClip("minigameError");
			cameraShake.Play();
			finger.Shake(new Vector3(0, 20));
		}
	}

	private Vector3 GetTutorialPosition() {
		switch (tutorialZone) {
			case 0:
				return zoneGreen.transform.position;
			case 1:
				return zoneYellow.transform.position;
			case 2:
				return zoneRed.transform.position;
			default:
				Debug.LogWarning("Invalid Zone");
				return Vector3.zero;
		}
	}

	private void ComboBonus() {
		lifeBarController.PlusBar(assemblyLineController.ClearTime);
		StartCoroutine(assemblyLineController.ClearLine());
		combo = 0;
		clearing = true;
	}

	private void ResetScore() {
		combo = 0;
		currentComboTime = 0;

		rewardXPMultiplier = 1f;
		rewardMoneyMultiplier = 1f;
		rewardShardMultiplier = 1f;
		score = 0;
	}
	void OnGUI() {
		GUI.Box( new Rect(0, 0, 50, 50), score.ToString());
	}
	private void CharacterScoredRight() {
		//if(IsTutorialRunning()){
			
		//}
		//else{
		combo++;
		currentComboTime = timeToCombo;
		//lifeBarController.PlusBar();
		UpdateScore(2);
		//}

		// Play appropriate sound
		AudioManager.Instance.PlayClip("clinicCorrect");
	}

	private void CharacterScoredWrong() {
		//lifeBarController.HurtBar();
		combo = 0;
		currentComboTime = 0;
		UpdateScore(-1);
		// Play an incorrect sound
		AudioManager.Instance.PlayClip("minigameError");
		cameraShake.Play();
	}

}
