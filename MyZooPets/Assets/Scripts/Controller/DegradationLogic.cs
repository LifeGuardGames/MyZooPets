using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//Logic for the daily maintenance system that drives retention
//and daily check-ins. 
//TO DO: need to store diff types of trigger and distinct between room and yard
public class DegradationLogic : Singleton<DegradationLogic>{
	public static event EventHandler<EventArgs> OnTriggerAffectsHealth;
	public static event EventHandler<EventArgs> OnRefreshTriggers;
	public event EventHandler<EventArgs> OnPetHit;
	
	// tut key
	public static string TIME_DECAY_TUT = "TUT_TIME_DECAY";

	// --- mood related degradation variables
	// if the pet's health is below this value, mood effects are doubled
	public float fHealthMoodThreshold;
	private const int MAX_TRIGGERS = 6;
	private List<DegradData> degradationTriggers; //list of triggers spawned

	public List<DegradData> DegradationTriggers{
		get{ return degradationTriggers;} 
	}

	void Awake(){		
		RefreshCheck();
	}

	void Start(){
		WellapadMissionController.Instance.OnMissionsRefreshed += RefreshCheck;
	}

	void OnApplicationPause(bool isPaused){
		//Refresh logic
		if(!isPaused)
			RefreshCheck();
	}

	private void RefreshCheck(object sender, EventArgs args){
		RefreshCheck();
	}

	private void RefreshCheck(){
		degradationTriggers = new List<DegradData>(); 
		RefreshDegradationCheck();
		SetUpTriggers();       
		UpdateNextPlayPeriodTime();
	}

