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
	public Animation cameraShake;
	public DoctorMatchZone zoneGreen;
	public DoctorMatchZone zoneYellow;
	public DoctorMatchZone zoneRed;
	public GameObject pointerPrefab;
	public ComboController comboController;
	public GameObject floatyPrefab;

	private int numOfCorrectDiagnose;
	private int combo = 0;
	private float timeToCombo = 3f;
	//Time between each correct diagnoses for it to be consistent
	private float currentComboTime = 0;
	private bool paused = true;
	private bool clearing = false;
	private UGUIFloaty currentFloaty = null;

	private DoctorMatchTutorial doctorMatchTutorial;
	private bool tutorial = false;
	private int tutorialZone = 0;
	private FingerController finger = null;
	private bool parentObject = false;

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
			parentObject = !parentObject;//ComboBonus();
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
			finger = Instantiate(pointerPrefab).GetComponent<FingerController>();
		}
		finger.transform.position = GetButtonTransform(tutorialZone).position;
	}

	public void BarFinger() {
		finger.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 50));
		finger.transform.position = lifeBarController.transform.position + new Vector3(120, -30);
		finger.Shake(new Vector3(20, 0), true);
	}
	// Input coming from button scripts
	public void OnZoneClicked(DoctorMatchButtonTypes buttonType) { //TODO: Combo has been removed
		if (tutorial) { //Tutorial uses seperate logic
			HandleTutorial(buttonType);
		} else {
			HandleNormal(buttonType);
		}

		comboController.UpdateCombo(combo);
		comboController.UpdateScore(score);
	}

	private void HandleNormal(DoctorMatchButtonTypes buttonType) {
		AssemblyLineItem poppedItem = assemblyLineController.PopFirstItem();
		if (poppedItem.ItemType == buttonType) {
			CharacterScoredRight();
		} else {
			CharacterScoredWrong();
		}
		SpawnFloatyText(poppedItem.ItemType == buttonType, GetButtonTransform((int)buttonType - 1));
		// Have item do what it does when activated
		poppedItem.Activate();
		assemblyLineController.ShiftAndAddNewItem();
	}

	private void HandleTutorial(DoctorMatchButtonTypes buttonType) {
		AssemblyLineItem poppedItem = assemblyLineController.PeekFirstItem();
		finger.StopShake(GetButtonTransform(tutorialZone).position);
		if (poppedItem.ItemType == buttonType) {
			poppedItem.Activate();
			assemblyLineController.PopFirstItem();
			assemblyLineController.MoveUpLine();
			UpdateScore(1);
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

	private void SpawnFloatyText(bool correct, Transform buttonTransform) {
		if (currentFloaty) {
			LeanTween.cancel(currentFloaty.gameObject);
			Destroy(currentFloaty.gameObject);
		}
		string text;
		Color color;
		int size = Random.Range(40, 50);//= Random.Range(75, 95);
		int index;
		if (correct) {
			index = Random.Range(0, 9);
			text = Localization.Localize("DOCTOR_RIGHT_" + index);
			color = new Color(Random.Range(.0f, .4f), Random.Range(.7f, 1f), Random.Range(.0f, .4f));
		} else {
			index = Random.Range(0, 9);
			text = Localization.Localize("DOCTOR_WRONG_" + index);
			color = new Color(Random.Range(.7f, 1f), Random.Range(.0f, .2f), Random.Range(.0f, .2f));
		}
		UGUIFloaty floaty = Instantiate(floatyPrefab).GetComponent<UGUIFloaty>();
		Vector3 offset = new Vector3(Random.Range(-10, 10), 50);
		Vector3 spawnPos = buttonTransform.position + new Vector3(0, 25);//buttonObject.transform.position-(Vector3)buttonObject.GetComponent<RectTransform>().rect.center;
		floaty.transform.SetParent(buttonTransform, true);
		floaty.transform.localScale = new Vector3(1, 1, 1);
		floaty.StartFloaty(spawnPos, text: text, textSize: size, riseTime: .6f, toMove: offset, color: color);
		currentFloaty = floaty;

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

	private void CharacterScoredRight() {
		combo++;
		currentComboTime = timeToCombo;
		UpdateScore(2);

		// Play appropriate sound
		AudioManager.Instance.PlayClip("clinicCorrect");
	}

	private void CharacterScoredWrong() {
		combo = 0;
		currentComboTime = 0;
		UpdateScore(-1);
		// Play an incorrect sound
		AudioManager.Instance.PlayClip("minigameError");
		cameraShake.Play();
	}

}
