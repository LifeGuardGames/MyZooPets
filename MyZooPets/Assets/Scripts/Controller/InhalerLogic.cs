using UnityEngine;
using System.Collections;

public class InhalerGameLogic : MonoBehaviour {
    //variables
    // numberOfTimesPlayed;
    // slotMachineCounter;
    // lastTimePlayed;
    // inhalerType;
    // currentStep;
    // currentInhalerSkin;

    //#region API (use this for UI)
    //get the current step
    public static int CurrentStep{get;set;}

    //True: user can play the inhaler game, False: user has reached the max play time
    //so can't play the game
    public static bool canPlayGame(){

    };

    //True: user accumulated 3 correct sequences so can get random item from slot machine
    //game, False: don't start the slot machine game
    public static bool canPlaySlotMachine(){

    };

    //True: the step that the user is currently on is correct, False: wrong step 
    public static bool isCurrentStepCorrect(int step){

    };

    //use this method to tell logic that the user has successfully finished the current step
    //so move on to the next step of the sequence
    public static void completedCurrentStep(int step){

    };
    //#endregion

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
