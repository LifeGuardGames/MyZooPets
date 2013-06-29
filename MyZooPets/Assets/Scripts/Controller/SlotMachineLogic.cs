using UnityEngine;
using System.Collections;

public static class SlotMachineLogic{
    private const int NUMBER_OF_SLOTS = 5;
    private const float SLOT_OFFSET = 0.2f;
    private static float[] chosenSlots = new float[3]; //the position offsite for 3 wheels
    private static int[] slots = new int[3];

    //getter & setters
    public static bool GameOver{get; set;} //is slot machine game done? 
    public static float[] ChosenSlots{
        get{return chosenSlots;}
    }

    //call back
    public delegate void SpinningCallBack();
    public static SpinningCallBack SpinEndCallBack; //notify UI when spinning is done

    //Generate the random slots and spin the wheels
    public static void GenerateRandomSlots(){
        for(int i=0; i<3; i++){
            slots[i] = Random.Range(0, NUMBER_OF_SLOTS-1);
            chosenSlots[i] = SLOT_OFFSET * (float)slots[i]; //calculate the offset for the wheels
        }
    }

    //check if the slots are 3 in a row
    public static bool CheckMatch(){
        return slots[0] == slots[1] && slots[0] == slots[2] && slots[1] == slots[2];
    }	
}
