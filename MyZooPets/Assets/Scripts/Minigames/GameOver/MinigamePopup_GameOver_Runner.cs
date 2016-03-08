using UnityEngine;
using System.Collections;

//---------------------------------------------------
// MinigamePopup_GameOver_Runner
// Game over screen for runner game.
//---------------------------------------------------

public class MinigamePopup_GameOver_Runner : MinigamePopup_GameOver {	
	protected override int GetReward (MinigameRewardTypes eType) {
		return RunnerGameManager.Instance.GetReward( eType );
	}
			
	protected override int GetScore () {
		return RunnerGameManager.Instance.GetScore();
	}

	protected override void _RewardBadges(){
		BadgeLogic.Instance.CheckSeriesUnlockProgress(BadgeType.Runner, RunnerGameManager.Instance.GetScore(), true);
	}

	protected override bool CheckAndFlagNewGameAd(){
		bool aux = RunnerGameManager.Instance.IsNewGameAd;
		RunnerGameManager.Instance.IsNewGameAd = false;
		return aux;
	}
}
