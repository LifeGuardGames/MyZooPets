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
		
		if(deltaHealth != 0){
			if(deltaHealth > 0)
				DataManager.Instance.GameData.Stats.AddHealth(deltaHealth);
			else if(deltaHealth < 0)
				DataManager.Instance.GameData.Stats.SubtractHealth(-1 * deltaHealth);
		}
		
		if(deltaMood != 0){
			if(deltaMood > 0)
				DataManager.Instance.GameData.Stats.AddMood(deltaMood);
			else if(deltaMood < 0)
				DataManager.Instance.GameData.Stats.SubtractMood(-1 * deltaMood);
		}
		
		// Tell HUDAnimator to animate and change
		hudAnimator.StartCoroutineCurveStats(deltaPoints, pointsLoc, deltaStars, starsLoc, deltaHealth, healthLoc, deltaMood, moodLoc, bPlaySounds);
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
