using UnityEngine;
using System.Collections;

public static class SlotMachineLogic{
    private const int NUMBER_OF_SLOTS = 5;
    private const float SLOT_OFFSET = 0.2f;
    private static float chosenSlot1;
    private static float chosenSlot2;
    private static float chosenSlot3;
    private static int slot1;
    private static int slot2;
    private static int slot3;

    public static void Init(){
        slot1 = Random.Range(1, NUMBER_OF_SLOTS);
        slot2 = Random.Range(1, NUMBER_OF_SLOTS);
        slot3 = Random.Range(1, NUMBER_OF_SLOTS);
        chosenSlot1 = SLOT_OFFSET * slot1;
        chosenSlot2 = SLOT_OFFSET * slot2; 
        chosenSlot3 = SLOT_OFFSET * slot3; 
    }

    public static bool CheckMatch(){
        return slot1 == slot2 && slot1 == slot3 && slot2 == slot3;
    }	
}
