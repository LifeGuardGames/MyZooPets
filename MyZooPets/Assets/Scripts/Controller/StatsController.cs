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

	public HUDAnimator hudAnimator;
	private bool isPetAnimationManagerPresent;
	private PanToMoveCamera scriptPan;
	void Awake(){
		// set pan script
		scriptPan = CameraManager.Instance.PanScript;
	}
	void Start(){
		// Check if hud animator is assigned already
		if(hudAnimator == null){
			GameObject hudAnimatorObject = GameObject.Find("HUDPanelUI");
			hudAnimator = hudAnimatorObject.GetComponent<HUDAnimator>();

			if(hudAnimator == null){
				Debug.LogError("Hud Animator can not be found in " + Application.loadedLevelName);
			}
		}
		
		isPetAnimationManagerPresent = PetAnimationManager.Instance;
		
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
//		if(GUI.Button(new Rect(600, 0, 100, 50), "+Stars")){
//			ChangeStats(deltaStars: 50);
//		}
//		if(GUI.Button(new Rect(700, 0, 100, 50), "-Stars")){
//			ChangeStats(deltaStars: -40);
//		}
//
//
//		if(GUI.Button(new Rect(0, 50, 100, 50), "+health")){
//			ChangeStats(deltaHealth: 100);
//		}

//		if(GUI.Button(new Rect(200, 50, 100, 50), "+mood")){
//			ChangeStats(deltaMood: 10);
//		}
//		if(GUI.Button(new Rect(300, 50, 100, 50), "-mood")){
//			ChangeStats(deltaMood: -10);
//		}
//		if(GUI.Button(new Rect(400, 50, 100, 50), "+xp")){
//			ChangeStats(deltaPoints: 100);
//		}
//		if(GUI.Button(new Rect(600, 50, 100, 50), "+Stars")){
//			ChangeStats(deltaStars: 50);
//		}
//		if(GUI.Button(new Rect(700, 50, 100, 50), "-Stars")){
//			ChangeStats(deltaStars: -40);
//		}
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
		default:
			Debug.LogError("No such display target for " + stat);
			break;
		}

		return statNumber;
	}
	
	/// <summary>
	/// Changes the stats.
	/// Locations are on screen space
	/// </summary>
	/// <param name="deltaPoints">Delta points.</param>
	/// <param name="pointsLoc">Points location.</param>
	/// <param name="deltaStars">Delta stars.</param>
	/// <param name="starsLoc">Stars location.</param>
	/// <param name="deltaHealth">Delta health.</param>
	/// <param name="healthLoc">Health location.</param>
	/// <param name="deltaMood">Delta mood.</param>
	/// <param name="moodLoc">Mood location.</param>
	/// <param name="bPlaySounds">If set to <c>true</c> play sounds.</param>
	/// <param name="bAtOnce">If set to <c>true</c> animate all stats at once.</param>
	/// <param name="bFloaty">If set to <c>true</c> spawn floaty on the pet. (this will not play sound)</param>
	/// <param name="isInternal">If set to <c>true</c> skip all animations / rewarding</param>
	public void ChangeStats(int deltaPoints = 0, Vector3 pointsLoc = default(Vector3),
	                        int deltaStars = 0, Vector3 starsLoc = default(Vector3),
	                        int deltaHealth = 0, Vector3 healthLoc = default(Vector3), 
	    					int deltaMood = 0, Vector3 moodLoc = default(Vector3),
							bool isPlaySounds = true, bool isAllAtOnce = false, bool isFloaty = false,
	                        bool isDisableStream = false, bool is3DObject = false, float animDelay = 0f,
	                        bool isInternal = false){;
		// Make necessary changes in the DataManager and HUDAnimator
		if(deltaPoints != 0){
			if(deltaPoints > 0){
				DataManager.Instance.GameData.Stats.AddPoints(deltaPoints);
			}
			else if(deltaPoints < 0){
				DataManager.Instance.GameData.Stats.SubtractPoints(-1 * deltaPoints);	// Wonky logic, accomodating here
			}
		}
	
		if(deltaStars != 0){
			if(deltaStars > 0){
				DataManager.Instance.GameData.Stats.AddStars(deltaStars);
			}
			else if(deltaStars < 0){
				DataManager.Instance.GameData.Stats.SubtractStars(-1 * deltaStars);
			}
		}
		
		// so that the pet animations play properly, make sure to change and check mood BEFORE health
		if(deltaMood != 0){
			
			PetMoods oldMood = DataManager.Instance.GameData.Stats.GetMoodState();

			if(deltaMood > 0){
				DataManager.Instance.GameData.Stats.AddMood(deltaMood);
			}
			else if(deltaMood < 0){
				DataManager.Instance.GameData.Stats.SubtractMood(-1 * deltaMood);
			}
			
			PetMoods newMood = DataManager.Instance.GameData.Stats.GetMoodState();
			
			if(isPetAnimationManagerPresent)
				CheckForMoodTransition(oldMood, newMood);
		}		
		
		if(deltaHealth != 0){
			PetHealthStates oldHealth = DataManager.Instance.GameData.Stats.GetHealthState();

			if(deltaHealth > 0){
				DataManager.Instance.GameData.Stats.AddHealth(deltaHealth);
			}
			else if(deltaHealth < 0){
				DataManager.Instance.GameData.Stats.SubtractHealth(-1 * deltaHealth);
			}

			PetHealthStates newHealth = DataManager.Instance.GameData.Stats.GetHealthState();
			
			if(isPetAnimationManagerPresent){
				CheckForHealthTransition(oldHealth, newHealth);
				CheckForZeroHealth();
			}
		}

		// If internal checked, skip all animations and reward checking
		if(isInternal == false){
			if(isFloaty && !bBeingDestroyed && PetFloatyUIManager.Instance){
				PetFloatyUIManager.Instance.CreateStatsFloaty(deltaPoints, deltaHealth, deltaMood, deltaStars);
			}

			//when stats are modified make sure PetAnimationManager knows about it
			if(isPetAnimationManagerPresent){
				PetAnimationManager.Instance.PetStatsModified(DataManager.Instance.GameData.Stats.Health,
				                                              DataManager.Instance.GameData.Stats.Mood);
			}

			// Adjust for custom positions using screen position for 3D objects
			if(is3DObject){
				if(pointsLoc != default(Vector3)){
					// WorldToScreen returns screen coordinates based on 0,0 being bottom left, so we need to transform those into respective NGUI Anchors
					pointsLoc = CameraManager.Instance.WorldToScreen(CameraManager.Instance.CameraMain, pointsLoc);
					InterfaceAnchors endAnchor = (InterfaceAnchors)Enum.Parse(typeof(InterfaceAnchors), Constants.GetConstant<String>("Points_Anchor"));
					pointsLoc = CameraManager.Instance.TransformAnchorPosition(pointsLoc, InterfaceAnchors.BottomLeft, endAnchor);
				}
				if(starsLoc != default(Vector3)){
					starsLoc = CameraManager.Instance.WorldToScreen(CameraManager.Instance.CameraMain, starsLoc);
					InterfaceAnchors endAnchor = (InterfaceAnchors)Enum.Parse(typeof(InterfaceAnchors), Constants.GetConstant<String>("Stars_Anchor"));
					starsLoc = CameraManager.Instance.TransformAnchorPosition(starsLoc, InterfaceAnchors.BottomLeft, endAnchor);
				}
				if(healthLoc != default(Vector3)){
					healthLoc = CameraManager.Instance.WorldToScreen(CameraManager.Instance.CameraMain, healthLoc);
					InterfaceAnchors endAnchor = (InterfaceAnchors)Enum.Parse(typeof(InterfaceAnchors), Constants.GetConstant<String>("Health_Anchor"));
					healthLoc = CameraManager.Instance.TransformAnchorPosition(healthLoc, InterfaceAnchors.BottomLeft, endAnchor);
				}
				if(moodLoc != default(Vector3)){
					moodLoc = CameraManager.Instance.WorldToScreen(CameraManager.Instance.CameraMain, moodLoc);
					InterfaceAnchors endAnchor = (InterfaceAnchors)Enum.Parse(typeof(InterfaceAnchors), Constants.GetConstant<String>("Mood_Anchor"));
					moodLoc = CameraManager.Instance.TransformAnchorPosition(moodLoc, InterfaceAnchors.BottomLeft, endAnchor);
				}
			}
			// Adjust for custom position using screen position for NGUI objects
			else{
				// Not needed yet
			}

			// Tell HUDAnimator to animate and change
			List<StatPair> listStats = new List<StatPair>();
			listStats.Add(new StatPair(HUDElementType.Points, deltaPoints, pointsLoc, deltaPoints > 0 ? hudAnimator.soundXP : null));
			listStats.Add(new StatPair(HUDElementType.Stars, deltaStars, starsLoc, deltaStars > 0 ? hudAnimator.soundStars : null));
			listStats.Add(new StatPair(HUDElementType.Health, deltaHealth, healthLoc));
			listStats.Add(new StatPair(HUDElementType.Mood, deltaMood, moodLoc));
			
			if(hudAnimator != null && !bBeingDestroyed){
				// Push this into the reward queue
				RewardQueueData.GenericDelegate function1 = delegate{
					StartCoroutine(hudAnimator.StartCurveStats(listStats, isPlaySounds, isAllAtOnce, isFloaty, animDelay));
				};
				RewardManager.Instance.AddToRewardQueue(function1);
			}

		//Check if there are enough coins/stars to unlock badge, we want to do this last after reward
		BadgeLogic.Instance.CheckSeriesUnlockProgress(BadgeType.Coin, DataManager.Instance.GameData.Stats.totalStars, true);
		}
	}

	//---------------------------------------------------
	// CheckForMoodTransition()
	// Checks to see if a mood transition is appropriate,
	// and if so, kicks it off on the pet animator.
	//---------------------------------------------------		
	private void CheckForMoodTransition(PetMoods oldMood, PetMoods newMood){
		if(bBeingDestroyed)
			return;
		
		// if, at this moment, the pet is not healthy, there will be no mood transitions
		PetHealthStates health = DataManager.Instance.GameData.Stats.GetHealthState();
		if(health != PetHealthStates.Healthy)
			return;
		
		// otherwise, let's actually check for a transition
		if(oldMood == PetMoods.Happy && newMood == PetMoods.Sad){
			// fire event to notify listeners
			if(OnHappyToSad != null)
				OnHappyToSad(this, EventArgs.Empty);
		}
		else if(oldMood == PetMoods.Sad && newMood == PetMoods.Happy){
			if(GatingManager.Instance.activeGates.ContainsKey(scriptPan.currentLocalPartition)){
				GatingManager.Instance.CanNowBlowFire();
			}
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
	private void CheckForHealthTransition(PetHealthStates oldHealth, PetHealthStates newHealth){
		// there are a bunch of cases here

		//HealthyHappySick --> SickVerySick or HealthySadSick --> SickVerySick
		if(oldHealth == PetHealthStates.Healthy && newHealth == PetHealthStates.VerySick){
			if(OnHealthyToVerySick != null){
				OnHealthyToVerySick(this, EventArgs.Empty);
			}
		}

		// Healthy --> HappySick or Healthy --> SadSick
		else if(oldHealth == PetHealthStates.Healthy && newHealth == PetHealthStates.Sick){
			if(OnHealthyToSick != null)
				OnHealthyToSick(this, EventArgs.Empty);
		}

		// VerySick --> HealthyHappy or VerySick --> HealthySad
		else if(oldHealth == PetHealthStates.VerySick && newHealth == PetHealthStates.Healthy){
			if( GatingManager.Instance.activeGates.ContainsKey(scriptPan.currentLocalPartition)){
				GatingManager.Instance.CanNowBlowFire();
			}
			if(OnVerySickToHealthy != null)
				OnVerySickToHealthy(this, EventArgs.Empty);
		}

		// Sick --> HealthyHappy or Sick --> HealthySad
		else if(oldHealth == PetHealthStates.Sick && newHealth == PetHealthStates.Healthy){
			if(GatingManager.Instance.activeGates.ContainsKey(scriptPan.currentLocalPartition)){
				GatingManager.Instance.CanNowBlowFire();
			}
			if(OnSickToHealthy != null)
				OnSickToHealthy(this, EventArgs.Empty);
		}

		// Sick --> VerySick
		else if(oldHealth == PetHealthStates.Sick && newHealth == PetHealthStates.VerySick){
			if(OnSickToVerySick != null)
				OnSickToVerySick(this, EventArgs.Empty);

		}	
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
	public string GetStatText(StatType statType){
		string key = "STAT_" + statType;
		string localizedStat = Localization.Localize(key);
		
		return localizedStat;
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
