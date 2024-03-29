using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

/// <summary>
/// Stats controller.
/// Takes care of modifying values in datamanager as well as controlling Hud animator
/// 
/// 				  DataManager (model)
/// 				/
/// 	StatsManager
/// 				\
/// 				  HUDAnimator (view)
/// </summary>
public class StatsManager : Singleton<StatsManager>{
	// Mood change events
	public static EventHandler<EventArgs> OnHappyToSad; 
	public static EventHandler<EventArgs> OnSadToHappy;
	public static EventHandler<EventArgs> OnHealthyToVerySick;
	public static EventHandler<EventArgs> OnHealthyToSick;
	public static EventHandler<EventArgs> OnSickToHealthy;
	public static EventHandler<EventArgs> OnVerySickToHealthy;
	public static EventHandler<EventArgs> OnSickToVerySick;
	public static EventHandler<EventArgs> OnZeroHealth;

	private HUDAnimator hudAnimator;
	private bool isPetAnimationManagerPresent;
	private PanToMoveCamera scriptPan;

	void Awake(){
		// set pan script
		if(SceneManager.GetActiveScene().name == SceneUtils.BEDROOM || SceneManager.GetActiveScene().name == SceneUtils.YARD) {
			scriptPan = CameraManager.Instance.PanScript;
		}
	}

	void Start(){
		hudAnimator = HUDUIManager.Instance.HudAnimator;
		isPetAnimationManagerPresent = PetAnimationManager.Instance != null;
		
		// listen for refresh message
		WellapadMissionController.Instance.OnMissionsRefreshed += OnMissionsRefreshed;		
	}

	public int GetStat(StatType stat){
		switch(stat){
		case StatType.Xp:
			return DataManager.Instance.GameData.Stats.Points;
		case StatType.Health:
			return DataManager.Instance.GameData.Stats.Health;
		case StatType.Hunger:
			return DataManager.Instance.GameData.Stats.Mood;
		case StatType.Coin:
			return DataManager.Instance.GameData.Stats.Stars;
		default:
			Debug.LogError("No such display target for " + stat);
			return 0;
		}
	}

