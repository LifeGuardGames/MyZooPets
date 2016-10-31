using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DoctorMatchManager : MinigameManager<DoctorMatchManager> {

	//=======================Events========================
	public static EventHandler<EventArgs> OnSpeedChange; 	// when the game speed changes
	public static EventHandler<EventArgs> OnCharacterScoredRight;
	public static EventHandler<EventArgs> OnCharacterScoredWrong;
	//=====================================================

	// Move this to constant XML -------
	public float minimumSpeed = 2f;
	public float startSpeed = 2f;
	public float speedIncreaseInterval = 0.2f;

	public float minimumFrequency = 1.5f;
	public float startFrequency = 1.5f;
	public float frequencyIncreaseInterval = 0.2f;

	public int speedUpMatchInterval = 5;	// Amount of pets correct needed before speedup occurs
	private int speedUpMatchTrack = 0;
	public AssemblyLineController assemblyLineController;

	public Sprite[] spriteList;

	private int numOfCorrectDiagnose;
	public int NumOfCorrectDiagnose{
		get{ return numOfCorrectDiagnose; }
	}

 //keep track of the number of correct diagnose

	void Awake(){
		quitGameScene = SceneUtils.BEDROOM;
	}
	
	protected override void _Start(){
	}	
	
	protected override void _OnDestroy(){
		OnCharacterScoredRight = null;
	}

	public override int GetReward(MinigameRewardTypes eType){
		// for now, just use the standard way
		return GetStandardReward(eType);
	}

	protected override void _NewGame(){
		// set num of correct diagnose for each new game
		numOfCorrectDiagnose = 0;

		// if the play hasn't played the tutorial yet, start it
		if(IsTutorialOn() && (IsTutorialOverride() || 
          !DataManager.Instance.GameData.Tutorial.IsTutorialFinished(DoctorMatchTutorial.TUT_KEY)))
			StartTutorial();
		else{
			assemblyLineController.Speed = startSpeed;
			assemblyLineController.Frequency = startFrequency;
			assemblyLineController.StartSpawning();
		}
	}

	protected override void _GameOver(){
		Analytics.Instance.DoctorMatchGameData(DataManager.Instance.GameData.HighScore.MinigameHighScore[GetMinigameKey()]);
		#if UNITY_IOS
		LeaderBoardManager.Instance.EnterScore((long)GetScore(), "DoctorLeaderBoard");
#endif
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

		if(!IsTutorialRunning()){
			int correctPoints = Constants.GetConstant<int>("Clinic_ScoredCharacterValue");
			UpdateScore(correctPoints);
			numOfCorrectDiagnose++;

			speedUpMatchTrack++;
			if(speedUpMatchTrack == speedUpMatchInterval){
				speedUpMatchTrack = 0;
				SpeedGameUp();
			}
		}
	
		// Play appropriate sound
		AudioManager.Instance.PlayClip("clinicCorrect");

		if(OnCharacterScoredRight != null)
			OnCharacterScoredRight(this, EventArgs.Empty);
	}

	public void CharacterScoredWrong(){
		// character was sent to wrong zone...lose a life!
		UpdateLives(-1);

		// play an incorrect sound
		AudioManager.Instance.PlayClip("minigameError");

		// also slow down the game, if this didn't cause us to have a game over
		MinigameStates eState = GetGameState();

		if(eState == MinigameStates.Playing && !IsTutorialRunning()){
			speedUpMatchTrack = 0;	// Reset speed track counter
			SlowGameDown();
		}

		if(OnCharacterScoredWrong != null)
			OnCharacterScoredWrong(this, EventArgs.Empty);
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

	/// <summary>
	/// Sets up assembly item sprite.
	/// For itemGroupNumber
	/// 0: random, 1: green, 2: yellow, 3: red
	/// </summary>
	/// <param name="assemblyLineItemObject">Assembly line item object.</param>
	/// <param name="itemGroupNumber">Item group number. </param>
	public void SetUpAssemblyItemSprite(GameObject assemblyLineItemObject, int itemGroupNumber = 0){
		AssemblyLineItem item = assemblyLineItemObject.GetComponent<AssemblyLineItem>();
		item.Speed = assemblyLineController.Speed;

		SpriteRenderer sprite = assemblyLineItemObject.transform.Find("Sprite").gameObject.GetComponent<SpriteRenderer>();

		int randomNum = itemGroupNumber;
		if(randomNum == 0)
			randomNum = UnityEngine.Random.Range(1,4);

		int chooseSpriteRandom = UnityEngine.Random.Range(0,5);

		switch(randomNum){
		case 1:
			item.itemKey = "green";
			sprite.sprite = spriteList[chooseSpriteRandom];
			break;
		case 2:
			item.itemKey = "yellow";
			sprite.sprite = spriteList[5+chooseSpriteRandom];
			break;
		case 3:
			item.itemKey = "red";
			sprite.sprite = spriteList[10+chooseSpriteRandom];
			break;
		default:
			Debug.LogError("Not valid item group");
			break;
		}
	}

	/// <summary>
	/// Sets up assembly item sprite.
	/// For itemGroupNumber
	/// 0: random, 1: green, 2: yellow, 3: red
	/// </summary>
	/// <param name="assemblyLineItemObject">Assembly line item object.</param>
	/// <param name="itemGroupNumber">Item group number. </param>
	/// seperated for tutorials as those sprites arn't random
	public void SetUpAssemblyItemSpriteTutorial(GameObject assemblyLineItemObject,int spriteNum, int itemGroupNumber = 0){
		AssemblyLineItem item = assemblyLineItemObject.GetComponent<AssemblyLineItem>();
		item.Speed = assemblyLineController.Speed;
		
		SpriteRenderer sprite = assemblyLineItemObject.transform.Find("Sprite").gameObject.GetComponent<SpriteRenderer>();
		
		switch(itemGroupNumber){
		case 1:
			item.itemKey = "green";
			sprite.sprite = spriteList[spriteNum];
			break;
		case 2:
			item.itemKey = "yellow";
			sprite.sprite = spriteList[5+spriteNum];
			break;
		case 3:
			item.itemKey = "red";
			sprite.sprite = spriteList[10+spriteNum];
			break;
		default:
			Debug.LogError("Not valid item group");
			break;
		}
	}

	private void StartTutorial(){
		// set our tutorial
		SetTutorial(new DoctorMatchTutorial());
	}	
}
