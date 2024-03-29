using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NinjaGameManager : NewMinigameManager<NinjaGameManager> {
	public GestureTrail trail;              // the gesture trail that follows the user's finger around
	public float timeBetweenSpawnGroups;    // time between spawn groups
	public bool bonusRound = false;         // triggers Bonus round
	public int bonusRoundEnemies;
	public int bonusRoundCounter;           // tracks number of boss round
	public int chain = 0;                   // number of enemies killed with out hitting a bomb 
	public BonusVisualController bonusVisualController;     // Controller that controls the bonus UI
	public Text scoreText;

	private bool spawning = true;           // stops the spawning to prevent the play from being horribly murdered mid bonus round
	private float comboTime = 0;            // aux time counter
	public float comboMaxTime = 0.25f;      // max time between cuts for a combo to increase
	private float timeCount = 0;            // used to count time between groups and between entries within a group
	private Vector2 trailDeltaMove;
	//private Vector3 lastPos = Vector3.zero; // the last position of the user's trail - comboing
	private List<NinjaDataEntry> currentTriggerEntries;     // current list of entries to spawn triggers from
	private FingerGestures.SwipeDirection lastDirection;    // record the last drag direction
	public bool isBouncyTime = false;
	public bool isTutorialRunning = false;
	private bool isPlaying = false;
	public bool isGameOver = true;
	public GameObject spawnParent;
	public MiniGameModes mode = MiniGameModes.None;
	public NinjaUIManager uiManager;

	//time attack
	public float time = 60;

	private int lifeCount;
	public int LifeCount {
		set { lifeCount = value; }
		get { return lifeCount; }
	}

	private int combo = 0;                  // the current combo level of the player
	public int Combo {
		get { return combo; }
		set { combo = value; }
	}

	private int bestCombo = 0;              // player's best combo in one run
	public int BestCombo {
		get { return bestCombo; }
		set { bestCombo = value; }
	}

	void Awake() {
		Application.targetFrameRate = 60;
		minigameKey = "NINJA";
		quitGameScene = SceneUtils.BEDROOM;
		//mode = MiniGameModes.Time;
	if(DataManager.Instance.GameData.MinGames.minGame == minigameKey) {
		mode = DataManager.Instance.GameData.MinGames.mode;
      }
		if(mode == MiniGameModes.Time) {
			uiManager.ShowTimer();
		}
		if(mode == MiniGameModes.None) {
			rewardMoneyMultiplier = 0.22f;
			rewardShardMultiplier = 0.25f;
			rewardXPMultiplier = 0.2f;
		}
		else if (mode == MiniGameModes.Speed) {
			rewardMoneyMultiplier = 0.22f * 2;
			rewardShardMultiplier = 0.25f * 2;
			rewardXPMultiplier = 0.2f * 2;
			Time.timeScale = 2.0f;
		}
		else {
			rewardMoneyMultiplier = 0.22f * 2;
			rewardShardMultiplier = 0.25f * 2;
			rewardXPMultiplier = 0.2f * 2;
		}
	}

	protected override void _Start() {

	}

	public Vector2 GetTrailDeltaMove() {
		return trailDeltaMove;
	}

	public void IncreaseChain() {
		chain++;
		if(chain % 25 == 0) {
			bonusRound = true;
			StartBonusVisuals();
		}
	}

	public void ResetChain() {
		chain = 0;
	}

	public void IncreaseCombo(int num) {
		if(!isTutorialRunning) {
			// increase the combo
			combo += num;

			// by default, increasing the combo resets the countdown before the combo expires
			comboTime = comboMaxTime;
		}
	}

	void OnDrag(DragGesture gesture) {
		if(!isPlaying) {
			return;
		}

		// update the ninja gesture cut trail
		UpdateTrail(gesture);

		// Figure out the direction of the trail
		Vector2 dir = gesture.TotalMove.normalized; // you could also use Gesture.DeltaMove to get the movement change since last frame
		FingerGestures.SwipeDirection swipeDir = FingerGestures.GetSwipeDirection(dir);

		if(swipeDir != lastDirection &&
			swipeDir != FingerGestures.SwipeDirection.UpperLeftDiagonal &&
			swipeDir != FingerGestures.SwipeDirection.UpperRightDiagonal &&
			swipeDir != FingerGestures.SwipeDirection.LowerLeftDiagonal &&
			swipeDir != FingerGestures.SwipeDirection.LowerRightDiagonal) {
			AudioManager.Instance.PlayClip("ninjaWhoosh", variations: 3);
		}
		lastDirection = swipeDir;

		switch(gesture.Phase) {
			case ContinuousGesturePhase.Updated:
				GameObject go = gesture.Selection;
				if(go) {
					//Debug.Log("Touching " + go.name);
					NinjaTrigger trigger = go.GetComponent<NinjaTrigger>();

					// if the trigger is null, check the parent...a little hacky, but sue me!
					if(trigger == null) {
						trigger = go.transform.parent.gameObject.GetComponent<NinjaTrigger>();
					}
					if(trigger) {
						trigger.OnCut(gesture.Position);
					}
				}
				break;
		}
	}

	protected override void _StartTutorial() {
		isTutorialRunning = true;
		isPlaying = true;
		isGameOver = false;
		new NinjaTutorial();
	}

	protected override void _NewGame() {
		NinjaTrigger[] toBeCleared = GameObject.FindObjectsOfType<NinjaTrigger>();
		foreach(NinjaTrigger trig in toBeCleared) {
			Destroy(trig.gameObject);
		}
		// reset variables
		score = 0;
		scoreText.text = Score.ToString();
		if(mode != MiniGameModes.Life) {
			lifeCount = 3;
		}
		else {
			lifeCount = 1;
		}
		comboTime = 0;
		combo = 0;
		bestCombo = 0;
		timeCount = 0;
		if(mode == MiniGameModes.Time) {
			time = 60f;
		}
		ResetChain();
		currentTriggerEntries = null;

		// Reset all states
		bonusVisualController.StopBonusVisuals();
		bonusRound = false;
		spawning = true;
		isPlaying = true;
		isGameOver = false;
		if(mode != MiniGameModes.Speed) {
			Time.timeScale = 1.0f;
		}
		else {
			Time.timeScale = 1.5f;
		}
		uiManager.NewGameUI();

		isContinueAllowed = IsContinueCheckDefaultTrue();
	}

	protected override void _QuitGame() {
		Application.targetFrameRate = 30;
	}

	protected override void _GameOver() {
		isGameOver = true;
		Time.timeScale = 1.0f;
		// Reset all states - Remove any visuals for combos
		bonusVisualController.StopBonusVisuals();
		bonusRound = false;
		spawning = false;

	}

	// Award the actual xp and money, called when tween is complete (Mission, Stats, Crystal, Badge, Analytics, Leaderboard)
	protected override void _GameOverReward() {
		WellapadMissionController.Instance.TaskCompleted("ScoreNinja", score);
		WellapadMissionController.Instance.TaskCompleted("ComboNinja", bestCombo);

		StatsManager.Instance.ChangeStats(
			xpDelta: rewardXPAux,
			xpPos: GenericMinigameUI.Instance.GetXPPanelPosition(),
			coinsDelta: rewardMoneyAux,
			coinsPos: GenericMinigameUI.Instance.GetCoinPanelPosition(),
			animDelay: 0.5f);

		FireCrystalManager.Instance.RewardShards(rewardShardAux);

		BadgeManager.Instance.CheckSeriesUnlockProgress(BadgeType.Ninja, Score, true);

		Analytics.Instance.NinjaGameData(DataManager.Instance.GameData.HighScore.MinigameHighScore[MinigameKey], bonusRoundCounter);
		
		#if UNITY_IOS
		LeaderBoardManager.Instance.EnterScore((long)Score, "NinjaLeaderBoard");
		#endif
	}

	protected override void _PauseGame() {
		if(!isGameOver) {
			Time.timeScale = 0.0f;
		}
	}

	protected override void _ResumeGame() {
		Time.timeScale = 1.0f;
	}

	protected override void _ContinueGame() {
		lifeCount = 1;
		bonusRound = false;
		spawning = true;
		isPlaying = true;
		isGameOver = false;
		uiManager.ContinueGameUI();
    }

	void Update() {
		if(isTutorialRunning || isGameOver) {
			return;
		}

		if(!isGameOver && mode == MiniGameModes.Time) {
			time -= Time.deltaTime;
			uiManager.UpdateTimer(time);
			if (time < 0) {
				GameOver();
			}
		}

		float deltaTime = Time.deltaTime;

		// update the player's combo
		UpdateComboTimer(deltaTime);

		// if there is a current group of spawn entries in process...
		if(currentTriggerEntries != null && currentTriggerEntries.Count > 0) {
			// count up
			timeCount += deltaTime;

			// if our time has surpassed the next entry's time, do it up and remove that entry
			NinjaDataEntry entry = currentTriggerEntries[0];
			float timeEntry = entry.GetTime();
			if(timeCount >= timeEntry) {
				SpawnGroup(entry);
				currentTriggerEntries.RemoveAt(0);
			}

			// if the list of current entries is empty...null the list and reset our count so we can count down again
			if(currentTriggerEntries.Count == 0) {
				currentTriggerEntries = null;
				timeCount = timeBetweenSpawnGroups;
			}
		}
		else if(timeCount <= 0) {
			// otherwise, there is no current group and it is time to start one, 
			// so figure out which one to begin
			NinjaScoring scoreKey;
			NinjaData data = null;
			if(spawning) {
				scoreKey = GetScoringKey();
				data = NinjaDataLoader.GetGroupToSpawn(NinjaModes.Classic, scoreKey);

				// cache the list -- ALMOST FOOLED ME....use new to copy the list
				currentTriggerEntries = new List<NinjaDataEntry>(data.GetEntries());
			}
		}
		else {
			timeCount -= deltaTime; // otherwise, there is no group and we still need to countdown before spawning the next group
		}
	}

	public GameObject SpawnTriggersTutorial(int num) {
		GameObject triggerPrefab = (GameObject)Resources.Load("NinjaTrigger" + num.ToString());
		Vector3 triggerLocation;
		switch(num) {
			case 1:
				triggerLocation = new Vector3(-4.5f, -2.7f, 0);
				break;
			case 2:
				triggerLocation = new Vector3(-3.3f, -1.4f, 0);
				break;
			case 3:
				triggerLocation = new Vector3(-1.8f, 0.25f, 0);
				break;
			case 4:
				triggerLocation = new Vector3(0.05f, 1.3f, 0);
				break;
			case 5:
				triggerLocation = new Vector3(2f, 2f, 0);
				break;
			case 6:
				triggerLocation = new Vector3(4f, 2.4f, 0);
				break;
			default:
				triggerLocation = new Vector3(0, 2.8f, 0);
				break;
		}

		//instantiate trigger
		GameObject triggerObject = GameObjectUtils.AddChild(spawnParent, triggerPrefab);
		triggerObject.transform.position = triggerLocation;
		triggerObject.transform.localEulerAngles = triggerPrefab.transform.localEulerAngles;

		Rigidbody triggerObjectRigidbody = triggerObject.GetComponent<Rigidbody>();
		triggerObjectRigidbody.useGravity = false;
		triggerObjectRigidbody.constraints = RigidbodyConstraints.None;
		triggerObjectRigidbody.constraints = RigidbodyConstraints.FreezePositionX |
			RigidbodyConstraints.FreezePositionY |
			RigidbodyConstraints.FreezeRotationX |
			RigidbodyConstraints.FreezeRotationY;

		return triggerObject;
	}

	/// <summary>
	/// Updates the trail.
	/// </summary>
	/// <param name="gesture">Gesture.</param>
	private void UpdateTrail(DragGesture gesture) {
		ContinuousGesturePhase ePhase = gesture.Phase;

		// the screen position the user's finger is at currently
		Vector2 vPos = gesture.Position;

		// based on phase of the gesture, call certain functions
		switch(ePhase) {
			case ContinuousGesturePhase.Started:
				trail.DragStarted(vPos);
				break;
			case ContinuousGesturePhase.Updated:
				trail.DragUpdated(vPos);
				break;
			case ContinuousGesturePhase.Ended:
				trail.DragEnded();
				break;
		}

		// save the last position for use with displaying combo
		//lastPos = vPos;
		trailDeltaMove = gesture.DeltaMove;
	}

	/// <summary>
	/// Spawns the group.
	/// </summary>
	/// <param name="entry">Entry.</param>
	private void SpawnGroup(NinjaDataEntry entry) {
		// create the proper list of objects to spawn
		int numOfTriggers = entry.GetTriggers();
		int numOfBombs = entry.GetBombs();
		int numOfPowUps = entry.GetPowUp();
		List<string> listObjects = new List<string>();

		for(int i = 0; i < numOfTriggers; ++i) {
			// NOTE: if want to add variation over time, use GetRandomTrigger(n to choose from)
			string randomTrigger =
				DataLoaderNinjaTriggersAndBombs.GetRandomTrigger(DataLoaderNinjaTriggersAndBombs.numTriggers);
			listObjects.Add(randomTrigger);
		}
		for(int i = 0; i < numOfBombs; ++i) {
			// NOTE: if want to add variation over time, use GetRandomBomb(n to choose from)
			string randomBomb =
				DataLoaderNinjaTriggersAndBombs.GetRandomBomb(DataLoaderNinjaTriggersAndBombs.numBombs);
			listObjects.Add(randomBomb);
		}

		for(int i = 0; i < numOfPowUps; ++i) {
			// NOTE: if want to add variation over time, use GetRandomBomb(n to choose from)
			string randomPowUps =
				DataLoaderNinjaTriggersAndBombs.GetRandomPowUp(DataLoaderNinjaTriggersAndBombs.numPowUps);
			listObjects.Add(randomPowUps);
		}
		// shuffle the list so everything is nice and mixed up
		listObjects.Shuffle();

		// create the proper object based off the entry's pattern
		NinjaPatterns patternType = entry.GetPattern();
		switch(patternType) {
			case NinjaPatterns.Separate:
				new SpawnGroupSeparate(listObjects);
				break;
			case NinjaPatterns.Clustered:
				new SpawnGroupCluster(listObjects);
				break;
			case NinjaPatterns.Meet:
				new SpawnGroupMeet(listObjects);
				break;
			case NinjaPatterns.Cross:
				new SpawnGroupCross(listObjects);
				break;
			case NinjaPatterns.Split:
				new SpawnGroupSplit(listObjects);
				break;
			case NinjaPatterns.Swarms:
				// Random is ambiguous need to specify unity engine
				int rand = UnityEngine.Random.Range(2, 5);
				StartCoroutine(WaitASec(rand, listObjects));
				break;
			default:
				Debug.LogError("Unhandled group type: " + patternType);
				break;
		}
	}

	/// <summary>
	/// Returns the current scoring key, based on the player's current score.  This function is a little hacky.
	/// </summary>
	/// <returns>The scoring key.</returns>
	private NinjaScoring GetScoringKey() {
		int nScore = Score;
		NinjaScoring eScore;

		if(bonusRound == true) {
			eScore = NinjaScoring.Bonus;
		}
		else if(nScore == 0)
			eScore = NinjaScoring.Start_1;
		else if(nScore > 0 && nScore < 3)
			eScore = NinjaScoring.Start_2;
		else if(nScore == 3)
			eScore = NinjaScoring.Start_3;
		else if(nScore > 3 && nScore < 25)
			eScore = NinjaScoring.Med;
		else
			eScore = NinjaScoring.High;

		return eScore;
	}

	private void UpdateComboTimer(float deltaTime) {
		// if the player doesn't have a combo going, don't bother
		if(combo <= 0) {
			return;
		}

		// update the time
		comboTime -= deltaTime;

		// if the time has expired, end the current combo
		if(comboTime <= 0) {
			OnComboEnd();
		}
	}

	public void _UpdateScore(int score) {
		UpdateScore(score);
		scoreText.text = Score.ToString();
	}

	private void OnComboEnd() {
		// give the player an additional point for each level of their combo
		if(combo > 2) {
			_UpdateScore(combo);
			uiManager.SpawnComboFloaty(Vector3.zero, combo);
		}

		// if the current combo was better than their best, update it
		if(combo > bestCombo) {
			bestCombo = combo;
		}

		// reset the combo down to 0
		combo = 0;
		if(Score > 50) {
			Time.timeScale = 1.25f;
		}
		else if(Score > 150) {
			Time.timeScale = 1.5f;
		}
		else if(Score > 250) {
			Time.timeScale = 2.0f;
		}
	}

	IEnumerator WaitASec(int _rand, List<string> listObjects) {
		for(int i = 0; i <= _rand; i++) {
			yield return new WaitForSeconds(0.1f);
			new SpawnGroupSwarms(listObjects);
		}
	}

	public void CheckEndBonus() {
		if(bonusRoundEnemies <= 0) {
			bonusVisualController.StopBonusVisuals();
			bonusRound = false;
			spawning = true;
		}
	}

	public void StartBonusVisuals() {
		AudioManager.Instance.PlayClip("ninjaBonus");
		bonusVisualController.PlayBonusVisuals();
	}

	public void BeginBoucePowerUp() {
		isBouncyTime = true;
		StartCoroutine("EndBounceTime");
	}

	IEnumerator EndBounceTime() {
		yield return new WaitForSeconds(10.0f);
		isBouncyTime = false;
	}

	public void UpdateLife(int deltaLife) {
		lifeCount += deltaLife;
		uiManager.OnLivesChanged(deltaLife);
	}
}