	/// <summary>
	/// Changes the stats.
	/// Locations are on screen space
	/// </summary>
	/// <param name="xpDelta">Delta points.</param>
	/// <param name="xpPos">Points location.</param>
	/// <param name="coinsDelta">Delta stars.</param>
	/// <param name="coinsPos">Stars location.</param>
	/// <param name="healthDelta">Delta health.</param>
	/// <param name="healthPos">Health location.</param>
	/// <param name="hungerDelta">Delta mood.</param>
	/// <param name="hungerPos">Mood location.</param>
	/// <param name="bFloaty">If set to <c>true</c> spawn floaty on the pet. (this will not play sound)</param>
	/// <param name="isInternal">If set to <c>true</c> skip all animations + rewarding</param>
	public void ChangeStats(int xpDelta = 0, Vector3 xpPos = default(Vector3),
	                        int coinsDelta = 0, Vector3 coinsPos = default(Vector3),
	                        int healthDelta = 0, Vector3 healthPos = default(Vector3), 
	    					int hungerDelta = 0, Vector3 hungerPos = default(Vector3),
							bool isFloaty = false, bool is3DObject = false, float animDelay = 0f,
	                        bool isInternal = false){

		// Make necessary changes in the DataManager and HUDAnimator
		if(xpDelta != 0){
			if(xpDelta > 0){
				DataManager.Instance.GameData.Stats.AddCurrentLevelXp(xpDelta);
			}
			else if(xpDelta < 0) {  // Invalid case
				Debug.LogError("Subtracting experience points");
			}
		}
	
		if(coinsDelta != 0){
			DataManager.Instance.GameData.Stats.UpdateCoins(coinsDelta);
		}
		
		// NOTE: so that the pet animations play properly, make sure to change and check mood BEFORE health
		if(hungerDelta != 0){
			PetMoods oldMood = DataManager.Instance.GameData.Stats.GetMoodState();
			DataManager.Instance.GameData.Stats.UpdateHunger(hungerDelta);
			PetMoods newMood = DataManager.Instance.GameData.Stats.GetMoodState();

			if(isPetAnimationManagerPresent) {
				CheckForMoodTransition(oldMood, newMood);
			}
		}		
		
		if(healthDelta != 0){
			PetHealthStates oldHealth = DataManager.Instance.GameData.Stats.GetHealthState();
			DataManager.Instance.GameData.Stats.UpdateHealth(healthDelta);
			PetHealthStates newHealth = DataManager.Instance.GameData.Stats.GetHealthState();
			
			if(isPetAnimationManagerPresent){
				CheckForHealthTransition(oldHealth, newHealth);
				CheckForZeroHealth();
			}
		}

		// If internal checked, skip all animations and reward checking
		if(isInternal == false){
			if(isFloaty && !bBeingDestroyed && PetFloatyUIManager.Instance){
				PetFloatyUIManager.Instance.CreateStatsFloaty(xpDelta, healthDelta, hungerDelta, coinsDelta);
			}

			//when stats are modified make sure PetAnimationManager knows about it
			if(isPetAnimationManagerPresent){
				PetAnimationManager.Instance.PetStatsModified(DataManager.Instance.GameData.Stats.Health,
				                                              DataManager.Instance.GameData.Stats.Mood);
			}

			// Adjust for custom positions using screen position for 3D objects
			if(is3DObject){
				if(xpPos != default(Vector3)){
					// WorldToScreen returns screen coordinates based on 0,0 being bottom left, so we need to transform those into respective NGUI Anchors
					xpPos = CameraManager.Instance.WorldToScreen(CameraManager.Instance.CameraMain, xpPos);
					Debug.LogWarning("COMMENTED OUT THINGS HERE, fix");
					//InterfaceAnchors endAnchor = (InterfaceAnchors)Enum.Parse(typeof(InterfaceAnchors), Constants.GetConstant<String>("Points_Anchor"));
					//xpPos = CameraManager.Instance.TransformAnchorPosition(xpPos, InterfaceAnchors.BottomLeft, endAnchor);
				}
				if(coinsPos != default(Vector3)){
					coinsPos = CameraManager.Instance.WorldToScreen(CameraManager.Instance.CameraMain, coinsPos);
					Debug.LogWarning("COMMENTED OUT THINGS HERE, fix");
					//InterfaceAnchors endAnchor = (InterfaceAnchors)Enum.Parse(typeof(InterfaceAnchors), Constants.GetConstant<String>("Stars_Anchor"));
					//coinsPos = CameraManager.Instance.TransformAnchorPosition(coinsPos, InterfaceAnchors.BottomLeft, endAnchor);
				}
				if(healthPos != default(Vector3)){
					healthPos = CameraManager.Instance.WorldToScreen(CameraManager.Instance.CameraMain, healthPos);
					Debug.LogWarning("COMMENTED OUT THINGS HERE, fix");
					//InterfaceAnchors endAnchor = (InterfaceAnchors)Enum.Parse(typeof(InterfaceAnchors), Constants.GetConstant<String>("Health_Anchor"));
					//healthPos = CameraManager.Instance.TransformAnchorPosition(healthPos, InterfaceAnchors.BottomLeft, endAnchor);
				}
				if(hungerPos != default(Vector3)){
					hungerPos = CameraManager.Instance.WorldToScreen(CameraManager.Instance.CameraMain, hungerPos);
					Debug.LogWarning("COMMENTED OUT THINGS HERE, fix");
					//InterfaceAnchors endAnchor = (InterfaceAnchors)Enum.Parse(typeof(InterfaceAnchors), Constants.GetConstant<String>("Mood_Anchor"));
					//hungerPos = CameraManager.Instance.TransformAnchorPosition(hungerPos, InterfaceAnchors.BottomLeft, endAnchor);
				}
			}
			// Adjust for custom position using screen position for NGUI objects
			else{
				// Not needed yet
			}

			// Tell HUDAnimator to animate and change
			List<StatPair> listStats = new List<StatPair>();
			listStats.Add(new StatPair(StatType.Xp, xpDelta, xpPos, xpDelta > 0 ? "XpSingleTick" : null));
            listStats.Add(new StatPair(StatType.Coin, coinsDelta, coinsPos, coinsDelta > 0 ? "CoinSingleTick" : null));
            listStats.Add(new StatPair(StatType.Health, healthDelta, healthPos, healthDelta > 0 ? "HealthSingleTick" : null));
			listStats.Add(new StatPair(StatType.Hunger, hungerDelta, hungerPos, hungerDelta > 0 ? "HungerSingleTick" : null));

			if(hudAnimator != null && !bBeingDestroyed){
				// Push this into the reward queue
				RewardQueueData.GenericDelegate function1 = delegate{
					StartCoroutine(hudAnimator.StartCurveStats(listStats, isFloaty, animDelay));
				};
				RewardManager.Instance.AddToRewardQueue(function1);
			}

			//Check if there are enough coins/stars to unlock badge, we want to do this last after reward
			BadgeManager.Instance.CheckSeriesUnlockProgress(BadgeType.Coin, DataManager.Instance.GameData.Stats.TotalStars, true);
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

	// Check to see if pet's health reaches zero. fire event if so 
	private void CheckForZeroHealth(){
		if(DataManager.Instance.GameData.Stats.Health <= 0) {
			if(OnZeroHealth != null) {
				OnZeroHealth(this, EventArgs.Empty);
			}
		}
	}

	#region Fire breathing logic
	public void UpdateFireBreaths(int deltaAmount){
		SetFireBreaths(DataManager.Instance.GameData.PetInfo.FireBreaths + deltaAmount);
	}

	private void SetFireBreaths(int amount){
		DataManager.Instance.GameData.PetInfo.SetFireBreaths(amount);	
	}

	/// <summary>
	/// When the user's current missions expire and must be refreshed.
	/// </summary>
	private void OnMissionsRefreshed(object sender, EventArgs args){
		SetFireBreaths(0);      // if the missions are refreshing, make sure the player can no longer breath fire
	}
	#endregion

	#if UNITY_EDITOR || DEVELOPMENT_BUILD
//	void OnGUI() {
//		if(GUI.Button(new Rect(0, 0, 100, 50), "+health")) {
//			ChangeStats(healthDelta: 10);
//		}
//		if(GUI.Button(new Rect(100, 0, 100, 50), "-health")) {
//			ChangeStats(healthDelta: -10);
//		}
//		if(GUI.Button(new Rect(200, 0, 100, 50), "+mood")) {
//			ChangeStats(hungerDelta: 10);
//		}
//		if(GUI.Button(new Rect(300, 0, 100, 50), "-mood")) {
//			ChangeStats(hungerDelta: -10);
//		}
//		if(GUI.Button(new Rect(400, 0, 100, 50), "+xp")) {
//			ChangeStats(xpDelta: 100);
//		}
//		if(GUI.Button(new Rect(600, 0, 100, 50), "+Stars")) {
//			ChangeStats(coinsDelta: 200);
//		}
//		if(GUI.Button(new Rect(700, 0, 100, 50), "-Stars")) {
//			ChangeStats(coinsDelta: -40);
//		}
//	}
	#endif
}
