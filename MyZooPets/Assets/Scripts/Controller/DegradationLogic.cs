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
		
		// calculate changes in the pets mood
		TimeSpan sinceLastPlayed = LgDateTime.GetTimeSinceLastPlayed();
		StartCoroutine( CalculateMoodDegradation( sinceLastPlayed ) );
		
		// set up triggers to be spawned by the UI manager
		SetUpTriggers();       
    }
	
	//---------------------------------------------------
	// SetUpTriggers()
	// This function just SETS UP the triggers and
	// where they should spawn.  The actual triggers are
	// spawned from the DegradationUIManager.
	//---------------------------------------------------	
	private void SetUpTriggers() {		
		// get list of available locations to spawn triggers
		List<Data_TriggerLocation> listAvailable = DataLoader_TriggerLocations.GetAvailableTriggerLocations( "Bedroom" );
		
		// get the number of triggers to spawn based on the previously uncleaned triggers and the new ones to spawn, with a max
		int numToSpawn = GetNumTriggersToSpawn();
		DataManager.Instance.GameData.Degradation.UncleanedTriggers = numToSpawn;
		
		List<Data_TriggerLocation> listChosen = ListUtils.GetRandomElements<Data_TriggerLocation>( listAvailable, numToSpawn );
		
        //create trigger data to be spawned
        for(int i = 0; i < listChosen.Count; i++){
			Data_TriggerLocation location = listChosen[i];
			
            // random prefab
            int objectIndex = UnityEngine.Random.Range(0, triggerPrefabs.Count);
			
			// to make things easier, if the user has not done the trigger tutorial yet, just override the random location and use 0
			// also, use the dust prefab...this is a soft setting...hopefully no one changes that array
			bool bTriggers = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains( TutorialManager_Bedroom.TUT_TRIGGERS );
			if ( !bTriggers && i == 0 ) {
				location = DataLoader_TriggerLocations.GetTriggerLocation( "TrigLoc_0", "Bedroom" );
				if ( location == null )
					Debug.Log("Tutorial trigger location not set up correctly");
				
				objectIndex = 3;
			}

            //spawn them at a pre define location ID is the order in which the data are created
            DataManager.Instance.GameData.Degradation.DegradationTriggers.Add(new DegradData(i, location.GetPosition(), objectIndex));
        }                

        DataManager.Instance.GameData.Degradation.LastTimeUserPlayedGame = LgDateTime.GetTimeNow(); //update last played time  		
	}
	
	//---------------------------------------------------
	// GetNumTriggersToSpawn()
	// Returns the correct number of triggers that should
	// spawn based.
	//---------------------------------------------------		
	private int GetNumTriggersToSpawn() {
		// get the new number of triggers to spawn based on how long the player has been absent
      	int nNewTriggers = GetNewTriggerCount();
		
		// get the number of triggers the player did not clean
		int nUncleanedTriggers = DataManager.Instance.GameData.Degradation.UncleanedTriggers;
		if ( nUncleanedTriggers < 0 )
			nUncleanedTriggers = 0;	// this is a safeguard...I think this will eventually be changed a bit though
		
		// add them together but check min/maxes
		int numToSpawn = Mathf.Min( MAX_TRIGGERS, nNewTriggers + nUncleanedTriggers );
		if ( numToSpawn < 0 ) {
			numToSpawn = 0;
			Debug.Log("Number of triggers to spawn somehow < 0...");
		}	
		
		return numToSpawn;
	}
	
	//---------------------------------------------------
	// GetNewTriggerCount()
	// Depending on how long the player has been away
	// and what time of day it is, return the number of
	// new triggers that should spawn.
	//---------------------------------------------------		
	private int GetNewTriggerCount() {
 		DateTime now = LgDateTime.GetTimeNow();
        TimeSpan sinceLastPlayed = LgDateTime.GetTimeSinceLastPlayed();
        int nNew = 0;

        if(sinceLastPlayed.Days > 0){ //reset if new day
            DataManager.Instance.GameData.Degradation.MorningTrigger = true;
            DataManager.Instance.GameData.Degradation.AfternoonTrigger = true;
        }
        if( PlayPeriodLogic.GetTimeFrame( now ) == TimeFrames.Morning ){ //morning
            if(DataManager.Instance.GameData.Degradation.MorningTrigger){
                nNew = 3;
                DataManager.Instance.GameData.Degradation.MorningTrigger = false;
            }
        }else{ //afternoon
            if(DataManager.Instance.GameData.Degradation.AfternoonTrigger){
                nNew = 3; 
                DataManager.Instance.GameData.Degradation.AfternoonTrigger = false;
            }
        }		
		
		return nNew;
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
		
		// penalties
		float fFirstHoursPenalty = Constants.GetConstant<float>( "HungerDamage_Short" );
		float fSecondHoursPenalty = Constants.GetConstant<float>( "HungerDamage_Long" );
		
		// get the pet's health %, because it affects how their mood changes
		float fHP = (float) ( DataManager.Instance.GameData.Stats.Health / 100.0f );
		float fMultiplier = Constants.GetConstant<float>( "HungerMultiplier_Healthy");
		if ( fHP < fHealthMoodThreshold )
			fMultiplier = Constants.GetConstant<float>( "HungerMultiplier_Sick");
		
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
        DegradData degradData = DataManager.Instance.GameData.Degradation.DegradationTriggers.Find(x => x.ID == trigger.ID);
		
		// instantiate a stats item from the trigger, but only if it's not the tutorial
		bool bTut = TutorialManager.Instance && TutorialManager.Instance.IsTutorialActive();
		if ( bTut == false ) {
			GameObject goPrefab = Resources.Load( "DroppedStat" ) as GameObject;
			GameObject goDroppedItem = Instantiate( goPrefab, new Vector3(0, 0, 0), Quaternion.identity ) as GameObject;
			
			int nXP = DataLoader_XpRewards.GetXP( "CleanTrigger", new Hashtable() );
			goDroppedItem.GetComponent<DroppedObject_Stat>().Init( HUDElementType.Points, nXP );
			
			// set the position of the newly spawned item to be wherever this item box is
			float fOFfsetY = Constants.GetConstant<float>( "ItemBoxTrigger_OffsetY" );
			Vector3 vPosition = new Vector3( trigger.gameObject.transform.position.x, trigger.gameObject.transform.position.y+fOFfsetY, trigger.gameObject.transform.position.z );
			goDroppedItem.transform.position = vPosition;
			
			// make the stats "burst" out
			goDroppedItem.GetComponent<DroppedObject>().Appear();			
		}	
		
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
