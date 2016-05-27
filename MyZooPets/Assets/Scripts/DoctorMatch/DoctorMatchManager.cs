using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class DoctorMatchManager : NewMinigameManager<DoctorMatchManager> {
	public enum DoctorMatchButtonTypes {
		None,
		Green,
		Yellow,
		Red
	}

	public AssemblyLineController assemblyLineController;
	public DoctorMatchLifeBarController lifeBarController;
	public ParticleFXController particleController;
	public ComboController comboController;
	public Animation cameraShake;
	public DoctorMatchZone zoneGreen;
	public DoctorMatchZone zoneYellow;
	public DoctorMatchZone zoneRed;
	public GameObject pointerPrefab;
	public GameObject floatyPrefab;

	private int numOfCorrectDiagnose;
	private int combo = 0;
	//Maximum amount of time for a correct diagnosis
	private float timeToCombo = 2f;
	private float currentComboTime = 0;
	private bool paused = true;

	private DoctorMatchTutorial doctorMatchTutorial;
	private bool tutorial = false;
	private int tutorialZone = 0;
	private FingerController finger = null;

	public bool Paused {
		get {
			return paused;
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
		}
		#endif
		if (currentComboTime > 0) {
			currentComboTime -= Time.deltaTime;
			if (currentComboTime <= timeToCombo / 2) {
				comboController.TimeLowColor(currentComboTime);
			}
		} else if (combo != 0) {
			ResetCombo();
		}
	}

	void Awake() {
		// Parent settings
		minigameKey = "DOCTOR";
		quitGameScene = SceneUtils.BEDROOM;
		comboController.Setup();
		ResetScore();
	}

	protected override void _Start() {
		zoneGreen.ToggleButtonInteractable(false);
		zoneYellow.ToggleButtonInteractable(false);
		zoneRed.ToggleButtonInteractable(false);
	}


	protected override void _NewGame() { //Reset everything then start again
		ResetScore();

		if (finger) { //Called if we complete the tutorial or leave restart early
			BarFinger();
		}

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

	public IEnumerator StartTutorial() {
		ResetScore();
		tutorial = true;
		yield return assemblyLineController.Initialize(true); //If a tutorial is called after a game is played, we need to wait a frame for objects to be cleared
		lifeBarController.ResetBar(); //Thus this needs to be a coroutine

		zoneGreen.ToggleButtonInteractable(true);
		zoneYellow.ToggleButtonInteractable(true);
		zoneRed.ToggleButtonInteractable(true);

		doctorMatchTutorial = new DoctorMatchTutorial();

		yield return new WaitForSeconds(.8f);
		finger.Shake(new Vector3(0, 20));

	}

	public void OnTimerBarEmpty() {
		lifeBarController.UpdateCount(assemblyLineController.Count);
	}

	public void SpawnFinger(int zone) {
		tutorialZone = zone;
		if (finger == null) {
			finger = Instantiate(pointerPrefab).GetComponent<FingerController>();
		}
		finger.transform.position = GetButtonTransform(tutorialZone).position;
		finger.Shake(new Vector3(0, 20));

	}

	public void BarFinger() {
		finger.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 50));
		finger.transform.position = lifeBarController.transform.position + new Vector3(120, -30);
		finger.Shake(new Vector3(20, 0), true);
	}

	public void UpdateCombo(int deltaCombo) {
		combo += deltaCombo;
		comboController.UpdateCombo(combo);
		currentComboTime = timeToCombo;
	}

	public override void UpdateScore(int deltaScore) {
		base.UpdateScore(deltaScore);
		comboController.UpdateScore(score);
	}

	public int GetComboLevel() {
		if ((combo + 1) % 10 == 0 && combo != 0) { //Big combo bonus
			return 2;
		} else if ((combo + 1) % 5 == 0 && combo != 0) { //Small combo bonus
			return 1;
		} else {
			return 0;
		}

	}

	// Input coming from button scripts
	public void OnZoneClicked(DoctorMatchButtonTypes buttonType) { //TODO: Combo has been removed
		AssemblyLineItem poppedItem = assemblyLineController.PopFirstItem();
		bool correct = poppedItem.ItemType == buttonType;
		Transform buttonTransform = GetButtonTransform((int)buttonType - 1);
		float comboMod = Mathf.Clamp(combo, 0, 10);
		if (correct) {
			StartCoroutine(particleController.SpawnFirework(comboMod, assemblyLineController.StartPosition.position));
			numOfCorrectDiagnose++;
			UpdateScore(2);
			ComboBonus();
			PlaySoundCorrect();
			UpdateCombo(1);
		} else {
			ResetCombo();
			UpdateScore(-1);
			AudioManager.Instance.PlayClip("minigameError");
			cameraShake.Play();
		}
		particleController.SpawnFloatyText(comboMod, correct, buttonTransform);

		if (tutorial) {
			HandleTutorial(poppedItem, correct);
		} else {
			HandleNormal(poppedItem);
		}
	}

	private void HandleNormal(AssemblyLineItem poppedItem) {
		poppedItem.Activate();
		if (!lifeBarController.IsEmpty()) {
			assemblyLineController.ShiftAndAddNewItem();
		} else if (!assemblyLineController.LineComplete) {
			assemblyLineController.MoveUpLine();
			assemblyLineController.VisibleCount();
			lifeBarController.UpdateCount(assemblyLineController.Count);
		} else {
			GameOver();
		}
	}

	private void HandleTutorial(AssemblyLineItem poppedItem, bool correct) { //TODO: Refactor this and HandleNormal into one clean method
		finger.StopShake(GetButtonTransform(tutorialZone).position);
		if (correct) {
			poppedItem.Activate();
			assemblyLineController.PopFirstItem();
			assemblyLineController.MoveUpLine();
			if (assemblyLineController.LineComplete) {
				doctorMatchTutorial.Advance();

			}
		} else {
			finger.Shake(new Vector3(0, 20));
		}
	}

	private Transform GetButtonTransform(int buttonType) {
		switch (buttonType) {
			case 0:
				return zoneGreen.transform;
			case 1:
				return zoneYellow.transform;
			case 2:
				return zoneRed.transform;
			default:
				Debug.LogWarning("Invalid Zone");
				return null;
		}
	}

	private void ComboBonus() {
		/*lifeBarController.PlusBar(assemblyLineController.ClearTime);
		StartCoroutine(assemblyLineController.ClearLine());
		combo = 0;
		clearing = true;*/
		if (comboController.ComboLevel == 2) { //Big combo bonus
			lifeBarController.PlusBar(1.5f);
		} else if (comboController.ComboLevel == 1) { //Small combo bonus
			UpdateScore(combo);
		}
	}

	private void ResetScore() {
		rewardXPMultiplier = 1f;
		rewardMoneyMultiplier = 1f;
		rewardShardMultiplier = 1f;
		score = 0;
		ResetCombo();
		comboController.UpdateScore(score);
	}

	private void ResetCombo() {
		combo = 0;
		currentComboTime = 0;
		comboController.UpdateCombo(combo);
	}

	private void PlaySoundCorrect() {
		Hashtable hashOverride = new Hashtable();
		hashOverride ["Pitch"] = Mathf.Clamp(.9f + ((float)combo / 30), 0, 1.25f); //Goes from .9 to 1.25 by increments of .0333 and then caps
		AudioManager.Instance.PlayClip("clinicCorrect", option: hashOverride);
	}
}
