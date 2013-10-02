using UnityEngine;
using System.Collections;

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
	
	void Start(){
		if(D.Assert(hudAnimatorObject != null, "Please attach hudanimator object")){
			hudAnimator = hudAnimatorObject.GetComponent<HUDAnimator>();
			D.Assert(hudAnimator != null, "No HUDAnimator script attached");
		}
	}	
	
	// Locations are on screen space
	public void ChangeStats(int deltaPoints, Vector3 pointsLoc, int deltaStars, Vector3 starsLoc, int deltaHealth, Vector3 healthLoc, int deltaMood, Vector3 moodLoc){
		ChangeStats(deltaPoints, pointsLoc, deltaStars, starsLoc, deltaHealth, healthLoc, deltaMood, moodLoc, true );
	}
	public void ChangeStats(int deltaPoints, Vector3 pointsLoc, int deltaStars, Vector3 starsLoc, int deltaHealth, Vector3 healthLoc, int deltaMood, Vector3 moodLoc, bool bPlaySounds){
		// Make necessary changes in the DataManager and HUDAnimator
		if(deltaPoints != 0){
			if(deltaPoints > 0)
				DataManager.Instance.Stats.AddPoints(deltaPoints);
			else if(deltaPoints < 0)
				DataManager.Instance.Stats.SubtractPoints(-1 * deltaPoints);	// Wonky logic, accomodating here
		}
	
		if(deltaStars != 0){
			if(deltaStars > 0)
				DataManager.Instance.Stats.AddStars(deltaStars);
			else if(deltaStars < 0)
				DataManager.Instance.Stats.SubtractStars(-1 * deltaStars);
		}
		
		// so that the pet animations play properly, make sure to change and check mood BEFORE health
		if(deltaMood != 0){
			
			PetMoods eOld = DataManager.Instance.Stats.GetMoodState();
			if(deltaMood > 0)
				DataManager.Instance.Stats.AddMood(deltaMood);
			else if(deltaMood < 0)
				DataManager.Instance.Stats.SubtractMood(-1 * deltaMood);
			
			PetMoods eNew = DataManager.Instance.Stats.GetMoodState();
			
			CheckForMoodTransition( eOld, eNew );
		}		
		
		if(deltaHealth != 0){
			PetHealthStates eOldHealth = DataManager.Instance.Stats.GetHealthState();
			if(deltaHealth > 0)
				DataManager.Instance.Stats.AddHealth(deltaHealth);
			else if(deltaHealth < 0)
				DataManager.Instance.Stats.SubtractHealth(-1 * deltaHealth);
			PetHealthStates eNewHealth = DataManager.Instance.Stats.GetHealthState();
			
			CheckForHealthTransition( eOldHealth, eNewHealth );
		}
		
		// Tell HUDAnimator to animate and change
		hudAnimator.StartCoroutineCurveStats(deltaPoints, pointsLoc, deltaStars, starsLoc, deltaHealth, healthLoc, deltaMood, moodLoc, bPlaySounds);
	}	
	
	//---------------------------------------------------
	// CheckForMoodTransition()
	// Checks to see if a mood transition is appropriate,
	// and if so, kicks it off on the pet animator.
	//---------------------------------------------------		
	private void CheckForMoodTransition( PetMoods eOld, PetMoods eNew ) {
		// if, at this moment, the pet is not healthy, there will be no mood transitions
		PetHealthStates eHealth = DataManager.Instance.Stats.GetHealthState();
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
