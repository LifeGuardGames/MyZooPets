using UnityEngine;
using System.Collections;

//---------------------------------------------------
// MinigamePopup_GameOver_Runner
// Game over screen for runner game.
//---------------------------------------------------

public class MinigamePopup_GameOver_Runner : MinigamePopup_GameOver {

	//---------------------------------------------------
	// GetReward()
	//---------------------------------------------------		
	protected override int GetReward (MinigameRewardTypes eType) {
		return RunnerGameManager.Instance.GetReward( eType );
	}
	
	//---------------------------------------------------
	// GetScore()
	//---------------------------------------------------		
	protected override int GetScore () {
		return RunnerGameManager.Instance.GetScore();
	}	
}
