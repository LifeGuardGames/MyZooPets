using UnityEngine;
using System.Collections;
using System;

//InhalerLogic stores and manipulate any inhaler game related data
//TO DO: reward algorithm for optimal window and practice inhaler game
public static class InhalerLogic{
    //variables
    // currentInhalerSkin;

    private static InhalerType currentInhalerType; //either Advair or Rescue inhaler
    private static int currentStep; //current step that user is on
    // private static bool isOptimalTime; //True: allow user to plan high reward inhaler game,
                                         //False: can only plan practice game
    private static TimeSpan optimalTimeWindow; //how long does the optimal window lasts
    private static bool canPlayGame; //true: play game, false: exit game
    private static bool isPracticeGame; //true: practice game less reward,
                                        //false: real game more reward.

    //=================API (use this for UI)==================
    /*
        call the api in this order
        1)Init(true);
        2)CanPlayGame
        3)IsCurrentStepCorrect
        4)IsDoneWithGame
        5)NextStep
    */
    //return the current step of a sequence
    public static int CurrentStep{
        get{return currentStep;}
    }

    //return the current type of the inhaler
    public static InhalerType CurrentInhalerType{
        get{return currentInhalerType;}
    }

    // //return slot machine count
    // public static int GetSlotMachineCount{
    //     get{return DataManager.SlotMachineCounter;}
    // }

    //can user play game? True: continue w game, False: prompt user to exit
    public static bool CanPlayGame{
        get{return canPlayGame;}
    }

    //true: practice game less reward, false: real game more reward.
    public static bool IsPracticeGame{
        get{return isPracticeGame;}
    }

    //Initialize game data and reset counters if it's a new day
    public static void Init(bool isPractice){
        canPlayGame = false;
        isPracticeGame = isPractice;
        if(isPracticeGame){ //practice inhaler game (teddy bear)
            canPlayGame = true;

        }else{ //regular inhaler game
            optimalTimeWindow = new TimeSpan(3, 0, 0); //optimal window lasts 3 hrs

            DateTime now = DateTime.Now;
            TimeSpan sinceLastPlayed = now.Date.Subtract(DataManager.LastInhalerGamePlayed.Date);
            DataManager.LastInhalerGamePlayed = now;

            //new day so resets counters
            if(sinceLastPlayed.Days > 0){
                DataManager.CanPlayGameMorning = true;
                DataManager.CanPlayGameAfternoon = true;
            }

            //check if time now is within the optimal time range
            // < 0 t1 is earlier than t2
            // 0 t1 is the same as t2
            // > 0 t1 later than t2
            Debug.Log("Optimal morning period is from " + DataManager.OptimalMorningStartTime + " to " + DataManager.OptimalMorningStartTime + optimalTimeWindow);
            Debug.Log("Optimal afternoon period is from " + DataManager.OptimalAfternoonStartTime + " to " + DataManager.OptimalAfternoonStartTime + optimalTimeWindow);
            int startResult = DateTime.Compare(now, DataManager.OptimalMorningStartTime);
            int endResult = DateTime.Compare(now, DataManager.OptimalMorningStartTime + optimalTimeWindow);
            if(startResult >= 0 && endResult <= 0){
                if(DataManager.CanPlayGameMorning){
                    canPlayGame = true;
                    DataManager.CanPlayGameMorning = false; //can only play optimal game once
                }
            }else if(now >= DataManager.OptimalAfternoonStartTime &&
                    now <= (DataManager.OptimalAfternoonStartTime + optimalTimeWindow)){
                if(DataManager.CanPlayGameAfternoon){
                    canPlayGame = true;
                    DataManager.CanPlayGameAfternoon = false; //can only play optimal game once
                }
            }else{}

        }

        int randomId = UnityEngine.Random.Range(0, 2);
        switch(randomId){
            case 0: currentInhalerType = InhalerType.Advair; break;
            case 1: currentInhalerType = InhalerType.Rescue; break;
        }
        //sets step to 1
        currentStep = 1;
    }

    //True: the step that the user is currently on is correct, False: wrong step
    public static bool IsCurrentStepCorrect(int step){
        return step == currentStep;
    }

    //use this method to tell logic that the user has successfully finished the current step
    //so move on to the next step of the sequence
    //True: done with the game , False: have more steps to go
    public static bool IsDoneWithGame(){
        bool retVal = false;

        if(currentInhalerType == InhalerType.Advair && currentStep == 6){
            retVal = true; //end of the sequence

        } else if(currentInhalerType == InhalerType.Rescue && currentStep == 7){
            retVal = true; //end of the sequence
        }
        return retVal;
    }

    //use this function to move on to the next step
    public static void NextStep(){
        currentStep++;
    }
    //================================================
}
