using UnityEngine;
using System.Collections;

//---------------------------------------------------
// MinigamePopup_GameOver_DGT
// Game over screen for the clinic game.
//---------------------------------------------------

public class MinigamePopup_GameOver_DGT : MinigamePopup_GameOver {
	
	//---------------------------------------------------
	// GetReward()
	//---------------------------------------------------		
	protected override int GetReward (MinigameRewardTypes eType) {
		return DGTManager.Instance.GetReward( eType );
	}
	
	//---------------------------------------------------
	// GetScore()
	//---------------------------------------------------		
	protected override int GetScore () {
		return DGTManager.Instance.GetScore();
	}

	protected override void _RewardBadges(){
	}
}
