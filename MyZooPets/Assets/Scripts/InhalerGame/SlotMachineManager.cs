using UnityEngine;
using System.Collections;

//TO DO: probably needs a Logic class
public class SlotMachineManager : MonoBehaviour {
    private const int NUMBER_OF_SLOTS = 5;
    private const float SLOT_OFFSET = 0.2f;

    private float[] chosenSlots = new float[3];
    private int[] slots = new int[3];
    private Transform[] wheels = new Transform[3];
    private bool gameOver = false;

    void Awake(){
        int counter = 0;
       foreach(Transform wheel in transform){
            wheels[counter] = wheel;
            counter++;
       }
    }
    
	// Use this for initialization
	void Start () {
       
	}

	// Update is called once per frame
	void Update () {
        if(!gameOver){ //keep checking if wheels have been speen if game is not over
            if(wheels[0].GetComponent<SpinningWheel>().doneSpinning &&
                wheels[1].GetComponent<SpinningWheel>().doneSpinning &&
                wheels[2].GetComponent<SpinningWheel>().doneSpinning){
                if(doneWithSpinningCallBack != null) doneWithSpinningCallBack();
                gameOver = true;
            }
        }
	}

    public bool SpinWhenReady(){
        bool retVal = false;
        //check counter
        if(InhalerLogic.GetSlotMachineCount <= 3){ //make sure not out of index
            for(int i = 0; i<InhalerLogic.GetSlotMachineCount; i++){
                wheels[i].Find("Screen").gameObject.SetActive(false);
            }
        }
        gameOver = false;
        if(InhalerLogic.GetSlotMachineCount == 3){
            StartGame();
            retVal = true;
        }
        return retVal;

    }

    //Generate the random slots and spin the wheels
    private void StartGame(){
        for(int i = 0; i<3; i++){
            slots[i] = Random.Range(0, NUMBER_OF_SLOTS-1);
            chosenSlots[i] = SLOT_OFFSET * (float)slots[i];
            wheels[i].GetComponent<SpinningWheel>().StartSpin(chosenSlots[i]);
        }
    }

    //check if the slots are 3 in a row
    private bool CheckMatch(){
        return slots[0] == slots[1] && slots[0] == slots[2] && slots[1] == slots[2];
    }

    //Notify other classes that the spinning are finished
    public delegate void DoneWithSpinningCallBack();
    public DoneWithSpinningCallBack doneWithSpinningCallBack;
}
