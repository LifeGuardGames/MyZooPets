using UnityEngine;
using System.Collections;

public class DoctorMatchManager : MinigameManager<DoctorMatchManager> {

	// Move this to constant XML -------
	public float speedIncreaseInterval = 1.0f;





	//---------------------------------------------------
	// GetReward()
	//---------------------------------------------------	
	public override int GetReward(MinigameRewardTypes eType){
		// for now, just use the standard way
		return GetStandardReward(eType);
	}

	//---------------------------------------------------
	// _Start()
	//---------------------------------------------------	
	protected override void _Start(){
	}	
	
	//---------------------------------------------------
	// _OnDestroy()
	//---------------------------------------------------	
	protected override void _OnDestroy(){
	}
	
	//---------------------------------------------------
	// _NewGame()
	//---------------------------------------------------	
	protected override void _NewGame(){
	}
	
	//---------------------------------------------------
	// _GameOver()
	//---------------------------------------------------		
	protected override void _GameOver(){
	}
	
	//---------------------------------------------------
	// _Update()
	//---------------------------------------------------
	protected override void _Update(){
	}
	
	//---------------------------------------------------
	// GetMinigameKey()
	//---------------------------------------------------	
	protected override string GetMinigameKey(){
		return "DoctorMatch";	
	}	
	
	protected override bool IsTutorialOn(){
		return Constants.GetConstant<bool>("IsDoctorMatchTutorialOn");
	}
}
