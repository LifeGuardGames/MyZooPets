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
	private HUDAnimator hudAnimator;
	
	void Start(){
		if(hudAnimatorObject == null)
			Debug.LogError("Please attach hudanimator object!");
		else{
			hudAnimator = hudAnimatorObject.GetComponent<HUDAnimator>();
			if(hudAnimator == null)
				Debug.LogError("No HUDAnimator script attached");
		}
	}
	
	public void ChangeStats(int deltaPoints, int deltaStars, int deltaHealth, int deltaMood, Vector3 ScreenCoordinate){
		
		// Make necessary changes in the DataManager and HUDAnimator
		if(deltaPoints != 0){
			if(deltaPoints > 0)
				DataManager.AddPoints(deltaPoints);
			else if(deltaPoints < 0)
				DataManager.SubtractPoints(-1 * deltaPoints);	// Wonky logic, accomodating here
			
			// Tell HUDAnimator to animate and change
//			hudAnimator.StartCurvePoints(deltaPoints);
		}
	
		if(deltaStars != 0){
			if(deltaStars > 0)
				DataManager.AddStars(deltaStars);
			else if(deltaStars < 0)
				DataManager.SubtractStars(-1 * deltaStars);
		
			// Tell HUDAnimator to animate and change
//			hudAnimator.StartCurveStars(deltaStars);
		}
		
		if(deltaHealth != 0){
			if(deltaHealth > 0)
				DataManager.AddHealth(deltaHealth);
			else if(deltaHealth < 0)
				DataManager.SubtractHealth(-1 * deltaHealth);
		
			// Tell HUDAnimator to animate and change
//			hudAnimator.StartCurveHealth(deltaHealth);
		}
		
		if(deltaMood != 0){
			if(deltaMood > 0)
				DataManager.AddMood(deltaMood);
			else if(deltaMood < 0)
				DataManager.SubtractMood(-1 * deltaMood);
		
			// Tell HUDAnimator to animate and change
//			hudAnimator.StartCurveMood(deltaMood);
		}
		
		hudAnimator.StartCoroutineCurveStats(deltaPoints, Vector3.zero, deltaStars, Vector3.zero, deltaHealth, Vector3.zero, deltaMood, Vector3.zero);
	}	
}
