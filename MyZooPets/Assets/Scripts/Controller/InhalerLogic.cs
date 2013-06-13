using UnityEngine;
using System.Collections;
using System;

//InhalerLogic stores and manipulate any inhaler game related data 
public static class InhalerLogic{
    //variables
    // currentInhalerSkin;

    private static InhalerType currentInhalerType; //either Advair or Rescue inhaler
    private static int currentStep; //current step that user is on


    //=================API (use this for UI)==================
    //return the current step of a sequence
    public static int CurrentStep{
        get{return currentStep;}
    }

    //return the current type of the inhaler
    public static InhalerType CurrentInhalerType{
        get{return currentInhalerType;}
    }

    //return slot machine count
    public static int GetSlotMachineCount{
        get{return DataManager.SlotMachineCounter;}
    }

    //Initialize game data and reset counters if it's a new day
    public static void Init(){
        DateTime now = DateTime.Now;
        TimeSpan sinceLastPlayed = now.Date.Subtract(DataManager.LastInhalerGamePlayed.Date);
        DataManager.LastInhalerGamePlayed = now;

        //new day so resets counters
        if(sinceLastPlayed.Days > 0){
            DataManager.SlotMachineCounter = 0;
            DataManager.NumberOfTimesPlayed = 6;
            DataManager.AdvairCount = 3;
            DataManager.RescueCount = 3;
        }

        //assign game inhaler
        if(DataManager.AdvairCount >= DataManager.RescueCount){
            currentInhalerType = InhalerType.Advair;
        }else{
            currentInhalerType = InhalerType.Rescue;
        }
        //sets step to 1
        currentStep = 1;

        //resets slot machine if played already
        if(DataManager.SlotMachineCounter == 3) DataManager.SlotMachineCounter = 0;
    }

    //True: user can play the inhaler game, False: user has reached the max play time
    //so can't play the game
    public static bool PlayGame(){
        return DataManager.NumberOfTimesPlayed != 0;
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

    //adjust counters at the end of the game
    public static void ResetGame(){
        if(currentInhalerType == InhalerType.Advair && currentStep > 5){
            DataManager.AdvairCount--;
        }else if(currentInhalerType == InhalerType.Rescue && currentStep > 6){
            DataManager.RescueCount--;
        }
        DataManager.NumberOfTimesPlayed--;
        DataManager.SlotMachineCounter++;
    }

    //use this function to move on to the next step
    public static void NextStep(){
        currentStep++;
    }
    //================================================
}
