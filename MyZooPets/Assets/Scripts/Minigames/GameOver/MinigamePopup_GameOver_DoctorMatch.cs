using UnityEngine;
using System.Collections;

//---------------------------------------------------
// MinigamePopup_GameOver_DoctorMatch
// Game over screen for the clinic game.
//---------------------------------------------------

public class MinigamePopup_GameOver_DoctorMatch : MinigamePopup_GameOver {
	
	//---------------------------------------------------
	// GetReward()
	//---------------------------------------------------		
	protected override int GetReward (MinigameRewardTypes eType) {
		return DoctorMatchManager.Instance.GetReward( eType );
	}
	
	//---------------------------------------------------
	// GetScore()
	//---------------------------------------------------		
	protected override int GetScore () {
		return DoctorMatchManager.Instance.GetScore();
	}	
}
