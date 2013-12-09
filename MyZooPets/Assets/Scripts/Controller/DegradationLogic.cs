using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//Logic for the daily maintenance system that drives retention
//and daily check-ins. 
//TO DO: need to store diff types of trigger and distinct between room and yard
public class DegradationLogic : Singleton<DegradationLogic> {
    public static event EventHandler<EventArgs> OnTriggerAffectsHealth;
	public event EventHandler<EventArgs> OnPetHit;
	
	// tut key
	public static string TIME_DECAY_TUT = "TimeMoodDecay";

    public List<GameObject> triggerPrefabs = new List<GameObject>(); //list of trigger objects
	
	public int nPoints;

	// --- mood related degradation variables
	// if the pet's health is below this value, mood effects are doubled
	public float fHealthMoodThreshold;
	public float fFirstHoursPenalty;
	public float fSecondHoursPenalty;
	
	private const int MAX_TRIGGERS = 6;

    public List<DegradData> DegradationTriggers{
        get{return DataManager.Instance.GameData.Degradation.DegradationTriggers;}
    }
    public List<GameObject> TriggerPrefabs{
        get{return triggerPrefabs;}
    }

    void Awake(){		
		// reset the degrad trigger list each time this logic runs, because we are no longer actually saving the triggers.
		// I (Joe) left this structure in because we might want to say something like, "X triggers remain" across all areas, or
		// something like that
		DataManager.Instance.GameData.Degradation.DegradationTriggers = new List<DegradData>();
		
        DateTime now = DateTime.Now;
        TimeSpan sinceLastPlayed = now.Date - DataManager.Instance.GameData.Degradation.LastTimeUserPlayedGame.Date;
        int numberOfTriggersToInit = 0;

        if(sinceLastPlayed.Days > 0){ //reset if new day
            DataManager.Instance.GameData.Degradation.MorningTrigger = true;
            DataManager.Instance.GameData.Degradation.AfternoonTrigger = true;
        }
        if( CalendarLogic.GetTimeFrame( now ) == TimeFrames.Morning ){ //morning
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
		StartCoroutine( CalculateMoodDegradation( sinceLastPlayed ) );
		
		// get list of available locations to spawn triggers
		List<Data_TriggerLocation> listAvailable = DataLoader_TriggerLocations.GetAvailableTriggerLocations( "Bedroom" );
		
		// get the number of triggers to spawn based on the previously uncleaned triggers and the new ones to spawn, with a max
		int numToSpawn = Mathf.Min( MAX_TRIGGERS, numberOfTriggersToInit + DataManager.Instance.GameData.Degradation.UncleanedTriggers );
		if ( numToSpawn < 0 ) {
			numToSpawn = 0;
			Debug.Log("Number of triggers to spawn somehow < 0...");
		}
		DataManager.Instance.GameData.Degradation.UncleanedTriggers = numToSpawn;
		
		List<Data_TriggerLocation> listChosen = ListUtils.GetRandomElements<Data_TriggerLocation>( listAvailable, numToSpawn );
		
        //create triggers
        for(int i = 0; i < listChosen.Count; i++){
			Data_TriggerLocation location = listChosen[i];
			
            // random prefab
            int objectIndex = UnityEngine.Random.Range(0, triggerPrefabs.Count);
			
			// to make things easier, if the user has not done the trigger tutorial yet, just override the random location and use 0
			// also, use the dust prefab...this is a soft setting...hopefully no one changes that array
			bool bTriggers = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains( TutorialManager_Bedroom.TUT_TRIGGERS );
			if ( !bTriggers ) {
				location = DataLoader_TriggerLocations.GetTriggerLocation( "TrigLoc_0", "Bedroom" );
				if ( location == null )
					Debug.Log("Tutorial trigger location not set up correctly");
				
				objectIndex = 3;
			}

            //spawn them at a pre define location
            //ID is the order in which the data are created
            DataManager.Instance.GameData.Degradation.DegradationTriggers.Add(new DegradData(i, location.GetPosition(), objectIndex));
            
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

    //use the method when a trigger has been destroyed by user
    public void ClearDegradationTrigger(DegradTrigger trigger){
		Vector3 triggerPos = trigger.gameObject.transform.position;
        DegradData degradData = DataManager.Instance.GameData.Degradation.DegradationTriggers.Find(x => x.ID == trigger.ID);
		
		// do a little magic here: get the world position of the trigger, turn that into screen coords, then take those coords (that are
		// BottomLeft NGUI coords) and turn them into NGUI top coords.
		Vector3 vTriggerPos = CameraManager.Instance.TransformAnchorPosition( CameraManager.Instance.WorldToScreen( CameraManager.Instance.cameraMain, triggerPos), InterfaceAnchors.BottomLeft, InterfaceAnchors.Top );
		
		StatsController.Instance.ChangeStats(nPoints, vTriggerPos, 50, vTriggerPos, 0, Vector3.zero, 0, Vector3.zero);
        DataManager.Instance.GameData.Degradation.DegradationTriggers.Remove(degradData);
		
		// subtract one from the triggers left to clean
		DataManager.Instance.GameData.Degradation.UncleanedTriggers -= 1;
		
		// if there are no degradation triggers left, send out a task completion message
		// note -- this will all probably have to change a bit as we get more complex (triggers in the yard, or other locations)
		if ( DataManager.Instance.GameData.Degradation.DegradationTriggers.Count == 0 )
			WellapadMissionController.Instance.TaskCompleted( "CleanRoom" );
    }
	
	//---------------------------------------------------
	// TriggerHitPet()
	// When a trigger particle effect hits the pet.
	//---------------------------------------------------		
	public void TriggerHitPet( DegradParticle trigger ) {
		// send out a callback
		if ( OnPetHit != null )
			OnPetHit( this, EventArgs.Empty );
		
		// damage the pet
		int nDamage = trigger.GetDamage();
		StatsController.Instance.ChangeStats(0, Vector3.zero, 0, Vector3.zero, -nDamage, Vector3.zero, 0, Vector3.zero);		
	}
}
