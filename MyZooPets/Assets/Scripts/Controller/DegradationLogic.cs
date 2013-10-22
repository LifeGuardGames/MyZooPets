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
	
	// tut key
	public static string TIME_DECAY_TUT = "TimeMoodDecay";

    public List<Location> triggerLocations = new List<Location>(); //a list of predefined locations
    public List<GameObject> triggerPrefabs = new List<GameObject>(); //list of trigger objects
	
	public int nPoints;

	// --- mood related degradation variables
	// if the pet's health is below this value, mood effects are doubled
	public float fHealthMoodThreshold;
	public float fFirstHoursPenalty;
	public float fSecondHoursPenalty;

    private float timer = 0;
    private float timeInterval = 5f; //time interval for trigger to affect health
    private const int NUMBER_OF_LOC = 6;
    private const int NUMBBER_OF_PREFABS = 6;

    public List<DegradData> DegradationTriggers{
        get{return DataManager.Instance.GameData.Degradation.DegradationTriggers;}
    }
    public List<Location> TriggerLocations{
        get{return triggerLocations;}
    }
    public List<GameObject> TriggerPrefabs{
        get{return triggerPrefabs;}
    }

    void Awake(){
        DateTime now = DateTime.Now;
        TimeSpan sinceLastPlayed = now.Date - DataManager.Instance.GameData.Degradation.LastTimeUserPlayedGame.Date;
        int numberOfTriggersToInit = 0;

        if(sinceLastPlayed.Days > 0){ //reset if new day
            DataManager.Instance.GameData.Degradation.MorningTrigger = true;
            DataManager.Instance.GameData.Degradation.AfternoonTrigger = true;
        }
        if(now.Hour > 12){ //morning
            if(DataManager.Instance.GameData.Degradation.MorningTrigger){
                numberOfTriggersToInit = 3;
                DataManager.Instance.GameData.Degradation.MorningTrigger = false;
            }
        }else{ //afternoon
            if(DataManager.Instance.GameData.Degradation.AfternoonTrigger){
                numberOfTriggersToInit = 3; 
                DataManager.Instance.GameData.Degradation.AfternoonTrigger = false;
            }
        }
		
		// calculate changes in the pets mood
		Debug.Log("About to calc mood loss...what's this?: " + sinceLastPlayed);
		StartCoroutine( CalculateMoodDegradation( sinceLastPlayed ) );

        //create triggers
        for(int i=0; i<numberOfTriggersToInit; i++){
            //don't add anymore triggers if there are already 6
            if(DataManager.Instance.GameData.Degradation.DegradationTriggers.Count == NUMBER_OF_LOC) break;

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
            DataManager.Instance.GameData.Degradation.DegradationTriggers.Add(new DegradData(i, locationIndex, objectIndex));
            
        }                

        DataManager.Instance.GameData.Degradation.LastTimeUserPlayedGame = DateTime.Now; //update last played time         
    }
		
	//---------------------------------------------------
	// CalculateMoodDegradation()
	// Depending on how long it has been since the user
	// last played, the pet will suffer some mood loss.
	//---------------------------------------------------	
	private IEnumerator CalculateMoodDegradation( TimeSpan timeSinceLastPlayed ) {
		// wait a frame, or else the notification manager won't work properly
		yield return 0;
		
		// amount to degrade mood by
		int nMoodLoss = 0;
		
		// get the pet's health %, because it affects how their mood changes
		float fHP = (float) ( DataManager.Instance.GameData.Stats.Health / 100.0f );
		float fMultiplier = 1;
		if ( fHP < fHealthMoodThreshold )
			fMultiplier = 2;
		
		// first part of the mood degradation -- the first 24 hours of not playing
		int nFirstHours = timeSinceLastPlayed.TotalHours > 24 ? 24 : (int) timeSinceLastPlayed.TotalHours;
		if ( nFirstHours > 0 )
			nMoodLoss += (int)( nFirstHours * ( fFirstHoursPenalty * fMultiplier ) );
		
		// second part of mood degradation -- anything after 24 hours of not playing
		int nSecondHours = (int)( timeSinceLastPlayed.TotalHours - 24 );
		if ( nSecondHours > 0 )
			nMoodLoss += (int) ( nSecondHours * ( fSecondHoursPenalty * fMultiplier ) );

		// actually change the pet's mood
		StatsController.Instance.ChangeStats(0, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero, -nMoodLoss, Vector3.zero);
		
		// if the player actually lost some mood, check and show the mood loss tutorial (if appropriate)
		if ( nMoodLoss > 0 && !DataManager.Instance.GameData.Tutorial.ListPlayed.Contains( TIME_DECAY_TUT ) )
			TutorialUIManager.Instance.StartTimeMoodDecayTutorial();
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
        DegradData degradData = DataManager.Instance.GameData.Degradation.DegradationTriggers.Find(x => x.ID == id);
        if(degradData != null)
            triggerPos = triggerLocations[degradData.PositionId].position;
        
		StatsController.Instance.ChangeStats(nPoints, UIUtility.Instance.mainCameraWorld2Screen(triggerPos), 
            50, UIUtility.Instance.mainCameraWorld2Screen(triggerPos), 0, Vector3.zero, 0, Vector3.zero);
        DataManager.Instance.GameData.Degradation.DegradationTriggers.Remove(degradData);
    }

    //Calculate health degration
    private void TriggerDegradesHealth(){
        int triggerCount = DataManager.Instance.GameData.Degradation.DegradationTriggers.Count;
        int minusHealth = 2;
        Debug.Log("degrad health");

        minusHealth = minusHealth * triggerCount; 
        StatsController.Instance.ChangeStats(0, Vector3.zero, 0, Vector3.zero, minusHealth * -1, Vector3.zero, 0, Vector3.zero);	// Convert to negative
    }

}
