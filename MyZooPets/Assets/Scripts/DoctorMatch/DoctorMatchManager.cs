using UnityEngine;
using System.Collections;

public class DoctorMatchManager : MinigameManager<DoctorMatchManager> {

	// Move this to constant XML -------
	public float speedIncreaseInterval = 1.0f;

	public AssemblyLineController assemblyLineController;


	private int numOfCorrectDiagnose; //keep track of the number of correct diagnose


	//---------------------------------------------------
	// GetReward()
	//---------------------------------------------------	
	public override int GetReward(MinigameRewardTypes eType){
		// for now, just use the standard way
		return GetStandardReward(eType);
	}

	//---------------------------------------------------
	// _Start()
	//---------------------------------------------------	
	protected override void _Start(){
	}	
	
	//---------------------------------------------------
	// _OnDestroy()
	//---------------------------------------------------	
	protected override void _OnDestroy(){
	}
	
	//---------------------------------------------------
	// _NewGame()
	//---------------------------------------------------	
	protected override void _NewGame(){
		// set num of correct diagnose for each new game
		numOfCorrectDiagnose = 0;


		assemblyLineController.StartSpawning();

	}
	
	//---------------------------------------------------
	// _GameOver()
	//---------------------------------------------------		
	protected override void _GameOver(){
		BadgeLogic.Instance.CheckSeriesUnlockProgress(BadgeType.PatientNumber, numOfCorrectDiagnose, true);
	}
	
	//---------------------------------------------------
	// _Update()
	//---------------------------------------------------
	protected override void _Update(){
	}
	
	//---------------------------------------------------
	// GetMinigameKey()
	//---------------------------------------------------	
	protected override string GetMinigameKey(){
		return "DoctorMatch";
	}	
	
	protected override bool IsTutorialOn(){
		return Constants.GetConstant<bool>("IsDoctorMatchTutorialOn");
	}

	public void CharacterScoredRight(){
		UpdateScore(1);
		numOfCorrectDiagnose++;

		// play appropriate sound
		AudioManager.Instance.PlayClip("clinicCorrect");

		// Analytics
//		Analytics.Instance.DiagnoseResult(Analytics.DIAGNOSE_RESULT_CORRECT, character.GetStage(), zone.GetStage());
	}

	public void CharacterScoredWrong(){
		// character was sent to wrong zone...lose a life!
		UpdateLives(-1);

		// play an incorrect sound
		AudioManager.Instance.PlayClip("clinicWrong");

		//Analytics
//		Analytics.Instance.DiagnoseResult(Analytics.DIAGNOSE_RESULT_INCORRECT, character.GetStage(), zone.GetStage());

		// also slow down the game, if this didn't cause us to have a game over
		MinigameStates eState = GetGameState();
		if(eState == MinigameStates.Playing)
			SlowGameDown();

		Debug.Log(eState);
	}

	private void SlowGameDown(){
		
	}
}
