using UnityEngine;
using System.Collections;
using System;

//InhalerLogic stores and manipulate any inhaler game related data
public class InhalerLogic : Singleton<InhalerLogic>{
	public static EventHandler<EventArgs> OnGameOver; //Game over show game over message
	public static EventHandler<EventArgs> OnNextStep; //Completed one step, so move on
	public const int RESCUE_NUM_STEPS = 8;
	private int currentStep = 1; //current step that user is on

	/*
        call the api in this order
        1)IsCurrentStepCorrect
        2)NextStep
    */

	//return the current step of a sequence
	public int CurrentStep{
		get{ return currentStep;}
	}

	public bool IsFirstTimeRescue{
		get{ return DataManager.Instance.GameData.Inhaler.IsFirstTimeRescue; }
		set{ DataManager.Instance.GameData.Inhaler.IsFirstTimeRescue = value; }
	}

	public bool IsTutorialCompleted{
		get{ return DataManager.Instance.GameData.Tutorial.IsTutorialFinished(TutorialManagerBedroom.TUT_INHALER);}
	}

	//True: the step that the user is currently on is correct, False: wrong step
	public bool IsCurrentStepCorrect(int step){
		bool retVal = step == currentStep;
		return retVal; 
	}

	public bool IsDoneWithGame(){
		return currentStep == RESCUE_NUM_STEPS-1;
	}

	//Use this function to move on to the next step
	public void NextStep(){
		if(IsDoneWithGame()){
			GameDone();
		}
		else{
			currentStep++;

			if(OnNextStep != null){
				OnNextStep(this, EventArgs.Empty);
			}
			else{
				Debug.LogError("OnNextStep has no listeners");
			}
		}
	}

	public void ResetGame(){
		currentStep = 1;
	}

	//---------------------------------------------------
	// GameDone()
	// Put anything in here that should happen as a result
	// of the pet using the daily inhaler.
	//---------------------------------------------------		
	private void GameDone(){
		InhalerGameUIManager.Instance.StopShowHintTimer();
		StatsController.Instance.ChangeStats(deltaHealth: 5, deltaMood: 100,isInternal: true);
		DateTime now = LgDateTime.GetTimeNow();
		DateTime then = DataManager.Instance.GameData.Inhaler.LastPlayPeriodUsed;
		TimeSpan lastTimeSinceInhaler = now - then;
		if(lastTimeSinceInhaler.TotalHours > 24){
			DataManager.Instance.GameData.Inhaler.timesUsedInARow = 0;
		}
		DataManager.Instance.GameData.Inhaler.timesUsedInARow++;
		// Save settings into data manager
		IsFirstTimeRescue = false;
		DataManager.Instance.GameData.Inhaler.LastPlayPeriodUsed = PlayPeriodLogic.GetCurrentPlayPeriod();
		DataManager.Instance.GameData.Inhaler.LastInhalerPlayTime = LgDateTime.GetTimeNow();

		if(OnGameOver != null){
			OnGameOver(this, EventArgs.Empty);
		}

		//finish inhaler tutorial 
		if(!IsTutorialCompleted){
			DataManager.Instance.GameData.Tutorial.ListPlayed.Add(TutorialManagerBedroom.TUT_INHALER);
		}
		
		// send out a task completion event for the wellapad
		WellapadMissionController.Instance.TaskCompleted("DailyInhaler");

		// calculate the next play period for the inhaler
	PlayPeriodLogic.Instance.InhalerGameDonePostLogic();
	}
}
