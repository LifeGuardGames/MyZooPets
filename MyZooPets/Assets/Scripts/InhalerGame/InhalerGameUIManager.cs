using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InhalerHintEventArgs : EventArgs{
	public bool IsDisplayingHint { get; set; }

	public InhalerHintEventArgs(bool isDisplayingHint = true){
		IsDisplayingHint = isDisplayingHint;
	}
}

public class InhalerGameUIManager : Singleton<InhalerGameUIManager>{
	public static EventHandler<InhalerHintEventArgs> HintEvent; //Fire this event when hints need to display 

	public GameObject progressBarObject;
	public GameObject inhalerBody;
	public Animator inhalerWholeObject;
	public SceneTransition scriptTransition;
	public bool tutOn; //turn tutorial on or off. for debuggin
	private bool isFirstTimeAux; // Keep track of this internally, need for gameover reward

	private bool showHint = false; //display swipe hints for the inhaler
	private bool runShowHintTimer = true; //True: start running hint timer
	private float timer = 0; //hint timer
	private float timeBeforeHints = 5.0f; //5 seconds before the hint is shown
	private int starIncrement = 0;
	public GameObject[] lightsToTurnOff;
	public ParticleSystemController[] particlesToTurnOff;


	public bool ShowHint{
		get{
			//take into consideration if user has seen the new gesture introduced in v1.2.8
			return showHint || InhalerLogic.Instance.IsNewToTapPrescriptionHint;
		}
	}

	void Awake(){
		RewardManager.OnAllRewardsDone += QuitInhalerGame;
	}

	void Start(){
		Input.multiTouchEnabled = true;
		InhalerLogic.OnGameOver += OnGameEnd;
		InhalerLogic.OnNextStep += OnNextStep;

		StartCoroutine(StartGame());

		isFirstTimeAux = InhalerLogic.Instance.IsFirstTimeRescue;
	}

	void OnDestroy(){
		InhalerLogic.OnGameOver -= OnGameEnd;
		InhalerLogic.OnNextStep -= OnNextStep;
		RewardManager.OnAllRewardsDone -= QuitInhalerGame;
	}

	//---------------------------------------------
	// If runShowHintTimer is true, hints will be hidden at first, 
	// and shown only when the user has not made the correct move
	// after a specified period of time (timeBeforeHints).
	// If it is false, that means the hints should be shown 
	// throughout the game (for someone's first time playing this).
	//----------------------------------------------
	void Update(){
		if(runShowHintTimer && !InhalerLogic.Instance.IsDoneWithGame()){
			ShowHintTimer(); // This checks and shows hints if necessary.
		}
	}
 
	private void HideProgressBar(){
		progressBarObject.SetActive(false);
	}

	private void ShowHUD(){
		HUDUIManager.Instance.ShowPanel();
	}

	private void HideHUD(){
		HUDUIManager.Instance.HidePanel();
	}

	public void HideInhaler(){
		inhalerWholeObject.Play("InhalerFade");
	}

	private void ShowInhaler(){
		inhalerBody.SetActive(true);
	}

	private IEnumerator StartGame(){
		yield return 0;

		HideHUD();
		SetUpHintTimer();

		//Start the first hint
		if(HintEvent != null)
			HintEvent(this, new InhalerHintEventArgs(isDisplayingHint: true));
	}

	//----------------------------------------------------------
	// SetUpHintTimer()
	// Decides whether hints should be display right away or wait
	// for the hint count down timer
	//----------------------------------------------------------
	private void SetUpHintTimer(){
		//User is new to the tap gesture we introduce in v1.2.8 force the tutorial to run again
		bool isNewToTapPrescriptionHint = InhalerLogic.Instance.IsNewToTapPrescriptionHint;

		//Show hint right away if it's users' first time
		if((InhalerLogic.Instance.IsFirstTimeRescue && tutOn) || (isNewToTapPrescriptionHint && tutOn)){ 
			runShowHintTimer = false;
			showHint = true;
		}
		else{
			runShowHintTimer = true;
			showHint = false;
			timer = 0;
		}
	}

	//----------------------------------------------------------
	// ShowHintTimer()
	//  Hints will be hidden at first, and shown only when the user 
	// has not made the correct move after a specified period of time (timeBeforeHints).
	//----------------------------------------------------------
	private void ShowHintTimer(){ // to be called in Update()
		timer += Time.deltaTime;
		if(timer > timeBeforeHints){
			showHint = true;

			Analytics.Instance.InhalerHintRequired(InhalerLogic.Instance.CurrentStep);

			if(HintEvent != null)
				HintEvent(this, new InhalerHintEventArgs(isDisplayingHint: true));

			runShowHintTimer = false;
		}
	}

	//----------------------------------------------------------
	// ResetHintTimer()
	//Timer is reset every time the current step changes
	//----------------------------------------------------------
	private void ResetHintTimer(){
		timer = 0;
		showHint = false; 
		runShowHintTimer = true;
	}

	//Event listener. Listens to when user moves on to the next step
	private void OnNextStep(object sender, EventArgs args){
		if(!InhalerLogic.Instance.IsFirstTimeRescue || !tutOn)
			ResetHintTimer();

		if(HintEvent != null)
			HintEvent(this, new InhalerHintEventArgs(isDisplayingHint: false));
	}

	//Event listener. Listens to game over message. Play fire animation 
	private void OnGameEnd(object sender, EventArgs args){
		ShowHUD();
		HideProgressBar();

		Invoke("GiveReward", 1.0f);
	}
	
	//Reward player after the animation is done
	private void GiveReward(){
		//Reward xp
		int nXP = DataLoaderXpRewards.GetXP("DailyInhaler", new Hashtable());
		StatsController.Instance.ChangeStats(deltaPoints: nXP, deltaStars: starIncrement);
		
		// Reward shards
		int fireShardReward = 50;	
		if(isFirstTimeAux){	// First time tutorial gives you full crystal
			fireShardReward = 100;
		}
		FireCrystalManager.Instance.RewardShards(fireShardReward);

		// Check for badge reward
		BadgeLogic.Instance.CheckSeriesUnlockProgress(BadgeType.Inhaler, 1, false);
	}

	private void QuitInhalerGame(object sender, EventArgs args){
		NotificationUIManager.Instance.CleanupNotification();
		LoadLevelUIManager.Instance.StartLoadTransition(SceneUtils.BEDROOM);
	}
}

