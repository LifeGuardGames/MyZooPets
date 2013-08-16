using UnityEngine;
using System.Collections;
using System;

//InhalerLogic stores and manipulate any inhaler game related data
public class InhalerLogic : Singleton<InhalerLogic>{
    private InhalerType currentInhalerType; //either Advair or Rescue inhaler
    private int currentStep; //current step that user is on
    private bool canPlayGame; //true: play game, false: exit game
    private bool isPracticeGame; //true: practice game less reward,
                                        //false: real game more reward.
    //====================Events============================
    public static EventHandler<EventArgs> OnGameOver; //Game over show game over message
    public static EventHandler<EventArgs> OnNextStep; //Completed one step, so move on
    public static EventHandler<EventArgs> OnWrongStep; //User did not follow the correct swipe sequence

    //=================API (use this for UI)==================
    /*
        call the api in this order
        1)IsCurrentStepCorrect
        2)NextStep
    */
    public const int ADVAIR_NUM_STEPS = 6;
    public const int RESCUE_NUM_STEPS = 7;

    //return the current step of a sequence
    public int CurrentStep{
        get{return currentStep;}
    }

    //return the current type of the inhaler
    public InhalerType CurrentInhalerType{
        get{return currentInhalerType;}
    }

    //true: practice game less reward, false: real game more reward.
    public bool IsPracticeGame{
        get{return isPracticeGame;}
    }

    //True: the step that the user is currently on is correct, False: wrong step
    public bool IsCurrentStepCorrect(int step){
        bool retVal = step == currentStep;
        if(!retVal){
            // if(D.Assert(OnWrongStep != null, "OnWrongStep has no listeners"))
            //     OnWrongStep(this, EventArgs.Empty);
        }
        //TO DO: Add game analytics here
        /*
            GA.API.Design.NewEvent("InhalerGame:" + currentInhalerType + ":" + currentStep + ":" + wrongstep);
        */
        return retVal; 
    }

    //Use this function to move on to the next step
    public void NextStep(){
        GA.API.Design.NewEvent("InhalerGame:" + Enum.GetName(typeof(InhalerType), currentInhalerType) + 
            ":" + currentStep + ":Correct");
        currentStep++;
        if(D.Assert(OnNextStep != null, "OnNextStep has no listeners"))
                OnNextStep(this, EventArgs.Empty);
        if(IsDoneWithGame()){ //Fire GameOver event if game is done
            if(D.Assert(OnGameOver != null, "OnGameOver has no listeners"))
                OnGameOver(this, EventArgs.Empty);
        }
    }
    //======================================================
    public void ResetGame(){
        switch(Application.loadedLevelName){
            case "InhalerGameTeddy":
                canPlayGame = true;

                int randomId = UnityEngine.Random.Range(0, 2);
                switch(randomId){
                    case 0: currentInhalerType = InhalerType.Advair; break;
                    case 1: currentInhalerType = InhalerType.Rescue; break;
                }
                isPracticeGame = true;
            break;
            case "InhalerGamePet":
                currentInhalerType = InhalerType.Advair;
                isPracticeGame = false;
            break;
        }
        currentStep = 1;
    }

    /*
        Ending sequence is one more than the total number of sequences
        True: done with the game , False: have more steps to go
    */
    private bool IsDoneWithGame(){
        bool retVal = false;
        if(currentInhalerType == InhalerType.Advair && currentStep == ADVAIR_NUM_STEPS){
            retVal = true; //end of the sequence
        }else if(currentInhalerType == InhalerType.Rescue && currentStep == RESCUE_NUM_STEPS){
            retVal = true; //end of the sequence
        }
        return retVal;
    }

    // //Initialize game data and reset counters if it's a new day
    // public void Init(bool isPractice){
    //     canPlayGame = false;
    //     isPracticeGame = isPractice;
    //     if(isPracticeGame){ //practice inhaler game (teddy bear)
            
    //     }else{ //regular inhaler game
            
    //         DateTime now = DateTime.Now;
    //         TimeSpan sinceLastPlayed = now.Date.Subtract(DataManager.Instance.Inhaler.LastInhalerPlayTime.Date);
    //         DataManager.Instance.Inhaler.LastInhalerPlayTime = now;

    //         if(sinceLastPlayed.Days > 0){ //reset if new day
    //             DataManager.Instance.Inhaler.PlayedInMorning = false;
    //             DataManager.Instance.Inhaler.PlayedInAfternoon = false;
    //         }
    //         if(now.Hour < 12){ //morning
    //             if(!DataManager.Instance.Inhaler.PlayedInMorning){ //can only play if not played in morning yet
    //                 canPlayGame = true;
    //                 DataManager.Instance.Inhaler.PlayedInMorning = true;
    //             }
    //         }else{ //afternoon
    //             if(!DataManager.Instance.Inhaler.PlayedInAfternoon){ //can only play if not played in afternoon yet
    //                 canPlayGame = true;
    //                 DataManager.Instance.Inhaler.PlayedInAfternoon = true;
    //             }
    //         }
    //         currentInhalerType = InhalerType.Advair;
    //     }
        
    //     //sets step to 1
    //     currentStep = 1;
    // }
}
