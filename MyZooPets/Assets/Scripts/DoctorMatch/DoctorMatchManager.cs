using System;
using UnityEngine;

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

	private int numOfCorrectDiagnose;
	private int combo=0;
	private float timeToCombo=3f; //Time between each correct diagnoses for it to be consistent
	private float currentComboTime=0;
	private bool paused=true;

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
			ComboBonus();
		}
		#endif
		if (currentComboTime>0){
			currentComboTime-=Time.deltaTime;
		} else if (combo!=0) {
			combo=0;
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

	public void StartTutorial() {
		// set our tutorial
		//	SetTutorial(new DoctorMatchTutorial());
	}

	protected override void _NewGame() { //Reset everything then start again
		ResetScore();

		assemblyLineController.Initialize();
		lifeBarController.ResetBar();
		lifeBarController.StartDraining();

		zoneGreen.ToggleButtonInteractable(true);
		zoneYellow.ToggleButtonInteractable(true);
		zoneRed.ToggleButtonInteractable(true);

	}

	protected override void _PauseGame(bool isShow) {
		Debug.Log(isShow);
		paused=!isShow;
		if (isShow) {
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

	// Input coming from button scripts
	public void OnZoneClicked(DoctorMatchButtonTypes buttonType) {
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
	void OnGUI(){
		GUI.Box(new Rect(Screen.width/2,Screen.height/2,80,40),"C : " + combo + "\n T: " + currentComboTime);
	}
	// When your timer runs out
	public void OnTimerBarEmpty() {
		GameOver();
	}
	private void ComboBonus() {
		lifeBarController.PlusBar(3f);
		assemblyLineController.ClearLine();
		combo=0;
	}
	private void ResetScore() {
		combo=0;
		currentComboTime=0;

		rewardXPMultiplier = 1f;
		rewardMoneyMultiplier = 1f;
		rewardShardMultiplier = 1f;
		score=0;
	}
	private void CharacterScoredRight() {
		//if(IsTutorialRunning()){
			
		//}
		//else{
		combo++;
		currentComboTime=timeToCombo;
		lifeBarController.PlusBar();
		UpdateScore(1);
		//}
		if (combo==5){
			ComboBonus();
		}
		// Play appropriate sound
		AudioManager.Instance.PlayClip("clinicCorrect");
	}

	private void CharacterScoredWrong() {
		lifeBarController.HurtBar();
		combo=0;
		currentComboTime=0;
		// Play an incorrect sound
		AudioManager.Instance.PlayClip("minigameError");
		cameraShake.Play();
	}
}
