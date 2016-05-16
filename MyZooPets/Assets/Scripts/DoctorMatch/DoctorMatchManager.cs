using UnityEngine;

public class DoctorMatchManager : MinigameManager<DoctorMatchManager> {
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
		quitGameScene = SceneUtils.BEDROOM;
	}
	
	protected override void _Start(){
		zoneGreen.ToggleButtonInteractable(false);
		zoneYellow.ToggleButtonInteractable(false);
		zoneRed.ToggleButtonInteractable(false);
	}	
	
	protected override void _OnDestroy(){
	}

	public override int GetReward(MinigameRewardTypes eType){
		// for now, just use the standard way
		return GetStandardReward(eType);
	}

	public void StartTutorial(){
		// set our tutorial
		SetTutorial(new DoctorMatchTutorial());
	}	

	protected override void _NewGame(){
		assemblyLineController.Initialize();
		lifeBarController.ResetBar();
		lifeBarController.StartDraining();

		zoneGreen.ToggleButtonInteractable(true);
		zoneYellow.ToggleButtonInteractable(true);
		zoneRed.ToggleButtonInteractable(true);
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

	protected override void _Update(){
	}

	protected override string GetMinigameKey(){
		return "Clinic";
	}	
	
	protected override bool IsTutorialOn(){
		return Constants.GetConstant<bool>("IsDoctorMatchTutorialOn");
	}

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
		// Game over, starting with 1 life
		UpdateLives(-1);
	}

	private void CharacterScoredRight(){
		if(IsTutorialRunning()){
			
		}
		else{
			lifeBarController.PlusBar();
			UpdateScore(1);
		}
	
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
