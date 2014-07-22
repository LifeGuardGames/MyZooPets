using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DoctorMatchManager : MinigameManager<DoctorMatchManager> {

	//=======================Events========================
	public static EventHandler<EventArgs> OnSpeedChange; 	// when the game speed changes
	//=====================================================

	// Move this to constant XML -------
	public float minimumSpeed = 2f;
	public float startSpeed = 2f;
	public float speedIncreaseInterval = 0.2f;

	public float minimumFrequency = 1.5f;
	public float startFrequency = 1.5f;
	public float frequencyIncreaseInterval = 0.1f;

	public int speedUpMatchInterval = 5;	// Amount of pets correct needed before speedup occurs
	private int speedUpMatchTrack = 0;
	public AssemblyLineController assemblyLineController;

	public Sprite[] spriteList;

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
		assemblyLineController.Speed = startSpeed;
		assemblyLineController.Frequency = startFrequency;
		assemblyLineController.StartSpawning();
	}
	
	//---------------------------------------------------
	// _GameOver()
	//---------------------------------------------------		
	protected override void _GameOver(){
		assemblyLineController.StopSpawning();

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

		speedUpMatchTrack++;
		if(speedUpMatchTrack == speedUpMatchInterval){
			speedUpMatchTrack = 0;
			SpeedGameUp();
		}

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

		if(eState == MinigameStates.Playing){
			speedUpMatchTrack = 0;	// Reset speed track counter
			SlowGameDown();
		}
	}

	private void SpeedGameUp(){
		float newSpeed = assemblyLineController.Speed + speedIncreaseInterval;
		float newFrequency = assemblyLineController.Frequency - frequencyIncreaseInterval;

		// Set assembly line speed for new spawns
		assemblyLineController.Speed = newSpeed;
		// Send out a message to all things on the assembly line letting them know their speed needs to change
		if(OnSpeedChange != null)
			OnSpeedChange(this, EventArgs.Empty);

		// Set assembly line spawn frequency
		assemblyLineController.Frequency = newFrequency;
	}

	private void SlowGameDown(){
		float newSpeed = Math.Max(assemblyLineController.Speed / 2f, minimumSpeed);	// Pick the higher of half of speed or minimum
		float newfrequency = Math.Max(assemblyLineController.Frequency / 2f, minimumFrequency);	// Pick the higher of half of speed or minimum

		// Cut speed in half for now for new spawns
		assemblyLineController.Speed = newSpeed;
		// Send out a message to all things on the assembly line letting them know their speed needs to change
		if(OnSpeedChange != null)
			OnSpeedChange(this, EventArgs.Empty);

		// Set assembly line spawn frequency
		assemblyLineController.Frequency = newfrequency;
	}

	public void SetUpRandomAssemblyItemSprite(GameObject assemblyLineItemObject){
		AssemblyLineItem item = assemblyLineItemObject.GetComponent<AssemblyLineItem>();
		item.Speed = assemblyLineController.Speed;

		SpriteRenderer sprite = assemblyLineItemObject.transform.Find("Sprite").gameObject.GetComponent<SpriteRenderer>();

		int randomNum = UnityEngine.Random.Range(1,4);

		int chooseSpriteRandom = UnityEngine.Random.Range(0,4);

		switch(randomNum){
		case 1:
			item.itemKey = "green";
			sprite.sprite = spriteList[chooseSpriteRandom];
			break;
		case 2:
			item.itemKey = "yellow";
			sprite.sprite = spriteList[4+chooseSpriteRandom];
			break;
		case 3:
			item.itemKey = "red";
			sprite.sprite = spriteList[8+chooseSpriteRandom];
			break;
		default:
			Debug.LogError("Not valid random number");
			break;
		}
//		Debug.Log(item.itemKey);
	}
}
