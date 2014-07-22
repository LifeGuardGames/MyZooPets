using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// Stats controller.
/// Takes care of modifying values in datamanager as well as controlling Hud animator
/// 
/// 					  DataManager (model)
/// 					/
/// 	StatsController
/// 					\
/// 					  HUDAnimator (view)
/// </summary>

public class StatsController : Singleton<StatsController>{
	//----------- Events ------------------------------
//	public EventHandler<EventArgs> OnBreathsChanged;		// when fire breath status changes
	public static EventHandler<EventArgs> OnHappyToSad; //when mood changes
	public static EventHandler<EventArgs> OnSadToHappy;
	public static EventHandler<EventArgs> OnHealthyToVerySick;
	public static EventHandler<EventArgs> OnHealthyToSick;
	public static EventHandler<EventArgs> OnSickToHealthy;
	public static EventHandler<EventArgs> OnVerySickToHealthy;
	public static EventHandler<EventArgs> OnSickToVerySick;
	public static EventHandler<EventArgs> OnZeroHealth;
	//-------------------------------------------------	

	private GameObject hudAnimatorObject;
	private HUDAnimator hudAnimator;
	
	// the pet animator
	public PetAnimator scriptPetAnim;
	
	// this is a bit hacky...but some levels don't have the pet, so we don't want to do pet related stuff
	private bool bCheckPet;
	
	void Start(){
		hudAnimatorObject = GameObject.Find("HUDPanel");
		hudAnimator = hudAnimatorObject.GetComponent<HUDAnimator>();
		
		if(D.Assert(hudAnimatorObject != null, "Please attach hudanimator object")){
			hudAnimator = hudAnimatorObject.GetComponent<HUDAnimator>();
			D.Assert(hudAnimator != null, "No HUDAnimator script attached");
		}
		
		bCheckPet = scriptPetAnim != null;
		
		// listen for refresh message
		WellapadMissionController.Instance.OnMissionsRefreshed += OnMissionsRefreshed;		
	}	

	#if UNITY_EDITOR || DEVELOPMENT_BUILD
//	void OnGUI(){
//		if(GUI.Button(new Rect(0, 0, 100, 50), "+health")){
//			ChangeStats(deltaHealth: 10);
//	 	}
//		 if(GUI.Button(new Rect(100, 0, 100, 50), "-health")){
//			ChangeStats(deltaHealth: -10);
//		 }
//		if(GUI.Button(new Rect(200, 0, 100, 50), "+mood")){
//			ChangeStats(deltaMood: 10);
//		}
//		if(GUI.Button(new Rect(300, 0, 100, 50), "-mood")){
//			ChangeStats(deltaMood: -10);
//		}
//		if(GUI.Button(new Rect(400, 0, 100, 50), "+xp")){
//			ChangeStats(deltaPoints: 100);
//		}
//		if(GUI.Button(new Rect(500, 0, 100, 50), "+Gems")){
//			ChangeStats(deltaGems: 5);
//		}
//		if(GUI.Button(new Rect(600, 0, 100, 50), "+Stars")){
//			ChangeStats(deltaStars: 50);
//		}
//		if(GUI.Button(new Rect(700, 0, 100, 50), "-Stars")){
//			ChangeStats(deltaStars: -40);
//		}
//
//	}
	#endif

	public int GetStat(HUDElementType stat){
		int statNumber = 0;
		switch(stat){
		case HUDElementType.Points:
			statNumber = DataManager.Instance.GameData.Stats.Points;
			break;
		case HUDElementType.Health:
			statNumber = DataManager.Instance.GameData.Stats.Health;
			break;
		case HUDElementType.Mood:
			statNumber = DataManager.Instance.GameData.Stats.Mood;
			break;
		case HUDElementType.Stars:
			statNumber = DataManager.Instance.GameData.Stats.Stars;
			break;
		case HUDElementType.Gems:
			statNumber = DataManager.Instance.GameData.Stats.Gems;
			break;
		default:
			Debug.LogError("No such display target for " + stat);
			break;
		}

		return statNumber;
	}