	//use the method when a trigger has been destroyed by user
	public void ClearDegradationTrigger(DegradTrigger trigger){
		// DegradData degradData = DataManager.Instance.GameData.Degradation.DegradationTriggers.Find(x => x.ID == trigger.ID);
		DegradData degradData = degradationTriggers.Find(x => x.TriggerID == trigger.ID);
		ImmutableDataTrigger triggerData = DataLoaderTriggers.GetTrigger(degradData.TriggerID);

		// instantiate a stats item from the trigger, but only if it's not the tutorial
		bool bTut = TutorialManager.Instance && TutorialManager.Instance.IsTutorialActive();
		if(bTut == false){
			GameObject goPrefab = Resources.Load("DroppedStat") as GameObject;
			GameObject goDroppedItem = Instantiate(goPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
			
			//Spawn floaty text to indicate trigger has been cleaned
			Hashtable option = new Hashtable();
			Vector3 floatUpPos = new Vector3(0, 2, 0);
			option.Add("parent", goDroppedItem);
			option.Add("textSize", 0.4f);
			option.Add("text", triggerData.FloatyDesc);
			option.Add("floatingUpPos", floatUpPos);

			FloatyUtil.SpawnFloatyText(option);

			//Init dropped item
			int nXP = DataLoaderXpRewards.GetXP("CleanTrigger", new Hashtable());
			goDroppedItem.GetComponent<DroppedObjectStat>().Init(HUDElementType.Points, nXP);
			
			// set the position of the newly spawned item to be wherever this item box is
			float fOFfsetY = Constants.GetConstant<float>("ItemBoxTrigger_OffsetY");
			Vector3 vPosition = new Vector3(trigger.gameObject.transform.position.x, 
			                                trigger.gameObject.transform.position.y + fOFfsetY, 
			                                trigger.gameObject.transform.position.z);
			goDroppedItem.transform.position = vPosition;
			
			// make the stats "burst" out
			goDroppedItem.GetComponent<DroppedObject>().Appear();			

			//send analytics event
			Analytics.Instance.TriggersCleaned(triggerData.ID);
		}	
		
		// DataManager.Instance.GameData.Degradation.DegradationTriggers.Remove(degradData);
		degradationTriggers.Remove(degradData);
		
		// subtract one from the triggers left to clean
		DataManager.Instance.GameData.Degradation.UncleanedTriggers -= 1;

		// if there are no degradation triggers left, send out a task completion message
		// note -- this will all probably have to change a bit as we get more complex (triggers in the yard, or other locations)
		// if ( DataManager.Instance.GameData.Degradation.DegradationTriggers.Count == 0 )
		if(degradationTriggers.Count == 0)
			WellapadMissionController.Instance.TaskCompleted("CleanRoom");
	}
	
	//---------------------------------------------------
	// TriggerHitPet()
	// When a trigger particle effect hits the pet.
	//---------------------------------------------------		
	public void TriggerHitPet(DegradParticle trigger){
		// send out a callback
		if(OnPetHit != null)
			OnPetHit(this, EventArgs.Empty);
		
		// damage the pet
		int nDamage = trigger.GetDamage();
//		StatsController.Instance.ChangeStats(0, Vector3.zero, 0, Vector3.zero, 
//		                                     -nDamage, Vector3.zero, 0, Vector3.zero, 
//		                                     bFloaty: true);
		StatsController.Instance.ChangeStats(deltaHealth: -nDamage, bFloaty: true);

		//Send analytics event
		Analytics.Instance.TriggerHitPet();    
	}

	private void RefreshDegradationCheck(){
		// don't do these checks if the player has not yet finished the tutorials (we don't want them losing health/hunger)
		bool bTutsDone = DataManager.Instance.GameData.Tutorial.AreTutorialsFinished();
		if(!bTutsDone)
			return;
		
		// calculate changes in the pets mood
		TimeSpan sinceLastPlayed = LgDateTime.GetTimeSinceLastPlayed();
		CalculateMoodDegradation(sinceLastPlayed);
		CalculateHealthDegradation();
	}
    
	//---------------------------------------------------
	// SetUpTriggers()
	// This function just SETS UP the triggers and
	// where they should spawn.  The actual triggers are
	// spawned from the DegradationUIManager.
	//---------------------------------------------------   
	private void SetUpTriggers(){      
		// get list of available locations to spawn triggers
		List<Data_TriggerLocation> listAvailable = DataLoaderTriggerLocations.GetAvailableTriggerLocations("Bedroom");
        
		// get the number of triggers to spawn based on the previously uncleaned triggers and the new ones to spawn, with a max
		int numToSpawn = GetNumTriggersToSpawn();
        
		DataManager.Instance.GameData.Degradation.UncleanedTriggers = numToSpawn;

		List<Data_TriggerLocation> listChosen = ListUtils.GetRandomElements<Data_TriggerLocation>(listAvailable, numToSpawn);
        
		//create trigger data to be spawned
		for(int i = 0; i < listChosen.Count; i++){
			Data_TriggerLocation location = listChosen[i];
            
			ImmutableDataTrigger randomTrigger = DataLoaderTriggers.GetRandomSceneTrigger("Bedroom");

			// random prefab
			// int objectIndex = UnityEngine.Random.Range(0, triggerPrefabs.Count);
            
			// to make things easier, if the user has not done the trigger tutorial yet, just override the random location and use 0
			// also, use the dust prefab...this is a soft setting...hopefully no one changes that array
			bool bTriggers = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TutorialManagerBedroom.TUT_TRIGGERS);
			if(!bTriggers && i == 0){
				location = DataLoaderTriggerLocations.GetTriggerLocation("TrigLoc_0", "Bedroom");
				if(location == null)
					Debug.LogError("Tutorial trigger location not set up correctly");
                
				// objectIndex = 3;
				randomTrigger = DataLoaderTriggers.GetTrigger("Trigger_3");
			}

			//spawn them at a pre define location ID is the order in which the data are created
			degradationTriggers.Add(new DegradData(randomTrigger.ID, location.GetPosition()));
		}                

		DataManager.Instance.GameData.Degradation.LastTimeUserPlayedGame = LgDateTime.GetTimeNow(); //update last played time       

		if(OnRefreshTriggers != null)
			OnRefreshTriggers(this, EventArgs.Empty);
	}
    
