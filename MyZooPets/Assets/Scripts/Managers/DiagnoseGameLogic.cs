using UnityEngine;
using System.Collections;

//This logic generates the current asthma stage of the game
//This logic will be used with DiagnoseUI
public static class DiagnoseGameLogic{

	public static AsthmaStage CurrentStage{get; set;}

    //check if the stage chosen by the user is correct
    public static bool IsThisStageCorrect(AsthmaStage asthmaStage){
        return asthmaStage.Equals(CurrentStage);
    }

    public static void Init(){
        GenerateRandomStage();
    }

    public static void ClaimReward(int deltaPoints, int deltaStars){
//		StatsController.Instance.ChangeStats(deltaPoints, Vector3.zero, deltaStars, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero);
    }

    //generate the sick stages randomly
    private static void GenerateRandomStage(){
        int randomNumber = Random.Range(0, 3);
        switch(randomNumber){
            case 0: CurrentStage = AsthmaStage.OK; break;
            case 1: CurrentStage = AsthmaStage.Sick; break;
            case 2: CurrentStage = AsthmaStage.Attack; break;
        }
    }
}
