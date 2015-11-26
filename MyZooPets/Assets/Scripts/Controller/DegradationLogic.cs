using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//Logic for the daily maintenance system that drives retention
//and daily check-ins. 
//TODO need to store diff types of trigger and distinct between room and yard
public class DegradationLogic : Singleton<DegradationLogic>{
	public static event EventHandler<EventArgs> OnTriggerAffectsHealth;
	public static event EventHandler<EventArgs> OnRefreshTriggers;
	public event EventHandler<EventArgs> OnPetHit;
	
	// tut key
//	public static string TIME_DECAY_TUT = "TUT_TIME_DECAY";

	// --- mood related degradation variables
	// if the pet's health is below this value, mood effects are doubled
	public float fHealthMoodThreshold;
	private const int MAX_TRIGGERS = 6;
	private List<DegradData> degradationTriggers; //list of triggers spawned
	public List<DegradData> DegradationTriggers{
		get{ return degradationTriggers;} 
	}

	private bool isAwakeCheck = true;

	void Awake(){		
		RefreshCheck();
	}

	void Start(){
		//WellapadMissionController.Instance.OnMissionsRefreshed += RefreshCheck;
	}

	void OnApplicationPause(bool isPaused){
		//Refresh logic
		if(!isPaused){
			if(isAwakeCheck){
				isAwakeCheck = false;
				return;
			}
			else{
				RefreshCheck();
			}
		}
		Analytics.Instance.RemainingTriggers(degradationTriggers.Count);
	}

	private void RefreshCheck(object sender, EventArgs args){
		RefreshCheck();
	}

	private void RefreshCheck(){
		degradationTriggers = new List<DegradData>(); 

		StatsDegradationCheck();
		SetUpTriggers();
//		UpdateNextPlayPeriodTime();
	}

