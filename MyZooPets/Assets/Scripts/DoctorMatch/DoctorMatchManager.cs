using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DoctorMatchManager : MinigameManager<DoctorMatchManager> {
	public AssemblyLineController assemblyLineController;
	public DoctorMatchLifeBarController lifeBarController;

	private int numOfCorrectDiagnose;
	public int NumOfCorrectDiagnose{
		get{ return numOfCorrectDiagnose; }
	}

	void Awake(){
		quitGameScene = SceneUtils.BEDROOM;
	}
	
	protected override void _Start(){
	}	
	
	protected override void _OnDestroy(){
	}

	public override int GetReward(MinigameRewardTypes eType){
		// for now, just use the standard way
		return GetStandardReward(eType);
	}

	private void StartTutorial(){
		// set our tutorial
		SetTutorial(new DoctorMatchTutorial());
	}	

	protected override void _NewGame(){

	}

	protected override void _GameOver(){
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

	public void CharacterScoredRight(){
		if(IsTutorialRunning()){
			
		}
		else{

		}
	
		// Play appropriate sound
		AudioManager.Instance.PlayClip("clinicCorrect");
	}

	public void CharacterScoredWrong(){
		// Game over, starting with 1 life
		UpdateLives(-1);

		// Play an incorrect sound
		AudioManager.Instance.PlayClip("minigameError");


	}

	public void SetUpAssemblyItemSprite(GameObject assemblyLineItemObject, int itemGroupNumber = 0){
		
	}
}
