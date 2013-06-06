using UnityEngine;
using System.Collections;

public class InhalerTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Init(){
        //Test1 going through Advair sequence
        Debug.Log("Test1: going through Advair sequence once");
        InhalerLogic.Init();
        if(InhalerLogic.PlayGame()){
            SimulatePerfectGame(1);
        }
        Debug.Log("===============");

        //Test2 go through 6 sequences
        Debug.Log("Test2: play 6 successful games");
        ResetCounters();
        for(int i=1; i<=6; i++){
            InhalerLogic.Init();
            Debug.Log("inhaler type: " + InhalerLogic.CurrentInhalerType);
            if(InhalerLogic.PlayGame()){

                SimulatePerfectGame(i);
            }
        }        
        Debug.Log("===============");

        //Test3 go over daily play max
        Debug.Log("Test3: go over daily play max");
        ResetCounters();
        for(int i=1; i<=7; i++){
            InhalerLogic.Init();
            if(InhalerLogic.PlayGame()){

                SimulatePerfectGame(i);
            }else{
                Debug.Log("******* over daily max");
            }
        }

        //Test4 missing steps
        Debug.Log("Test4: missing steps");
        ResetCounters();
        InhalerLogic.Init();
        if(InhalerLogic.PlayGame()){
            for(int i=1; i<=5; i++){
                bool result = InhalerLogic.IsCurrentStepCorrect(3);
                Debug.Log("Step " + i + ": " + result);
            }
        }
    }

    private void ResetCounters(){
        DataManager.SlotMachineCounter = 0;
        DataManager.AdvairCount = 3;
        DataManager.RescueCount = 3;
        DataManager.NumberOfTimesPlayed = 6;
    }

    private void SimulatePerfectGame(int numOfTimes){
        int numOfSteps;
        if(numOfTimes%2 == 0){
            numOfSteps = 6; //rescue
        }else{
            numOfSteps = 5; //advair
        }
        for(int i=1; i<= numOfSteps ; i++){
            bool result = InhalerLogic.IsCurrentStepCorrect(i);
            Debug.Log("Step " + i + ": " + result);

            if(InhalerLogic.IsDoneWithGame()){
                
                Debug.Log("slot machine count: " + InhalerLogic.GetSlotMachineCount);
            }
        }
    }
}
