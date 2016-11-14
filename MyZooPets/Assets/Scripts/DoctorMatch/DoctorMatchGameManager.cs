using UnityEngine;
using System.Collections;

public class DoctorMatchGameManager : NewMinigameManager<DoctorMatchGameManager> {
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
	public GameObject redBar;
	public GameObject toClearText;
	public Canvas mainCanvas;

	private int numOfCorrectDiagnose;
	public int NumOfCorrectDiagnose {
		get { return numOfCorrectDiagnose; }
	}

	private DoctorMatchTutorial doctorMatchTutorial;
	private bool isTutorial = false;
	private int tutorialZone = 0;
	private FingerController finger = null;
	private IEnumerator multipleFinger = null;
	private float lastPress;
	private float timeToShake = 2f;
	private int bonusStack = 0;
	//If they do nothing for 5 seconds. Shake the finger

	void Update() {
		if(IsPaused) {
			return;
		}
#if UNITY_EDITOR
		if(Input.GetKeyDown(KeyCode.LeftArrow)) {
			OnZoneClicked(DoctorMatchButtonTypes.Green);
		}
		else if(Input.GetKeyDown(KeyCode.UpArrow)) {
			OnZoneClicked(DoctorMatchButtonTypes.Yellow);
		}
		else if(Input.GetKeyDown(KeyCode.RightArrow)) {
			OnZoneClicked(DoctorMatchButtonTypes.Red);
		}
		else if(Input.GetKeyDown(KeyCode.W)) {
			lifeBarController.KillBar();
		}
#endif
		if(isTutorial && finger != null && zoneGreen.button.interactable) {
			lastPress += Time.deltaTime;
			if(lastPress > timeToShake) {
				StartCoroutine(finger.Shake(new Vector3(0, 10)));
				lastPress = 0;
			}
		}
	}

	void Awake() {
		minigameKey = "CLINIC";
		quitGameScene = SceneUtils.BEDROOM;
		rewardMoneyMultiplier = 0.7f;
		rewardShardMultiplier = 0.3f;
		rewardXPMultiplier = 0.1f;

		ResetScore();
	}

	protected override void _Start() {
		zoneGreen.ToggleButtonInteractable(false);
		zoneYellow.ToggleButtonInteractable(false);
		zoneRed.ToggleButtonInteractable(false);
	}

	protected override void _StartTutorial() {
		StartCoroutine(StartTutorialHelper());
	}

	public IEnumerator StartTutorialHelper() {
		ResetScore();
		isTutorial = true;
		yield return assemblyLineController.Initialize(true); //If a tutorial is called after a game is played, we need to wait a frame for objects to be cleared
		lifeBarController.ResetBar(); //Thus this needs to be a coroutine

		zoneGreen.ToggleButtonInteractable(true);
		zoneYellow.ToggleButtonInteractable(true);
		zoneRed.ToggleButtonInteractable(true);

		doctorMatchTutorial = new DoctorMatchTutorial();
		tutorial = doctorMatchTutorial;
	}

	protected override void _NewGame() {	//Reset everything then start again
		ResetScore();

		if(finger) {						//Called if we complete the tutorial or restart early
			BarFinger();
		}

		isTutorial = false;
		StartCoroutine(assemblyLineController.Initialize(false));
		lifeBarController.ResetBar();
		lifeBarController.StartDraining();
		lifeBarController.UpdateCount(-1);
		zoneGreen.ToggleButtonInteractable(true);
		zoneYellow.ToggleButtonInteractable(true);
		zoneRed.ToggleButtonInteractable(true);
		bonusStack = 0;
		redBar.SetActive(true);
		toClearText.SetActive(false);

		isContinueAllowed = IsContinueCheckDefaultTrue();
	}

	protected override void _PauseGame() {
		lifeBarController.StopDraining();
	}

	protected override void _ResumeGame() {
		if(!isTutorial) {
			lifeBarController.StartDraining();
		}
	}

	protected override void _ContinueGame() {
		lifeBarController.PlusBar(33);
		assemblyLineController.PopulateQueue(compare: true);
		lifeBarController.UpdateCount(-1);
		redBar.SetActive(true);
		toClearText.SetActive(false);
	}

	protected override void _GameOver() {
		lifeBarController.StopDraining();
		zoneGreen.ToggleButtonInteractable(false);
		zoneYellow.ToggleButtonInteractable(false);
		zoneRed.ToggleButtonInteractable(false);
	}

	// Award the actual xp and money, called when tween is complete (Mission, Stats, Crystal, Badge, Analytics, Leaderboard)
	protected override void _GameOverReward() {
		WellapadMissionController.Instance.TaskCompleted("ScoreClinic", Score); 

		StatsManager.Instance.ChangeStats(
			xpDelta: rewardXPAux,
			xpPos: GenericMinigameUI.Instance.GetXPPanelPosition(),
			coinsDelta: rewardMoneyAux,
			coinsPos: GenericMinigameUI.Instance.GetCoinPanelPosition(),
			animDelay: 0.5f);

		FireCrystalManager.Instance.RewardShards(rewardShardAux);
		BadgeManager.Instance.CheckSeriesUnlockProgress(BadgeType.DoctorMatch, NumOfCorrectDiagnose, true);
		Analytics.Instance.DoctorMatchGameData(DataManager.Instance.GameData.HighScore.MinigameHighScore[minigameKey], comboController.Combo);
#if UNITY_IOS
		LeaderBoardManager.Instance.EnterScore((long)Score, "DoctorLeaderBoard");
#endif
	}

	protected override void _QuitGame() {
	}

	public void OnTimerBarEmpty() {
		int toClear = assemblyLineController.Count + bonusStack;
		lifeBarController.UpdateCount(toClear);
		redBar.SetActive(false);
		toClearText.SetActive(true);
	}