	//---------------------------------------------------
	// GetNumTriggersToSpawn()
	// Returns the correct number of triggers that should
	// spawn based.
	//---------------------------------------------------       
	private int GetNumTriggersToSpawn(){
		// get the new number of triggers to spawn based on how long the player has been absent
		int nNewTriggers = GetNewTriggerCount();
        
		// get the number of triggers the player did not clean
		int nUncleanedTriggers = DataManager.Instance.GameData.Degradation.UncleanedTriggers;
		if(nUncleanedTriggers < 0)
			nUncleanedTriggers = 0; // this is a safeguard...I think this will eventually be changed a bit though
        
		// add them together but check min/maxes
		int numToSpawn = Mathf.Min(MAX_TRIGGERS, nNewTriggers + nUncleanedTriggers);
		if(numToSpawn < 0){
			numToSpawn = 0;
			Debug.LogError("Number of triggers to spawn somehow < 0...");
		}   
        
		return numToSpawn;
	}
    
	//---------------------------------------------------
	// GetNewTriggerCount()
	// Depending on how long the player has been away
	// and what time of day it is, return the number of
	// new triggers that should spawn.
	//---------------------------------------------------       
	private int GetNewTriggerCount(){
		int nNew = 0;
		int numOfMissedPlayPeriod = GetNumOfMissedPlayPeriod();
		MutableDataDegradation degradationData = DataManager.Instance.GameData.Degradation;

		//There are missed play periods
		if(numOfMissedPlayPeriod > 0){
			//max of 2 missed play period will be accounted
			if(numOfMissedPlayPeriod > 2)
				numOfMissedPlayPeriod = 2;

			//calculate num of new triggers
			nNew = numOfMissedPlayPeriod * 3;

			//update lastTriggerSpawnedPlayPeriod. Important that we update it here
			//otherwise more triggers will be spawned if user return from pause
			degradationData.LastTriggerSpawnedPlayPeriod = PlayPeriodLogic.GetCurrentPlayPeriod();

		}
        //No missed play periods. spawn triggers for Next Play Period
        else{
			DateTime now = LgDateTime.GetTimeNow();
			DateTime lastTriggerSpawnedPlayPeriod = degradationData.LastTriggerSpawnedPlayPeriod;
			TimeSpan timeSinceLastTriggerSpawned = now - lastTriggerSpawnedPlayPeriod;

			//only spawn new trigger if time hasn't been rewind somehow
			if(lastTriggerSpawnedPlayPeriod <= now){
				//new play period need to refresh variable
				if(timeSinceLastTriggerSpawned.TotalHours >= 12){
					degradationData.IsTriggerSpawned = false;
				}

				if(!degradationData.IsTriggerSpawned){
					nNew = 3;
					degradationData.IsTriggerSpawned = true;
					degradationData.LastTriggerSpawnedPlayPeriod = PlayPeriodLogic.Instance.NextPlayPeriod;
				}
			}
		}

		return nNew;
	}
        
