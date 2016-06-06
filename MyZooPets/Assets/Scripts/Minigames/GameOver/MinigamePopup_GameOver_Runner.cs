using UnityEngine;
using System.Collections;

public class MinigamePopup_GameOver_Runner : MinigamePopup_GameOver {	
	protected override int GetReward (MinigameRewardTypes eType) {
		return OldRunnerManager.Instance.GetReward( eType );
	}
			
	protected override int GetScore () {
		return OldRunnerManager.Instance.GetScore();
	}

	protected override void RewardBadges(){
		BadgeManager.Instance.CheckSeriesUnlockProgress(BadgeType.Runner, OldRunnerManager.Instance.GetScore(), true);
	}

	protected override bool CheckAndFlagNewGameAd(){
		bool aux = OldRunnerManager.Instance.IsNewGameAd;
		OldRunnerManager.Instance.IsNewGameAd = false;
		return aux;
	}
}