	// Locations are on screen space
	// public void ChangeStats(int deltaPoints, Vector3 pointsLoc, int deltaStars, 
	// 	Vector3 starsLoc, int deltaHealth, Vector3 healthLoc, int deltaMood, Vector3 moodLoc){
	// 	ChangeStats(deltaPoints, pointsLoc, deltaStars, starsLoc, deltaHealth, healthLoc, deltaMood, moodLoc, true);
	// }
	
	/// <summary>
	/// Changes the stats.
	/// </summary>
	/// <param name="deltaPoints">Delta points.</param>
	/// <param name="pointsLoc">Points location.</param>
	/// <param name="deltaStars">Delta stars.</param>
	/// <param name="starsLoc">Stars location.</param>
	/// <param name="deltaGems">Delta gems.</param>
	/// <param name="gemsLoc">Gems location.</param>
	/// <param name="deltaHealth">Delta health.</param>
	/// <param name="healthLoc">Health location.</param>
	/// <param name="deltaMood">Delta mood.</param>
	/// <param name="moodLoc">Mood location.</param>
	/// <param name="bPlaySounds">If set to <c>true</c> play sounds.</param>
	/// <param name="bAtOnce">If set to <c>true</c> animate all stats at once.</param>
	/// <param name="bFloaty">If set to <c>true</c> spawn floaty on the pet. (this will not play sound)</param>
	public void ChangeStats(int deltaPoints = 0, Vector3 pointsLoc = default(Vector3), 
	                        int deltaStars = 0, Vector3 starsLoc = default(Vector3),
	                        int deltaGems = 0, Vector3 gemsLoc = default(Vector3),
	                        int deltaHealth = 0, Vector3 healthLoc = default(Vector3), 
	    					int deltaMood = 0, Vector3 moodLoc = default(Vector3), 
							bool bPlaySounds = true, bool bAtOnce = false, bool bFloaty = false){

		// Make necessary changes in the DataManager and HUDAnimator
		if(deltaPoints != 0){
			if(deltaPoints > 0)
				DataManager.Instance.GameData.Stats.AddPoints(deltaPoints);
			else if(deltaPoints < 0)
				DataManager.Instance.GameData.Stats.SubtractPoints(-1 * deltaPoints);	// Wonky logic, accomodating here
		}
	
		if(deltaStars != 0){
			if(deltaStars > 0){
				DataManager.Instance.GameData.Stats.AddStars(deltaStars);

				//Check if there are enough coins/stars to unlock badge
				BadgeLogic.Instance.CheckSeriesUnlockProgress(BadgeType.Coin, 
					GetStat(HUDElementType.Stars), true);
			}
			else if(deltaStars < 0)
				DataManager.Instance.GameData.Stats.SubtractStars(-1 * deltaStars);
		}

		if(deltaGems != 0){
			if(deltaGems > 0){
				DataManager.Instance.GameData.Stats.AddGems(deltaGems);
			}
			else if(deltaGems < 0){
				DataManager.Instance.GameData.Stats.SubstractGems(-1 * deltaGems);
			}
		}
		
		// so that the pet animations play properly, make sure to change and check mood BEFORE health
		if(deltaMood != 0){
			
			PetMoods eOld = DataManager.Instance.GameData.Stats.GetMoodState();

			if(deltaMood > 0){
				DataManager.Instance.GameData.Stats.AddMood(deltaMood);
			}
			else if(deltaMood < 0){
				DataManager.Instance.GameData.Stats.SubtractMood(-1 * deltaMood);
			}
			
			PetMoods eNew = DataManager.Instance.GameData.Stats.GetMoodState();
			
			if(bCheckPet)
				CheckForMoodTransition(eOld, eNew);
		}		
		
		if(deltaHealth != 0){
			PetHealthStates eOldHealth = DataManager.Instance.GameData.Stats.GetHealthState();
			if(deltaHealth > 0)
				DataManager.Instance.GameData.Stats.AddHealth(deltaHealth);
			else if(deltaHealth < 0)
				DataManager.Instance.GameData.Stats.SubtractHealth(-1 * deltaHealth);
			PetHealthStates eNewHealth = DataManager.Instance.GameData.Stats.GetHealthState();
			
			if(bCheckPet){
				CheckForHealthTransition(eOldHealth, eNewHealth);
				CheckForZeroHealth();
			}
		}
		
		if(bFloaty && !bBeingDestroyed && PetFloatyUIManager.Instance){
			PetFloatyUIManager.Instance.CreateStatsFloaty(deltaPoints, deltaHealth, deltaMood, deltaStars);
		}
			
		// Tell HUDAnimator to animate and change
		List<StatPair> listStats = new List<StatPair>();
		listStats.Add(new StatPair(HUDElementType.Points, deltaPoints, pointsLoc, deltaPoints > 0 ? hudAnimator.soundXP : null));
		listStats.Add(new StatPair(HUDElementType.Stars, deltaStars, starsLoc, deltaStars > 0 ? hudAnimator.soundStars : null));
		listStats.Add(new StatPair(HUDElementType.Gems, deltaGems, gemsLoc, deltaGems > 0 ? hudAnimator.soundStars : null));
		listStats.Add(new StatPair(HUDElementType.Health, deltaHealth, healthLoc));
		listStats.Add(new StatPair(HUDElementType.Mood, deltaMood, moodLoc));
		
		if(hudAnimator != null && !bBeingDestroyed)
			StartCoroutine(hudAnimator.StartCurveStats(listStats, bPlaySounds, bAtOnce, bFloaty));
	}	