	//---------------------------------------------------
	// CalculateMoodDegradation()
	// Depending on how long it has been since the user
	// last played, the pet will suffer some mood loss.
	//---------------------------------------------------   
	private void CalculateMoodDegradation(TimeSpan timeSinceLastPlayed){
		// wait a frame, or else the notification manager won't work properly
        
		// amount to degrade mood by
		int nMoodLoss = 0;
        
		// penalties
		float fFirstHoursPenalty = Constants.GetConstant<float>("HungerDamage_Short");
		float fSecondHoursPenalty = Constants.GetConstant<float>("HungerDamage_Long");
		
		// get the pet's health %, because it affects how their mood changes
		float fHP = (float)(DataManager.Instance.GameData.Stats.Health / 100.0f);
		float fMultiplier = Constants.GetConstant<float>("HungerMultiplier_Healthy");
		if(fHP < fHealthMoodThreshold)
			fMultiplier = Constants.GetConstant<float>("HungerMultiplier_Sick");
        
		// first part of the mood degradation -- the first 24 hours of not playing
		int nFirstHours = timeSinceLastPlayed.TotalHours > 24 ? 24 : (int)timeSinceLastPlayed.TotalHours;
		if(nFirstHours > 0)
			nMoodLoss += (int)(nFirstHours * (fFirstHoursPenalty * fMultiplier));
        
		// second part of mood degradation -- anything after 24 hours of not playing
		int nSecondHours = (int)(timeSinceLastPlayed.TotalHours - 24);
		if(nSecondHours > 0)
			nMoodLoss += (int)(nSecondHours * (fSecondHoursPenalty * fMultiplier));

		// actually change the pet's mood
//		StatsController.Instance.ChangeStats(0, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero, -nMoodLoss, Vector3.zero);
		StatsController.Instance.ChangeStats(deltaMood: -nMoodLoss);
        
		// if the player actually lost some mood, check and show the mood loss tutorial (if appropriate)
		if(nMoodLoss > 0 && !DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TIME_DECAY_TUT))
			StartCoroutine(MoodDegradTutorial());
	}

	//-----------------------------------------------
	// MoodDeradTutorial()
	// Yield one frame before calling a separate class
	// because CalculateMoodDeradation is called in Awake()
	// and Awake() execution order is not guaranteed between classes 
	//-----------------------------------------------
	private IEnumerator MoodDegradTutorial(){
		yield return 0;
		TutorialUIManager.Instance.StartTimeMoodDecayTutorial();
	}

	//---------------------------------------------------   
	// CalculateHealthDegradation()
	// Health degrads each time user misses inhaler or a 
	// play period
	//---------------------------------------------------   
	private void CalculateHealthDegradation(){
		// wait a frame, or else the notification manager won't work properly

		int numOfMissedPlayPeriod = GetNumOfMissedPlayPeriod();

		if(numOfMissedPlayPeriod > 0){
			//max punishment is 2 play period
			if(numOfMissedPlayPeriod > 2)
				numOfMissedPlayPeriod = 2;

//			StatsController.Instance.ChangeStats(0, Vector3.zero, 0, Vector3.zero, 
//                numOfMissedPlayPeriod * -20, Vector3.zero, 0, Vector3.zero);
			StatsController.Instance.ChangeStats(deltaHealth: numOfMissedPlayPeriod * -20);
		}
	}

	//-------------------------------------------------------------------------- 
	// UpdateNextPlayPeriodTime()
	// Health Degradation and Trigger Degradation both depends on NextPlayPeriod
	// for timing logic, so if we ever need to update NextPlayPeriod it needs to be
	// done only after both functions run their logic with the same NextPlayPeriod value
	//-------------------------------------------------------------------------- 
	private void UpdateNextPlayPeriodTime(){
		int numOfMissedPlayPeriod = GetNumOfMissedPlayPeriod();

		if(numOfMissedPlayPeriod > 0){
			PlayPeriodLogic.Instance.CalculateCurrentPlayPeriod();
			// Debug.Log("current play period: " + PlayPeriodLogic.Instance.NextPlayPeriod);
		}
	}

	//-------------------------------------------------------------------------- 
	// GetNumOfMissedPlayPeriod()
	//
	//-------------------------------------------------------------------------- 
	private int GetNumOfMissedPlayPeriod(){
		int missedPlayPeriod = 0;
		DateTime nextPlayPeriod = PlayPeriodLogic.Instance.NextPlayPeriod;

		//different in hours between now and next play period
		//check only if nextplayperiod is less than the time now
		if(nextPlayPeriod <= LgDateTime.GetTimeNow()){
			TimeSpan timeSinceStartOfPlayPeriod = LgDateTime.GetTimeNow() - nextPlayPeriod;
			// Debug.Log("health degrade timeSinceStartOfPlayPeriod: " + timeSinceStartOfPlayPeriod);

			//if within 12 hours no punishment
			//if > 12 hrs punishment for every 12 hrs miss
			missedPlayPeriod = (int)timeSinceStartOfPlayPeriod.TotalHours / 12;

			// Debug.Log("health degrade numOfMissedPlayPeriod: " + missedPlayPeriod);
		}

		return missedPlayPeriod;
	}
}