	//use the method when a trigger has been destroyed by user
	public void ClearDegradationTrigger(DegradTrigger trigger){
		// DegradData degradData = DataManager.Instance.GameData.Degradation.DegradationTriggers.Find(x => x.ID == trigger.ID);
		DegradData degradData = degradationTriggers.Find(x => x.TriggerID == trigger.ID);
		ImmutableDataTrigger triggerData = DataLoaderTriggers.GetTrigger(degradData.TriggerID);

		// instantiate a stats item from the trigger, but only if it's not the tutorial
		bool bTut = TutorialManager.Instance && TutorialManager.Instance.IsTutorialActive();
		if(bTut == false){
			int nXP = DataLoaderXpRewards.GetXP("CleanTrigger", new Hashtable());
			StatsController.Instance.ChangeStats(deltaPoints: nXP, pointsLoc: trigger.transform.position, is3DObject: true);

//			GameObject goPrefab = Resources.Load("DroppedStat") as GameObject;
//			GameObject goDroppedItem = Instantiate(goPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
//			
//			//Spawn floaty text to indicate trigger has been cleaned
//			Hashtable option = new Hashtable();
//			Vector3 floatUpPos = new Vector3(0, 5, 0);
//			option.Add("parent", trigger.gameObject);
//			option.Add("textSize", 1f);
//			option.Add("text", triggerData.FloatyDesc);
//			option.Add("floatingUpPos", floatUpPos);
//
//			FloatyUtil.SpawnFloatyText(option);
//
//			//Init dropped item
//			int nXP = DataLoaderXpRewards.GetXP("CleanTrigger", new Hashtable());
//			goDroppedItem.GetComponent<DroppedObjectStat>().Init(HUDElementType.Points, nXP);
//			
//			// set the position of the newly spawned item to be wherever this item box is
//			float fOFfsetY = Constants.GetConstant<float>("ItemBoxTrigger_OffsetY");
//			Vector3 vPosition = new Vector3(trigger.gameObject.transform.position.x, 
//			                                trigger.gameObject.transform.position.y + fOFfsetY, 
//			                                trigger.gameObject.transform.position.z);
//			goDroppedItem.transform.position = vPosition;
//			
//			// make the stats "burst" out
//			goDroppedItem.GetComponent<DroppedObject>().Appear();			

			//send analytics event
			//Analytics.Instance.TriggersCleaned(triggerData.ID);
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

	/// <summary>
	/// Triggers hit pet. When a trigger particle effect hits the pet.
	/// </summary>
	/// <param name="trigger">Trigger.</param>
	public void TriggerHitPet(DegradParticle trigger){
		// send out a callback
		if(OnPetHit != null)
			OnPetHit(this, EventArgs.Empty);
		
		// damage the pet
		int damage = trigger.Damage;

		StatsController.Instance.ChangeStats(deltaHealth: -damage, isFloaty: true);

		//Send analytics event
	//	Analytics.Instance.TriggerHitPet();    
	}

	private void StatsDegradationCheck(){
		// don't do these checks if the player has not yet finished the tutorials (we don't want them losing health/hunger)
		bool isTutorialDone = DataManager.Instance.GameData.Tutorial.AreTutorialsFinished();
		if(!isTutorialDone){
			return;
		}
		
		// calculate changes in the pets mood
//		TimeSpan sinceLastPlayed = LgDateTime.GetTimeSpanSinceLastPlayed();
//		CalculateMoodDegradation(sinceLastPlayed);
		CalculateHealthDegradation();
	}
   
	/// <summary>
	/// Sets up triggers.
	/// This function just SETS UP the triggers and
	/// where they should spawn.  The actual triggers are
	/// spawned from the DegradationUIManager.
	/// </summary>
	private void SetUpTriggers(){

//		Debug.Log("===SETTING UP TRIGGERS");
		// get list of available locations to spawn triggers
		List<ImmutableDataTriggerLocation> listAvailable = DataLoaderTriggerLocations.GetAvailableTriggerLocations("Bedroom");
        
		// get the number of triggers to spawn based on the previously uncleaned triggers and the new ones to spawn, with a max
		int numToSpawn = GetNumTriggersToSpawn();
//		Debug.Log("===Spawning " + numToSpawn);
		DataManager.Instance.GameData.Degradation.UncleanedTriggers = numToSpawn;
		List<ImmutableDataTriggerLocation> listChosen = ListUtils.GetRandomElements<ImmutableDataTriggerLocation>(listAvailable, numToSpawn);
        
		//create trigger data to be spawned
		for(int i = 0; i < listChosen.Count; i++){
			ImmutableDataTriggerLocation location = listChosen[i];
            
			ImmutableDataTrigger randomTrigger = DataLoaderTriggers.GetRandomSceneTrigger("Bedroom");

			// to make things easier, if the user has not done the trigger tutorial yet, just override the random location and use 0
			// also, use the dust prefab...this is a soft setting...hopefully no one changes that array
			bool isTriggerTutDone = DataManager.Instance.GameData.Tutorial.IsTutorialFinished(TutorialManagerBedroom.TUT_TRIGGERS);

			if(!isTriggerTutDone){
				if(i == 0){
					location = DataLoaderTriggerLocations.GetTriggerLocation("TrigLoc_0", "Bedroom");
					
					if(location == null){
						Debug.LogError("Tutorial trigger location not set up correctly");
					}
					
					randomTrigger = DataLoaderTriggers.GetTrigger("Trigger_3");
				}
			}
			//spawn them at a pre define location ID is the order in which the data are created
			degradationTriggers.Add(new DegradData(randomTrigger.ID, location.Partition, location.Position));
		}                
		
		if(OnRefreshTriggers != null){
			OnRefreshTriggers(this, EventArgs.Empty);
		}
	}
    
	//---------------------------------------------------
	// GetNumTriggersToSpawn()
	// Returns the correct number of triggers that should
	// spawn based.
	//---------------------------------------------------       
	private int GetNumTriggersToSpawn(){
		// get the new number of triggers to spawn based on how long the player has been absent
		int newTriggers = GetNewTriggerCount();

		// get the number of triggers the player did not clean
		int uncleanedTriggers = DataManager.Instance.GameData.Degradation.UncleanedTriggers;
		if(uncleanedTriggers < 0){
			uncleanedTriggers = 0; // this is a safeguard...I think this will eventually be changed a bit though
		}

		// add them together but check min/maxes
		int numToSpawn = Mathf.Min(MAX_TRIGGERS, newTriggers + uncleanedTriggers);
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
		int newTriggers = 0;
		bool isTriggerTutDone = DataManager.Instance.GameData.Tutorial.IsTutorialFinished(TutorialManagerBedroom.TUT_TRIGGERS);
		MutableDataDegradation degradationData = DataManager.Instance.GameData.Degradation;

//		isTriggerTutDone = true;	// DEBUG

		if(!isTriggerTutDone){
			int uncleanedTriggers = DataManager.Instance.GameData.Degradation.UncleanedTriggers;

			// only spawn one trigger if trigger tutorial is not done yet
			if(uncleanedTriggers == 0){
				newTriggers = 1;
				degradationData.IsTriggerSpawned = true;
			}

			//need to update this time every time this function is called while trigger tutorial is not done yet otherwise
			//logic gets messed up
			degradationData.LastPlayPeriodTriggerSpawned = PlayPeriodLogic.GetCurrentPlayPeriod();
		}
		else{
			int playPeriodsOffset = GetNumPlayPeriodsOffset();

			//There are missed play periods
			if(playPeriodsOffset > 1){
				//max of 2 missed play period will be accounted
				if(playPeriodsOffset > 2){
					playPeriodsOffset = 2;
				}

				//calculate num of new triggers
				newTriggers = playPeriodsOffset * 3;

				//update lastTriggerSpawnedPlayPeriod. Important that we update it here
				//otherwise more triggers will be spawned if user return from pause
				degradationData.LastPlayPeriodTriggerSpawned = PlayPeriodLogic.GetCurrentPlayPeriod();
				Debug.Log("Missed play periods spawning " + newTriggers + "triggers");
			}
	        //No missed play periods. spawn triggers for Next Play Period
	        else{
				DateTime now = LgDateTime.GetTimeNow();
				DateTime lastTriggerSpawnedPlayPeriod = degradationData.LastPlayPeriodTriggerSpawned;
				TimeSpan timeSinceLastTriggerSpawned = now - lastTriggerSpawnedPlayPeriod;

				//only spawn new trigger if time hasn't been rewind somehow
				if(lastTriggerSpawnedPlayPeriod <= now){
					//new play period need to refresh variable
					if(timeSinceLastTriggerSpawned.TotalHours >= 12){
						degradationData.IsTriggerSpawned = false;
					}

					if(!degradationData.IsTriggerSpawned){
						newTriggers = 3;
						degradationData.IsTriggerSpawned = true;
						degradationData.LastPlayPeriodTriggerSpawned = PlayPeriodLogic.GetCurrentPlayPeriod();
					}
				}
			}
		}
		return newTriggers;
	}
        
//	//---------------------------------------------------
//	// CalculateMoodDegradation()
//	// Depending on how long it has been since the user
//	// last played, the pet will suffer some mood loss.
//	//---------------------------------------------------   
//	private void CalculateMoodDegradation(TimeSpan timeSinceLastPlayed){
//		// amount to degrade mood by
//		int moodLoss = 0;
//        
//		// penalties
//		float firstHoursPenalty = Constants.GetConstant<float>("HungerDamage_Short");
//		float secondHoursPenalty = Constants.GetConstant<float>("HungerDamage_Long");
//		
//		// get the pet's health %, because it affects how their mood changes
//		float hp = (float)(DataManager.Instance.GameData.Stats.Health / 100.0f);
//		float multiplier = Constants.GetConstant<float>("HungerMultiplier_Healthy");
//		if(hp < fHealthMoodThreshold){
//			multiplier = Constants.GetConstant<float>("HungerMultiplier_Sick");
//		}
//        
//		// first part of the mood degradation -- the first 24 hours of not playing
//		int firstHours = timeSinceLastPlayed.TotalHours > 24 ? 24 : (int)timeSinceLastPlayed.TotalHours;
//		if(firstHours > 0){
//			moodLoss += (int)(firstHours * (firstHoursPenalty * multiplier));
//		}
//        
//		// second part of mood degradation -- anything after 24 hours of not playing
//		int secondHours = (int)(timeSinceLastPlayed.TotalHours - 24);
//		if(secondHours > 0){
//			moodLoss += (int)(secondHours * (secondHoursPenalty * multiplier));
//		}
//
//		// actually change the pet's mood
//		StatsController.Instance.ChangeStats(deltaMood: -moodLoss);
//        
//		// if the player actually lost some mood, check and show the mood loss tutorial (if appropriate)
//		// also only spawn tutorial if pet is healthy. Other notifications will be spawned
//		// when pet is not healthy
////		bool isMoodDecayTutorialDone = DataManager.Instance.GameData.Tutorial.IsTutorialFinished(TIME_DECAY_TUT);
////		PetHealthStates healthState = DataManager.Instance.GameData.Stats.GetHealthState();
////		if(moodLoss > 0 && !isMoodDecayTutorialDone && healthState == PetHealthStates.Healthy)
////			StartCoroutine(MoodDegradTutorial());
//	}

	//-----------------------------------------------
	// MoodDeradTutorial()
	// Yield one frame before calling a separate class
	// because CalculateMoodDeradation is called in Awake()
	// and Awake() execution order is not guaranteed between classes 
	//-----------------------------------------------
//	private IEnumerator MoodDegradTutorial(){
//		yield return 0;
//		TutorialUIManager.Instance.StartTimeMoodDecayTutorial();
//	}

	//---------------------------------------------------   
	// CalculateHealthDegradation()
	// Health degrads each time user misses inhaler or a 
	// play period
	//---------------------------------------------------   
	private void CalculateHealthDegradation(){
		// wait a frame, or else the notification manager won't work properly

		int numPlayPeriodOffset = GetNumPlayPeriodsOffset();

		if(numPlayPeriodOffset > 1){
			//max punishment is 2 play period
			if(numPlayPeriodOffset > 3){
				numPlayPeriodOffset = 3;
			}
			Debug.Log("Missed play period, punishing user with " + ((numPlayPeriodOffset - 1) * -20) + " health");
			StatsController.Instance.ChangeStats(deltaHealth: (numPlayPeriodOffset - 1) * -20);
		}
	}

	//-------------------------------------------------------------------------- 
	// UpdateNextPlayPeriodTime()
	// Health Degradation and Trigger Degradation both depends on NextPlayPeriod
	// for timing logic, so if we ever need to update NextPlayPeriod it needs to be
	// done only after both functions run their logic with the same NextPlayPeriod value
	//-------------------------------------------------------------------------- 
//	private void UpdateNextPlayPeriodTime(){
//		int numOfMissedPlayPeriod = GetNumOfMissedPlayPeriod();	//TODO tie this to inhaler
//
//		if(numOfMissedPlayPeriod > 0){
//			Debug.LogWarning("DANGING LOGIC FIX");
////			PlayPeriodLogic.Instance.CalculateCurrentPlayPeriod();
//			// Debug.Log("current play period: " + PlayPeriodLogic.Instance.NextPlayPeriod);
//		}
//	}

	/// <summary>
	/// Gets the number play periods offset
	/// If number is 1, that means you did not miss any play periods
	/// </summary>
	/// <returns>The number play periods offset.</returns>
	private int GetNumPlayPeriodsOffset(){
		int playPeriodsOffset = 0;
		DateTime lastPlayPeriod = PlayPeriodLogic.Instance.GetLastPlayPeriod();

		// Difference in hours between now and next play period
		TimeSpan timeSinceStartOfPlayPeriod = LgDateTime.GetTimeNow() - lastPlayPeriod;

		//if within 12 hours no punishment
		//if > 12 hrs punishment for every 12 hrs miss
		playPeriodsOffset = (int)timeSinceStartOfPlayPeriod.TotalHours / 12;

		//Debug.Log("last play period " + lastPlayPeriod + " || time since start of play period " + timeSinceStartOfPlayPeriod + " || missed play period " + (playPeriodsOffset-1));
		return playPeriodsOffset;
	}
}
