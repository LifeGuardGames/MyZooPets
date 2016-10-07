using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Logic for the daily maintenance system that drives retention and daily check-ins
/// TODO need to store diff types of trigger and distinct between room and yard
/// </summary>
public class DegradationLogic : Singleton<DegradationLogic> {
	public static event EventHandler<EventArgs> OnTriggerAffectsHealth;
	public static event EventHandler<EventArgs> OnRefreshTriggers;
	public event EventHandler<EventArgs> OnPetHit;

	// --- mood related degradation variables
	// if the pet's health is below this value, mood effects are doubled
	public float fHealthMoodThreshold;
	private const int MAX_TRIGGERS = 6;

	private List<DegradData> degradationTriggers; //list of triggers spawned
	public List<DegradData> DegradationTriggers {
		get { return degradationTriggers; }
	}

	private bool isAwakeCheck = true;

	public bool IsTesting() {
		return Constants.GetConstant<bool>("TestingDegrad");
	}

	void Awake() {
		RefreshCheck();
	}

	void Start() {
		//WellapadMissionController.Instance.OnMissionsRefreshed += RefreshCheck;
	}

	void OnApplicationPause(bool isPaused) {
		//Refresh logic
		if(!isPaused) {
			if(isAwakeCheck) {
				isAwakeCheck = false;
				return;
			}
			else {
				RefreshCheck();
			}
		}
		if(degradationTriggers != null) {
			Analytics.Instance.RemainingTriggers(degradationTriggers.Count);
		}
	}

	private void RefreshCheck(object sender, EventArgs args) {
		RefreshCheck();
	}

	private void RefreshCheck() {
		Debug.Log("REFRESH CHECKING");
		if((!PlayPeriodLogic.Instance.IsFirstPlayPeriod() && DataManager.Instance.GameData.Tutorial.AreBedroomTutorialsFinished())
			|| IsTesting()) {

			Debug.Log("REFRESH CHECKING PASS");
			degradationTriggers = new List<DegradData>();

			StatsDegradationCheck();
			SetUpTriggers();
			//			UpdateNextPlayPeriodTime();
		}
	}

	//use the method when a trigger has been destroyed by user
	public void ClearDegradationTrigger(DegradTrigger trigger) {
		// DegradData degradData = DataManager.Instance.GameData.Degradation.DegradationTriggers.Find(x => x.ID == trigger.ID);
		DegradData degradData = degradationTriggers.Find(x => x.TriggerID == trigger.ID);

		// instantiate a stats item from the trigger, but only if it's not the tutorial
		bool bTut = TutorialManager.Instance && TutorialManager.Instance.IsTutorialActive();
		if(bTut == false) {
			int nXP = DataLoaderXpRewards.GetXP("CleanTrigger", new Hashtable());
			StatsManager.Instance.ChangeStats(xpDelta: nXP, xpPos: trigger.transform.position, is3DObject: true);
		}

		// DataManager.Instance.GameData.Degradation.DegradationTriggers.Remove(degradData);
		degradationTriggers.Remove(degradData);

		// subtract one from the triggers left to clean
		DataManager.Instance.GameData.Degradation.UncleanedTriggers -= 1;

		// if there are no degradation triggers left, send out a task completion message
		// note -- this will all probably have to change a bit as we get more complex (triggers in the yard, or other locations)
		// if ( DataManager.Instance.GameData.Degradation.DegradationTriggers.Count == 0 )
		if(degradationTriggers.Count == 0) {
			WellapadMissionController.Instance.TaskCompleted("CleanRoom");
			AudioManager.Instance.LowerBackgroundVolume(0.1f);
			AudioManager.Instance.PlayClip("inhalerCapstone");
			AudioManager.Instance.backgroundMusic = "bgBedroom";
			AudioManager.Instance.StartCoroutine("PlayBackground");
		}
	}

	/// <summary>
	/// When a trigger particle effect hits the pet.
	/// </summary>
	public void TriggerHitPet(DegradParticle trigger) {
		// send out a callback
		if(OnPetHit != null) {
			OnPetHit(this, EventArgs.Empty);
		}
		// damage the pet
		int damage = trigger.Damage;

		StatsManager.Instance.ChangeStats(healthDelta: -damage, isFloaty: true);
	}

	private void StatsDegradationCheck() {
		// don't do these checks if the player has not yet finished the tutorials (we don't want them losing health/hunger)
		bool isTutorialDone = DataManager.Instance.GameData.Tutorial.AreBedroomTutorialsFinished();
		if(!isTutorialDone) {
			return;
		}
		// calculate changes in the pets mood
		CalculateHealthDegradation();
	}

