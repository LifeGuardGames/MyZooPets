using UnityEngine;
using System.Collections;
using System;

//InhalerLogic stores and manipulate any inhaler game related data
public class InhalerLogic : Singleton<InhalerLogic>{
    public static EventHandler<EventArgs> OnGameOver; //Game over show game over message
    public static EventHandler<EventArgs> OnNextStep; //Completed one step, so move on

    public const int RESCUE_NUM_STEPS = 8;

    private InhalerType currentInhalerType; //either Advair or Rescue inhaler
    private int currentStep = 1; //current step that user is on
    private bool canPlayGame; //true: play game, false: exit game

    //=================API (use this for UI)==================
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
        get{return DataManager.Instance.Inhaler.FirstTimeRescue;}
        set{DataManager.Instance.Inhaler.FirstTimeRescue = value;}
    }

    //True: the step that the user is currently on is correct, False: wrong step
    public bool IsCurrentStepCorrect(int step){
        bool retVal = step == currentStep;
        // if(!retVal){
            // if(D.Assert(OnWrongStep != null, "OnWrongStep has no listeners"))
            //     OnWrongStep(this, EventArgs.Empty);
        // }
        //TO DO: Add game analytics here
        /*
            GA.API.Design.NewEvent("InhalerGame:" + currentInhalerType + ":" + currentStep + ":" + wrongstep);
        */
        return retVal; 
    }

    //Use this function to move on to the next step
    public void NextStep(){
        currentStep++;
        if(D.Assert(OnNextStep != null, "OnNextStep has no listeners"))
                OnNextStep(this, EventArgs.Empty);
        if(IsDoneWithGame()){ //Fire GameOver event if game is done
            IsFirstTimeRescue = false;
            CalendarLogic.Instance.RecordGivingInhaler(); 
            if(D.Assert(OnGameOver != null, "OnGameOver has no listeners"))
                OnGameOver(this, EventArgs.Empty);
        }
    }
    //======================================================

    public void ResetGame(){
        currentStep = 1;
    }

    /*
        Ending sequence is one more than the total number of sequences
        True: done with the game , False: have more steps to go
    */
    private bool IsDoneWithGame(){
        return currentStep == RESCUE_NUM_STEPS;
    }
}