	//---------------------------------------------------
	// CheckForMoodTransition()
	// Checks to see if a mood transition is appropriate,
	// and if so, kicks it off on the pet animator.
	//---------------------------------------------------		
	private void CheckForMoodTransition(PetMoods eOld, PetMoods eNew){
		if(bBeingDestroyed)
			return;
		
		// if, at this moment, the pet is not healthy, there will be no mood transitions
		PetHealthStates eHealth = DataManager.Instance.GameData.Stats.GetHealthState();
		if(eHealth != PetHealthStates.Healthy)
			return;
		
		// otherwise, let's actually check for a transition
		if(eOld == PetMoods.Happy && eNew == PetMoods.Sad){
			// pet is going from happy to sad
			scriptPetAnim.Transition("Transition_HappySad");

			// fire event to notify listeners
			if(OnHappyToSad != null)
				OnHappyToSad(this, EventArgs.Empty);
		}
		else if(eOld == PetMoods.Sad && eNew == PetMoods.Happy){
			// pet is going from sad to happy	
			scriptPetAnim.Transition("Transition_SadHappy");

			// fire event
			if(OnSadToHappy != null)
				OnSadToHappy(this, EventArgs.Empty);
		}
	}
	
	//---------------------------------------------------
	// CheckForHealthTransition()
	// Checks to see if a health transition is appropriate,
	// and if so, kicks it off on the pet animator.  This
	// is kind of messy.
	//---------------------------------------------------	
	private void CheckForHealthTransition(PetHealthStates eOld, PetHealthStates eNew){
		// there are a bunch of cases here

		//HealthyHappySick --> SickVerySick or HealthySadSick --> SickVerySick
		if(eOld == PetHealthStates.Healthy && eNew == PetHealthStates.VerySick){
			// if the pet has gone from health to very sick in one fell swoop, we need to queue up both transitions
			PetMoods mood = DataManager.Instance.GameData.Stats.GetMoodState();	

			if(mood == PetMoods.Happy)
				scriptPetAnim.Transition("Transition_HealthyHappySick");
			else if(mood == PetMoods.Sad)
				scriptPetAnim.Transition("Transition_HealthySadSick");

			scriptPetAnim.Transition("Transition_SickVerySick");

			if(OnHealthyToVerySick != null){
				OnHealthyToVerySick(this, EventArgs.Empty);
			}
		}

		// Healthy --> HappySick or Healthy --> SadSick
		else if(eOld == PetHealthStates.Healthy && eNew == PetHealthStates.Sick){
			PetMoods mood = DataManager.Instance.GameData.Stats.GetMoodState();	

			if(mood == PetMoods.Happy)
				scriptPetAnim.Transition("Transition_HealthyHappySick");
			else if(mood == PetMoods.Sad)
				scriptPetAnim.Transition("Transition_HealthySadSick");

			if(OnHealthyToSick != null)
				OnHealthyToSick(this, EventArgs.Empty);
				
		}

		// VerySick --> HealthyHappy or VerySick --> HealthySad
		else if(eOld == PetHealthStates.VerySick && eNew == PetHealthStates.Healthy){
			// pet is going from very sick to healthy; play both transitions
			scriptPetAnim.Transition("Transition_VerySickSick");

			PetMoods mood = DataManager.Instance.GameData.Stats.GetMoodState();	

			if(mood == PetMoods.Happy)
				scriptPetAnim.Transition("Transition_SickHealthyHappy");
			else if(mood == PetMoods.Sad)
				scriptPetAnim.Transition("Transition_SickHealthySad");

			if(OnVerySickToHealthy != null)
				OnVerySickToHealthy(this, EventArgs.Empty);
		}

		// Sick --> HealthyHappy or Sick --> HealthySad
		else if(eOld == PetHealthStates.Sick && eNew == PetHealthStates.Healthy){
			PetMoods mood = DataManager.Instance.GameData.Stats.GetMoodState();	

			if(mood == PetMoods.Happy)
				scriptPetAnim.Transition("Transition_SickHealthyHappy");
			else if(mood == PetMoods.Sad)
				scriptPetAnim.Transition("Transition_SickHealthySad");

			if(OnSickToHealthy != null)
				OnSickToHealthy(this, EventArgs.Empty);
		}

		// Sick --> VerySick
		else if(eOld == PetHealthStates.Sick && eNew == PetHealthStates.VerySick){
			scriptPetAnim.Transition("Transition_SickVerySick");

			if(OnSickToVerySick != null)
				OnSickToVerySick(this, EventArgs.Empty);

		}

		// VerySick --> Sick
		else if(eOld == PetHealthStates.VerySick && eNew == PetHealthStates.Sick)
			scriptPetAnim.Transition("Transition_VerySickSick");
		
	}

