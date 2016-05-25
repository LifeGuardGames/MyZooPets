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
	public GameObject fireworkPrefab;
	public RectTransform comboTransform;
	public RectTransform counterTransform;

	private ParticleSystem partSystem;
	private ParticleSystem.Particle[] particles = new ParticleSystem.Particle[90];
	private Vector3 particleAim;
	private float particleRunTime;

	private int particleCount = 0;
	private int numOfCorrectDiagnose;
	private int combo = 0;
	private float timeToCombo = 2f;
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
		}
		#endif
		if (currentComboTime > 0) {
			currentComboTime -= Time.deltaTime;
		} else if (combo != 0) {
			ResetCombo();
		}
		if (particleCount != 0)
			MoveParticles();

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

		yield return new WaitForSeconds(.8f);
		finger.Shake(new Vector3(0, 20));
	
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

	//protected override bool IsTutorialOn(){
	//	return Constants.GetConstant<bool>("IsDoctorMatchTutorialOn");
	//}


	// When your timer runs out

	public void FinishClear() {
		clearing = false;
	}

	public void OnTimerBarEmpty() {
		//GameOver();
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
	// Input coming from button scripts
	public void OnZoneClicked(DoctorMatchButtonTypes buttonType) { //TODO: Combo has been removed
		if (tutorial) { //Tutorial uses seperate logic
			HandleTutorial(buttonType);
		} else {
			HandleNormal(buttonType);
		}
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
		if (!lifeBarController.IsEmpty())
			assemblyLineController.ShiftAndAddNewItem();
		else if (!assemblyLineController.lineComplete) {
			assemblyLineController.MoveUpLine();
		} else {
			GameOver();
		}
	}

	private void HandleTutorial(DoctorMatchButtonTypes buttonType) { //TODO: Refactor this and HandleNormal into one clean method
		AssemblyLineItem poppedItem = assemblyLineController.PeekFirstItem();
		finger.StopShake(GetButtonTransform(tutorialZone).position);
		if (poppedItem.ItemType == buttonType) {
			ComboBonus();
			StartCoroutine(SpawnFirework());
			UpdateCombo(1);
			poppedItem.Activate();
			assemblyLineController.PopFirstItem();
			assemblyLineController.MoveUpLine();
			UpdateScore(1);
			AudioManager.Instance.PlayClip("clinicCorrect");
			if (assemblyLineController.lineComplete) {
				doctorMatchTutorial.Advance();

			}
		} else {
			ResetCombo();
			AudioManager.Instance.PlayClip("minigameError");
			cameraShake.Play();
			finger.Shake(new Vector3(0, 20));
		}
		SpawnFloatyText(poppedItem.ItemType == buttonType, GetButtonTransform((int)buttonType - 1));

	}

	private void SpawnFloatyText(bool correct, Transform buttonTransform) {
		float comboMod = Mathf.Clamp(combo, 0, 10);
		string wordText;
		string comboText;
		Color color;
		int size = 35 + (int)comboMod * 2;//= Random.Range(75, 95);
		int index = Random.Range(0, 9);
		if (correct) { //TODO: Color differently based on combo
			wordText = Localization.Localize("DOCTOR_RIGHT_" + index);
			comboText = "x" + combo;
			color = new Color(0, 1 - (10 - comboMod) / 30, 0);//new Color(Random.Range(.0f, .4f), Random.Range(.7f, 1f), Random.Range(.0f, .4f));
		} else {
			comboText = "X";
			wordText = Localization.Localize("DOCTOR_WRONG_" + index);
			color = new Color(Random.Range(.7f, 1f), Random.Range(.0f, .2f), Random.Range(.0f, .2f));
		}

		UGUIFloaty wordFloaty = Instantiate(floatyPrefab).GetComponent<UGUIFloaty>();
		Vector3 wordOffset = new Vector3(Random.Range(-20, 20), 50);
		Vector3 spawnPos = buttonTransform.position + new Vector3(0, 25);//buttonObject.transform.position-(Vector3)buttonObject.GetComponent<RectTransform>().rect.center;
		wordFloaty.transform.SetParent(buttonTransform, true);
		wordFloaty.transform.localScale = new Vector3(1, 1, 1);
		wordFloaty.StartFloaty(spawnPos, text: wordText, textSize: size, riseTime: .6f, toMove: wordOffset, color: color);

		float comboBonusScalar = 1; //Applied to the size during these special ones
		if (GetComboLevel() == 2) { //Big combo bonus
			comboBonusScalar = 2f;
		} else if (GetComboLevel() == 1) { //Small combo bonus
			comboBonusScalar = 1.5f;
		}

		Vector3 comboOffset = new Vector3(Random.Range(-20, 20), 30);
		UGUIFloaty comboFloaty = Instantiate(floatyPrefab).GetComponent<UGUIFloaty>();
		comboFloaty.transform.SetParent(buttonTransform, true);
		comboFloaty.transform.localScale = new Vector3(1, 1, 1);
		comboFloaty.StartFloaty(spawnPos, text: comboText, textSize: (int)(size / 1.2 * comboBonusScalar), riseTime: .6f, toMove: comboOffset, color: color);

	}

	private IEnumerator SpawnFirework() { 
		float comboMod = Mathf.Clamp(combo, 0, 10);
		GameObject firework = Instantiate(fireworkPrefab);
		firework.transform.position = assemblyLineController.StartPosition.position;
		ParticleSystem pSystem = firework.GetComponent<ParticleSystem>();
		//pSystem.startLifetime=10;
		float comboBonusScalar = 1; //Applied to the size during these special ones
		if (GetComboLevel() == 2) { //Big combo bonus
			pSystem.startColor = Color.blue;
			comboBonusScalar = 2f;
			particleAim = counterTransform.position;
		} else if (GetComboLevel() == 1) { //Small combo bonus
			pSystem.startColor = Color.green;
			comboBonusScalar = 1.5f;
			particleAim = comboTransform.position;
		}
		ParticleSystem.LimitVelocityOverLifetimeModule emissionModule = pSystem.limitVelocityOverLifetime; //HACK: Currently, you cannot modify particle system module curves directly, so we save it here and modify it later
		AnimationCurve ourCurve = new AnimationCurve();
		for (float i = 0; i <= 1; i += .1f) {
			ourCurve.AddKey(i, 250 * Mathf.Pow(i - 1, 4)); //Kind of like quadratic but it gets roughly flat after .5
		}
		emissionModule.limit = new ParticleSystem.MinMaxCurve((1 + comboMod / 20) * comboBonusScalar, ourCurve);

		pSystem.startSize *= (1 + comboMod / 10) * comboBonusScalar;

		//We need to wait a frame so that the burst at 0 seconds can happen and GetParticles is populated
		yield return new WaitForEndOfFrame();
		if (comboBonusScalar != 1) { //Activated
			int tempCount = pSystem.GetParticles(particles);
			for (int i = 0; i < tempCount; i++) { //These particles need to live longer so that we can bring them out
				particles [i].startLifetime = 1f; //Must be larger than .5f, but actual value does not matter
				particles [i].lifetime = 1f;
			}
			pSystem.SetParticles(particles, tempCount);
			partSystem = pSystem;
			yield return new WaitForSeconds(.5f);
			AnimationCurve newCurve = new AnimationCurve();
			newCurve.AddKey(0, 0);
			newCurve.AddKey(1, 0);
			particleRunTime = 0;
			emissionModule.limit = new ParticleSystem.MinMaxCurve(0, newCurve); //1 second has passed, freeze our particles
			particleCount = tempCount;
			pSystem.GetParticles(particles);

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
		if (GetComboLevel() == 2) { //Big combo bonus
			lifeBarController.PlusBar(1.5f);
		} else if (GetComboLevel() == 1) { //Small combo bonus
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

	private void CharacterScoredRight() {
		
		UpdateScore(2);
		ComboBonus();
		// Play appropriate sound
		Hashtable hashOverride = new Hashtable();
		hashOverride ["Pitch"] = Mathf.Clamp(.9f + ((float)combo / 30), 0, 1.25f); //Goes from .9 to 1.25 by increments of .0333 and then caps
		AudioManager.Instance.PlayClip("clinicCorrect", option: hashOverride);
		StartCoroutine(SpawnFirework());
		UpdateCombo(1);
	}

	private void CharacterScoredWrong() {
		ResetCombo();
		UpdateScore(-1);
		// Play an incorrect sound
		AudioManager.Instance.PlayClip("minigameError");
		cameraShake.Play();
	}



	private void MoveParticles() { 
		for (int i = 0; i < particleCount; i++) {
			particles [i].position = Vector3.Lerp(particles [i].position, particleAim, particleRunTime);	
		}
		partSystem.SetParticles(particles, particleCount);
		particleRunTime += Time.deltaTime;
		if (particleRunTime > .5) {
			particleCount = 0;
			Destroy(partSystem.gameObject);
		}
	}
}
