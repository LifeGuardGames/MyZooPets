using UnityEngine;
using System.Collections;
using System;

//InhalerLogic stores and manipulate any inhaler game related data
public class InhalerLogic : Singleton<InhalerLogic>{
    public static EventHandler<EventArgs> OnGameOver; //Game over show game over message
    public static EventHandler<EventArgs> OnNextStep; //Completed one step, so move on
    public const int RESCUE_NUM_STEPS = 7;
    private int currentStep = 1; //current step that user is on

    /*
        call the api in this order
        1)IsCurrentStepCorrect
        2)NextStep
    */

    //return the current step of a sequence
    public int CurrentStep{
        get{return currentStep;}
    }

    public bool IsFirstTimeRescue{
        get{return DataManager.Instance.GameData.Inhaler.FirstTimeRescue;}
        set{DataManager.Instance.GameData.Inhaler.FirstTimeRescue = value;}
    }

    public bool IsTutorialCompleted{
        get{return DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TutorialManager_Bedroom.TUT_INHALER);}
    }

    //True: the step that the user is currently on is correct, False: wrong step
    public bool IsCurrentStepCorrect(int step){
        bool retVal = step == currentStep;
        return retVal; 
    }

    //Use this function to move on to the next step
    public void NextStep(){
        if(IsDoneWithGame())
            GameDone();

        //Send analytics event
        Analytics.Instance.InhalerSwipeSequences(Analytics.STEP_STATUS_COMPLETE, currentStep);

        currentStep++;

        if(D.Assert(OnNextStep != null, "OnNextStep has no listeners"))
            OnNextStep(this, EventArgs.Empty);
    } 

    public void ResetGame(){
        currentStep = 1;
    }

	//---------------------------------------------------
	// GameDone()
	// Put anything in here that should happen as a result
	// of the pet using the daily inhaler.
	//---------------------------------------------------		
	private void GameDone() {		
		// play game over sound
		AudioManager.Instance.PlayClip( "inhalerDone" );
        IsFirstTimeRescue = false;

        if(OnGameOver != null)
            OnGameOver(this, EventArgs.Empty);

        //finish inhaler tutorial 
        if(!IsTutorialCompleted)
            DataManager.Instance.GameData.Tutorial.ListPlayed.Add( TutorialManager_Bedroom.TUT_INHALER );
		
		// send out a task completion event for the wellapad
		WellapadMissionController.Instance.TaskCompleted( "DailyInhaler" );

        // Check for badge reward
        BadgeLogic.Instance.CheckSeriesUnlockProgress(BadgeType.Inhaler, 1, false);
		
		// calculate the next play period for the inhaler
		PlayPeriodLogic.Instance.CalculateNextPlayPeriod();
	}	
	
    //---------------------------------------------------       
    // IsDoneWithGame()
    // True: done with the game , False: have more steps to go
    //---------------------------------------------------       
    private bool IsDoneWithGame(){
        return currentStep == RESCUE_NUM_STEPS;
    }
}
