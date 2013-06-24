using UnityEngine;
using System.Collections;

public class SlotMachineManager : MonoBehaviour {
    private const int NUMBER_OF_SLOTS = 5;
    private const float SLOT_OFFSET = 0.2f;

    private float[] chosenSlots = new float[3]; //the position offsite for 3 wheels
    private int[] slots = new int[3];
    private Transform[] wheels = new Transform[3]; //reference to the 3 wheels inside slot machine
    private bool gameOver = false;

    void Awake(){
        int counter = 0;

        foreach(Transform wheel in transform){ //obtain reference to 3 spinning wheels
            wheels[counter] = wheel;
            counter++;
        }
    }

	// Use this for initialization
	void Start () {
        StartGame();
	}

	// Update is called once per frame
	void Update () {
        if(!gameOver){ //keep checking if wheels have been spinned if game is not over
            if(wheels[0].GetComponent<SpinningWheel>().doneSpinning &&
                wheels[1].GetComponent<SpinningWheel>().doneSpinning &&
                wheels[2].GetComponent<SpinningWheel>().doneSpinning){
                if(SpinEndCallBack != null) SpinEndCallBack();
                gameOver = true;
            }
        }
	}

    //Generate the random slots and spin the wheels
    private void StartGame(){
        for(int i = 0; i<3; i++){
            slots[i] = Random.Range(0, NUMBER_OF_SLOTS-1);
            chosenSlots[i] = SLOT_OFFSET * (float)slots[i]; //calculate the offset for the wheels
            wheels[i].GetComponent<SpinningWheel>().StartSpin(chosenSlots[i], i);    
        }
    }

    //check if the slots are 3 in a row
    public bool CheckMatch(){
        return slots[0] == slots[1] && slots[0] == slots[2] && slots[1] == slots[2];
    }

    //Notify other classes that the spinning are finished
    public delegate void SpinningCallBack();
    public SpinningCallBack SpinEndCallBack;
}