	public void SpawnFinger(int zone) {
		lastPress = 0;
		tutorialZone = zone;
		if(finger == null) {
			finger = GameObjectUtils.AddChildGUI(mainCanvas.gameObject, pointerPrefab).GetComponent<FingerController>();
			finger.transform.SetSiblingIndex(2);
        }
		else {
			LeanTween.cancel(finger.gameObject);
		}
		if(tutorialZone == 3) {
			finger.transform.position = GetCorrectTransform().position;//GetButtonTransform((int)assemblyLineController.PeekFirstItem().ItemType).transform.position;
		}
		else {
			finger.transform.position = GetButtonTransform(tutorialZone).position;
		}
		multipleFinger = finger.RepeatShake(3 - tutorialZone, new Vector3(0, 20));
		StartCoroutine(multipleFinger);
	}

	public void BarFinger() {
		finger.transform.rotation = Quaternion.Euler(0, 180, 50);
		finger.transform.position = lifeBarController.transform.position + new Vector3(120, -30);
		multipleFinger = finger.RepeatShake(2, new Vector3(20, 0), true);
		StartCoroutine(multipleFinger);
	}

	public override void UpdateScore(int deltaScore) {
		base.UpdateScore(deltaScore);
		comboController.UpdateScore(score);
	}

	public void TempLockZones() {
		zoneGreen.TempLock(.15f);
		zoneYellow.TempLock(.15f);
		zoneRed.TempLock(.15f);
	}

	public void DisableZones() {
		zoneGreen.button.interactable = false;
		zoneRed.button.interactable = false;
		zoneYellow.button.interactable = false;
		comboController.StopCounting();
	}

	public void EnableZones() {
		comboController.StartCounting();
		zoneGreen.button.interactable = true;
		zoneRed.button.interactable = true;
		zoneYellow.button.interactable = true;
	}

	// Input coming from button scripts
	public void OnZoneClicked(DoctorMatchButtonTypes buttonType) {
		AssemblyLineItem peakedItem = assemblyLineController.PeekFirstItem();
		bool correct = peakedItem.ItemType == buttonType;
		float comboMod = comboController.ComboMod;
		if(correct) { //The same feedback is provided whether or not the game is in tutorial
			StartCoroutine(particleController.SpawnFirework(comboMod, assemblyLineController.StartPosition.position));
			numOfCorrectDiagnose++;
			UpdateScore(2);
			ComboBonus();
			PlaySoundCorrect();
			comboController.IncrementCombo();
		}
		else {
			comboController.ResetCombo();
			UpdateScore(-1);
			BloodPanelManager.Instance.PlayBlood();
			AudioManager.Instance.PlayClip("minigameError");
		}

		if(isTutorial) { //But line movement and populating is different in each mode
			HandleTutorial(peakedItem, correct);
		}
		else {
			HandleNormal(peakedItem);
		}
	}

	private void HandleNormal(AssemblyLineItem poppedItem) {
		assemblyLineController.PopFirstItem();
		poppedItem.Activate();
		int toClear = assemblyLineController.Count + bonusStack;
		lifeBarController.UpdateCount(toClear);
		if(!lifeBarController.IsEmpty || bonusStack > 0) {
			assemblyLineController.ShiftAndAddNewItem();
			if(lifeBarController.IsEmpty && bonusStack > 0) {
				bonusStack--;
			}
		}
		else if(!assemblyLineController.LineComplete) {
			assemblyLineController.MoveUpLine();
		}
		else {
			GameOver();
		}
	}

	private void HandleTutorial(AssemblyLineItem poppedItem, bool correct) { //TODO: Refactor this and HandleNormal into one clean method
		lastPress = 0;
		if(multipleFinger != null) {
			StopCoroutine(multipleFinger);
			multipleFinger = null;
		}
		if(tutorialZone == 3) {
			finger.StopShake(GetCorrectTransform().position);
		}
		else {
			finger.StopShake(GetButtonTransform(tutorialZone).position);
		}
		if(correct) {
			assemblyLineController.PopFirstItem();
			poppedItem.Activate();
			assemblyLineController.MoveUpLine(false);
			if(assemblyLineController.LineComplete) {
				doctorMatchTutorial.Advance();
			}
			else if(tutorialZone == 3) {
				finger.transform.position = GetCorrectTransform().position;//GetButtonTransform((int)assemblyLineController.PeekFirstItem().ItemType).transform.position;
			}
		}
		else {
			StartCoroutine(finger.Shake(new Vector3(0, 10), .25f));
		}
	}

	private Transform GetButtonTransform(int buttonType) {
		switch(buttonType) {
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

	private Transform GetCorrectTransform() {
		AssemblyLineItem item = assemblyLineController.PeekFirstItem();
		return GetButtonTransform((int)item.ItemType - 1);
	}

	private void ComboBonus() {
		if(comboController.ComboLevel == 2 && !isTutorial) { //Big combo bonus (does not apply during tutorial)
			if(assemblyLineController.Count != 1) {
				bonusStack += 3;
			}
			else {
				assemblyLineController.PopulateQueue(compare: true, count: 3, indexOffset: 1);
			}
		}
		else if(comboController.ComboLevel == 1) { //Small combo bonus
			UpdateScore(comboController.Combo);
		}
	}

	private void ResetScore() {

		score = 0;
		comboController.ResetCombo();
		comboController.UpdateScore(0);
	}

	private void PlaySoundCorrect() {
		Hashtable hashOverride = new Hashtable();
		hashOverride["Pitch"] = .9f + (comboController.ComboMod) / 30f;
		AudioManager.Instance.PlayClip("clinicCorrect", option: hashOverride);
	}
}
