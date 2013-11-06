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

public class StatsController : Singleton<StatsController> {
	
	public GameObject hudAnimatorObject;
	public HUDAnimator hudAnimator;
	
	// the pet animator
	public PetAnimator scriptPetAnim;
	
	// this is a bit hacky...but some levels don't have the pet, so we don't want to do pet related stuff
	private bool bCheckPet;
	
	void Start(){
		if(D.Assert(hudAnimatorObject != null, "Please attach hudanimator object")){
			hudAnimator = hudAnimatorObject.GetComponent<HUDAnimator>();
			D.Assert(hudAnimator != null, "No HUDAnimator script attached");
		}
		
		bCheckPet = scriptPetAnim != null;
	}	
	
	// Locations are on screen space
	public void ChangeStats(int deltaPoints, Vector3 pointsLoc, int deltaStars, Vector3 starsLoc, int deltaHealth, Vector3 healthLoc, int deltaMood, Vector3 moodLoc){
		ChangeStats(deltaPoints, pointsLoc, deltaStars, starsLoc, deltaHealth, healthLoc, deltaMood, moodLoc, true );
	}
	public void ChangeStats(int deltaPoints, Vector3 pointsLoc, int deltaStars, Vector3 starsLoc, int deltaHealth, Vector3 healthLoc, int deltaMood, Vector3 moodLoc, bool bPlaySounds){
		// Make necessary changes in the DataManager and HUDAnimator
		if(deltaPoints != 0){
			if(deltaPoints > 0)
				DataManager.Instance.GameData.Stats.AddPoints(deltaPoints);
			else if(deltaPoints < 0)
				DataManager.Instance.GameData.Stats.SubtractPoints(-1 * deltaPoints);	// Wonky logic, accomodating here
		}
	
		if(deltaStars != 0){
			if(deltaStars > 0)
				DataManager.Instance.GameData.Stats.AddStars(deltaStars);
			else if(deltaStars < 0)
				DataManager.Instance.GameData.Stats.SubtractStars(-1 * deltaStars);
		}
		
		// so that the pet animations play properly, make sure to change and check mood BEFORE health
		if(deltaMood != 0){
			
			PetMoods eOld = DataManager.Instance.GameData.Stats.GetMoodState();
			if(deltaMood > 0)
				DataManager.Instance.GameData.Stats.AddMood(deltaMood);
			else if(deltaMood < 0)
				DataManager.Instance.GameData.Stats.SubtractMood(-1 * deltaMood);
			
			PetMoods eNew = DataManager.Instance.GameData.Stats.GetMoodState();
			
			if ( bCheckPet )
				CheckForMoodTransition( eOld, eNew );
		}		
		
		if(deltaHealth != 0){
			PetHealthStates eOldHealth = DataManager.Instance.GameData.Stats.GetHealthState();
			if(deltaHealth > 0)
				DataManager.Instance.GameData.Stats.AddHealth(deltaHealth);
			else if(deltaHealth < 0)
				DataManager.Instance.GameData.Stats.SubtractHealth(-1 * deltaHealth);
			PetHealthStates eNewHealth = DataManager.Instance.GameData.Stats.GetHealthState();
			
			if ( bCheckPet )
				CheckForHealthTransition( eOldHealth, eNewHealth );
		}
		
		// Tell HUDAnimator to animate and change
		List<StatPair> listStats = new List<StatPair>();
		listStats.Add( new StatPair(HUDElementType.Points, deltaPoints, pointsLoc, deltaPoints > 0 ?  hudAnimator.strSoundXP : null ) );
		listStats.Add( new StatPair(HUDElementType.Stars, deltaStars, starsLoc, deltaStars > 0 ?  hudAnimator.strSoundStars : null ) );
		listStats.Add( new StatPair(HUDElementType.Health, deltaHealth, healthLoc ) );
		listStats.Add( new StatPair(HUDElementType.Mood, deltaMood, moodLoc ) );
		StartCoroutine( hudAnimator.StartCurveStats( listStats, bPlaySounds ) );
	}	
	
	//---------------------------------------------------
	// CheckForMoodTransition()
	// Checks to see if a mood transition is appropriate,
	// and if so, kicks it off on the pet animator.
	//---------------------------------------------------		
	private void CheckForMoodTransition( PetMoods eOld, PetMoods eNew ) {
		// if, at this moment, the pet is not healthy, there will be no mood transitions
		PetHealthStates eHealth = DataManager.Instance.GameData.Stats.GetHealthState();
		if ( eHealth != PetHealthStates.Healthy )
			return;
		
		// otherwise, let's actually check for a transition
		if ( eOld == PetMoods.Happy && eNew == PetMoods.Sad ) {
			// pet is going from happy to sad
			scriptPetAnim.Transition( "Transition_HappySad" );
		}
		else if ( eOld == PetMoods.Sad && eNew == PetMoods.Happy ) {
			// pet is going from sad to happy	
			scriptPetAnim.Transition( "Transition_SadHappy" );
		}
	}
	
	//---------------------------------------------------
	// CheckForHealthTransition()
	// Checks to see if a health transition is appropriate,
	// and if so, kicks it off on the pet animator.  This
	// is kind of messy.
	//---------------------------------------------------	
	private void CheckForHealthTransition( PetHealthStates eOld, PetHealthStates eNew ) {
		// there are a bunch of cases here
		if ( eOld == PetHealthStates.Healthy && eNew == PetHealthStates.VerySick ) {
			// if the pet has gone from health to very sick in one fell swoop, we need to queue up both transitions
			scriptPetAnim.Transition( "Transition_HealthySick" );	
			scriptPetAnim.Transition( "Transition_HealthyVerySick" );
		}
		else if ( eOld == PetHealthStates.Healthy && eNew == PetHealthStates.Sick )
			scriptPetAnim.Transition( "Transition_HealthySick" );
		else if ( eOld == PetHealthStates.VerySick && eNew == PetHealthStates.Healthy ) {
			// pet is going from very sick to healthy; play both transitions
			scriptPetAnim.Transition( "Transition_VerySickSick" );
			scriptPetAnim.Transition( "Transition_SickHealthy" );
		}
		else if ( eOld == PetHealthStates.Sick && eNew == PetHealthStates.Healthy )
			scriptPetAnim.Transition( "Transition_SickHealthy" );
		else if ( eOld == PetHealthStates.Sick && eNew == PetHealthStates.VerySick )
			scriptPetAnim.Transition( "Transition_SickVerySick" );
		else if ( eOld == PetHealthStates.VerySick && eNew == PetHealthStates.Sick )
			scriptPetAnim.Transition( "Transition_VerySickSick" );
		
	}
	
	
	//---------------------------------------------------
	// GetStatText()
	// Returns the localized stat text for incoming
	// stat id.
	//---------------------------------------------------	
	public string GetStatText( StatType eStat ) {
		string strKey = "STAT_" + eStat;
		string strLocalizedStat = Localization.Localize( strKey );
		
		return strLocalizedStat;
	}
}
