using System;
using UnityEngine;

public class DoctorMatchManager : NewMinigameManager<DoctorMatchManager> {
	public enum DoctorMatchButtonTypes{
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
	public int NumOfCorrectDiagnose{
		get{ return numOfCorrectDiagnose; }
	}

	void Awake(){
		// Parent settings
		minigameKey = "DOCTOR";
		quitGameScene = SceneUtils.BEDROOM;
		rewardXPMultiplier = 1f;
		rewardMoneyMultiplier = 1f;
		rewardShardMultiplier = 1f;
    }
	
	protected override void _Start(){
		zoneGreen.ToggleButtonInteractable(false);
		zoneYellow.ToggleButtonInteractable(false);
		zoneRed.ToggleButtonInteractable(false);
	}

	public void StartTutorial(){
		// set our tutorial
	//	SetTutorial(new DoctorMatchTutorial());
	}

	protected override void _NewGame(){
		assemblyLineController.Initialize();
		lifeBarController.ResetBar();
		lifeBarController.StartDraining();

		zoneGreen.ToggleButtonInteractable(true);
		zoneYellow.ToggleButtonInteractable(true);
		zoneRed.ToggleButtonInteractable(true);
	}

	protected override void _PauseGame() {
		// not implemented yet!
	}

	protected override void _GameOver(){
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
		StatsController.Instance.ChangeStats(
			deltaPoints: rewardXPAux,
			pointsLoc: GenericMinigameUI.Instance.GetXPPanelPosition(),
			deltaStars: rewardMoneyAux,
			starsLoc: GenericMinigameUI.Instance.GetCoinPanelPosition(),
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
	public void OnZoneClicked(DoctorMatchButtonTypes buttonType){
		AssemblyLineItem poppedItem = assemblyLineController.PopFirstItem();
		if(poppedItem.ItemType == buttonType){
			CharacterScoredRight();
		}
		else{
			CharacterScoredWrong();
		}

		// Have item do what it does when activated
		poppedItem.Activate();

		assemblyLineController.ShiftAndAddNewItem();
	}

	// When your timer runs out
	public void OnTimerBarEmpty(){
		GameOver();
	}

	private void CharacterScoredRight(){
		//if(IsTutorialRunning()){
			
		//}
		//else{
			lifeBarController.PlusBar();
			UpdateScore(1);
		//}
	
		// Play appropriate sound
		AudioManager.Instance.PlayClip("clinicCorrect");
	}

	private void CharacterScoredWrong(){
		lifeBarController.HurtBar();

		// Play an incorrect sound
		AudioManager.Instance.PlayClip("minigameError");
		cameraShake.Play();
    }
}
