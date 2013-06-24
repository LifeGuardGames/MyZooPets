using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//Logic for the daily maintenance system that drives retention
//and daily check-ins. 
//TO DO: need to store diff types of trigger and distinct between room and yard
public class DegradationLogic : MonoBehaviour {
    public bool isDebug = false;
    public GameObject degradTest;

    void Start(){
        if(isDebug){
            DataManager.LastTimeUserPlayedGame = new DateTime(2013, 6, 19);
            DataManager.DegradationTriggers = new List<DegradData>();
        }

        DateTime now = DateTime.Now;
        TimeSpan sinceLastPlayed = now.Date - DataManager.LastTimeUserPlayedGame.Date;
        int daysMissed = sinceLastPlayed.Days;
        int numberOfTriggersToInit = 0;;

        if(daysMissed > 1){
            if(daysMissed < 3){ //level 1 degradation
                numberOfTriggersToInit = 2;
            }else if(daysMissed >= 3 && daysMissed < 5){ //level 2 degradation
                numberOfTriggersToInit = 4;  
            }else if(daysMissed >= 5){ //level 3 degradation
                numberOfTriggersToInit = 6;
            }

            //create triggers
            for(int i=0; i<numberOfTriggersToInit; i++){
                //spawn them at a pre define location
                Vector3 position = new Vector3(i*3, 0, 0);
                DataManager.DegradationTriggers.Add(new DegradData(i, position)); 
            }                
        }
        //instantiate triggers in the game
        for(int i=0; i<DataManager.DegradationTriggers.Count; i++){
            //choose random trigger. using cube for testing
            GameObject trigger = (GameObject)Instantiate(degradTest, 
                DataManager.DegradationTriggers[i].Position, Quaternion.identity);
            trigger.GetComponent<DegradTriggerManager>().id = i;
        }

        DataManager.LastTimeUserPlayedGame = DateTime.Now;
    }

    //use the method when a trigger has been destroyed by user
    public static void ClearDegradationTrigger(int id){
        DegradData degradData = DataManager.DegradationTriggers[0];
        DataManager.DegradationTriggers.Remove(degradData);
    }

}
