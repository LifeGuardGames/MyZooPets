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
    //====================Events==============================
    public class TriggerDestroyedEventArgs : EventArgs{ //arguments that will be passed to Event Handler
        public Vector3 TriggerPosition {get; set;}
    }
    public static event EventHandler<TriggerDestroyedEventArgs> OnTriggerDestroyed;
    public static event EventHandler<EventArgs> OnTriggerAffectsHealth;
    //=====================================================

    public List<DegradData> DegradationTriggers{
        get{return DataManager.DegradationTriggers;}
    }
    public List<Location> triggerLocations = new List<Location>(); //a list of predefined locations
    public List<GameObject> triggerPrefabs = new List<GameObject>(); //list of trigger objects

    private float timer = 0;
    private float timeInterval = 30f; //time interval for trigger to affect health
    private const int NUMBER_OF_LOC = 6;
    private const int NUMBBER_OF_PREFABS = 6;

    void Awake(){
        DateTime now = DateTime.Now;
        TimeSpan sinceLastPlayed = now.Date - DataManager.LastTimeUserPlayedGame.Date;
        int numberOfTriggersToInit = 0;;

        if(sinceLastPlayed.Days > 0){ //reset if new day
            DataManager.MorningTrigger = true;
            DataManager.AfternoonTrigger = true;
        }
        if(now.Hour > 12){ //morning
            if(DataManager.MorningTrigger){
                numberOfTriggersToInit = 3;
                DataManager.MorningTrigger = false;
            }
        }else{ //afternoon
            if(DataManager.AfternoonTrigger){
                numberOfTriggersToInit = 3; 
                DataManager.AfternoonTrigger = false;
            }

        }

        //create triggers
        for(int i=0; i<numberOfTriggersToInit; i++){
            //don't add anymore triggers if there are already 6
            if(DataManager.DegradationTriggers.Count == NUMBER_OF_LOC) break;

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
            DataManager.DegradationTriggers.Add(new DegradData(i, locationIndex, objectIndex));
            
        }                

        DataManager.LastTimeUserPlayedGame = DateTime.Now; //update last played time         
    }

    void Update(){
        timer -= Time.deltaTime;
        if (timer <= 0){
            TriggerDegradesHealth();
            timer = timeInterval;
        }
    }

    //use the method when a trigger has been destroyed by user
    public void ClearDegradationTrigger(int id){
		Vector3 triggerPos = Vector3.zero;
        DegradData degradData = DataManager.DegradationTriggers.Find(x => x.ID == id);
        if(OnTriggerDestroyed != null){ //call event handler if not empty
            TriggerDestroyedEventArgs args = new TriggerDestroyedEventArgs();
            args.TriggerPosition = triggerLocations[degradData.PositionId].position;
			triggerPos = args.TriggerPosition;
        }else{
            Debug.LogError("Trigger Destroyed listener is null");
        }
		StatsController.Instance.ChangeStats(250, UIUtility.Instance.mainCameraWorld2Screen(triggerPos), 50, UIUtility.Instance.mainCameraWorld2Screen(triggerPos), 0, Vector3.zero, 0, Vector3.zero);
        DataManager.DegradationTriggers.Remove(degradData);
    }

    //triggers decreases health every 30 sec
    private void TriggerDegradesHealth(){
        int triggerCount = DataManager.DegradationTriggers.Count;
        int minusHealth = 0;

        int additionalTrigger = triggerCount - 3;
        if(additionalTrigger >= 0){ //3 or more triggers. add health punishments
            minusHealth = 10 + additionalTrigger * 10;
        }

        StatsController.Instance.ChangeStats(0, Vector3.zero, 0, Vector3.zero, minusHealth * -1, Vector3.zero, 0, Vector3.zero);	// Convert to negative
    }

}
