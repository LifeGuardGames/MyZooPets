using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class NinjaManager : MinigameManager<NinjaManager>{
	// combo related
	public float comboMaxTime;		// max time between cuts for a combo
	public GestureTrail trail; // the gesture trail that follows the user's finger around
	public float timeBetweenSpawnGroups;	// time between spawn groups
	public bool bonusRound = false;			// triggers Bonus round
	public int bonusRoundEnemies;
	public int bonusRoundCounter;			// tracks number of boss round
	public int chain = 0;			// number of enemies killed with out hitting a bomb 
	public BonusVisualController bonusVisualController;	// Controller that controls the bonus UI

	private bool spawning = true;			//stops the spawning to prevent the play from being horibly murdered mid bonus round
	private float comboTime = 0;	// time counter
	private int combo = 0;			// the current combo level of the player
	private int bestCombo = 0;		// player's best combo in one run
	private float timeCount = 0; // used to count time between groups and between entries within a group
	private Vector2 trailDeltaMove;
	private Vector3 lastPos = Vector3.zero; // the last position of the user's trail - comboing
//	private Vector3 lastTrailPosition = Vector3.zero;	// The position of trail one frame ago
//	private int lastTrailFrameCount
	private List<NinjaDataEntry> currentTriggerEntries; // current list of entries to spawn triggers from
	private FingerGestures.SwipeDirection lastDirection; // record the last drag direction

	void Awake(){
		Application.targetFrameRate = 60;
		quitGameScene = SceneUtils.BEDROOM;
	}

	protected override void _Start(){
	}
	
	protected override void _OnDestroy(){
		Application.targetFrameRate = 30;
	}

	public Vector2 GetTrailDeltaMove(){
		return trailDeltaMove;
	}
			
	public override int GetReward(MinigameRewardTypes eType){
		// for now, just use the standard way
		return GetStandardReward(eType);
	}

	public void IncreaseChain(){
		chain++;
		if(chain % 25 == 0){
			bonusRound = true;
			StartBonusVisuals();
		}
	}

	public void ResetChain(){
		chain = 0;
	}

	/// <summary>
	/// Gets the current combo level.
	/// </summary>
	/// <returns>The combo.</returns>
	public int GetCombo(){
		return combo;	
	}

	public int GetComboBest(){
		return bestCombo;	
	}
	
	/// <summary>
	/// Sets the combo.
	/// </summary>
	/// <param name="num">Number.</param>
	private void SetCombo(int num){
		combo = num;	
	}
	
	private void SetComboBest(int num){
		bestCombo = num;	
	}

	/// <summary>
	/// Gets the max combo time. The max time between successful cuts before the 
	/// player's combo expries
	/// </summary>
	/// <returns>The max combo time.</returns>
	private float GetMaxComboTime(){
		return comboMaxTime;
	}	

	/// <summary>
	/// Increases the combo.
	/// </summary>
	/// <param name="num">Number.</param>
	public void IncreaseCombo(int num){
		if(!IsTutorialRunning()){
			// increase the combo
			int nCombo = GetCombo();
			nCombo += num;
			SetCombo(nCombo);
			
			// by default, increasing the combo resets the countdown before the combo expires
			comboTime = GetMaxComboTime();
		}
	}

	void OnDrag(DragGesture gesture){
		// check is playing
		if(GetGameState() != MinigameStates.Playing){
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
			swipeDir != FingerGestures.SwipeDirection.LowerRightDiagonal){
			AudioManager.Instance.PlayClip("ninjaWhoosh", variations:3);
		}
		
		lastDirection = swipeDir;
		//--------------- end -----------------------
		
		switch(gesture.Phase){
		case ContinuousGesturePhase.Updated:

			GameObject go = gesture.Selection;
			if(go){
				//Debug.Log("Touching " + go.name);
				NinjaTrigger trigger = go.GetComponent<NinjaTrigger>();
				
				// if the trigger is null, check the parent...a little hacky, but sue me!
				if(trigger == null){
					trigger = go.transform.parent.gameObject.GetComponent<NinjaTrigger>();
				}
				
				if(trigger){
					trigger.OnCut(gesture.Position);
				}
			}
			break;
		}
	}

	protected override void _NewGame(){		
		// reset variables
		comboTime = 0;
		combo = 0;
		bestCombo = 0;
		timeCount = 0;
		ResetChain();
		currentTriggerEntries = null;

		// Reset all states
		bonusVisualController.StopBonusVisuals();
		bonusRound = false;
		spawning = true;

		if(IsTutorialOn() && (IsTutorialOverride() || 
			!DataManager.Instance.GameData.Tutorial.IsTutorialFinished(NinjaTutorial.TUT_KEY))){

			StartTutorial();
		}
	}
			
	protected override void _GameOver(){
		// Reset all states - Remove any visuals for combos
		bonusVisualController.StopBonusVisuals();
		bonusRound = false;
		spawning = true;

		// send out combo task
		int nBestCombo = GetComboBest();
		WellapadMissionController.Instance.TaskCompleted("Combo" + GetMinigameKey(), nBestCombo);
		Analytics.Instance.NinjaHighScore(DataManager.Instance.GameData.HighScore.MinigameHighScore[GetMinigameKey()]);
		Analytics.Instance.NinjaBonusRounds(bonusRoundCounter);
		Analytics.Instance.NinjaTimesPlayedTick();
#if UNITY_IOS
		LeaderBoardManager.Instance.EnterScore((long)GetScore(), "NinjaLeaderBoard");
#endif
	}

	protected override string GetMinigameKey(){
		return "Ninja";	
	}

	protected override bool IsTutorialOn(){
		return Constants.GetConstant<bool>("IsTriggerSlashTutorialOn");
	}

	protected override void _Update(){
		if(IsTutorialRunning()){
			return;
		}

		float deltaTime = Time.deltaTime;

		// update the player's combo
		UpdateComboTimer(deltaTime);
		
		// if there is a current group of spawn entries in process...
		if(currentTriggerEntries != null && currentTriggerEntries.Count > 0){
			// count up
			timeCount += deltaTime;
			
			// if our time has surpassed the next entry's time, do it up and remove that entry
			NinjaDataEntry entry = currentTriggerEntries[0];
			float timeEntry = entry.GetTime();
			if(timeCount >= timeEntry){
				SpawnGroup(entry);
				currentTriggerEntries.RemoveAt(0);
			}
			
			// if the list of current entries is empty...null the list and reset our count so we can count down again
			if(currentTriggerEntries.Count == 0){
				currentTriggerEntries = null;
				timeCount = timeBetweenSpawnGroups;
			}
		}
		else if(timeCount <= 0){
			// otherwise, there is no current group and it is time to start one, 
			// so figure out which one to begin
			NinjaScoring scoreKey;
			NinjaData data = null;
			if(spawning){
				scoreKey = GetScoringKey();
				data = NinjaDataLoader.GetGroupToSpawn(NinjaModes.Classic, scoreKey);

				// cache the list -- ALMOST FOOLED ME....use new to copy the list
				currentTriggerEntries = new List<NinjaDataEntry>(data.GetEntries());
			}
		}
		else{
			timeCount -= deltaTime;	// otherwise, there is no group and we still need to countdown before spawning the next group
		}
	}

	public GameObject SpawnTriggersTutorial(int num){
		GameObject triggerPrefab = (GameObject)Resources.Load("NinjaTrigger" + num.ToString());
		Vector3 triggerLocation;
		switch(num){
		case 1:
			triggerLocation = new Vector3(0, 1.8962f, 0);
			break;
		case 2:
			triggerLocation = new Vector3(1.4f, 2.9321f, 0);
			break;
		case 3:
			triggerLocation = new Vector3(-1.9353f, 1.0183f, 0);
			break;
		case 4:
			triggerLocation = new Vector3(3.0671f, 3.8541f, 0);
			break;
		case 5:
			triggerLocation = new Vector3(-3.5413f, -0.14406f, 0);
			break;
		case 6:
			triggerLocation = new Vector3(4.7322f, 4.5339f, 0);
			break;
		default:
			triggerLocation = new Vector3(0, 2.8f, 0);
			break;
		}
		//instantiate trigger 
		GameObject triggerObject = (GameObject)Instantiate(triggerPrefab, triggerLocation, 
		                                                    triggerPrefab.transform.localRotation);
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
	private void UpdateTrail(DragGesture gesture){
		ContinuousGesturePhase ePhase = gesture.Phase;

		// the screen position the user's finger is at currently
		Vector2 vPos = gesture.Position;

		// based on phase of the gesture, call certain functions
		switch(ePhase){
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
		lastPos = vPos;

		trailDeltaMove = gesture.DeltaMove;
	}

	/// <summary>
	/// Spawns the group.
	/// </summary>
	/// <param name="entry">Entry.</param>
	private void SpawnGroup(NinjaDataEntry entry){
		// create the proper list of objects to spawn
		int numOfTriggers = entry.GetTriggers();
		int numOfBombs = entry.GetBombs();
		int numOfPowUps = entry.GetPowUp();
		List<string> listObjects = new List<string>();

		for(int i = 0; i < numOfTriggers; ++i){
			// NOTE: if want to add variation over time, use GetRandomTrigger(n to choose from)
			string randomTrigger = 
				DataLoaderNinjaTriggersAndBombs.GetRandomTrigger(DataLoaderNinjaTriggersAndBombs.numTriggers);
			listObjects.Add(randomTrigger);
		}
		for(int i = 0; i < numOfBombs; ++i){
			// NOTE: if want to add variation over time, use GetRandomBomb(n to choose from)
			string randomBomb = 
				DataLoaderNinjaTriggersAndBombs.GetRandomBomb(DataLoaderNinjaTriggersAndBombs.numBombs);
			listObjects.Add(randomBomb);
		}

		for(int i = 0; i < numOfPowUps; ++i){
			// NOTE: if want to add variation over time, use GetRandomBomb(n to choose from)
			string randomPowUps = 
				DataLoaderNinjaTriggersAndBombs.GetRandomPowUp(DataLoaderNinjaTriggersAndBombs.numPowUps);
			listObjects.Add(randomPowUps);
		}
		// shuffle the list so everything is nice and mixed up
		listObjects.Shuffle();
		
		// create the proper object based off the entry's pattern
		NinjaPatterns patternType = entry.GetPattern();
		switch(patternType){
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
	/// Returns the current scoring key, based on the
	/// player's current score.  This function is a little
	/// hacky.
	/// </summary>
	/// <returns>The scoring key.</returns>
	private NinjaScoring GetScoringKey(){
		int nScore = GetScore();
		NinjaScoring eScore;

		if(bonusRound == true){
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

	/// <summary>
	/// Updates the combo timer.
	/// </summary>
	/// <param name="fDelta">F delta.</param>
	private void UpdateComboTimer(float deltaTime){
		// if the player doesn't have a combo going, don't bother
		int combo = GetCombo();
		if(combo <= 0)
			return;
		
		// update the time
		comboTime -= deltaTime;
		
		// if the time has expired, end the current combo
		if(comboTime <= 0)
			OnComboEnd();
	}

	private void OnComboEnd(){
		// give the player an additional point for each level of their combo
		int combo = GetCombo();
		if(combo > 2){
			UpdateScore(combo);
				        	
			// get the right text for combo
			string strText = Localization.Localize("NINJA_COMBO");
			strText = String.Format(strText, combo);
			
			// get the position of where to spawn the floaty text -- the last place the user's finger was (using this for now)
			Vector3 position = lastPos;
			position.y *= CameraManager.Instance.GetRatioDifference();
			position.x *= CameraManager.Instance.GetRatioDifference();
			position = CameraManager.Instance.TransformAnchorPosition(position, InterfaceAnchors.BottomLeft, InterfaceAnchors.Center);
	
			// set up the hashtable full of options
			Hashtable option = new Hashtable();
			option.Add("parent", GameObject.Find("Anchor-Center"));
			option.Add("text", strText);
			option.Add("prefab", "NinjaComboFloatyText");
			option.Add("position", position);
			option.Add("textSize", Constants.GetConstant<float>("Ninja_ComboTextSize"));
			
			// spawn floaty text
			FloatyUtil.SpawnFloatyText(option);				
		}		
		
		// if the current combo was better than their best, update it
		int bestCombo = GetComboBest();
		if(combo > bestCombo)
			SetComboBest(combo);
		
		// reset the combo down to 0
		SetCombo(0);	
	}
	
	private void StartTutorial(){
		SetTutorial(new NinjaTutorial());
	}

	IEnumerator WaitASec(int _rand, List<string> listObjects){
		for(int i = 0; i <= _rand; i++){
			yield return new WaitForSeconds(0.1f);
			new SpawnGroupSwarms(listObjects);
		}
	}

	public void CheckEndBonus(){
		if(bonusRoundEnemies <= 0){
			bonusVisualController.StopBonusVisuals();
			bonusRound = false;
			spawning = true;
		}
	}

	public void StartBonusVisuals(){
		AudioManager.Instance.PlayClip("ninjaBonus");
		bonusVisualController.PlayBonusVisuals();
	}
}
