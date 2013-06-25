using UnityEngine;
using System.Collections;

//This logic generates the current asthma stage of the game
//This logic will be used with DiagnoseUI
public class DiagnoseGameLogic : MonoBehaviour {

	public AsthmaStage CurrentStage{get; set;}

    //check if the stage chosen by the user is correct
    public bool IsThisStageCorrect(AsthmaStage asthmaStage){
        return asthmaStage.Equals(CurrentStage);
    }

    // Use this for initialization
    void Start () {
    
    }
    
    // Update is called once per frame
    void Update () {
    
    }

    private void GenerateRandomStage(){
        int randomNumber = Random.Range(0, 3);
        switch(randomNumber){
            case 0: CurrentStage = AsthmaStage.Discomfort; break;
            case 1: CurrentStage = AsthmaStage.Sick; break;
            case 2: CurrentStage = AsthmaStage.Attack; break;
        }
    }
}
