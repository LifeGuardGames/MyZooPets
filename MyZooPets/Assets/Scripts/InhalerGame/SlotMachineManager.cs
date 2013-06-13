using UnityEngine;
using System.Collections;

public class SlotMachineManager : MonoBehaviour {
    private const int NUMBER_OF_SLOTS = 5;
    private const float SLOT_OFFSET = 0.2f;

    private float[] chosenSlots = new float[3];
    private int[] slots = new int[3];

	// Use this for initialization
	void Start () {
	   // StartGa?me();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //Generate the random slots and spin the wheels
    public void StartGame(){
        int counter = 0;
        foreach(Transform wheel in transform){
            slots[counter] = Random.Range(0, NUMBER_OF_SLOTS-1);
            chosenSlots[counter] = SLOT_OFFSET * (float)slots[counter];
            wheel.GetComponent<SpinningWheel>().StartSpin(chosenSlots[counter]);
            counter++;
        } 
    }

    //check if the slots are 3 in a row
    private bool CheckMatch(){
        return slots[0] == slots[1] && slots[0] == slots[2] && slots[1] == slots[2];
    }
}