	/// <summary>
	/// Sets up triggers.
	/// This function just SETS UP the triggers and
	/// where they should spawn.  The actual triggers are
	/// spawned from the DegradationUIManager.
	/// </summary>
	private void SetUpTriggers() {
		Debug.Log("SETTING UP TRIGGERS");
		// get list of available locations to spawn triggers
		List<ImmutableDataTriggerLocation> listAvailable = DataLoaderTriggerLocations.GetAvailableTriggerLocations("Bedroom");

		// get the number of triggers to spawn based on the previously uncleaned triggers and the new ones to spawn, with a max
		int numToSpawn = GetNumTriggersToSpawn();

		DataManager.Instance.GameData.Degradation.UncleanedTriggers = numToSpawn;
		List<ImmutableDataTriggerLocation> listChosen = ListUtils.GetRandomElements(listAvailable, numToSpawn);

		//create trigger data to be spawned
		for(int i = 0; i < listChosen.Count; i++) {
			ImmutableDataTriggerLocation location = listChosen[i];
			ImmutableDataTrigger randomTrigger = DataLoaderTriggers.GetRandomSceneTrigger("Bedroom");

			//spawn them at a pre defined location ID is the order in which the data are created
			degradationTriggers.Add(new DegradData(randomTrigger.ID, location.Partition, location.Position));
		}
		if(degradationTriggers.Count > 0) {
			AudioManager.Instance.backgroundMusic = "bgClinic";     // TODO check if this actually works
			AudioManager.Instance.StartCoroutine("PlayBackground");
		}
		if(OnRefreshTriggers != null) {
			OnRefreshTriggers(this, EventArgs.Empty);
		}
	}

	// Returns the correct number of triggers that should spawn based.   
	private int GetNumTriggersToSpawn() {
		// get the new number of triggers to spawn based on how long the player has been absent
		int newTriggers = GetNewTriggerCount();

		// get the number of triggers the player did not clean
		int uncleanedTriggers = DataManager.Instance.GameData.Degradation.UncleanedTriggers;
		if(uncleanedTriggers < 0) {
			uncleanedTriggers = 0; // this is a safeguard...I think this will eventually be changed a bit though
		}

		// add them together but check min/maxes
		int numToSpawn = Mathf.Min(MAX_TRIGGERS, newTriggers + uncleanedTriggers);
		if(numToSpawn < 0) {
			numToSpawn = 0;
			Debug.LogError("Number of triggers to spawn somehow < 0...");
		}

		return numToSpawn;
	}

	/// <summary>
	/// Depending on how long the player has been away and what time of day it is, return the number of
	/// new triggers that should spawn. 
	/// <summary>
	private int GetNewTriggerCount() {
		int newTriggers = 0;
		MutableDataDegradation degradationData = DataManager.Instance.GameData.Degradation;
		int playPeriodsOffset = GetNumPlayPeriodsOffset();

		//There are missed play periods
		if(playPeriodsOffset > 1) {
			//max of 2 missed play period will be accounted
			if(playPeriodsOffset > 2) {
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
		else {
			DateTime now = LgDateTime.GetTimeNow();
			DateTime lastTriggerSpawnedPlayPeriod = degradationData.LastPlayPeriodTriggerSpawned;
			TimeSpan timeSinceLastTriggerSpawned = now - lastTriggerSpawnedPlayPeriod;

			//only spawn new trigger if time hasn't been rewind somehow
			if(lastTriggerSpawnedPlayPeriod <= now) {
				//new play period need to refresh variable
				if(timeSinceLastTriggerSpawned.TotalHours >= 12) {
					degradationData.IsTriggerSpawned = false;
				}

				if(!degradationData.IsTriggerSpawned) {
					newTriggers = 3;
					degradationData.IsTriggerSpawned = true;
					degradationData.LastPlayPeriodTriggerSpawned = PlayPeriodLogic.GetCurrentPlayPeriod();
				}
			}
		}
		return newTriggers;
	}

	/// <summary>
	/// Health degrads each time user misses inhaler or a play period
	/// </summary>
	private void CalculateHealthDegradation() {
		// wait a frame, or else the notification manager won't work properly

		int numPlayPeriodOffset = GetNumPlayPeriodsOffset();

		if(numPlayPeriodOffset > 1) {
			//max punishment is 2 play period
			if(numPlayPeriodOffset > 3) {
				numPlayPeriodOffset = 3;
			}
			Debug.Log("Missed play period, punishing user with " + ((numPlayPeriodOffset - 1) * -20) + " health");
			StatsManager.Instance.ChangeStats(healthDelta: (numPlayPeriodOffset - 1) * -20);
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
	private int GetNumPlayPeriodsOffset() {
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
