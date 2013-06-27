using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//Logic for the daily maintenance system that drives retention
//and daily check-ins. 
//TO DO: need to store diff types of trigger and distinct between room and yard
public class DegradationLogic : MonoBehaviour {
    [System.Serializable]
    public class Location{ //make it serializable 
        public bool isTaken; //if the position has been taken or not
        public Vector3 position; //the position of the degradation trigger
        
        public Location(bool isTaken, Vector3 position){
            this.isTaken = isTaken;
            this.position = position;
        }
    }

    public bool isDebug = false; //developer option. force the trigger to show
    public List<Location> triggerLocations = new List<Location>(); //a list of predefined locations
    public List<GameObject> triggerPrefabs = new List<GameObject>(); //list of trigger objects

    public class TriggerDestroyedEventArgs : EventArgs{ //arguments that will be passed to Event Handler
        public Vector3 TriggerPosition {get; set;}
    }
    public delegate void TriggerDestroyEventHandler(object sender, TriggerDestroyedEventArgs e);
    public event TriggerDestroyEventHandler TriggerDestroyed;

    private const int NUMBER_OF_LOC = 6;
    private const int NUMBBER_OF_PREFABS = 6;

    public void Init(){
        if(isDebug){
            DataManager.LastTimeUserPlayedGame = new DateTime(2013, 6, 19);
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
                //random location and prefab
                int locationIndex = UnityEngine.Random.Range(0, NUMBER_OF_LOC);
                int objectIndex = UnityEngine.Random.Range(0, NUMBBER_OF_PREFABS);

                Location triggerLocation = triggerLocations[locationIndex];
                if(triggerLocation.isTaken){ //if spot is already taken find the next empty in the list
                    locationIndex = triggerLocations.FindIndex(x => x.isTaken == false);
                }
                triggerLocation.isTaken = true;
                
                //spawn them at a pre define location
                DataManager.DegradationTriggers.Add(new DegradData(i, locationIndex, objectIndex)); 
            }                
        }
        DataManager.LastTimeUserPlayedGame = DateTime.Now; //update last played time
    }

    //use the method when a trigger has been destroyed by user
    public void ClearDegradationTrigger(int id){
        DegradData degradData = DataManager.DegradationTriggers[0];
        if(TriggerDestroyed != null){ //call event handler if not empty
            TriggerDestroyedEventArgs args = new TriggerDestroyedEventArgs();
            args.TriggerPosition = triggerLocations[degradData.PositionId].position;
            TriggerDestroyed(this, args);
        }
        DataManager.DegradationTriggers.Remove(degradData);
    }

}
