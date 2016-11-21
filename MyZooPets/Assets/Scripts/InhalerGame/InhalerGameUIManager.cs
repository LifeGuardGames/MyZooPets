using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InhalerHintEventArgs : EventArgs {
	public bool IsDisplayingHint { get; set; }

	public InhalerHintEventArgs(bool isDisplayingHint = true) {
		IsDisplayingHint = isDisplayingHint;
	}
}

public class InhalerGameUIManager : Singleton<InhalerGameUIManager> {
	public static EventHandler<InhalerHintEventArgs> HintEvent; //Fire this event when hints need to display 

	public GameObject progressBarObject;
	public GameObject inhalerBody;
	public Animator inhalerWholeObject;
	public bool tutOn;						//turn tutorial on or off. for debuggin
	private bool isFirstTimeAux;			// Keep track of this internally, need for gameover reward

	private bool showHint = false;			//display swipe hints for the inhaler
	private bool runShowHintTimer = true;	//True: start running hint timer
	public float timer = 0;					//hint timer
	private float timeBeforeHints = 5.0f;	//5 seconds before the hint is shown
	private int starIncrement = 0;
	public GameObject[] lightsToTurnOff;
	public ParticleSystemController[] particlesToTurnOff;
	public List<GameObject> sliderNodes;	//list of UI nodes to show game steps
	
	public bool ShowHint {
		get { return showHint; }
	}

	public void StopShowHintTimer() {
		runShowHintTimer = false;
	}

	void Awake() {
		RewardManager.OnAllRewardsDone += QuitInhalerGame;
	}

	void Start() {
		Input.multiTouchEnabled = false;

		// Reset the progress UI
		foreach(GameObject go in sliderNodes) {
			go.SetActive(false);
		}

		StartCoroutine(StartGame());

		isFirstTimeAux = InhalerGameManager.Instance.IsFirstTimeRescue;
	}

	void OnDestroy() {
		RewardManager.OnAllRewardsDone -= QuitInhalerGame;
	}

	//---------------------------------------------
	// If runShowHintTimer is true, hints will be hidden at first, 
	// and shown only when the user has not made the correct move
	// after a specified period of time (timeBeforeHints).
	// If it is false, that means the hints should be shown 
	// throughout the game (for someone's first time playing this).
	//----------------------------------------------
	void Update() {
		//if(runShowHintTimer && !InhalerLogic.Instance.IsDoneWithGame()){
		if(runShowHintTimer) {
			timer += Time.deltaTime;
			if(timer > timeBeforeHints) {
				showHint = true;

				Analytics.Instance.InhalerHintRequired(InhalerGameManager.Instance.CurrentStep);

				if(HintEvent != null) {
					HintEvent(this, new InhalerHintEventArgs(isDisplayingHint: true));
				}

				runShowHintTimer = false;
			}
		}
	}

	public void HideInhaler() {
		inhalerWholeObject.Play("InhalerFade");
	}

	private void ShowInhaler() {
		inhalerBody.SetActive(true);
	}

	private IEnumerator StartGame() {
		yield return 0;
		HUDUIManager.Instance.HidePanel();
		InventoryUIManager.Instance.HidePanel();

		//Show hint right away if it's users' first time
		if((InhalerGameManager.Instance.IsFirstTimeRescue && tutOn)) {
			runShowHintTimer = false;
			showHint = true;
		}
		else {
			runShowHintTimer = true;
			showHint = false;
			timer = 0;
		}

		//Start the first hint
		if(HintEvent != null) {
			HintEvent(this, new InhalerHintEventArgs(isDisplayingHint: true));
		}
	}

	// Called from gameManager to see when next step
	public void NextStepUI(int step) {
		//Timer is reset every time the current step changes
		if(!InhalerGameManager.Instance.IsFirstTimeRescue || !tutOn) {
			timer = 0;
			showHint = false;
			runShowHintTimer = true;
		}

		if(HintEvent != null) {
			HintEvent(this, new InhalerHintEventArgs(isDisplayingHint: false));
		}

		// Actually show the number of steps completed in UI
		sliderNodes[step - 2].SetActive(true);
	}

	public void GameEndUI() {
		HUDUIManager.Instance.ShowPanel();
		progressBarObject.SetActive(false);
		Invoke("GiveReward", 1.0f);
	}

	//Reward player after the animation is done
	private void GiveReward() {
		// Reward XP
		int xp = DataLoaderXpRewards.GetXP("DailyInhaler", new Hashtable());
		StatsManager.Instance.ChangeStats(xpDelta: xp, coinsDelta: starIncrement);

		// Reward shards
		int fireShardReward = 50;
		if(isFirstTimeAux) {    // First time tutorial gives you full crystal
			fireShardReward = 100;
		}
		FireCrystalManager.Instance.RewardShards(fireShardReward);

		// Check for badge reward
		BadgeManager.Instance.CheckSeriesUnlockProgress(BadgeType.Inhaler, 1, false);
		BadgeManager.Instance.CheckSeriesUnlockProgress(BadgeType.Retention, DataManager.Instance.GameData.Inhaler.timesUsedInARow, true);
	}

	private void QuitInhalerGame(object sender, EventArgs args) {
		LoadLevelManager.Instance.StartLoadTransition(SceneUtils.BEDROOM);
	}
}