	//---------------------------------------------------	
	// CheckForZeroHealth()
	// Check to see if pet's health reaches zero. fire event
	// if so 
	//---------------------------------------------------	
	private void CheckForZeroHealth(){
		int health = DataManager.Instance.GameData.Stats.Health;

		if(health <= 0)
		if(OnZeroHealth != null)
			OnZeroHealth(this, EventArgs.Empty);
	}
	
	
	//---------------------------------------------------
	// GetStatText()
	// Returns the localized stat text for incoming
	// stat id.
	//---------------------------------------------------	
	public string GetStatText(StatType eStat){
		string strKey = "STAT_" + eStat;
		string strLocalizedStat = Localization.Localize(strKey);
		
		return strLocalizedStat;
	}

	/// <summary>
	/// Gets the name of the stat icon.
	/// Returns the sprite name of the icon for the 
	/// incoming stat.
	/// </summary>
	/// <returns>The stat icon name.</returns>
	/// <param name="eStat">E stat.</param>
	public string GetStatIconName(HUDElementType eStat){
		string strKey = "PetStatsIcon_" + eStat;
		string strSprite = Constants.GetConstant<string>(strKey);
		return strSprite;
	}

	/// <summary>
	/// Changes the fire breaths the pet has.
	/// </summary>
	/// <param name="amount">Amount.</param>
	public void ChangeFireBreaths(int amount){
		int breaths = DataManager.Instance.GameData.PetInfo.FireBreaths;
		int newBreaths = breaths + amount;
		SetFireBreaths(newBreaths);
	}

	private void SetFireBreaths(int amount){
		DataManager.Instance.GameData.PetInfo.SetFireBreaths(amount);
		
		// send out an event that fire breaths have changed
//		if(OnBreathsChanged != null)
//			OnBreathsChanged(this, EventArgs.Empty);		
	}
	
	//---------------------------------------------------
	// OnMissionsRefreshed()
	// When the user's current missions expire and must
	// be refreshed.
	//---------------------------------------------------		
	private void OnMissionsRefreshed(object sender, EventArgs args){
		// if the missions are refreshing, make sure the player can no longer breath fire
		SetFireBreaths(0);
	}	
}
