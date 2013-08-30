using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//Logic for the daily maintenance system that drives retention
//and daily check-ins. 
//TO DO: need to store diff types of trigger and distinct between room and yard
public class DegradationLogic : Singleton<DegradationLogic> {
    public static event EventHandler<EventArgs> OnTriggerAffectsHealth;
    
    [System.Serializable]
    public class Location{ //make it serializable 
        public bool isTaken; //if the position has been taken or not
        public Vector3 position; //the position of the degradation trigger
        
        public Location(bool isTaken, Vector3 position){
            this.isTaken = isTaken;
            this.position = position;
        }
    }

    public List<Location> triggerLocations = new List<Location>(); //a list of predefined locations
    public List<GameObject> triggerPrefabs = new List<GameObject>(); //list of trigger objects

    private float timer = 0;
    private float timeInterval = 10f; //time interval for trigger to affect health
    private const int NUMBER_OF_LOC = 6;
    private const int NUMBBER_OF_PREFABS = 6;

    public List<DegradData> DegradationTriggers{
        get{return DataManager.Instance.Degradation.DegradationTriggers;}
    }
    public List<Location> TriggerLocations{
        get{return triggerLocations;}
    }
    public List<GameObject> TriggerPrefabs{
        get{return triggerPrefabs;}
    }

    void Awake(){
        DateTime now = DateTime.Now;
        TimeSpan sinceLastPlayed = now.Date - DataManager.Instance.Degradation.LastTimeUserPlayedGame.Date;
        int numberOfTriggersToInit = 0;;

        if(sinceLastPlayed.Days > 0){ //reset if new day
            DataManager.Instance.Degradation.MorningTrigger = true;
            DataManager.Instance.Degradation.AfternoonTrigger = true;
        }
        if(now.Hour > 12){ //morning
            if(DataManager.Instance.Degradation.MorningTrigger){
                numberOfTriggersToInit = 3;
                DataManager.Instance.Degradation.MorningTrigger = false;
            }
        }else{ //afternoon
            if(DataManager.Instance.Degradation.AfternoonTrigger){
                numberOfTriggersToInit = 3; 
                DataManager.Instance.Degradation.AfternoonTrigger = false;
            }
        }

        //create triggers
        for(int i=0; i<numberOfTriggersToInit; i++){
            //don't add anymore triggers if there are already 6
            if(DataManager.Instance.Degradation.DegradationTriggers.Count == NUMBER_OF_LOC) break;

            //random location and prefab
            int locationIndex = UnityEngine.Random.Range(0, NUMBER_OF_LOC);
            int objectIndex = UnityEngine.Random.Range(0, NUMBBER_OF_PREFABS);

            Location triggerLocation = triggerLocations[locationIndex];
            if(triggerLocation.isTaken){ //if spot is already taken find the next empty in the list
                locationIndex = triggerLocations.FindIndex(x => x.isTaken == false);
            }
            triggerLocation.isTaken = true;
            
            //spawn them at a pre define location
            //ID is the order in which the data are created
            DataManager.Instance.Degradation.DegradationTriggers.Add(new DegradData(i, locationIndex, objectIndex));
            
        }                

        DataManager.Instance.Degradation.LastTimeUserPlayedGame = DateTime.Now; //update last played time         
    }

    void Update(){
        if(TutorialLogic.Instance.FirstTimeDegradTrigger) return; //no degradation during tutorial phase
        timer -= Time.deltaTime;
        if (timer <= 0){
            TriggerDegradesHealth();
            timer = timeInterval;
        }
    }

    //use the method when a trigger has been destroyed by user
    public void ClearDegradationTrigger(int id){
		Vector3 triggerPos = Vector3.zero;
        DegradData degradData = DataManager.Instance.Degradation.DegradationTriggers.Find(x => x.ID == id);
        triggerPos = triggerLocations[degradData.PositionId].position;

		StatsController.Instance.ChangeStats(250, UIUtility.Instance.mainCameraWorld2Screen(triggerPos), 
            50, UIUtility.Instance.mainCameraWorld2Screen(triggerPos), 0, Vector3.zero, 0, Vector3.zero);
        DataManager.Instance.Degradation.DegradationTriggers.Remove(degradData);
    }

    //triggers decreases health every 30 sec
    private void TriggerDegradesHealth(){
        int triggerCount = DataManager.Instance.Degradation.DegradationTriggers.Count;
        int minusHealth = 0;

        int additionalTrigger = triggerCount - 3;
        if(additionalTrigger >= 0){ //3 or more triggers. add health punishments
            minusHealth = 10 + additionalTrigger * 10;
        }

        StatsController.Instance.ChangeStats(0, Vector3.zero, 0, Vector3.zero, minusHealth * -1, Vector3.zero, 0, Vector3.zero);	// Convert to negative
    }

}
